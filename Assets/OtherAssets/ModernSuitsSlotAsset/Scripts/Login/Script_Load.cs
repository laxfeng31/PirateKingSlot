using DG.Tweening;
using InchSky.WebRequest;
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

public class Script_Load : MonoBehaviour
{
    public int scene;
    //public GameObject Popup;
    //public Text PopupText;
    //public Transform PopupContainer;

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
    [SerializeField]
    private string gameID;

    public bool TestRay;

    private SlotPlayer SP
    {
        get { return SlotPlayer.Instance; }
    }

    [Serializable]
    public class userData
    {
        public string Message;
        public string PlatformUID;
        public string Status;
        public int GPoint;
        public int Freespin;
        public string WGroup;
        public string Agent;
    }

    // Start is called before the first frame update
    void Start()
    {
        checkUserStatus();
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    public void loadPressed_Test()
    {
        string platformUID = PlayerPrefs.GetString("CurrentPUID");
        //string platformUID = "GAMEDO_2";
        string portal_md5 = WebRequestManager.Md5Sum(rayPortal);
        long timeStamp1 = WebRequestManager.DateTimeToUnixTimestamp(DateTime.Now);
        string sign = method + platformUID + platform + timeStamp1 + rayKey + portal_md5;
        string sign_sha1 = WebRequestManager.Sha1Sum(sign);

        Debug.Log(sign);
        process_Test(platformUID,platform,timeStamp1,method, sign_sha1);
    }

    void process_Test(string _platformUID, string _platform, long _timestamp,string _method,string _sign)
    {

        JSONObject j = new JSONObject();
        j.AddField("platformUID", _platformUID);
        j.AddField("platform", _platform);
        j.AddField("time", _timestamp);
        j.AddField("method", _method);
        j.AddField("sign", _sign);

        string _jsonString = j.ToString();
        Debug.Log(_jsonString);
        StartCoroutine(WebRequestManager.Post(rayURL, _jsonString,
            (result)=> 
            {
                userData thisUser = JsonUtility.FromJson<userData>(result);
                Debug.Log(thisUser.GPoint);
                if (thisUser.Message == "success")
                {
                    SP.SetCoinsCount(thisUser.GPoint);
                    Debug.Log(result);
                }
                else
                {
                    Debug.Log(result);
                }
            },
            (error)=> 
            { 
            
            }));
    }

    

    public void checkUserPoint()
    {
        string platformUID = PlayerPrefs.GetString("CurrentPUID");
        string portal_md5 = WebRequestManager.Md5Sum(rayPortal);
        long timeStamp1 = WebRequestManager.DateTimeToUnixTimestamp(DateTime.Now);
        string sign = method + platformUID + platform + timeStamp1 + rayKey + portal_md5;
        string sign_sha1 = WebRequestManager.Sha1Sum(sign);
        
        checkUserPoint_Test(platformUID, platform, timeStamp1, method, sign_sha1);
    }

    void checkUserPoint_Test(string _platformUID, string _platform, long _timestamp, string _method, string _sign)
    {

        JSONObject j = new JSONObject();
        j.AddField("platformUID", _platformUID);
        j.AddField("platform", _platform);
        j.AddField("time", _timestamp);
        j.AddField("method", _method);
        j.AddField("sign", _sign);

        string _jsonString = j.ToString();
        StartCoroutine(WebRequestManager.Post(rayURL, _jsonString,
            (result)=> 
            {
                userData thisUser = JsonUtility.FromJson<userData>(result);
                if (thisUser.Message == "success")
                {
                    Debug.Log("Local's " + SlotPlayer.Instance.Coins + ", Ray's" + thisUser.GPoint);

                    Debug.Log(result);
                }
                else
                {
                    Debug.Log(result);
                }
            },
            (error)=> 
            { 
            
            }));
    }

   
    public void checkUserStatus()
    {
        string platformUID = PlayerPrefs.GetString("CurrentPUID");
        string portal_md5 = WebRequestManager.Md5Sum(rayPortal);
        long timeStamp1 = WebRequestManager.DateTimeToUnixTimestamp(DateTime.Now);
        string sign = "USERGAMESTATUS" + platformUID + platform + timeStamp1 + gameID + rayKey + portal_md5;
        string sign_sha1 = WebRequestManager.Sha1Sum(sign);
        
        //Debug.Log(sign);

        JSONObject j = new JSONObject();
        j.AddField("platformUID", platformUID);
        j.AddField("platform", platform);
        j.AddField("time", timeStamp1);
        j.AddField("gameid", gameID);
        j.AddField("method", "USERGAMESTATUS");
        j.AddField("sign", sign_sha1);

        string _jsonString = j.ToString();
        //Debug.Log(_jsonString);
        StartCoroutine(WebRequestManager.Post("https://ezwin2u.rayhtw.com/api/api_user_gamestatus.php", _jsonString,
            (result)=> 
            {
                userData thisUser = JsonUtility.FromJson<userData>(result);
                //Debug.Log(thisUser.GPoint);
                if (thisUser.Message == "success")
                {
                    SP.SetCoinsCount(thisUser.GPoint);
                    Debug.Log("thisUser.Freespin :"+ thisUser.Freespin);
                    SP.SetFreeSpinsCount(thisUser.Freespin);
                    if (!SP.HasFreeSpin)
                    {
                        SlotController.Instance.beforeFeatureGame = true;
                    }
                    else
                    {
                        SlotController.Instance.SpinPress();
                    }
                    //Debug.Log(result);
                }
                else
                {
                    Debug.Log(result);
                }
            },
            (error)=> 
            { 
            
            }));
    }

    

    void checkLoginStatus()
    {
        if (PlayerPrefs.GetString("UserLoggedIn") != "Yes")
        {

        }
        else if (PlayerPrefs.GetString("UserLoggedIn") == "Yes")
        {
            SceneManager.LoadScene(scene);
        }
    }
}
