using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;

public class ServerConn
{


    private WebSocket ws;

    public delegate void GameStateHandlerDelegate(string gameStateJson);
    private GameStateHandlerDelegate gameStateHandlerDelegate;


    public ServerConn(string gameServerUrl) {
        this.InitWebSocketClient(gameServerUrl);
    }

    // INTERFACE METHODS

    public void SynchToServer(string gameStateJson) {
        this.SendWebsocketClientMessage(gameStateJson);
    }

    public void RegisterSyncFromServerHandler(GameStateHandlerDelegate d) {
        this.gameStateHandlerDelegate = d;
    }

    // WEBSOCKET HELPERS

    private void InitWebSocketClient(string gameServerUrl)
    {
        // create websocket connection
        this.ws = new WebSocket(gameServerUrl);
        this.ws.Connect();
        // add message handler callback
        this.ws.OnMessage += this.ProcessServerMessage;
    }

    private void ProcessServerMessage(object sender, MessageEventArgs e)
    {
        this.gameStateHandlerDelegate(e.Data);
    }

    private void SendWebsocketClientMessage(string messageJson)
    {
        this.ws.Send(messageJson);
    }

    
}