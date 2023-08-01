using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public GameObject market_prefab;
    public GameObject market_parent;
    public GameObject popup;
    public GameObject adv_prefab;
    public GameObject adv_parent;
    public GameObject exit_popup;

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
        Global.is_loaded_adv = false;
        try
        {
            GameObject.Find("Canvas/top_background/title").GetComponent<Text>().text = Global.userinfo.pubs[Global.curPubIndex].name;
            GameObject.Find("Canvas/top_background/price").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].price) + "원";
            GameObject.Find("Canvas/top_background/completed_payment").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].paid_price);
            GameObject.Find("Canvas/top_background/pending_payment").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].pending_price);
            StartCoroutine(downloadImage(Global.userinfo.pubs[Global.curPubIndex].img_url, Global.imgPath + Path.GetFileName(Global.userinfo.pubs[Global.curPubIndex].img_url),
                GameObject.Find("Canvas/top_background/avatar").gameObject));
            StartCoroutine(LoadAdvertisement());
            StartCoroutine(LoadMarkets());
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    IEnumerator downloadImage(string url, string pathToSaveImage, GameObject imgObj)
    {
        yield return new WaitForSeconds(0.001f);
        url = Global.wine_server_url + url;
        Image img = imgObj.GetComponent<Image>();
        if (File.Exists(pathToSaveImage))
        {
            Debug.Log(pathToSaveImage + " exists");
            StartCoroutine(LoadPictureToTexture(pathToSaveImage, img));
        }
        else
        {
            Debug.Log(pathToSaveImage + " downloading--");
            WWW www = new WWW(url);
            StartCoroutine(_downloadImage(www, pathToSaveImage, img));
        }
    }

    IEnumerator LoadPictureToTexture(string name, Image img)
    {
        //Debug.Log("load image = " + Global.prePath + name);
        WWW pictureWWW = new WWW(Global.prePath + name);
        yield return pictureWWW;

        if (img != null)
        {
            img.sprite = Sprite.Create(pictureWWW.texture, new Rect(0, 0, pictureWWW.texture.width, pictureWWW.texture.height), new Vector2(0, 0), 8f, 0, SpriteMeshType.FullRect);
        }
    }

    private IEnumerator _downloadImage(WWW www, string savePath, Image img)
    {
        yield return www;
        //Check if we failed to send
        if (string.IsNullOrEmpty(www.error))
        {
            saveImage(savePath, www.bytes, img);
        }
        else
        {
            UnityEngine.Debug.Log("Error: " + www.error);
        }
    }

    void saveImage(string path, byte[] imageBytes, Image img)
    {
        try
        {
            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllBytes(path, imageBytes);
            //Debug.Log("Download Image: " + path.Replace("/", "\\"));
            StartCoroutine(LoadPictureToTexture(path, img));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (popup.activeSelf)
                {
                    popup.SetActive(false);
                }
                else
                {
                    exit_popup.SetActive(true);
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

    public void onStatus()
    {
        SceneManager.LoadScene("status");
    }

    public void onRemain()
    {
        SceneManager.LoadScene("remain");
    }

    public void onMonth()
    {
        SceneManager.LoadScene("month");
    }

    public void onDay()
    {
        SceneManager.LoadScene("day");
    }

    public void onMarket()
    {
        SceneManager.LoadScene("table");
    }

    public void onSetting()
    {
        SceneManager.LoadScene("setting");
    }

    public void onSelMarket()
    {
        popup.SetActive(true);
    }

    IEnumerator Destroy_Object(GameObject obj)
    {
        DestroyImmediate(obj);
        yield return null;
    }

    IEnumerator LoadMarkets()
    {
        while (market_parent.transform.childCount > 0)
        {
            StartCoroutine(Destroy_Object(market_parent.transform.GetChild(0).gameObject));
        }
        while (market_parent.transform.childCount > 0)
        {
            yield return new WaitForSeconds(0.01f);
        }
        GameObject[] m_marketItem = new GameObject[Global.userinfo.pubs.Count];
        for (int i = 0; i < m_marketItem.Length; i++)
        {
            m_marketItem[i] = Instantiate(market_prefab);
            m_marketItem[i].transform.SetParent(market_parent.transform);
            m_marketItem[i].transform.localScale = Vector3.one;
            m_marketItem[i].transform.Find("Toggle").Find("Label").GetComponent<Text>().text = Global.userinfo.pubs[i].name;
            m_marketItem[i].transform.Find("Toggle").GetComponent<Toggle>().group = market_parent.GetComponent<ToggleGroup>();
            if(i == Global.curPubIndex)
            {
                m_marketItem[i].transform.Find("Toggle").GetComponent<Toggle>().isOn = true;
            }
            m_marketItem[i].transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener((value) => {   // you are missing this
                    onSelPopup(value);       // this is just a basic method call within another method
                }   // and this one
            );   // closing the AddListener method parameter
        }
    }

    public void onSelPopup(bool value)
    {
        for(int i = 0; i < market_parent.transform.childCount; i ++)
        {
            if (market_parent.transform.GetChild(i).Find("Toggle").GetComponent<Toggle>().isOn)
            {
                try
                {
                    Global.curPubIndex = i;
                    GameObject.Find("Canvas/top_background/title").GetComponent<Text>().text = Global.userinfo.pubs[Global.curPubIndex].name;
                    GameObject.Find("Canvas/top_background/price").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].price) + "원";
                    GameObject.Find("Canvas/top_background/completed_payment").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].paid_price);
                    GameObject.Find("Canvas/top_background/pending_payment").GetComponent<Text>().text = Global.GetPriceFormat(Global.userinfo.pubs[Global.curPubIndex].pending_price);
                    StartCoroutine(downloadImage(Global.userinfo.pubs[Global.curPubIndex].img_url, Global.imgPath + Path.GetFileName(Global.userinfo.pubs[Global.curPubIndex].img_url),
                        GameObject.Find("Canvas/top_background/avatar").gameObject));
                }catch(Exception ex)
                {

                }
                popup.SetActive(false); 
            }
        }
    }

    IEnumerator LoadAdvertisement()
    {
        GameObject[] advObj = new GameObject[Global.adverList.Count];
        List<AdvertisementInfo> new_adv = new List<AdvertisementInfo>();
        while (new_adv.Count < Global.adverList.Count)
        {
            int ran = UnityEngine.Random.Range(-1, Global.adverList.Count);
            if (ran != -1 && ran != Global.adverList.Count && !new_adv.Contains(Global.adverList[ran]))
            {
                new_adv.Add(Global.adverList[ran]);
            }
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < new_adv.Count; i++)
        {
            advObj[i] = Instantiate(adv_prefab);
            advObj[i].transform.SetParent(adv_parent.transform);
            advObj[i].transform.localScale = Vector3.one;
            advObj[i].transform.localPosition = Vector3.zero;
            advObj[i].transform.GetComponent<RectTransform>().anchorMin = Vector3.zero;
            advObj[i].transform.GetComponent<RectTransform>().anchorMax = Vector3.one;
            float left = 0;
            float right = 0;
            advObj[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((left - right) / 2, 0f);
            advObj[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-(left + right), 0);
            StartCoroutine(downloadImage(new_adv[i].img_url, Global.imgPath + Path.GetFileName(new_adv[i].img_url), advObj[i]));
            advObj[i].transform.Find("id").GetComponent<Text>().text = new_adv[i].id.ToString();
            if (new_adv.Count == 1)
            {
                advObj[i].GetComponent<Image>().type = Image.Type.Simple;
            }
            else
            {
                advObj[i].GetComponent<Image>().type = Image.Type.Sliced;
            }
        }
        Global.is_loaded_adv = true;
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
