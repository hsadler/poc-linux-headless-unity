using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    private ServerConn serverConn;
    private const string GAME_SERVER_URL = "ws://localhost:5000";

    // UNITY HOOKS

    void Awake()
    {
        this.serverConn = new ServerConn(GAME_SERVER_URL);
        this.serverConn.RegisterServerMessageHandler(this.ServerMessageHandler);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // INTERFACE METHODS

    // IMPLEMENTATION METHODS

    private void ServerMessageHandler(string serverMessageJson)
    {
        Debug.Log("Server message received: " + serverMessageJson);
    }

}