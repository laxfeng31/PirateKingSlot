using InchSky.WebRequest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InchSky
{
    public class Random : MonoBehaviour
    {
        public Text Textbox;

        public float min = 10000;
        public float max = 50000;

        public int updateTime = 5;

        double currentValue;
        double RandomVariable;
        string thisGame;
        void Start()
        {
            Textbox = GetComponent<Text>();
            //Connect.Instance.ConnectedCallback += GetJackpot;
            thisGame = PlayerPrefs.GetString("this_game");
            if (gameObject.name != "RandomJackpot")
            {
                StartCoroutine("setTextBox1");
            } else
            {
                GetJackpot();
            }
            
        }

        void GetJackpot()
        {
            string portal_md5 = WebRequestManager.Md5Sum("KINGSLOT");

            string sign = "GETJACKPOT" + "i8n8c8hAsNky" + portal_md5;
            string sign_sha1 = WebRequestManager.Sha1Sum(sign);

            JSONObject j = new JSONObject();
            j.AddField("method", "GETJACKPOT");
            j.AddField("sign", sign_sha1);

            //Connect.Instance.EmitGetJackpot(j, (result) =>
            //{
            //    JackpotResult jackpot = JsonUtility.FromJson<JackpotResult>(result.ToString());
            //    currentValue = double.Parse(jackpot.jackpotValue) * PlayerPrefs.GetFloat(thisGame) / 100;
            //});

        }

        void Update()
        {
            if (gameObject.name == "RandomJackpot")
            {
                Textbox.text = currentValue.ToString("n2");
            }
            else
            {
                Textbox.text = RandomVariable.ToString("n2");
            }

        }
        public void setTextbox()
        {

            RandomVariable = UnityEngine.Random.Range(min * 100, max * 100);
            RandomVariable = RandomVariable / 100;
        }

        IEnumerator setTextBox1()
        {
            setTextbox();
            yield return new WaitForSeconds(1f);
            StartCoroutine("setTextBox1");

        }

    }
}