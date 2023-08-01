using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using System;

public class DayManager : MonoBehaviour
{
    public GameObject popup;
    public Text monthTxt;
    DateTime now_date;
    DateTime cur_sel_date;
    int now_free_days = 0;

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
        now_date = Convert.ToDateTime(Global.userinfo.pubs[Global.curPubIndex].sdate);
        DateTime now_first_day = new DateTime(now_date.Year, now_date.Month, 1);
        switch (now_first_day.DayOfWeek)
        {
            case DayOfWeek.Sunday: now_free_days = 0; break;
            case DayOfWeek.Monday: now_free_days = 1; break;
            case DayOfWeek.Tuesday: now_free_days = 2; break;
            case DayOfWeek.Wednesday: now_free_days = 3; break;
            case DayOfWeek.Thursday: now_free_days = 4; break;
            case DayOfWeek.Friday: now_free_days = 5; break;
            case DayOfWeek.Saturday: now_free_days = 6; break;
        }
        Debug.Log("now = " + now_date.DayOfWeek);
        cur_sel_date = now_date;
        LoadDays(cur_sel_date);
    }

    void LoadDays(DateTime cur_date)
    {
        GameObject.Find("Canvas/middle/day/Text").GetComponent<Text>().text = cur_date.Year.ToString() + "년 " + cur_date.Month.ToString() + "월 " + cur_date.Day.ToString() + "일";
        WWWForm form = new WWWForm();
        form.AddField("pub_id", Global.userinfo.pubs[Global.curPubIndex].id);
        form.AddField("day", cur_date.Year.ToString() + '-' + Global.GetNoFormat(cur_date.Month) + '-' + Global.GetNoFormat(cur_date.Day));
        WWW www = new WWW(Global.api_url + Global.get_day_api, form);
        StartCoroutine(GetDayInfo(www));
    }

    IEnumerator GetDayInfo(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            string result = jsonNode["suc"].ToString();
            if (result == "1")
            {
                GameObject.Find("Canvas/range/Viewport/Content/use/price1").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["sell_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/use/price2").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["discount_sum"].AsInt + jsonNode["point_sum"].AsInt);
                GameObject.Find("Canvas/range/Viewport/Content/use/price3").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["service_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/use/price4").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["de_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/use/price5").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["real_sell_sum"]);

                GameObject.Find("Canvas/range/Viewport/Content/pay/price1").GetComponent<Text>().text = jsonNode["paid_cnt"];
                GameObject.Find("Canvas/range/Viewport/Content/pay/price2").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["paid_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/pay/price3").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["card_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/pay/price4").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["money_sum"]);

                GameObject.Find("Canvas/range/Viewport/Content/prepay/price1").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["st_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/prepay/price2").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["prepaid_sum"]);
                GameObject.Find("Canvas/range/Viewport/Content/prepay/price3").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["use_price"]);
                GameObject.Find("Canvas/range/Viewport/Content/prepay/price4").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["del_price"]);
                GameObject.Find("Canvas/range/Viewport/Content/prepay/price5").GetComponent<Text>().text = Global.GetPriceFormat(jsonNode["last_price"]);
            }
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

    public void onBackBtn()
    {
        SceneManager.LoadScene("main");
    }

    void LoadDaysInMonth(DateTime cur_date)
    {
        Debug.Log(cur_date + " loads days");
        DateTime cur_first_date = new DateTime(cur_date.Year, cur_date.Month, 1);
        int freedays = 0;
        switch (cur_first_date.DayOfWeek)
        {
            case DayOfWeek.Sunday: freedays = 0; break;
            case DayOfWeek.Monday: freedays = 1; break;
            case DayOfWeek.Tuesday: freedays = 2; break;
            case DayOfWeek.Wednesday: freedays = 3; break;
            case DayOfWeek.Thursday: freedays = 4; break;
            case DayOfWeek.Friday: freedays = 5; break;
            case DayOfWeek.Saturday: freedays = 6; break;
        }
        int daysCnt = DateTime.DaysInMonth(cur_first_date.Year, cur_first_date.Month);
        for (int i = 0; i < 42; i++)
        {
            try
            {
                if ((i + 1 == cur_date.Day + now_free_days))
                {
                    popup.transform.Find("center/day").gameObject.transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("curdate");
                }
                else
                {
                    popup.transform.Find("center/day").gameObject.transform.GetChild(i).GetComponent<Image>().sprite = null;
                }
                if (i + 1 <= freedays)
                {
                    popup.transform.Find("center/day").gameObject.transform.GetChild(i).Find("day").GetComponent<Text>().text = "";
                }
                else if (i + 1 <= freedays + daysCnt)
                {
                    popup.transform.Find("center/day").gameObject.transform.GetChild(i).Find("day").GetComponent<Text>().text = (i - freedays + 1).ToString();
                }
                else
                {
                    popup.transform.Find("center/day").gameObject.transform.GetChild(i).Find("day").GetComponent<Text>().text = "";
                }
                int sel_date = i - freedays + 1;
                popup.transform.Find("center/day").gameObject.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate () { onSelDay(sel_date); });
            }
            catch (Exception ex)
            {
                Debug.Log(i + "-- error.");
            }
        }
    }

    public void onChangeDay()
    {
        popup.SetActive(true);
        monthTxt.text = string.Format("{0:D4}.{1:D2}", cur_sel_date.Year, cur_sel_date.Month);
        LoadDaysInMonth(cur_sel_date);
    }

    public void onSelDay(int day)
    {
        Debug.Log(day + "sel event.");
        if (day < 1 || day > 31)
            return;
        DateTime tmp_date = Convert.ToDateTime(popup.transform.Find("top/Month").GetComponent<Text>().text);
        DateTime new_date = new DateTime(tmp_date.Year, tmp_date.Month, day);
        Debug.Log(new_date + " selected.");
        popup.SetActive(false);
        cur_sel_date = new_date;
        LoadDays(cur_sel_date);
    }

    public void onPrevMonth()
    {
        DateTime curdatetime = Convert.ToDateTime(popup.transform.Find("top/Month").GetComponent<Text>().text);
        Debug.Log(curdatetime);
        int curYear = curdatetime.Year;
        int curMonth = curdatetime.Month;
        if (curMonth > 1)
        {
            curMonth--;
        }
        else
        {
            curYear--;
            curMonth = 12;
        }
        popup.transform.Find("top/Month").GetComponent<Text>().text = curYear.ToString() + "." + curMonth.ToString();
        cur_sel_date = new DateTime(curYear, curMonth, curdatetime.Day);
        LoadDaysInMonth(cur_sel_date);
    }

    public void onNextMonth()
    {
        DateTime curdatetime = Convert.ToDateTime(popup.transform.Find("top/Month").GetComponent<Text>().text);
        Debug.Log(curdatetime);
        int curYear = curdatetime.Year;
        int curMonth = curdatetime.Month;
        if (curMonth < 12)
        {
            curMonth++;
        }
        else
        {
            curYear++;
            curMonth = 1;
        }
        popup.transform.Find("top/Month").GetComponent<Text>().text = curYear.ToString() + "." + curMonth.ToString();
        cur_sel_date = new DateTime(curYear, curMonth, curdatetime.Day);
        LoadDaysInMonth(cur_sel_date);
    }
}
