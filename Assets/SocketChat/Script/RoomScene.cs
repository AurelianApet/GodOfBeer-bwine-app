using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScene : MonoBehaviour
{
    [SerializeField]
    InputField _chatInput;

    [SerializeField]
    Text _txtChatBox;

    [SerializeField]
    Text _txtUserList;

    private List<ServerModel.Message> _messageList = new List<ServerModel.Message>();

    private void Start()
    {
        RefreshUserList();

        NetworkManager.it.AddEventCallback(ServerMethod.RECEIVE_MESSAGE,
            (data) =>
            {
                if (_messageList.Count > 10)
                {
                    _messageList.RemoveAt(0);
                }

                ServerModel.Message message = JsonUtility.FromJson<ServerModel.Message>(data);
                _messageList.Add(message);
                RefreshChatRoom();
            });

        NetworkManager.it.AddEventCallback(ServerMethod.OTHER_USER_CONNECT,
            (data) =>
            {
                var userDic = GeneralDataManager.it.userDictionary;
                var user = JsonUtility.FromJson<ServerModel.User>(data);
                if (!userDic.ContainsKey(user.name))
                {
                    userDic.Add(user.name, user);
                }
                RefreshUserList();
            });

        NetworkManager.it.AddEventCallback(ServerMethod.OTHER_USER_DISCONNECT,
            (data) =>
            {
                var userDic = GeneralDataManager.it.userDictionary;
                var user = JsonUtility.FromJson<ServerModel.User>(data);
                if (userDic.ContainsKey(user.name))
                {
                    userDic.Remove(user.name);
                }
                RefreshUserList();
            });
    }

    private void RefreshChatRoom()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var message in _messageList)
        {
            sb.Append("\n");
            sb.Append(message.from);
            sb.Append(":");
            sb.Append(message.to);
            sb.Append(":");
            sb.Append(message.message);
        }
        _txtChatBox.text = sb.ToString();
    }

    private void RefreshUserList()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var user in GeneralDataManager.it.userDictionary)
        {
            if (user.Key == GeneralDataManager.it.currentUser.name)
            {
                sb.Append(string.Format("<color=#00FFFF>{0}</color>", user.Value.name));
            }
            else
            {
                sb.Append(user.Value.name);
            }
            sb.Append("\n");
        }
        _txtUserList.text = sb.ToString();
    }

    private void SendMessage()
    {
        string txtMsg = _chatInput.text;
        _chatInput.text = string.Empty;

        ServerModel.Message message = new ServerModel.Message() { from = GeneralDataManager.it.currentUser.name, to = "", message = txtMsg };
        NetworkManager.it.Emit(ServerMethod.SEND_MESSAGE, message.ToJSON());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();

            // Focus Chat
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_chatInput.gameObject, null);
            _chatInput.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));
        }
    }
}
