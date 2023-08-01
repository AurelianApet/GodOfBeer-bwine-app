using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Linq;

public struct UserInfo
{
    public int id;
    public string userID;
    public string password;
    public int role;
    public string storeName;
    public List<PubInfo> pubs;
}

public struct PubInfo
{
    public int id;
    public string name;
    public string bus_id;
    public string sdate;
    public int price;
    public int total_cnt;
    public int paid_price;
    public int paid_cnt;
    public int pending_price;
    public int pending_cnt;
    public string img_url;
    //public RemainInfo rinfo;
}

public struct RemainInfo
{
    public string id;
    public string name;
    public int total_amount;
    public int remaining_amount;
    public float temperature;
    public int serial_number;
    public string last_update_datetime;
}

public struct DayInfo
{
    public int day;
    public int sum;
}

public struct CategoryInfo
{
    public string id;
    public string name;
    public List<MenuInfo> menulist;
}

public struct MenuInfo
{
    public string id;
    public string name;
    public int price;
    public bool is_soldout;
    public bool is_takeout;
}

public struct TableGroup
{
    public string id;
    public string name;
    public int is_pay_after;
    public List<TableInfo> tablelist;
}

public struct TableInfo
{
    public string id;
    public string name;
    public int is_pay_after;//1-후불, 0-선불
    public int order_price;
    public int order_amount;
    public int is_blank;//1-
    public List<TagInfo> taglist;
}
public struct TagInfo
{
    public string id;
    public string name;
    public int is_used;//1-»ç¿ëÁß
}

public struct CurTableInfo
{
    public int tgIndex;
    public int tIndex;
    public string tg_id;
    public string t_id;
    public string name;
    public int is_pay_after;//1-ÈÄºÒ, 0-¼±ºÒ
}

public struct MyOrderInfo
{
    public List<OrderCartInfo> ordercartinfo;
    public int total_price;
    public TableSelectedInformation tsInfo;
}

public struct OrderCartInfo
{
    public string id;
    public string tag_name;
    public string menu_id;
    public string name;
    public int price;
    public int amount;
    public string trno;
    public int status;//1: ÁÖ¹®, 2: Á¶¸®Áß, 3: Á¶¸®¿Ï·á, 0-ÁÖ¹®Àü
    public string order_time;//ÁÖ¹®½Ã°£
}

public struct TableSelectedInformation
{
    public string tg_id;
    public string t_id;
}

public struct AdvertisementInfo
{
    public int id;
    public string img_url;
    public string url;
    public string detail_img;
    public string name;
}

public class Global
{
    //setting information
    public static bool is_id_saved = false;
    public static bool is_auto_login = false;

    public static int newStatusBarValue;
    public static UserInfo userinfo = new UserInfo();
    public static int curPubIndex = 0;//role_pubmanager
    public static List<TableGroup> tableGroupList = new List<TableGroup>();
    public static CurTableInfo cur_tInfo = new CurTableInfo();
    public static List<MyOrderInfo> myorderlist = new List<MyOrderInfo>();
    public static List<AdvertisementInfo> adverList = new List<AdvertisementInfo>();
    public static int curSelAdvId = -1;
    public static bool is_loaded_adv = false;

    //image download path
    public static string imgPath = "";
    public static string prePath = "";

    //api
    //public static string server_address = "localhost";
    //public static string server_address = "192.168.151.202";
    public static string server_address = "114.203.87.215";
    
    static string api_server_domain = "http://" + server_address;
    static string api_server_port = "3006";
    static string wine_server_port = "3007";
    public static string api_url = api_server_domain + ":" + api_server_port + "/";
    public static string wine_server_url = api_server_domain + ":" + wine_server_port + "/";
    static string api_prefix = "m-api/manager/";
    public static string get_table_group_api = api_prefix + "get-tablegroup";
    public static string login_api = api_prefix + "login";
    public static string get_remain_api = api_prefix + "get-remain";
    public static string get_month_api = api_prefix + "get-month";
    public static string get_day_api = api_prefix + "get-day";
    public static string get_myorderlist_api = api_prefix + "get-myorderlist";
    public static string get_marketstatus_api = api_prefix + "get-market";
    public static string view_advertisemenet_api = api_prefix + "view-advertisement";
    public static string view_advUrl_api = api_prefix + "view-advUrl";

    //socket server
    public static string socket_server = "ws://" + server_address + ":" + api_server_port;

    public static string GetPriceFormat(float price)
    {
        return string.Format("{0:N0}", price);
    }

    public static string GetDateFormat(string str)
    {
        DateTime condate = Convert.ToDateTime(str);
        return condate.Year + "년" + condate.Month + '월' + condate.Day + '일';
    }

    public static string GetNoFormat(int ono)
    {
        return string.Format("{0:D2}", ono);
    }

    public static string GetOrderTimeFormat(string ordertime)
    {
        try
        {
            return string.Format("{0:D2}", Convert.ToDateTime(ordertime).Hour) + ":" + string.Format("{0:D2}", Convert.ToDateTime(ordertime).Minute);
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static string GetTemperature(float tem)
    {
        return string.Format("{0}°C", tem);
    }

    public static void setStatusBarValue(int value)
    {
        Global.newStatusBarValue = value;
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                try
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(setStatusBarValueInThread));
                }catch(Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }
    }

    private static void setStatusBarValueInThread()
    {
#if UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    window.Call("setFlags", Global.newStatusBarValue, -1);
                }
            }
        }
#endif
    }
}


