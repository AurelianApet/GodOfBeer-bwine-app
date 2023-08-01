using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDataManager : MonoBehaviour
{
    public static GeneralDataManager it
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                _instance = go.AddComponent<GeneralDataManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    public static GeneralDataManager _instance;

    public ServerModel.User currentUser = null;
    public Dictionary<string, ServerModel.User> userDictionary = new Dictionary<string, ServerModel.User>();
}
