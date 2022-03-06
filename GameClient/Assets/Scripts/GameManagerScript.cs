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

    private GameObject mainPlayer;
    //private Vector3 newP


    // UNITY HOOKS

    void Awake()
    {
        this.isCentralClient = Environment.GetEnvironmentVariable("CENTRAL_GAME_CLIENT") == "1";
        //this.isCentralClient = true;
        if (this.isCentralClient)
        {
            Application.targetFrameRate = (int)(1F / Time.fixedDeltaTime);
        }
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
        else
        {
            // interpolate all entity positions displays from server
            this.gameBall.GetComponent<EntityInterpolation>().InterpolatePosition(this.newGameBallPosition);
        }
        
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void ServerMessageHandler(string serverMessage)
    {
        this.newGameBallPosition = Functions.StringToVector3(serverMessage);
    }

}