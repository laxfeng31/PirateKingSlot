using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;

public class Script_Marquee : MB_Singleton<Script_Marquee>
{
    public string marqueeText;

    [SerializeField]
    Text m_TxtMsg;//跑马灯text.
    Queue<string> m_MsgQueue;//灯队列.
    //Font m_Font;
    bool isScrolling = false;//判断当前text中的跑马灯是否跑完.

    IEnumerator Start()
    {
        Init();
        using (UnityWebRequest www = UnityWebRequest.Get("https://www.inchsky.com/KingSlot/API/marquee.txt"))
        {
            yield return www.SendWebRequest();

            if (www.downloadHandler.text != "" || www.downloadHandler.text != null)
            {

                marqueeText = www.downloadHandler.text;

            }

        }

        AddMessage(marqueeText);
    }

    public void Init()
    {
        m_MsgQueue = new Queue<string>();
    }
    /// <summary>
    /// 添加跑马灯信息.
    /// </summary>
    /// <param name="msg"></param>
    public void AddMessage(string msg)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Init();
        }
        m_MsgQueue.Enqueue(msg);
        if (isScrolling) return;
        StartCoroutine(Scrolling());
    }

    public IEnumerator Scrolling()
    {
        float beginX = 450;
        float leftX = -400;
        while (m_MsgQueue.Count > 0)
        {
            float duration = 10f;
            float speed = 50f;
            int loop = 0;
            string msg = m_MsgQueue.Dequeue();
            m_TxtMsg.text = msg;
            float txtWidth = m_TxtMsg.preferredWidth;//文本自身的长度.
            Vector3 pos = m_TxtMsg.rectTransform.localPosition;
            float distance = beginX - leftX + txtWidth;
            duration = distance / speed;
            isScrolling = true;
            while (loop == 0)
            {

                m_TxtMsg.rectTransform.localPosition = new Vector3(beginX, pos.y, pos.z);
                m_TxtMsg.rectTransform.DOLocalMoveX(-distance, duration).SetEase(Ease.Linear);
                yield return new WaitForSeconds(duration);
            }
            yield return null;
        }
        isScrolling = false;
        //gameObject.SetActive(false);
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
