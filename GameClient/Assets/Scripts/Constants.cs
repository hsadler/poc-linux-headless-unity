using System;

public static class Constants
{
    public const string GAME_SERVER_URL = "ws://localhost:5000";
    public const bool IS_CENTRAL_CLIENT = true;
    // client message types
    public const string CLIENT_MESSAGE_TYPE_PLAYER_ENTER = "CLIENT_MESSAGE_TYPE_PLAYER_ENTER";
    public const string CLIENT_MESSAGE_TYPE_PLAYER_EXIT = "CLIENT_MESSAGE_TYPE_PLAYER_EXIT";
    public const string CLIENT_MESSAGE_TYPE_PLAYER_POSITION = "CLIENT_MESSAGE_TYPE_PLAYER_POSITION";
}