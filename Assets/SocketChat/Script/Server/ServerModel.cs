using UnityEngine;
using System;

namespace ServerModel
{
    [Serializable]
    public class User
    {
        public string name;

        public JSONObject ToJSON()
        {
            return new JSONObject(JsonUtility.ToJson(this));
        }
    }

    [Serializable]
    public class Message
    {
        public string from;
        public string to;
        public string type;
        public string message;
        public string is_repair;
        public string room_id;
        public string is_web;

        public JSONObject ToJSON()
        {
            return new JSONObject(JsonUtility.ToJson(this));
        }
    }
}