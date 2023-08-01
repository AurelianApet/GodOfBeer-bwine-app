using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System;

public class StatusManager : MonoBehaviour
{
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
        WWWForm form = new WWWForm();
        form.AddField("pub_id", Global.userinfo.pubs[Global.curPubIndex].id);
        form.AddField("sdate", Global.userinfo.pubs[Global.curPubIndex].sdate);
        WWW www = new WWW(Global.api_url + Global.get_marketstatus_api, form);
        StartCoroutine(GetMarketStatus(www));
    }

    IEnumerator GetMarketStatus(WWW www)
    {
        yield return www;
        if(www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            string result = jsonNode["suc"].ToString();
            if (result == "1")
            {
        try
        {
                    PubInfo pubInfo = Global.userinfo.pubs[Global.curPubIndex];
                    pubInfo.paid_cnt = jsonNode["paid_cnt"].AsInt;
                    pubInfo.pending_cnt = jsonNode["pending_cnt"].AsInt;
                    pubInfo.paid_price = jsonNode["paid_price"].AsInt;
                    pubInfo.pending_price = jsonNode["pending_price"].AsInt;
                    pubInfo.price = jsonNode["price"].AsInt;
                    Global.userinfo.pubs[Global.curPubIndex] = pubInfo;
                    GameObject.Find("Canvas/range/day").GetComponent<Text>().text = Global.GetDateFormat(Global.userinfo.pubs[Global.curPubIndex].sdate);
            GameObject.Find("Canvas/range/paid_amount").GetComponent<Text>().text = Global.userinfo.pubs[Global.curPubIndex].paid_cnt.ToString();
            GameObject.Find("Canvas/range/not_paid_table").GetComponent<Text>().text = Global.userinfo.pubs[Global.curPubIndex].pending_cnt.ToString();
            GameObject.Find("Canvas/range/paid_price").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].paid_price);
            GameObject.Find("Canvas/range/peing_price").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].pending_price);
            GameObject.Find("Canvas/range/sum").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].price);
                }
                catch (Exception ex)
        {
            Debug.Log(ex);
        }

            }
            else
            {
            }
        }
        else
        {
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
        SceneManager.LoadScene("main");
    }
}
