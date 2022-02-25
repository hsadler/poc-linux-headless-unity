using System;

[Serializable]
public class ServerMessageGeneric
{
    public string messageType;
}

// EXAMPLES

//[Serializable]
//public class ServerMessageGameState
//{
//    public string messageType;
//    public GameState gameState;
//}

//[Serializable]
//public class ServerMessagePlayerEnter
//{
//    public string messageType;
//    public Player player;
//}

//[Serializable]
//public class ServerMessagePlayerExit
//{
//    public string messageType;
//    public string playerId;
//}

//[Serializable]
//public class ServerMessagePlayerUpdate
//{
//    public string messageType;
//    public Player player;
//}