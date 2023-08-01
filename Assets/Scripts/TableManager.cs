using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;

public class TableManager : MonoBehaviour
{
    public GameObject table_group_Item;
    public GameObject table_group_parent;
    public GameObject table_item;
    public GameObject table_parent;
    public GameObject socketPrefab;
    GameObject socketObj;
    SocketIOComponent socket;

    GameObject[] m_tableGroupItem;
    GameObject[] m_tableItem;
    int total_table_group_cnt = -1;
    string first_table_group = "";
    string old_tg_no = "";
    bool loading = false;

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
        WWW www = new WWW(Global.api_url + Global.get_table_group_api, form);
        StartCoroutine(GetTableGrouplist(www));
    }

    IEnumerator GetTableGrouplist(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            Debug.Log("get table group list from api");
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            Debug.Log(jsonNode);
            Global.tableGroupList = new List<TableGroup>();
            JSONNode tg_list = JSON.Parse(jsonNode["tablegrouplist"].ToString()/*.Replace("\"", "")*/);
            total_table_group_cnt = tg_list.Count;
            if (total_table_group_cnt > 0)
            {
                try
                {
                    first_table_group = tg_list[0]["id"];
                }
                catch (Exception ex)
                {

                }
            }
            for (int i = 0; i < total_table_group_cnt; i++)
            {
                Debug.Log("loading table group list..");
                TableGroup tgInfo = new TableGroup();
                try
                {
                    tgInfo.id = tg_list[i]["id"];
                    tgInfo.name = tg_list[i]["name"];
                    tgInfo.is_pay_after = tg_list[i]["is_pay_after"].AsInt;
                    tgInfo.tablelist = new List<TableInfo>();
                }
                catch (Exception ex)
                {
                    Debug.Log("exception");
                }
                JSONNode table_list = JSON.Parse(tg_list[i]["tablelist"].ToString());
                int tableCnt = table_list.Count;
                for (int j = 0; j < tableCnt; j++)
                {
                    TableInfo tinfo = new TableInfo();
                    try
                    {
                        tinfo.id = table_list[j]["id"];
                        tinfo.name = table_list[j]["name"];
                        tinfo.is_pay_after = table_list[j]["is_pay_after"].AsInt;
                        tinfo.order_price = table_list[j]["order_price"].AsInt;
                        tinfo.order_amount = table_list[j]["order_amount"].AsInt;
                        tinfo.taglist = new List<TagInfo>();
                        JSONNode tag_list = JSON.Parse(table_list[j]["taglist"].ToString());
                        int tagCnt = tag_list.Count;
                        for (int k = 0; k < tagCnt; k++)
                        {
                            TagInfo taginfo = new TagInfo();
                            taginfo.id = tag_list[k]["id"];
                            taginfo.name = tag_list[k]["name"];
                            taginfo.is_used = tag_list[k]["is_used"].AsInt;
                            tinfo.taglist.Add(taginfo);
                        }

                        tinfo.is_blank = table_list[j]["is_blank"].AsInt;
                        Debug.Log("price :" + tinfo.is_blank);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }
                    tgInfo.tablelist.Add(tinfo);
                }
                Global.tableGroupList.Add(tgInfo);
            }
            //UI에 추가
            m_tableGroupItem = new GameObject[total_table_group_cnt];
            for (int i = 0; i < total_table_group_cnt; i++)
            {
                m_tableGroupItem[i] = Instantiate(table_group_Item);
                m_tableGroupItem[i].transform.SetParent(table_group_parent.transform);

                m_tableGroupItem[i].transform.GetComponent<RectTransform>().anchorMin = Vector3.zero;
                m_tableGroupItem[i].transform.GetComponent<RectTransform>().anchorMax = Vector3.one;
                float left = 0;
                float right = 0;
                m_tableGroupItem[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((left - right) / 2, 0f);
                m_tableGroupItem[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-(left + right), 0);
                m_tableGroupItem[i].transform.localScale = Vector3.one;

                try
                {
                    m_tableGroupItem[i].transform.Find("name").GetComponent<Text>().text = Global.tableGroupList[i].name;
                    m_tableGroupItem[i].transform.Find("id").GetComponent<Text>().text = Global.tableGroupList[i].id.ToString();
                    string tg_id = Global.tableGroupList[i].id;
                    m_tableGroupItem[i].GetComponent<Button>().onClick.AddListener(delegate () { StartCoroutine(LoadTableList(tg_id)); });
                }
                catch (Exception ex)
                {

                }
            }

            Debug.Log(first_table_group);
            if (!loading && total_table_group_cnt > 0 && first_table_group != "")
                StartCoroutine(LoadTableList(first_table_group));
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    IEnumerator Destroy_Object(GameObject obj)
    {
        DestroyImmediate(obj);
        yield return null;
    }

    IEnumerator LoadTableList(string id)
    {
        Debug.Log("load table from " + id);
        //UI 내역 초기화
        while (table_parent.transform.childCount > 0)
        {
            StartCoroutine(Destroy_Object(table_parent.transform.GetChild(0).gameObject));
            //yield return new WaitForSeconds(0.01f);
            //DestroyImmediate(table_parent.transform.GetChild(0).gameObject);
        }
        while (table_parent.transform.childCount > 0)
        {
            yield return new WaitForSeconds(0.01f);
        }
        try
        {
            for (int i = 0; i < table_group_parent.transform.childCount; i++)
            {
                if(table_group_parent.transform.GetChild(i).Find("id").GetComponent<Text>().text == old_tg_no.ToString())
                {
                    table_group_parent.transform.GetChild(i).transform.Find("name").GetComponent<Text>().fontStyle = FontStyle.Normal;
                }
                if (table_group_parent.transform.GetChild(i).Find("id").GetComponent<Text>().text == id.ToString())
                {
                    table_group_parent.transform.GetChild(i).transform.Find("name").GetComponent<Text>().fontStyle = FontStyle.Bold;
                }
            }
        }
        catch (Exception ex)
        {

        }

        old_tg_no = id;
        List<TableInfo> tbList = new List<TableInfo>();
        for (int i = 0; i < Global.tableGroupList.Count; i++)
        {
            if (Global.tableGroupList[i].id == id)
            {
                tbList = Global.tableGroupList[i].tablelist; break;
            }
        }
        //UI에 로딩
        int tbCnt = tbList.Count;
        m_tableItem = new GameObject[tbCnt];
        loading = true;
        for (int i = 0; i < tbCnt; i++)
        {
            m_tableItem[i] = Instantiate(table_item);
            m_tableItem[i].transform.SetParent(table_parent.transform);
            m_tableItem[i].transform.GetComponent<RectTransform>().anchorMin = Vector3.zero;
            m_tableItem[i].transform.GetComponent<RectTransform>().anchorMax = Vector3.one;
            float left = 0;
            float right = 0;
            m_tableItem[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((left - right) / 2, 0f);
            m_tableItem[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-(left + right), 0);
            m_tableItem[i].transform.localScale = Vector3.one;
            try
            {
                m_tableItem[i].transform.Find("id").GetComponent<Text>().text = tbList[i].id.ToString();
                m_tableItem[i].transform.Find("name").GetComponent<Text>().text = tbList[i].name;
                if (tbList[i].is_blank == 0)
                {
                    m_tableItem[i].transform.Find("name").GetComponent<Text>().fontStyle = FontStyle.Bold;
                    m_tableItem[i].transform.Find("name").GetComponent<Text>().color = Color.black;
                    if (tbList[i].is_pay_after == 1 && tbList[i].order_price > 0f)
                    {
                        m_tableItem[i].transform.Find("price").GetComponent<Text>().text = Global.GetPriceFormat(tbList[i].order_price);
                        m_tableItem[i].transform.Find("price").GetComponent<Text>().fontStyle = FontStyle.Bold;
                        m_tableItem[i].transform.Find("price").GetComponent<Text>().color = Color.black;
                    }
                    else
                    {
                        m_tableItem[i].transform.Find("price").GetComponent<Text>().color = Color.grey;
                    }
                }
                else
                {
                    m_tableItem[i].transform.Find("name").GetComponent<Text>().color = Color.grey;
                }
                string tid = tbList[i].id;
                m_tableItem[i].GetComponent<Button>().onClick.AddListener(delegate () { onTable(id, tid); });
            }
            catch (Exception ex)
            {
                Debug.Log(ex);

            }
            yield return new WaitForSeconds(0.001f);
        }
        loading = false;
    }

    void onTable(string id, string tid)
    {
        Global.cur_tInfo.tg_id = id;
        Global.cur_tInfo.t_id = tid;
        for (int i = 0; i < Global.tableGroupList.Count; i++)
        {
            if (Global.tableGroupList[i].id == id)
            {
                Global.cur_tInfo.tgIndex = i;
                for (int j = 0; j < Global.tableGroupList[i].tablelist.Count; j++)
                {
                    if (Global.tableGroupList[i].tablelist[j].id == tid)
                    {
                        Global.cur_tInfo.tIndex = j;
                        Global.cur_tInfo.name = Global.tableGroupList[i].tablelist[j].name;
                        Global.cur_tInfo.is_pay_after = Global.tableGroupList[i].tablelist[j].is_pay_after;
                        break;
                    }
                }
                break;
            }
        }
        Debug.Log(Global.cur_tInfo.name + " clicked.");
        SceneManager.LoadScene("table_detail");
    }

    public void onBack()
    {
        SceneManager.LoadScene("main");
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
}
