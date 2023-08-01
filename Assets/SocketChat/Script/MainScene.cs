using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField]
    InputField _nickNameInputField;

    [SerializeField]
    Text _txtWaitNetwork;

    private bool _waitJoin = false;

    private void Start()
    {
        NetworkManager.it.AddEventCallback(ServerMethod.CONNECT,
            (data) =>
            {
                _txtWaitNetwork.text  = "Connected";
                _txtWaitNetwork.color = new Color(0, 0, 1);
            });

        NetworkManager.it.AddEventCallback(ServerMethod.OTHER_USER_CONNECT,
            (data) =>
            {
                var userDic = GeneralDataManager.it.userDictionary;
                var user    = JsonUtility.FromJson<ServerModel.User>(data);

                if (!userDic.ContainsKey(user.name))
                {
                    // 현재 채팅에 접속해있는 유저를 등록
                    userDic.Add(user.name, user);
                    if (user.name == _nickNameInputField.text)
                    {
                        GeneralDataManager.it.currentUser = user;

                        _txtWaitNetwork.text = "Complete";
                        _txtWaitNetwork.color = new Color(0, 0, 1);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("RoomScene");
                    }
                }
            });
    }

    public void OnJoin()
    {
        if (_waitJoin || string.IsNullOrEmpty(_nickNameInputField.text) || _txtWaitNetwork.text != "Connected")
        {
            return;
        }

        _waitJoin = true;
        _txtWaitNetwork.color = new Color(1, 0, 0);
        _txtWaitNetwork.text  = "Wait Join Request";

        ServerModel.User user = new ServerModel.User() { name = _nickNameInputField.text };
        NetworkManager.it.Emit(ServerMethod.USER_CONNECT, user.ToJSON());
    }
}