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

public class Script_Save : MonoBehaviour
{
    public int scene;
    public string game_ID;
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
    }

    [Serializable]
    public class matchData
    {
        public string Message;
    }

    [Serializable]
    public class logStatus
    {
        public string Message;
        public long Active;
    }

    // Start is called before the first frame update
   

    public void saveMatchHistory(double _oldpoint, double _activepoint, double _newpoint, double totalBet, int totalLinePlay,int totalLineWin, int currentFreeSpin,string modeType)
    {
        
        //Ray
        string platformUID = PlayerPrefs.GetString("CurrentPUID");
        double newpoint = _newpoint;
        double activepoint = _activepoint;
        double oldpoint = _oldpoint;
        double playpoint;
        if (modeType == "FREE" || modeType == "BONUS")
        {
            playpoint = 0;
        }
        else
        {
            playpoint = totalBet;
        }
        double resultpoint = _activepoint + playpoint;
        int lineplay = totalLinePlay;
        int linepayout = totalLineWin;
        int freespin = currentFreeSpin;
        string gameid = game_ID;
        long timeStamp1 = WebRequestManager.DateTimeToUnixTimestamp(DateTime.Now);
        string tran_no = gameid + DateTime.Now.ToString("yyMMddhhmmss") + UnityEngine.Random.Range(0, 9) + UnityEngine.Random.Range(0, 9) + UnityEngine.Random.Range(0, 9);
        string sign_ray = method + platformUID + platform + timeStamp1 + gameid + (long)oldpoint + activepoint + (long)newpoint + tran_no + playpoint +
            resultpoint + lineplay + linepayout + freespin + rayKey + WebRequestManager.Md5Sum(rayPortal);
        string sign_sha1 = WebRequestManager.Sha1Sum(sign_ray);



        JSONObject j = new JSONObject();
        j.AddField("platformUID", platformUID);
        j.AddField("oldpoint", (long)oldpoint);
        j.AddField("activepoint", (long)activepoint);
        j.AddField("newpoint", (long)newpoint);
        j.AddField("gameid", gameid);
        j.AddField("playpoint", (long)playpoint);
        j.AddField("resultpoint", (long)resultpoint);
        j.AddField("lineplay", lineplay);
        j.AddField("linepayout", linepayout);
        j.AddField("freespin", freespin);
        j.AddField("tran_no", tran_no);
        j.AddField("time", timeStamp1);
        j.AddField("platform", platform);
        j.AddField("method", method);
        j.AddField("sign", sign_sha1);

        string _jsonString = j.ToString();
       
        StartCoroutine(WebRequestManager.Post(rayURL, _jsonString, (result)=> 
        {
            matchData thisMatch = JsonUtility.FromJson<matchData>(result);

            if (thisMatch.Message == "success")
            {
                //SlotMenuController.Instance.enableSpin = true;
                Debug.Log(result);
            }
            else
            {
                //SlotMenuController.Instance.enableSpin = false;
                Debug.Log(result);

            }
        },(error) => 
        { 
        
        }));
    }

   

    
   

    void processGameRTP(string _game_id, int _bet, int _win, string _sign)
    {

        JSONObject j = new JSONObject();
        j.AddField("game_id", _game_id);
        j.AddField("bet", _bet);
        j.AddField("win", _win);
        j.AddField("sign", _sign);

        string _jsonString = j.ToString();
        //StartCoroutine(PostGameRTP(game_rtp_url, _jsonString));
    }

    IEnumerator PostGameRTP(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

        yield return request.SendWebRequest();

        if (string.IsNullOrEmpty(request.error))
        {

        }
        else
        {

        }

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