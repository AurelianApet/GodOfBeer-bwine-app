using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using SimpleJSON;
using System;
using SocketIO;

public class SettingManager : MonoBehaviour
{
    public Toggle autoLogin;

    void Awake()
    {
        Screen.fullScreen = false;
        Screen.orientation = ScreenOrientation.Portrait;
#if UNITY_ANDROID
        Global.setStatusBarValue(2048); // WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            GameObject.Find("Canvas/range/account").GetComponent<Text>().text = Global.userinfo.storeName;
            if (Global.is_auto_login)
            {
                GameObject.Find("Canvas/range/Toggle").GetComponent<Toggle>().isOn = true;
            }
            else
            {
                GameObject.Find("Canvas/range/autologin").GetComponent<Toggle>().isOn = false;
            }
        }catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("main");
            }
        }
    }

    float time = 0f;

    void FixedUpdate()
    {
        //if (!Input.anyKey)
        //{
        //    time += Time.deltaTime;
        //}
        //else
        //{
        //    if (time != 0f)
        //    {
        //        GameObject.Find("touch").GetComponent<AudioSource>().Play();
        //        time = 0f;
        //    }
        //}
    }

    public void onBack()
    {
        Save();
        SceneManager.LoadScene("main");
    }

    void Save()
    {
        if (GameObject.Find("Canvas/range/Toggle").GetComponent<Toggle>().isOn)
        {
            Global.is_auto_login = true;
            PlayerPrefs.SetInt("autoSave", 1);
        }
        else
        {
            Global.is_auto_login = false;
            PlayerPrefs.SetInt("autoSave", 0);
        }
    }
    public void Logout()
    {
        Save();
        SceneManager.LoadScene("login");
    }
}
