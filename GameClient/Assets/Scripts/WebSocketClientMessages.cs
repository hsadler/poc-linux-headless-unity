using System;

// base client message
[Serializable]
public class BaseClientMessage
{
    public string clientId;
    public string messageType;
}

[Serializable]
public class ClientMessageCentralClientConnect : BaseClientMessage
{

    public ClientMessageCentralClientConnect(string clientId)
    {
        this.clientId = clientId;
        this.messageType = Constants.MESSAGE_TYPE_CENTRAL_CLIENT_CONNECT;
    }

}

[Serializable]
public class ClientMessagePlayerClientConnect : BaseClientMessage
{

    public ClientMessagePlayerClientConnect(string clientId)
    {
        this.clientId = clientId;
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_CLIENT_CONNECT;
    }

}

[Serializable]
public class ClientMessagePlayerCreate : BaseClientMessage
{

    public PlayerEntity player;

    public ClientMessagePlayerCreate(string clientId, PlayerEntity player)
    {
        this.clientId = clientId;
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_CREATE;
        this.player = player;
    }

}

[Serializable]
public class ClientMessagePlayerDestroy : BaseClientMessage
{

    public string goUUID;

    public ClientMessagePlayerDestroy(string clientId, string goUUID)
    {
        this.clientId = clientId;
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_DESTROY;
        this.goUUID = goUUID;
    }

}

[Serializable]
public class ClientMessagePlayerState : BaseClientMessage
{

    public string playerId;
    public PlayerEntity player;

    public ClientMessagePlayerState(string clientId, PlayerEntity playerModel)
    {
        this.clientId = clientId;
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_STATE;
        this.player = playerModel;
    }

}
