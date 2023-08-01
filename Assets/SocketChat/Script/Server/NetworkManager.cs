using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkManager : SocketIOComponent
{
    public static NetworkManager it
    {
        get
        {
            return _instance;
        }
    }
    private static NetworkManager _instance;
    public delegate void SocketCallback(string inData);

    Dictionary<string, System.Delegate> _eventCallbackDic = new Dictionary<string, System.Delegate>();

    public override void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        base.Start();

        /// make singleton based
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        /// set socket function
        List<string> methodList = ServerMethod.GetConnectMethod();
        foreach (var methodName in methodList)
        {
            On(methodName, OnRequest);
        }
    }

    private void OnRequest(SocketIOEvent inData)
    {
        string callFunc = inData.name;
        if (_eventCallbackDic.ContainsKey(callFunc))
        {
            SocketCallback callback = (_eventCallbackDic[callFunc] as SocketCallback);
            callback(inData.data == null ? string.Empty : inData.data.ToString());
        }
    }

    /// <summary>
    /// 소켓 이벤트 콜백 등록
    /// </summary>
    /// <param name="inKey"></param>
    /// <param name="inEvent"></param>
    public void AddEventCallback(string inKey, SocketCallback inEvent)
    {
        if (_eventCallbackDic.ContainsKey(inKey))
        {
            _eventCallbackDic[inKey] = (_eventCallbackDic[inKey] as SocketCallback) + inEvent;
        }
        else
        {
            _eventCallbackDic.Add(inKey, inEvent);
        }
    }

    /// <summary>
    /// 등록된 이벤트 모든 콜백 삭제
    /// </summary>
    /// <param name="inKey"></param>
    /// <param name="inEvent"></param>
    public void RemoveEventCallback(string inKey)
    {
        if (_eventCallbackDic.ContainsKey(inKey))
        {
            _eventCallbackDic.Remove(inKey);
        }
    }

    /// <summary>
    /// 등록된 이벤트 특정 콜백 삭제
    /// </summary>
    /// <param name="inKey"></param>
    /// <param name="inEvent"></param>
    public void RemoveEventCallback(string inKey, SocketCallback inEvent)
    {
        if (_eventCallbackDic.ContainsKey(inKey))
        {
            _eventCallbackDic[inKey] = (_eventCallbackDic[inKey] as SocketCallback) - inEvent;
        }
    }

    /// <summary>
    /// 모든 이벤트 삭제
    /// </summary>
    public void RemoveAllEvent()
    {
        _eventCallbackDic.Clear();
    }
}
