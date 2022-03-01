using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    [NonSerialized]
    public bool isCentralClient;
    public ServerConn serverConn;

    public GameObject gameBallPrefab;

    private GameObject gameBall;
    private Vector3 newGameBallPosition = Vector3.zero;
    private Vector3 gameBallVelocity = Vector3.zero;


    // UNITY HOOKS

    void Awake()
    {
        this.isCentralClient = Environment.GetEnvironmentVariable("CENTRAL_GAME_CLIENT") == "1";
        this.serverConn = new ServerConn(Constants.GAME_SERVER_URL);
        this.serverConn.RegisterServerMessageHandler(this.ServerMessageHandler);
    }

    void Start()
    {
        this.gameBall = GameObject.Instantiate(this.gameBallPrefab, Vector3.zero, Quaternion.identity);
        if(this.isCentralClient)
        {
            this.gameBall.GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }

    void Update()
    {
        if (this.isCentralClient)
        {
            this.serverConn.SendClientMessageToServer(this.gameBall.transform.position.ToString());
        }
        else if (this.gameBall.transform.position != this.newGameBallPosition)
        {
            // smooth movement of central game-client controlled ball position
            var d = Vector3.Distance(this.gameBall.transform.position, this.newGameBallPosition);
            if (d < 1f)
            {
                this.gameBall.transform.position = Vector3.SmoothDamp(
                    this.gameBall.transform.position,
                    this.newGameBallPosition,
                    ref this.gameBallVelocity,
                    0.01F
                );
                // alt smoothing method (seems choppier)
                //this.gameBall.transform.position = Vector3.MoveTowards(
                //    this.gameBall.transform.position,
                //    this.newGameBallPosition,
                //    100 * Time.deltaTime
                //);
            }
            else
            {
                this.gameBall.transform.position = this.newGameBallPosition;
            }
        }
        
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void ServerMessageHandler(string serverMessage)
    {
        this.newGameBallPosition = Functions.StringToVector3(serverMessage);
    }

}