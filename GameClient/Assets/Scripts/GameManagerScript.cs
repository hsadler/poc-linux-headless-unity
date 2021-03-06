using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    [NonSerialized]
    public string clientId;
    [NonSerialized]
    public bool isCentralClient;
    [NonSerialized]
    public ServerConn serverConn;

    private Queue<string> serverMessageQueue = new Queue<string>();

    public GameObject gameBallPrefab;
    public GameObject playerPrefab;

    private IDictionary<string, GameObject> idToGameBallGO = new Dictionary<string, GameObject>();
    private IDictionary<string, GameObject> idToPlayerGO = new Dictionary<string, GameObject>();

    private Vector3 gameBallStartPos = new Vector3(-5, 0, 0);
    private Vector3 playerStartPos = new Vector3(0, 3, 0);

    // static reference to the singleton instance
    public static GameManagerScript instance { get; private set; }

    // UNITY HOOKS

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        this.clientId = Functions.GenUUID();
        this.isCentralClient = Environment.GetEnvironmentVariable("CENTRAL_GAME_CLIENT") == "1";
        //this.isCentralClient = true;
        if (this.isCentralClient)
        {
            Application.targetFrameRate = (int)(1F / Time.fixedDeltaTime);
        }
        // connect to server
        this.serverConn = new ServerConn(Constants.GAME_SERVER_URL, this.isCentralClient);
        // register server message handling
        this.serverConn.RegisterServerMessageHandler((string serverMessage) => {
            this.serverMessageQueue.Enqueue(serverMessage);
        });
        // signal that player has joined
        if (!this.isCentralClient)
        {
            var playerJoinMessage = new PlayerJoinMessage(this.clientId);
            this.serverConn.SendClientMessageToServer(JsonUtility.ToJson(playerJoinMessage));
        }
    }

    void Start()
    {
        if(this.isCentralClient)
        {
            InvokeRepeating("CreateGameBall", 0, 5);
        }
    }

    void Update()
    {
        if (this.isCentralClient)
        {
            // send entire game state to server
            var gameStateSerializer = this.SerializeGameState();
            var message = new GameStateMessage(this.clientId, gameStateSerializer);
            this.serverConn.SendClientMessageToServer(JsonUtility.ToJson(message));
        }
        else
        {
            this.CheckAndSendPlayerInput();
        }
        this.ProcessServerMessagesFromQueue();
    }

    void OnDestroy()
    {
        var playerLeaveMessage = new PlayerLeaveMessage(this.clientId);
        this.serverConn.SendClientMessageToServer(JsonUtility.ToJson(playerLeaveMessage));
        this.serverConn.CloseConnection();
    }

    // INTERFACE METHODS

    public GameObject GetPlayerGOByClientId(string clientId)
    {
        foreach (KeyValuePair<string, GameObject> entry in this.idToPlayerGO)
        {
            if (entry.Value.GetComponent<PlayerScript>().ownerClientId == clientId)
            {
                return entry.Value;
            }
        }
        return null;
    }

    public bool DestroyPlayerById(string uuid)
    {
        if (this.idToPlayerGO.ContainsKey(uuid))
        {
            GameObject.Destroy(this.idToPlayerGO[uuid]);
            this.idToPlayerGO.Remove(uuid);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DestroyGameBallById(string uuid)
    {
        if (this.idToGameBallGO.ContainsKey(uuid))
        {
            GameObject.Destroy(this.idToGameBallGO[uuid]);
            this.idToGameBallGO.Remove(uuid);
            return true;
        } else
        {
            return false;
        }
    }

    // IMPLEMENTATION METHODS

    private GameStateSerializer SerializeGameState()
    {
        // serialize and send entire game state to connected player-clients
        var gameStateSerializer = new GameStateSerializer();
        foreach (KeyValuePair<string, GameObject> entry in this.idToGameBallGO)
        {
            var pos = new PositionSerializer(entry.Value.transform.position.x, entry.Value.transform.position.y);
            var gb = new GameBallSerializer(
                uuid: entry.Key,
                position: pos
            );
            gameStateSerializer.gameBalls.Add(gb);
        }
        foreach (KeyValuePair<string, GameObject> entry in this.idToPlayerGO)
        {
            var playerGO = entry.Value;
            string playerUUID = entry.Key;
            var pos = new PositionSerializer(playerGO.transform.position.x, playerGO.transform.position.y);
            var player = new PlayerSerializer(
                uuid: playerUUID,
                position: pos,
                ownerClientId: playerGO.GetComponent<PlayerScript>().ownerClientId,
                name: "test name"
            ); ;
            gameStateSerializer.players.Add(player);
        }
        return gameStateSerializer;
    }

    private void CreateGameBall()
    {
        string entityUUID = Functions.GenUUID();
        var gameBall = GameObject.Instantiate(this.gameBallPrefab, this.gameBallStartPos, Quaternion.identity);
        gameBall.GetComponent<BallScript>().entityUUID = entityUUID;
        gameBall.GetComponent<Rigidbody2D>().gravityScale = 1;
        this.idToGameBallGO.Add(entityUUID, gameBall);
    }

    private void ProcessServerMessagesFromQueue()
    {
        while(this.serverMessageQueue.Count > 0)
        {
            string serverMessage = this.serverMessageQueue.Dequeue();
            this.RouteServerMessage(serverMessage);
        }
    }

    private void CheckAndSendPlayerInput()
    {
        string keyDown = null;
        if (Input.GetKeyDown(KeyCode.A))
        {
            keyDown = KeyCode.A.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            keyDown = KeyCode.D.ToString();
        }
        if (keyDown != null)
        {
            var playerInputMessage = new PlayerInputMessage(this.clientId, keyDown, Constants.KEY_INTERACTION_TYPE_DOWN);
            this.serverConn.SendClientMessageToServer(JsonUtility.ToJson(playerInputMessage));
        }
        string keyUp = null;
        if (Input.GetKeyUp(KeyCode.A))
        {
            keyUp = KeyCode.A.ToString();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            keyUp = KeyCode.D.ToString();
        }
        if (keyUp != null)
        {
            var playerInputMessage = new PlayerInputMessage(this.clientId, keyUp, Constants.KEY_INTERACTION_TYPE_UP);
            this.serverConn.SendClientMessageToServer(JsonUtility.ToJson(playerInputMessage));
        }
    }

    private void RouteServerMessage(string serverMessage)
    {
        // parse message type
        string messageType = JsonUtility.FromJson<BaseMessage>(serverMessage).messageType;
        // route message to handler based on message type
        if(this.isCentralClient)
        {
            switch (messageType)
            {
                // if is central-client
                case Constants.MESSAGE_TYPE_PLAYER_JOIN:
                    this.HandlePlayerJoinMessage(serverMessage);
                    break;
                case Constants.MESSAGE_TYPE_PLAYER_LEAVE:
                    this.HandlePlayerLeaveMessage(serverMessage);
                    break;
                case Constants.MESSAGE_TYPE_PLAYER_INPUT:
                    this.HandlePlayerInputMessage(serverMessage);
                    break;
                default:
                    Debug.LogWarning("Server message not processed: " + serverMessage);
                    break;
            }
        } else
        {
            switch (messageType)
            {
                // if is player-client
                case Constants.MESSAGE_TYPE_GAME_STATE:
                    this.HandleGameStateMessage(serverMessage);
                    break;
                default:
                    Debug.LogWarning("Server message not processed: " + serverMessage);
                    break;
            }
        }
    }

    // is central-client message handlers

    private void HandlePlayerJoinMessage(string serverMessage)
    {
        var playerJoinMessage = JsonUtility.FromJson<PlayerJoinMessage>(serverMessage);
        var playerGO = GameObject.Instantiate(this.playerPrefab, this.playerStartPos, Quaternion.identity);
        playerGO.GetComponent<PlayerScript>().ownerClientId = playerJoinMessage.clientId;
        playerGO.GetComponent<Rigidbody2D>().gravityScale = 1F;
        this.idToPlayerGO.Add(Functions.GenUUID(), playerGO);
    }

    private void HandlePlayerLeaveMessage(string serverMessage)
    {
        var playerLeaveMessage = JsonUtility.FromJson<PlayerLeaveMessage>(serverMessage);
        GameObject toDeletePlayerGO = null;
        string toDeletePlayerUUID = null;
        foreach (KeyValuePair<string, GameObject> entry in this.idToPlayerGO)
        {
            string playerUUID = entry.Key;
            GameObject playerGO = entry.Value;
            if (playerGO.GetComponent<PlayerScript>().ownerClientId == playerLeaveMessage.clientId)
            {
                toDeletePlayerGO = playerGO;
                toDeletePlayerUUID = playerUUID;
            }
        }
        if (toDeletePlayerGO != null && toDeletePlayerUUID != null)
        {
            GameObject.Destroy(toDeletePlayerGO);
            this.idToPlayerGO.Remove(toDeletePlayerUUID);
        }
    }

    private void HandlePlayerInputMessage(string serverMessage)
    {
        var playerInputMessage = JsonUtility.FromJson<PlayerInputMessage>(serverMessage);
        var playerGO = this.GetPlayerGOByClientId(playerInputMessage.clientId);
        Vector3 direction = Vector3.zero;
        if (playerInputMessage.key == KeyCode.A.ToString())
        {
            direction = Vector3.left;
        } else if (playerInputMessage.key == KeyCode.D.ToString())
        {
            direction = Vector3.right;
        }
        if(direction != Vector3.zero)
        {
            playerGO.GetComponent<Rigidbody2D>().AddForce(direction * 1000);
        }
    }

    // is player-client message handlers

    private void HandleGameStateMessage(string serverMessage)
    {
        var gameStateMessage = JsonUtility.FromJson<GameStateMessage>(serverMessage);
        var entityUUIDsToDelete = new List<string>();
        // create or update players
        var idToPlayerSerializer = new Dictionary<string, PlayerSerializer>();
        foreach (PlayerSerializer player in gameStateMessage.gameState.players)
        {
            idToPlayerSerializer.Add(player.uuid, player);
            var pos = new Vector3(player.position.x, player.position.y, 0);
            if (this.idToPlayerGO.ContainsKey(player.uuid))
            {
                var playerGO = this.idToPlayerGO[player.uuid];
                playerGO.GetComponent<EntityInterpolation>().InterpolateToPosition(pos);
            }
            else
            {
                var playerGO = GameObject.Instantiate(this.playerPrefab, pos, Quaternion.identity);
                var playerScript = playerGO.GetComponent<PlayerScript>();
                playerScript.ownerClientId = player.ownerClientId;
                if (player.ownerClientId == this.clientId)
                {
                    playerScript.isMainPlayer = true;
                    playerScript.mainPlayerIndicator.SetActive(true);
                }
                this.idToPlayerGO.Add(player.uuid, playerGO);
            }
        }
        // delete players
        foreach (KeyValuePair<string, GameObject> entry in this.idToPlayerGO)
        {
            string playerUUID = entry.Key;
            if (!idToPlayerSerializer.ContainsKey(playerUUID))
            {
                entityUUIDsToDelete.Add(playerUUID);
            }
        }
        // create or update balls
        var idToGameBallSerializer = new Dictionary<string, GameBallSerializer>();
        foreach (GameBallSerializer gameBall in gameStateMessage.gameState.gameBalls)
        {
            idToGameBallSerializer.Add(gameBall.uuid, gameBall);
            var pos = new Vector3(gameBall.position.x, gameBall.position.y, 0);
            if (this.idToGameBallGO.ContainsKey(gameBall.uuid))
            {
                var gameBallGO = this.idToGameBallGO[gameBall.uuid];
                gameBallGO.GetComponent<EntityInterpolation>().InterpolateToPosition(pos);
            }
            else
            {
                var gameBallGO = GameObject.Instantiate(this.gameBallPrefab, pos, Quaternion.identity);
                this.idToGameBallGO.Add(gameBall.uuid, gameBallGO);
            }
        }
        // game balls to delete
        foreach (KeyValuePair<string, GameObject> entry in this.idToGameBallGO)
        {
            string gameBallUUID = entry.Key;
            if (!idToGameBallSerializer.ContainsKey(gameBallUUID))
            {
                entityUUIDsToDelete.Add(gameBallUUID);
            }
        }
        // delete game objects
        foreach (string entityUUID in entityUUIDsToDelete)
        {
            // try to destroy all types
            this.DestroyPlayerById(entityUUID);
            this.DestroyGameBallById(entityUUID);
        }
    }

}