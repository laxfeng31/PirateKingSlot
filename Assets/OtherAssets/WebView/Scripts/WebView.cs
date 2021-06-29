using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using Mkey;
using UnityEngine.SceneManagement;

namespace Wizcorp.Web
{
    public class WebView : MonoBehaviour
    {
        
        [SerializeField]
        private string URL = "http://inchsky.com/webtemp/wheel/wheel1.html";
        [SerializeField]
        private string imageURL = "http://inchsky.com/webtemp/wheel/img/poro.png";
        [SerializeField]
        private string jsString = "var loadingBar = document.getElementById('bootContainer'); loadingBar.setAttribute('style','display:flex !important;'); loadingBar.children[0].children[0].src = 'https://inchsky.com/KingSlot/admin/app/images/poro.png';" +
                "loadingBar.children[0].children[0].setAttribute('style','width:100%;height:100%;position:absolute;top:0;left:0;'); loadingBar.children[1].children[0].setAttribute('style','display:none;');";
        [SerializeField]
        private TextMeshProUGUI lblContext = null;
        [SerializeField]
        private int gametype;
        [SerializeField]
        private int gametheme;

        [Header("Timers")]
        [SerializeField]
        private long hideDelay = 1000;
        [SerializeField]
        private float loadingDuration = 4000;

        [Serializable]
        public class urlData
        {
            public string Message;
            public string Game_url;
        }

