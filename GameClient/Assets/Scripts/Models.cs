using System;

// base game entity
[Serializable]
public class BaseGameEntity
{
    public string uuid;
    public Position position;
}

[Serializable]
public class PlayerEntity : BaseGameEntity
{
    public bool active;
    public string name;
    public PlayerEntity(string uuid, Position position, bool active, string name)
    {
        this.uuid = uuid;
        this.position = position;
        this.active = active;
        this.name = name;
    }
}

[Serializable]
public class GameBallEntity : BaseGameEntity
{
    public GameBallEntity(string uuid, Position position)
    {
        this.uuid = uuid;
        this.position = position;
    }
}

[Serializable]
public class Position
{
    public float x;
    public float y;
    public Position(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
