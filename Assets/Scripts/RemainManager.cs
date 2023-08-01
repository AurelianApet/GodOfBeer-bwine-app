using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System;

public class RemainManager : MonoBehaviour
{
    public GameObject item;
    public GameObject itemParent;

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
        WWW www = new WWW(Global.api_url + Global.get_remain_api, form);
        StartCoroutine(GetRemain(www));
    }

    IEnumerator GetRemain(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            string result = jsonNode["suc"].ToString();
            if (result == "1")
            {
                JSONNode remainlist = JSON.Parse(jsonNode["remainlist"].ToString());
                GameObject[] remainItem = new GameObject[remainlist.Count];
                for(int i = 0; i < remainlist.Count; i++)
                {
                    RemainInfo remainInfo = new RemainInfo();
                    remainInfo.id = remainlist[i]["id"];
                    remainInfo.name = remainlist[i]["name"];
                    remainInfo.total_amount = remainlist[i]["total_amount"].AsInt;
                    remainInfo.remaining_amount = remainlist[i]["remaining_amount"].AsInt;
                    remainInfo.temperature = remainlist[i]["temperature"].AsFloat;
                    remainInfo.last_update_datetime = remainlist[i]["last_update_datetime"];
                    remainInfo.serial_number = remainlist[i]["serial_number"];

                    remainItem[i] = Instantiate(item);
                    remainItem[i].transform.SetParent(itemParent.transform);
                    remainItem[i].transform.Find("content/name").GetComponent<Text>().text = remainInfo.serial_number + " " + remainInfo.name;
                    remainItem[i].transform.Find("content/remain").GetComponent<Text>().text = Global.GetPriceFormat(remainInfo.remaining_amount) + " ml";
                    DateTime lasttime = Convert.ToDateTime(remainInfo.last_update_datetime);
                    double difftime = (DateTime.Now - lasttime).TotalSeconds;
                    int diff_day = Convert.ToInt32(Math.Floor(difftime / 3600 / 24));
                    int rtime = Convert.ToInt32(difftime - diff_day * 3600 * 24);
                    int diff_hour = Convert.ToInt32(Math.Floor(rtime / 3600d));
                    rtime -= diff_hour * 3600;
                    int diff_min = Convert.ToInt32(Math.Floor(rtime / 60d));
                    remainItem[i].transform.Find("content/time").GetComponent<Text>().text =
                        diff_day + "일 " + Global.GetNoFormat(diff_hour) + ":" + Global.GetNoFormat(diff_min);
                    remainItem[i].transform.Find("content/value").GetComponent<Slider>().value = diff_day / 30f;
                    if(diff_day < 10)
                    {
                        remainItem[i].transform.Find("content/value/Fill Area/Fill").GetComponent<Image>().color = Color.green;
                    }else if(diff_day < 20)
                    {
                        remainItem[i].transform.Find("content/value/Fill Area/Fill").GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        remainItem[i].transform.Find("content/value/Fill Area/Fill").GetComponent<Image>().color = Color.red;
                    }
                    remainItem[i].transform.Find("box/temperature").GetComponent<Text>().text = Global.GetTemperature(remainInfo.temperature);
                    remainItem[i].transform.Find("box/size").GetComponent<Text>().text = Global.GetPriceFormat(remainInfo.total_amount) + "ml";
                    float ratio =  remainInfo.remaining_amount * 1.0f / remainInfo.total_amount * 1.0f;
                    remainItem[i].transform.Find("box/Slider").GetComponent<Slider>().value = ratio;
                    remainItem[i].transform.localScale = Vector3.one;
                    remainItem[i].transform.localPosition = Vector3.zero;
                }
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

    public void onBackBtn()
    {
        SceneManager.LoadScene("main");
    }
}
