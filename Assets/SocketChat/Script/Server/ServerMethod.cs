using System.Collections.Generic;

public class ServerMethod
{
    public static readonly string CONNECT               = "connect";
    public static readonly string DISCONNECT            = "disconnect";
    public static readonly string USER_CONNECT          = "user_connect";
    public static readonly string SEND_MESSAGE          = "send_message";
    public static readonly string RECEIVE_MESSAGE       = "receive_message";
    public static readonly string OTHER_USER_CONNECT    = "other_user_connect";
    public static readonly string OTHER_USER_DISCONNECT = "other_user_disconnect";

    public static List<string> GetConnectMethod()
    {
        return new List<string>()
        {
            CONNECT,
            DISCONNECT,
            USER_CONNECT,
            SEND_MESSAGE,
            RECEIVE_MESSAGE,
            OTHER_USER_CONNECT,
            OTHER_USER_DISCONNECT
        };
    }
}