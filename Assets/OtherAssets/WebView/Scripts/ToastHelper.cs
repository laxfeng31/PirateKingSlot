using TMPro;
using UnityEngine;

public class ToastHelper : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lblMessage = null;

    private AndroidJavaClass androidJavaClass = null;

    private void Start()
    {
        androidJavaClass = new AndroidJavaClass("com.gamedotech.unityandroidwebviewlib.AndroidWebView");
        SendToAndroid("Hello World!");
    }

    private void SendToAndroid(string message)
    {
        androidJavaClass.CallStatic("ShowToast", message, name);
    }

    private void ReceiveFromAndroid(string message)
    {
        if (lblMessage) lblMessage.text = message;
    }
}
