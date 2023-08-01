using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField userId;
    public InputField password;
    public Toggle idSave;
    public Toggle autoSave;
    public GameObject err_popup;
    public Text err_str;
    public GameObject exit_popup;

    // Start is called before the first frame update
    void Start()
    {
        if (Global.is_id_saved)
        {
            userId.text = Global.userinfo.userID;
            idSave.isOn = true;
        }
        if (Global.is_auto_login)
        {
            autoSave.isOn = true;
        }
    }

    void Awake()
    {
        Screen.fullScreen = false;
        Screen.orientation = ScreenOrientation.Portrait;
#if UNITY_ANDROID
        Global.setStatusBarValue(2048); // WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN
#endif
    }
    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                exit_popup.SetActive(true);
            }
        }
    }

    public void Login()
    {
        if (userId.text == "")
        {
            err_str.text = "Username을 입력하세요.";
            err_popup.SetActive(true);
        } else if (password.text == "")
        {
            err_str.text = "비밀번호를 입력하세요.";
            err_popup.SetActive(true);
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("userID", userId.text);
            form.AddField("password", password.text);
            form.AddField("type", 1);
            WWW www = new WWW(Global.api_url + Global.login_api, form);
            StartCoroutine(ProcessLogin(www, idSave.isOn, userId.text, autoSave.isOn, password.text));
        }
    }

    IEnumerator ProcessLogin(WWW www, bool is_idsave, string username, bool is_autosave, string password)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            string result = jsonNode["suc"].ToString();
            if (result == "1")
            {
                if (is_idsave)
                {
                    PlayerPrefs.SetInt("idSave", 1);
                    PlayerPrefs.SetString("id", username);
                    Global.is_id_saved = true;
                }
                else
                {
                    PlayerPrefs.SetInt("idSave", 0);
                    Global.is_id_saved = false;
                }
                if (is_autosave)
                {
                    Debug.Log("autosave");
                    PlayerPrefs.SetInt("autoSave", 1);
                    PlayerPrefs.SetString("id", username);
                    PlayerPrefs.SetString("pwd", password);
                    Global.is_auto_login = true;
                }
                else
                {
                    PlayerPrefs.SetInt("autoSave", 0);
                    Global.is_auto_login = false;
                }
                Global.userinfo.userID = username;
                Global.userinfo.password = password;
                Global.userinfo.role = jsonNode["role"];
                Global.userinfo.pubs = new List<PubInfo>();
                Global.userinfo.storeName = jsonNode["storename"];
                JSONNode pubinfo = JSON.Parse(jsonNode["pubs"].ToString());
                for(int i = 0; i < pubinfo.Count; i++)
                {
                    PubInfo pinfo = new PubInfo();
                    pinfo.id = pubinfo[i]["id"];
                    pinfo.name = pubinfo[i]["name"];
                    pinfo.paid_price = pubinfo[i]["paid_price"].AsInt;
                    pinfo.paid_cnt = pubinfo[i]["paid_cnt"].AsInt;
                    pinfo.price = pubinfo[i]["price"].AsInt;
                    pinfo.total_cnt = pubinfo[i]["total_cnt"].AsInt;
                    pinfo.pending_price = pubinfo[i]["pending_price"].AsInt;
                    pinfo.pending_cnt = pubinfo[i]["pending_cnt"].AsInt;
                    pinfo.img_url = pubinfo[i]["image"];
                    pinfo.bus_id = pubinfo[i]["bus_id"];
                    pinfo.sdate = pubinfo[i]["sdate"];
                    Global.userinfo.pubs.Add(pinfo);
                }

                JSONNode advInfo = JSON.Parse(jsonNode["advertisement"].ToString());
                for (int i = 0; i < advInfo.Count; i++)
                {
                    AdvertisementInfo adInfo = new AdvertisementInfo();
                    adInfo.id = advInfo[i]["id"].AsInt;
                    adInfo.name = advInfo[i]["name"];
                    adInfo.img_url = advInfo[i]["img_url"];
                    adInfo.detail_img = advInfo[i]["detail_img"];
                    adInfo.url = advInfo[i]["url"];
                    Global.adverList.Add(adInfo);
                }

                SceneManager.LoadScene("main");
            }
            else
            {
                err_str.text = jsonNode["msg"];
                err_popup.SetActive(true);
            }
        }
        else
        {
            err_str.text = "로그인 조작시 알지 못할 오류가 발생하였습니다.";
            Debug.Log(www.error);
            err_popup.SetActive(true);
        }
    }

    public void onPopup()
    {
        err_popup.SetActive(false);
    }

    public void onCancelExit()
    {
        exit_popup.SetActive(false);
    }

    public void onConfirmExit()
    {
        Application.Quit();
    }
}
