using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TBEasyWebCam;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class QRManager : MonoBehaviour
{
    public QRCodeDecodeController e_qrController;
    public GameObject resetBtn;
    public GameObject scanLineObj;
    public GameObject err_popup;
    public Text err_str;
    public GameObject popup;
    public Text popup_str;
    public Text title;

    /// <summary>
    /// when you set the var is true,if the result of the decode is web url,it will open with browser.
    /// </summary>
    private void Start()
    {
        Screen.fullScreen = false;
        if (this.e_qrController != null)
        {
            this.e_qrController.onQRScanFinished += new QRCodeDecodeController.QRScanFinished(this.qrScanFinished);
        }
    }

    private void Update()
    {
    }

    public void onGoBack()
    {
    }

    private void qrScanFinished(string dataText)
    {
        if (this.resetBtn != null)
        {
            this.resetBtn.SetActive(true);
        }
        if (this.scanLineObj != null)
        {
            this.scanLineObj.SetActive(false);
        }
    }

    public void Reset()
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.Reset();
        }
        if (this.resetBtn != null)
        {
            this.resetBtn.SetActive(false);
        }
        if (this.scanLineObj != null)
        {
            this.scanLineObj.SetActive(true);
        }
    }

    public void Play()
    {
        Reset();
        if (this.e_qrController != null)
        {
            this.e_qrController.StartWork();
        }
    }

    public void Stop()
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.StopWork();
        }

        if (this.resetBtn != null)
        {
            this.resetBtn.SetActive(false);
        }
        if (this.scanLineObj != null)
        {
            this.scanLineObj.SetActive(false);
        }
    }

    public void GotoNextScene(string scenename)
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.StopWork();
        }
        SceneManager.LoadScene(scenename);
    }

    public void onConfirmErrPopup()
    {
        err_popup.SetActive(false);
    }
}
