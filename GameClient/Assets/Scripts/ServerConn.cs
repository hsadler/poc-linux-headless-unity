using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;

public class ServerConn
{

    private WebSocket ws;

    public delegate void ServerMessageHandlerDelegate(string serverMessageJson);
    private ServerMessageHandlerDelegate serverMessageHandlerDelegate;


    public ServerConn(string gameServerUrl) {
        this.InitWebSocketClient(gameServerUrl);
    }

    // INTERFACE METHODS

    public void SendClientMessageToServer(string clientMessageJson) {
        this.ws.Send(clientMessageJson);
    }

    public void RegisterServerMessageHandler(ServerMessageHandlerDelegate d) {
        this.serverMessageHandlerDelegate = d;
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
        this.serverMessageHandlerDelegate(e.Data);
    }
    
}