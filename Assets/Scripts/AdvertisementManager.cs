using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdvertisementManager : MonoBehaviour
{
    string img_url = "";
    string url = "";
    int id = -1;

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
        for (int i = 0; i < Global.adverList.Count; i++)
        {
            if (Global.adverList[i].id == Global.curSelAdvId)
            {
                id = Global.adverList[i].id;
                img_url = Global.adverList[i].detail_img;
                url = Global.adverList[i].url;
                break;
            }
        }
        StartCoroutine(downloadImage(img_url, Global.imgPath + Path.GetFileName(img_url), GameObject.Find("Canvas/background").gameObject));
        viewAdvertisement();
    }

    void viewAdvertisement()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        WWW www = new WWW(Global.api_url + Global.view_advertisemenet_api, form);
        StartCoroutine(ProcessViewAdvertisement(www));
    }

    IEnumerator ProcessViewAdvertisement(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
        }
        else
        {
        }
    }

    void viewAdvUrl()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        WWW www = new WWW(Global.api_url + Global.view_advUrl_api, form);
        StartCoroutine(ProcessViewAdvUrl(www));
    }

    IEnumerator ProcessViewAdvUrl(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
        }
        else
        {
        }
        Application.OpenURL(url);
    }

    IEnumerator downloadImage(string url, string pathToSaveImage, GameObject imgObj)
    {
        yield return new WaitForSeconds(0.001f);
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("main");
            }
        }
    }

    public void Viewmore()
    {
        viewAdvUrl();
    }

    public void onBack()
    {
        SceneManager.LoadScene("main");
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
