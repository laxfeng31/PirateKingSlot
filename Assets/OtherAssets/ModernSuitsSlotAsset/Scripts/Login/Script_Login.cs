using DG.Tweening;
using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using InchSky.WebRequest;

public class Script_Login : MonoBehaviour
{
    public int scene;
    public InputField inputUsername;
    public InputField inputPassword;
    public GameObject Popup;
    public Text PopupText;
    public Transform PopupContainer;
    [SerializeField]
    private string rayURL;
    [SerializeField]
    private string rayKey;
    [SerializeField]
    private string rayPortal;
    [SerializeField]
    private string platform;
    [SerializeField]
    private string method;

    public bool TestRay;

    private SlotPlayer SP
    {
        get { return SlotPlayer.Instance; }
    }

    [Serializable]
    public class userData
    {
        public int id;
        public string username;
        public int balance;
        public string API_LOGIN;
        public string Message;
        public string PlatformUID;
        public string Name;
        public string Status;
        public int GPoint;
        public string WGroup;
        public string Agent;
    }

    [Serializable]
    public class matchData
    {
        public string Message;
    }



    public void logInPressed()
    {
        if (TestRay)
        {
            string userid = inputUsername.text;
            string password = inputPassword.text;
            string portal_md5 = WebRequestManager.Md5Sum(rayPortal);
            long timeStamp1 = WebRequestManager.DateTimeToUnixTimestamp(DateTime.Now);
            string sign = method + userid + password + platform + timeStamp1 + rayKey + portal_md5;
            string sign_sha1 = WebRequestManager.Sha1Sum(sign);

            Debug.Log(sign);
            processLogin(userid, password, timeStamp1, sign_sha1);
        }
    }

    void processLogin(string _userid, string _password, long _time, string _sign)
    {
        JSONObject j = new JSONObject();
        if (TestRay)
        {
            j.AddField("username", _userid);
            j.AddField("password", _password);
            j.AddField("platform", platform);
            j.AddField("time", _time);
            j.AddField("method", method);
            j.AddField("sign", _sign);
        }
        else
        {
            j.AddField("username", _userid);
            j.AddField("password", _password);
            j.AddField("sign", _sign);
        }
        string _jsonString = j.ToString();

        Debug.Log(_jsonString);
        if (TestRay)
        {
            StartCoroutine(WebRequestManager.Post(rayURL, _jsonString, (result) => 
            {
                userData thisUser = JsonUtility.FromJson<userData>(result);
                if (TestRay)
                {
                    if (thisUser.Message == "success")
                    {
                        doPopUp("Logging In.");
                        PlayerPrefs.SetString("CurrentPUID", thisUser.PlatformUID);
                        PlayerPrefs.SetString("CurrentName", thisUser.Name);
                        PlayerPrefs.SetString("AgentCode", thisUser.Agent);
                        PlayerPrefs.SetString("UserLoggedIn", "Yes");
                        PlayerPrefs.SetString("current_Scene", "Lobby");
                        SceneManager.LoadScene("Loading");

                        //saveMatchHistory(thisUser.GPoint, 1, "");
                
                    }
                    else
                    {
                        doPopUp("Error Message:" + result);
                        Debug.Log(result);
                    }
                }
                else
                {
                    if (thisUser.API_LOGIN == "success")
                    {
                        doPopUp("Logging In.");
                        PlayerPrefs.SetInt("CurrentUID", thisUser.id);
                        PlayerPrefs.SetString("CurrentName", thisUser.username);
                        PlayerPrefs.SetString("UserLoggedIn", "Yes");
                        PlayerPrefs.SetString("current_Scene", "Lobby");
                        SceneManager.LoadScene("Loading");
                    }
                    else if (thisUser.API_LOGIN == "password incorrect")
                    {
                        doPopUp("Username & Password not match.");
                    }
                    else
                    {
                        doPopUp("Error Message:" + result);
                        Debug.Log(result);
                    }
                }
            },(result) => 
            {
                doPopUp("Error Message:" + result);
            }));
        }
        
    }

    
    
    void doPopUp(string content)
    {
        Popup.SetActive(true);
        PopupText.text = content;
        PopupContainer.DOScale(1, 0.5f);
    }

    public void closePopUp()
    {
        StartCoroutine("closePopUpFunction");
    }

    IEnumerator closePopUpFunction()
    {
        PopupContainer.DOScale(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Popup.SetActive(false);
    }

    void checkLoginStatus()
    {
        if (PlayerPrefs.GetString("UserLoggedIn") != "Yes")
        {
            
        }
        else if (PlayerPrefs.GetString("UserLoggedIn") == "Yes")
        {
            PlayerPrefs.SetString("current_Scene", "Lobby");
            SceneManager.LoadScene(scene);
        }
    }

    public void logout()
    {
        PlayerPrefs.SetInt("CurrentUID", 0);
        PlayerPrefs.SetString("CurrentName", null);
        PlayerPrefs.SetString("UserLoggedIn", null);
    }
}