        #region shared
        public void CallBack(string message)
        {
            if (lblContext) lblContext.text = message;
        }
        #endregion

#if UNITY_ANDROID
        IEnumerator CallWebView()
        {
            using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
              
                using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    string this_url = "";
                  
                    if (URL == null || URL == "")
                    {
                        yield return (GameUrl_Post("https://ezwin2u.rayhtw.com/tpg/tpg_login.php",
                            (value2) => {

                                this_url = value2;
                                Debug.Log(this_url);
                            }
                        ));
                        SlotPlayer.Instance.urlWebSlot = this_url;
                        SceneManager.LoadScene("WebViewSlot");
                        //currentActivity.Call("OpenWebView", this_url, GetJS(), hideDelay);
                    }
                    else
                    {
                     
                        this_url = URL;
                        Debug.Log(this_url);
                        SlotPlayer.Instance.urlWebSlot = this_url;
                        SceneManager.LoadScene("WebViewSlot");
                        //currentActivity.Call("OpenWebView", this_url, GetJS(), hideDelay);

                    }
                }
            }
        }

        public void CallWeb()
        {
  
            StartCoroutine(CallWebView());
        }

        private void Start()
        {
            //using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            //{
            //    using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
            //    {
            //        currentActivity.Call("SetupCallback", gameObject.name, "CallBack", "Calling back from Android");
            //    }
            //}

            //CallWebView();

        }

        private string GetJS()
        {

            //return "var loadingBar = document.getElementById('bootContainer'); loadingBar.setAttribute('style','display:flex !important;'); loadingBar.children[0].children[0].src = 'https://inchsky.com/KingSlot/admin/app/images/poro.png';" +
            //    "loadingBar.children[0].children[0].setAttribute('style','width:100%;height:100%;position:absolute;top:0;left:0;'); loadingBar.children[1].children[0].setAttribute('style','display:none;');";

            return @"var loadingDivElement = document.createElement('div');
                    loadingDivElement.id = 'custom-loading';
                    loadingDivElement.className = 'align-center';
                    loadingDivElement.style.position = 'fixed';
                    loadingDivElement.style.display = 'flex';
                    loadingDivElement.style.width = '100%';
                    loadingDivElement.style.height = '100%';
                    loadingDivElement.style.top = 0;
                    loadingDivElement.style.left = 0;
                    loadingDivElement.style.backgroundColor = 'black';
                    loadingDivElement.style.zIndex = 1000000;
                    loadingDivElement.style.textAlign = 'center';
                    loadingDivElement.style.alignItems = 'center';
                    loadingDivElement.style.justifyContent = 'center';
                    loadingDivElement.style.flexDirection = 'column';

                    var logoDivElement = document.createElement('div');
                    loadingDivElement.appendChild(logoDivElement);
                    var logoImgElement = document.createElement('img');
                    logoImgElement.src = '" + imageURL + @"';
                    logoImgElement.style.width = '100%';
                    logoImgElement.style.height = '100%';
                    logoDivElement.appendChild(logoImgElement);

                    var bodyElement = document.getElementsByTagName('body')[0];
                    bodyElement.appendChild(loadingDivElement);
                    
                    var delayedLoadingTimeout = setTimeout(() => {
                    var loadingInterval = setInterval(() => {
                            if ($('#bootContainer').css('display') == 'none') {
                                $('#custom-loading').fadeOut(1000, () => {
                                    loadingDivElement.style.display = 'none';
                                })

                                clearInterval(loadingInterval);
                            } else if ($('#preloading').css('display') == 'none') {
                                $('#custom-loading').fadeOut(1000, () => {
                                    loadingDivElement.style.display = 'none';
                                })

                                clearInterval(loadingInterval);
                            } 
                        }, 500);
                    }, " + loadingDuration + @");

                    var backBtn = document.createElement('button');
                    backBtn.innerHTML = 'Back';
                    backBtn.style.top = '0px';
                    backBtn.style.right = '0px';
                    backBtn.style.position = 'absolute';
                    backBtn.style.zIndex = 900;
                    backBtn.onclick = () => { backInterface.onBackButtonPressed(); };
                    bodyElement.appendChild(backBtn);

                    bodyElement.style.height = '100%';
                    bodyElement.style.overflow = 'hidden';";
        }

        IEnumerator GameUrl_Post(string url, Action<String> callback)
        {
            string game_url = URL;
            string platformUID = "EZWIN2U_4";
            long time = DateTimeToUnixTimestamp(DateTime.Now);
            string platform = "EZWIN2U";
            string currency = "MYR";
            int game_type = gametype;
            int game_theme = gametheme;
            string lang = "EN";
            string method = "TPG_LOGIN";
            string key = "gfkj#c5";
            string portal = "EZWIN2U";
            string portal_md5 = Md5Sum(portal);
            string sign = sha1Sum(method + platformUID + platform + time + currency + game_type + game_theme + lang + key + portal_md5);

            Debug.Log(sign);

            JSONObject j = new JSONObject();
            j.AddField("platformUID", platformUID);
            j.AddField("time", time);
            j.AddField("platform", platform);
            j.AddField("currency", currency);
            j.AddField("gametype", game_type);
            j.AddField("game_theme", game_theme);
            j.AddField("lang", lang);
            j.AddField("method", method);
            j.AddField("sign", sign);

            string _jsonString = j.ToString();
            Debug.Log(_jsonString);

            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(_jsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            string value = null;
            if (request.isHttpError)
            {
                Debug.Log("HTTP");
            }
            else if (request.isNetworkError)
            {
                Debug.Log("Network");
            }

            if (string.IsNullOrEmpty(request.error))
            {
                Debug.Log(request.downloadHandler.text);
                urlData this_url = JsonUtility.FromJson<urlData>(request.downloadHandler.text);
                value = this_url.Game_url;
            }

            callback(value);

        }

        //UNIVERSAL FUNCTIONS
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public string Md5Sum(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));//大  "X2",小"x2"    
            }
            return sb.ToString();
        }

        public string sha1Sum(string input)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha1.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));//大  "X2",小"x2"    
            }
            return sb.ToString();
        }
#endif

#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void _nativeLog();
	[DllImport("__Internal")]
	private static extern void _openURL(string url);
	[DllImport("__Internal")]
	private static extern void _setupCallBack(string gameObject, string methodName);

	// Connect with button onClick event
	public void CallWebView()
	{
		_openURL(URL);
	}

	void Start()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_setupCallBack(this.gameObject.name, "CallBack");

			_nativeLog();
		}
	}

#endif
    }
}