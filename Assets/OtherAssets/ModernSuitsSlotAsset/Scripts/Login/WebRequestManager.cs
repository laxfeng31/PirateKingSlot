using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace InchSky.WebRequest 
{
    public class WebRequestManager : MonoBehaviour
    {

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public static string Sha1Sum(string input)
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

        public static string Md5Sum(string input)
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


        public static IEnumerator Post(string url, string bodyJsonString, Action<string> callback, Action<string> errorCallback)
        {

            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            
            yield return request.SendWebRequest();
            
            if (request.isHttpError)
            {
                Debug.Log("HTTP");
                errorCallback.Invoke(request.isHttpError.ToString());
            }
            else if (request.isNetworkError)
            {
                //Debug.Log("Network");
                errorCallback.Invoke("Network Error");
            }

            if (string.IsNullOrEmpty(request.error))
            {
                //Debug.Log(request.downloadHandler.text);
                callback.Invoke(request.downloadHandler.text);
                
            }
            else
            {
                //Debug.Log("Error Message:" + request.error);
                errorCallback.Invoke(request.error);
            }

        }

        public static IEnumerator WebReturn(string url, Action<string> callback, Action<string> errorCallback)
        {

            var request = new UnityWebRequest(url);
           
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

            yield return request.SendWebRequest();

            if (request.isHttpError)
            {
                Debug.Log("HTTP");
                errorCallback.Invoke("HTTP");
            }
            else if (request.isNetworkError)
            {
                Debug.Log("Network");
                errorCallback.Invoke("Network Error");
            }

            if (string.IsNullOrEmpty(request.error))
            {
                //Debug.Log(request.downloadHandler.text);
                callback.Invoke(request.downloadHandler.text);

            }
            else
            {
                Debug.Log("Error Message:" + request.error);
                errorCallback.Invoke(request.error);
            }

        }

        public static IEnumerator PostWaitResult(string url, string bodyJsonString, Action<string> callback, Action contLoop)
        {
            Debug.Log("Before send" + bodyJsonString);
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

            var operation = request.SendWebRequest();

            
            while (!operation.isDone || request.isHttpError || request.isNetworkError || string.IsNullOrEmpty(request.downloadHandler.text))
            {
               
                contLoop.Invoke();
                yield return null;
            }
            
            callback.Invoke(request.downloadHandler.text);
            
        }



    }
}

