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

    public GameObject gameBallPrefab;
    public GameObject mainPlayerPrefab;

    private GameObject gameBall;
    private Vector3 newGameBallPosition = Vector3.zero;

    private IDictionary<string, BaseGameEntity> uuidToGameEntity;
    private IDictionary<string, GameObject> uuidToGO;
    //private GameObject mainPlayer;
    //private Vector3 newMainPlayerPosition;

    private Vector3 playerStartPos = new Vector3(-3, 0, 0);

    // UNITY HOOKS

    void Awake()
    {
        this.clientId = Functions.GenUUID();
        this.isCentralClient = Environment.GetEnvironmentVariable("CENTRAL_GAME_CLIENT") == "1";
        if (this.isCentralClient)
        {
            Application.targetFrameRate = (int)(1F / Time.fixedDeltaTime);
        }
        // connect to server
        this.serverConn = new ServerConn(Constants.GAME_SERVER_URL, this.isCentralClient);
        this.serverConn.RegisterServerMessageHandler(this.HandleServerMessage);
        //// signal client connected
        //if(this.isCentralClient)
        //{
        //    string m = JsonUtility.ToJson(new ClientMessageCentralClientConnect(this.clientId));
        //    this.serverConn.SendClientMessageToServer(m);
        //} else
        //{
        //    string m = JsonUtility.ToJson(new ClientMessagePlayerClientConnect(this.clientId));
        //    this.serverConn.SendClientMessageToServer(m);
        //}
    }

    void Start()
    {
        if(this.isCentralClient)
        {
            this.CentralClientCreateGameBall();
        }
    }

    void Update()
    {
        if (this.isCentralClient)
        {
            // TODO: REFACTOR TO USE SERIALIZED MESSAGE
            //this.serverConn.SendClientMessageToServer(this.gameBall.transform.position.ToString());
        }
        else
        {
            // interpolate all entity positions displays from server
            //this.gameBall.GetComponent<EntityInterpolation>().InterpolatePosition(this.newGameBallPosition);
        }
    }

    void OnDestroy()
    {
        this.serverConn.CloseConnection();
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void CentralClientCreateGameBall()
    {
        this.gameBall = GameObject.Instantiate(this.gameBallPrefab, Vector3.zero, Quaternion.identity);
        this.gameBall.GetComponent<Rigidbody2D>().gravityScale = 1;
    }   

    private void HandleServerMessage(string serverMessage)
    {
        // parse message type
        string messageType = JsonUtility.FromJson<ServerMessageGeneric>(serverMessage).messageType;
        // route message to handler based on message type
        switch (messageType)
        {
            //case Constants.MESSAGE_TYPE_PLAYER_CLIENT_CONNECT:
            //    this.HandlePlayerClientConnectServerMessage(serverMessage);
            //    break;
            case Constants.MESSAGE_TYPE_PLAYER_INPUT:
                this.HandlePlayerInputServerMessage(serverMessage);
                break;
            //case Constants.MESSAGE_TYPE_PLAYER_CREATE:
            //    this.HandlePlayerCreateServerMessage(serverMessage);
            //    break;
            //case Constants.MESSAGE_TYPE_PLAYER_DESTROY:
            //    this.HandlePlayerDestroyServerMessage(serverMessage);
            //    break;
            //case Constants.MESSAGE_TYPE_PLAYER_STATE:
            //    this.HandlePlayerStateServerMessage(serverMessage);
            //    break;
            default:
                Debug.LogWarning("Server message not processed: " + serverMessage);
                break;
        }
    }

    private void HandlePlayerClientConnectServerMessage(string serverMessage)
    {
        Debug.Log("receiving 'player client connect' message: " + serverMessage);
        //var playerConnectMessage = JsonUtility.FromJson<ClientMessagePlayerClientConnect>(serverMessage);
        //var playerGO = GameObject.Instantiate(this.mainPlayerPrefab, this.playerStartPos, Quaternion.identity);
        //playerGO.GetComponent<Rigidbody2D>().gravityScale = 1;
        //string uuid = Functions.GenUUID();
        //this.uuidToGO.Add(uuid, playerGO);
        //var startPos = new Position(this.playerStartPos.x, this.playerStartPos.y);
        //string m = JsonUtility.ToJson(new ClientMessagePlayerCreate(
        //    playerConnectMessage.clientId,
        //    new PlayerEntity(uuid, startPos, true, "new player")
        //));
        //this.serverConn.SendClientMessageToServer(m);
    }

    private void HandlePlayerCreateServerMessage(string serverMessage)
    {
        // STUB
        Debug.Log("receiving 'player create' message: " + serverMessage);
    }

    private void HandlePlayerDestroyServerMessage(string serverMessage)
    {
        // STUB
    }

    private void HandlePlayerStateServerMessage(string serverMessage)
    {
        // STUB
    }

    private void HandlePlayerInputServerMessage(string serverMessage)
    {
        // STUB
    }

}