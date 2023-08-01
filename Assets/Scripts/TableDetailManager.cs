using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using SocketIO;

public class TableDetailManager : MonoBehaviour
{
    public GameObject order_item;
    public GameObject order_parent;
    public Text total_priceTxt;
    public GameObject socketPrefab;
    GameObject socketObj;
    SocketIOComponent socket;

    GameObject[] m_orderList;
    float time = 0f;
    float total_price = 0f;

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
        GameObject.Find("Canvas/top/title").GetComponent<Text>().text = Global.cur_tInfo.name;
        WWWForm form = new WWWForm();
        form.AddField("sdate", Global.userinfo.pubs[Global.curPubIndex].sdate);
        form.AddField("pub_id", Global.userinfo.pubs[Global.curPubIndex].id);
        form.AddField("table_id", Global.cur_tInfo.t_id);
        WWW www = new WWW(Global.api_url + Global.get_myorderlist_api, form);
        StartCoroutine(GetMyorderlistFromApi(www));
    }
    
    IEnumerator Destroy_Object(GameObject obj)
    {
        DestroyImmediate(obj);
        yield return null;
    }

    IEnumerator GetMyorderlistFromApi(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            JSONNode molist = JSON.Parse(jsonNode["myorderlist"].ToString()/*.Replace("\"", "")*/);
            
            //UI 내역 초기화
            while (order_parent.transform.childCount > 0)
            {
                StartCoroutine(Destroy_Object(order_parent.transform.GetChild(0).gameObject));
                //yield return new WaitForSeconds(0.01f);
                //DestroyImmediate(table_parent.transform.GetChild(0).gameObject);
            }
            while (order_parent.transform.childCount > 0)
            {
                yield return new WaitForSeconds(0.01f);
            }
            Global.myorderlist.Clear();

            int totalCnt = 0;
            for (int i = 0; i < molist.Count; i++)
            {
                MyOrderInfo minfo = new MyOrderInfo();
                minfo.total_price = molist[i]["total_price"];
                total_price += molist[i]["total_price"];
                JSONNode menulist = JSON.Parse(molist[i]["menulist"].ToString());
                List<OrderCartInfo> ocinfolist = new List<OrderCartInfo>();
                for (int j = 0; j < menulist.Count; j++)
                {
                    OrderCartInfo ocinfo = new OrderCartInfo();
                    ocinfo.amount = menulist[j]["quantity"];
                    ocinfo.price = menulist[j]["price"].AsInt;
                    ocinfo.menu_id = menulist[j]["menu_id"];
                    ocinfo.trno = menulist[j]["trno"];
                    ocinfo.name = menulist[j]["name"];
                    ocinfo.tag_name = menulist[j]["tag_name"];
                    ocinfo.order_time = menulist[j]["reg_datetime"];
                    ocinfo.status = menulist[j]["status"].AsInt;
                    ocinfo.id = menulist[j]["order_id"];
                    ocinfolist.Add(ocinfo);
                    totalCnt++;
                }
                minfo.ordercartinfo = ocinfolist;
                Global.myorderlist.Add(minfo);
            }

            m_orderList = new GameObject[totalCnt];
            totalCnt = 0;
            for (int i = 0; i < Global.myorderlist.Count; i++)
            {
                for (int j = 0; j < Global.myorderlist[i].ordercartinfo.Count; j++)
                {
                    //UI
                    m_orderList[totalCnt] = Instantiate(order_item);
                    m_orderList[totalCnt].transform.SetParent(order_parent.transform);
                    m_orderList[totalCnt].transform.GetComponent<RectTransform>().anchorMin = Vector3.zero;
                    m_orderList[totalCnt].transform.GetComponent<RectTransform>().anchorMax = Vector3.one;
                    float left = 0;
                    float right = 0;
                    m_orderList[totalCnt].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((left - right) / 2, 0f);
                    m_orderList[totalCnt].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-(left + right), 0);
                    m_orderList[totalCnt].transform.localScale = Vector3.one;

                    try
                    {
                        m_orderList[totalCnt].transform.Find("id").GetComponent<Text>().text = Global.myorderlist[i].ordercartinfo[j].id.ToString();
                        m_orderList[totalCnt].transform.Find("no").GetComponent<Text>().text = Global.myorderlist[i].ordercartinfo[j].menu_id.ToString();
                        m_orderList[totalCnt].transform.Find("name").GetComponent<Text>().text = Global.myorderlist[i].ordercartinfo[j].name;
                        m_orderList[totalCnt].transform.Find("amount").GetComponent<Text>().text = Global.myorderlist[i].ordercartinfo[j].amount.ToString();
                        m_orderList[totalCnt].transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(Global.myorderlist[i].ordercartinfo[j].price);
                        m_orderList[totalCnt].transform.Find("time").GetComponent<Text>().text = Global.GetOrderTimeFormat(Global.myorderlist[i].ordercartinfo[j].order_time);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                    totalCnt++;
                }
            }
            total_priceTxt.text = Global.GetPriceFormat(total_price);
        }
    }

    public void onBack()
    {
        SceneManager.LoadScene("table");
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("table");
            }
        }
    }

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
