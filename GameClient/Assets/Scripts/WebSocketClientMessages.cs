using System;

// base client message
[Serializable]
public class BaseClientMessage
{
    public string messageType;
    public string clientId;
}

[Serializable]
public class PlayerJoinMessage : BaseClientMessage
{
    public PlayerJoinMessage(string clientId)
    {
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_JOIN;
        this.clientId = clientId;
    }
}

[Serializable]
public class GameStateMessage : BaseClientMessage
{
    public GameStateSerializer gameState;
    public GameStateMessage(string clientId, GameStateSerializer gameState)
    {
        this.messageType = Constants.MESSAGE_TYPE_GAME_STATE;
        this.clientId = clientId;
        this.gameState = gameState;
    }
}
