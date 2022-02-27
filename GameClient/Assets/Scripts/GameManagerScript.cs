using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    public ServerConn serverConn;

    private GameObject gameBall;


    // UNITY HOOKS

    void Awake()
    {
        this.serverConn = new ServerConn(Constants.GAME_SERVER_URL);
        this.serverConn.RegisterServerMessageHandler(this.ServerMessageHandler);
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
        Debug.Log("Server message received: " + serverMessage);
    }

}