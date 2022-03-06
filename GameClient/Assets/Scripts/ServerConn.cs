using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;

public class ServerConn
{

    private WebSocket ws;

    public delegate void ServerMessageHandlerDelegate(string serverMessage);
    private ServerMessageHandlerDelegate serverMessageHandlerDelegate;


    public ServerConn(string gameServerUrl, bool isCentralClient) {
        if (isCentralClient)
        {
            gameServerUrl += "?isCentralClient=1";
        }
        this.InitWebSocketClient(gameServerUrl);
    }

    // INTERFACE METHODS

    public void SendClientMessageToServer(string clientMessage) {
        this.ws.Send(clientMessage);
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