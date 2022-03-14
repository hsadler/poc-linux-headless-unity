using System;

// base client message
[Serializable]
public class BaseMessage
{
    public string messageType;
    public string clientId;
}

[Serializable]
public class GameStateMessage : BaseMessage
{
    public GameStateSerializer gameState;
    public GameStateMessage(string clientId, GameStateSerializer gameState)
    {
        this.messageType = Constants.MESSAGE_TYPE_GAME_STATE;
        this.clientId = clientId;
        this.gameState = gameState;
    }
}

[Serializable]
public class PlayerJoinMessage : BaseMessage
{
    public PlayerJoinMessage(string clientId)
    {
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_JOIN;
        this.clientId = clientId;
    }
}

[Serializable]
public class PlayerLeaveMessage : BaseMessage
{
    public PlayerLeaveMessage(string clientId)
    {
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_LEAVE;
        this.clientId = clientId;
    }
}

[Serializable]
public class PlayerInputMessage : BaseMessage
{
    public string key;
    public string keyInteractionType;
    public PlayerInputMessage(string clientId, string key, string keyInteractionType)
    {
        this.messageType = Constants.MESSAGE_TYPE_PLAYER_INPUT;
        this.clientId = clientId;
        this.key = key;
        this.keyInteractionType = keyInteractionType;
    }
}
