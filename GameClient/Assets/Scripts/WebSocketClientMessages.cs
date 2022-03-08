using System;

// base client message
[Serializable]
public class BaseClientMessage
{
    public string clientId;
    public string messageType;
}

[Serializable]
public class GameStateMessage : BaseClientMessage
{
    public GameStateSerializer gameState;
    public GameStateMessage(string clientId, string messageType, GameStateSerializer gameState)
    {
        this.clientId = clientId;
        this.messageType = messageType;
        this.gameState = gameState;
    }
}


//[Serializable]
//public class ClientMessageCentralClientConnect : BaseClientMessage
//{

//    public ClientMessageCentralClientConnect(string clientId)
//    {
//        this.clientId = clientId;
//        this.messageType = Constants.MESSAGE_TYPE_CENTRAL_CLIENT_CONNECT;
//    }

//}

