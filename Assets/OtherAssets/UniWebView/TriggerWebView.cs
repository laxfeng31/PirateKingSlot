using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using DarkTonic.MasterAudio;
using UnityEngine.SceneManagement;

public class TriggerWebView : MonoBehaviour
{
    [SerializeField]
    UniWebView webView;
    [SerializeField]
    GameObject loadingPanel;
    void Start()
    {
        MasterAudio.StopEverything();
        loadingPanel.gameObject.SetActive(true);
        string url = SlotPlayer.Instance.urlWebSlot;
        webView.UrlOnStart = url;
        webView.Frame = new Rect(0, Screen.height, Screen.height, Screen.width);
        webView.Show(false, UniWebViewTransitionEdge.None, 0.4f, () =>
        {
            StartCoroutine(CheckLoading());
        });



    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    IEnumerator CheckLoading()
    {
        bool isLoading = true;
        
        while (isLoading)
        {

            webView.EvaluateJavaScript("document.getElementById('preloading').style.display;", (result) => {
                if (result.resultCode.Equals("0"))
                {
                    if (result.data == "none")
                    {

                        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                        loadingPanel.gameObject.SetActive(false);
                        isLoading = false;
                    }
                   
                }

            });
            webView.EvaluateJavaScript("document.getElementById('bootContainer').style.display;", (result) => {
                if (result.resultCode.Equals("0"))
                {
                    if (result.data == "none")
                    {

                        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                        loadingPanel.gameObject.SetActive(false);
                        isLoading = false;
                    }

                }

            });

            yield return new WaitForSeconds(0.5f);
        }

    }
}
