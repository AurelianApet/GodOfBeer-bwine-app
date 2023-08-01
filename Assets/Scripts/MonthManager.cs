using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System;

public class MonthManager : MonoBehaviour
{
    //public GameObject dayItemPrefab;
    //public GameObject dayItemParent;
    int Year;
    int Month;
    int Day;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.fullScreen = false;
#if UNITY_ANDROID
        Global.setStatusBarValue(2048); // WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        DateTime cur_date = Convert.ToDateTime(Global.userinfo.pubs[Global.curPubIndex].sdate);
        Year = cur_date.Year;
        Month = cur_date.Month;
        Day = cur_date.Day;
        GameObject.Find("Canvas/middle/month/Text").GetComponent<Text>().text = string.Format("{0:D4}.{1:D2}", Year, Month);
        LoadDays();
    }

    void LoadDays()
    {
        WWWForm form = new WWWForm();
        form.AddField("pub_id", Global.userinfo.pubs[Global.curPubIndex].id);
        form.AddField("month", string.Format("{0:D4}-{1:D2}", Year, Month));
        WWW www = new WWW(Global.api_url + Global.get_month_api, form);
        StartCoroutine(GetMonthInfo(www));
    }

    IEnumerator GetMonthInfo(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            string result = jsonNode["suc"].ToString();
            if (result == "1")
            {
                GameObject.Find("Canvas/middle/sum/total_price").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["sum"]);
                GameObject.Find("Canvas/middle/sum/card_price").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["card_sum"]);
                GameObject.Find("Canvas/middle/sum/money_price").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["money_sum"]);
                JSONNode dayinfo = JSON.Parse(jsonNode["dayInfo"].ToString());
                List<DayInfo> mInfo = new List<DayInfo>();
                for(int i = 0; i < dayinfo.Count; i ++)
                {
                    DayInfo d = new DayInfo();
                    d.day = dayinfo[i]["day"].AsInt;
                    d.sum = dayinfo[i]["day_sum"].AsInt;
                    mInfo.Add(d);
                }
                //while (dayItemParent.transform.childCount > 0)
                //{
                //    StartCoroutine(Destroy_Object(dayItemParent.transform.GetChild(0).gameObject));
                //    //yield return new WaitForSeconds(0.01f);
                //    //DestroyImmediate(table_parent.transform.GetChild(0).gameObject);
                //}
                //while (dayItemParent.transform.childCount > 0)
                //{
                //    yield return new WaitForSeconds(0.01f);
                //}
                int freedays = 0;
                DateTime cur_date = new DateTime(Year, Month, 1);
                switch (cur_date.DayOfWeek)
                {
                    case DayOfWeek.Sunday: freedays = 0; break;
                    case DayOfWeek.Monday: freedays = 1; break;
                    case DayOfWeek.Tuesday: freedays = 2; break;
                    case DayOfWeek.Wednesday: freedays = 3; break;
                    case DayOfWeek.Thursday: freedays = 4; break;
                    case DayOfWeek.Friday: freedays = 5; break;
                    case DayOfWeek.Saturday: freedays = 6; break;
                }
                int daysCnt = DateTime.DaysInMonth(cur_date.Year, cur_date.Month);
                //GameObject[] m_DayItem = new GameObject[daysCnt + freedays];
                //for (int i = 0; i < daysCnt + freedays; i++)
                //{
                //    m_DayItem[i] = Instantiate(dayItemPrefab);
                //    m_DayItem[i].transform.SetParent(dayItemParent.transform);
                //    m_DayItem[i].transform.GetComponent<RectTransform>().anchorMin = Vector3.zero;
                //    m_DayItem[i].transform.GetComponent<RectTransform>().anchorMax = Vector3.one;
                //    float left = 0;
                //    float right = 0;
                //    m_DayItem[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((left - right) / 2, 0f);
                //    m_DayItem[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-(left + right), 0);
                //    m_DayItem[i].transform.localScale = Vector3.one;
                //    if(i >= freedays)
                //    {
                //        m_DayItem[i].transform.Find("day").GetComponent<Text>().text = (i - freedays + 1).ToString();
                //        m_DayItem[i].transform.Find("amount").GetComponent<Text>().text = ((i - freedays + 1) * 1000).ToString();
                //    }
                //}
                for (int i = 1; i <= 42; i++)
                {
                    if (i <= freedays)
                    {
                        GameObject.Find("Canvas/range/days").gameObject.transform.GetChild(i).Find("day").GetComponent<Text>().text = "";
                        GameObject.Find("Canvas/range/days").gameObject.transform.GetChild(i).Find("amount").GetComponent<Text>().text = "";
                    }
                    else if (i <= freedays + daysCnt)
                    {
                        for(int j = 0; j < mInfo.Count; j++)
                        {
                            if(mInfo[j].day == i - freedays)
                            {
                                GameObject.Find("Canvas/range/days").gameObject.transform.GetChild(i).Find("amount").GetComponent<Text>().text = Global.GetPriceFormat(mInfo[j].sum);
                            }
                        }
                        GameObject.Find("Canvas/range/days").gameObject.transform.GetChild(i).Find("day").GetComponent<Text>().text = (i - freedays).ToString();
                    }
                    else
                    {
                        GameObject.Find("Canvas/range/days").gameObject.transform.GetChild(i).Find("day").GetComponent<Text>().text = "";
                        GameObject.Find("Canvas/range/days").gameObject.transform.GetChild(i).Find("amount").GetComponent<Text>().text = "";
                    }
                }
            }
        }
    }

    IEnumerator Destroy_Object(GameObject obj)
    {
        DestroyImmediate(obj);
        yield return null;
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

    public void onPrev()
    {
        if (Month > 1)
        {
            Month--;
        }
        else
        {
            Year--;
            Month = 12;
        }
        GameObject.Find("Canvas/middle/month/Text").GetComponent<Text>().text = string.Format("{0:D4}.{1:D2}", Year, Month);
        LoadDays();
    }

    public void onNext()
    {
        DateTime curDate = new DateTime(Year, Month, 1).AddMonths(1);
        Year = curDate.Year;
        Month = curDate.Month;
        GameObject.Find("Canvas/middle/month/Text").GetComponent<Text>().text = string.Format("{0:D4}.{1:D2}", Year, Month);
        LoadDays();
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

}
