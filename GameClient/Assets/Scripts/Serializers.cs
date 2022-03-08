using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameStateSerializer
{
    public List<PlayerSerializer> players;
    public List<GameBallSerializer> gameBalls;
    public GameStateSerializer()
    {
        this.players = new List<PlayerSerializer>();
        this.gameBalls = new List<GameBallSerializer>();
    }
}

[Serializable]
public class BaseGameEntitySerializer
{
    public string uuid;
    public PositionSerializer position;
}

[Serializable]
public class PlayerSerializer : BaseGameEntitySerializer
{

    public string name;

    public PlayerSerializer(string uuid, PositionSerializer position, string name)
    {
        this.uuid = uuid;
        this.position = position;
        this.name = name;
    }
}

[Serializable]
public class GameBallSerializer : BaseGameEntitySerializer
{
    public GameBallSerializer(string uuid, PositionSerializer position)
    {
        this.uuid = uuid;
        this.position = position;
    }
}

[Serializable]
public class PositionSerializer
{
    public float x;
    public float y;
    public PositionSerializer(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
