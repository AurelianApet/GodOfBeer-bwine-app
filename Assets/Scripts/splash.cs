using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class splash : MonoBehaviour
{
    // Start is called before the first frame update
    public float delay_time = 0.5f;

    IEnumerator Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.fullScreen = false;
#if UNITY_ANDROID
        Global.setStatusBarValue(2048); // WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN
#endif

#if UNITY_IPHONE
		Global.imgPath = Application.persistentDataPath + "/biz_img/";
#elif UNITY_ANDROID
        Global.imgPath = Application.persistentDataPath + "/biz_img/";
#else
if( Application.isEditor == true ){ 
    	Global.imgPath = "/img/";
} 
#endif

#if UNITY_IPHONE
		Global.prePath = @"file://";
#elif UNITY_ANDROID
        Global.prePath = @"file:///";
#else
		Global.prePath = @"file://" + Application.dataPath.Replace("/Assets","/");
#endif

        //delete all downloaded images
        try
        {
            if (Directory.Exists(Global.imgPath))
            {
                Directory.Delete(Global.imgPath, true);
            }
        }
        catch (Exception)
        {

        }


        if (PlayerPrefs.GetInt("idSave") == 1)
        {
            Global.is_id_saved = true;
            Global.userinfo.userID = PlayerPrefs.GetString("id");
        }
        else
        {
            Global.is_id_saved = false;
        }
        if(PlayerPrefs.GetInt("autoSave") == 1)
        {
            Debug.Log("auto save");
            Global.is_auto_login = true;
            Global.userinfo.userID = PlayerPrefs.GetString("id");
            Global.userinfo.password = PlayerPrefs.GetString("pwd");
            WWWForm form = new WWWForm();
            form.AddField("userID", Global.userinfo.userID);
            form.AddField("password", Global.userinfo.password);
            form.AddField("type", 1);
            WWW www = new WWW(Global.api_url + Global.login_api, form);
            StartCoroutine(ProcessLogin(www));
        }
        else
        {
            Global.is_auto_login = false;
            yield return new WaitForSeconds(delay_time);
            SceneManager.LoadScene("login");
        }
    }

    IEnumerator ProcessLogin(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            Debug.Log(jsonNode);
            string result = jsonNode["suc"].ToString();
            if (result == "1")
            {
                Global.userinfo.role = jsonNode["role"];
                Global.userinfo.pubs = new List<PubInfo>();
                Global.userinfo.storeName = jsonNode["storename"];
                JSONNode pubinfo = JSON.Parse(jsonNode["pubs"].ToString());
                for (int i = 0; i < pubinfo.Count; i++)
                {
                    PubInfo pinfo = new PubInfo();
                    pinfo.id = pubinfo[i]["id"];
                    pinfo.name = pubinfo[i]["name"];
                    pinfo.paid_price = pubinfo[i]["paid_price"].AsInt;
                    pinfo.paid_cnt = pubinfo[i]["paid_cnt"].AsInt;
                    pinfo.price = pubinfo[i]["price"].AsInt;
                    pinfo.total_cnt = pubinfo[i]["total_cnt"].AsInt;
                    pinfo.pending_price =pubinfo[i]["pending_price"].AsInt;
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
                    adInfo.url = advInfo[i]["url"];
                    adInfo.detail_img = advInfo[i]["detail_img"];
                    Global.adverList.Add(adInfo);
                }

                yield return new WaitForSeconds(delay_time);
                SceneManager.LoadScene("main");
            }
            else
            {
                yield return new WaitForSeconds(delay_time);
                SceneManager.LoadScene("login");
            }
        }
        else
        {
            yield return new WaitForSeconds(delay_time);
            SceneManager.LoadScene("login");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
