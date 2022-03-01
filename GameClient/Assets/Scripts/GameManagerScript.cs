using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    public ServerConn serverConn;
    public bool isCentralClient;

    private GameObject gameBall;


    // UNITY HOOKS

    void Awake()
    {
        this.serverConn = new ServerConn(Constants.GAME_SERVER_URL);
        this.serverConn.RegisterServerMessageHandler(this.ServerMessageHandler);
        this.isCentralClient = Environment.GetEnvironmentVariable("CENTRAL_GAME_CLIENT") == "1";
    }

    void Start()
    {
        this.gameBall = GameObject.FindGameObjectWithTag("Ball");
    }

    void Update()
    {
        
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void ServerMessageHandler(string serverMessage)
    {
        //Debug.Log("Server message received: " + serverMessage);
    }

}