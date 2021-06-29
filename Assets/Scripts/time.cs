using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using InchSky.WebRequest;

public class time : MonoBehaviour
{
    public Text text;
    public Text JackpotText;
    float dTime;
    public float jackpotValue;
    public float speed = 25;

    //Jackpot
    public Sprite[] number;
    public Image[] field;
    Image[] activeObj;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();

        string platformUID = PlayerPrefs.GetString("CurrentPUID");
        string portal_md5 = WebRequestManager.Md5Sum("KINGSLOT");
        double playpoint = 0;
        int modetype = 0;
        
        
        string sign = platformUID + "GETJACKPOT" + "i8n8c8hAsNky" + portal_md5;
        string sign_sha1 = WebRequestManager.Sha1Sum(sign);
        Debug.Log(sign);
        JSONObject j = new JSONObject();
        j.AddField("id", platformUID);
        j.AddField("bet", playpoint.ToString());
        j.AddField("modetype", modetype);
        j.AddField("method", "GETJACKPOT");
        j.AddField("sign", sign_sha1);

        //StartCoroutine(WebRequestManager.Post("https://www.inchsky.com/KingSlot/API/getJackpot.php", j.ToString(),
        //    (result) =>
        //    {
        //        //jackpotValue = int.Parse(result);
        //        activeObj = new Image[field.Length];
        //        SetValue(jackpotValue);
        //    },
        //    (error)=> 
        //    { 
            
        //    }));

        

        
        
    }

    void Print(int activeObj, int scores, int field1)
    {
        field[field1].sprite = number[scores];
    }

    void SetValue(float scores)
    {
        int Convert = 1;
        for (int i = 0;i < field.Length; i++)
        {
            int scoreConvert = ((int)scores / Convert) % 10;
            Print(i, scoreConvert, i);
            Convert *= 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //dTime += Time.deltaTime + speed;
        jackpotValue += speed * Time.deltaTime;
        //string minutes;
        //string seconds;
        //Debug.Log(jackpotValue);
        SetValue(jackpotValue);
        //if (jackpotValue != 0)
        //{
        //    minutes = Mathf.Floor((jackpotValue % 3600) / 60).ToString("0000");
        //    seconds = (jackpotValue % 60).ToString("00");
        //}
        //else
        //{
        //    minutes = Mathf.Floor((dTime % 3600) / 60).ToString("0000");
        //    seconds = (dTime % 60).ToString("00");
        //}
        //Debug.Log(jackpotValue);
        //text.text = jackpotValue.ToString("0.00");
        if(JackpotText)JackpotText.text = jackpotValue.ToString("0.00");
    }
}
