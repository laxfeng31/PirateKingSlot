using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mkey;
//>>>>>>>>>>>>>>>> (8/4/2021)
public class AutoLongPress : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool IsIncrease;
    public float timeStart = 1f;
    public float timeInterval = 0.5f;

    int count = 0;

    public AutpButtonPick autoBtnClick;

    bool isPressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!SlotController.Instance.IsSpinning)
        {
            if(!SlotController.Instance.PlayFreeSpins)
            {
                if (!isPressed)
                {
                    isPressed = true;
                    StartCoroutine(StartLongPressRutine());
                }
            }        
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        count = 0;
    }

    IEnumerator StartLongPressRutine()
    {
        yield return new WaitForSeconds(timeStart);
        while (isPressed)
        {
            if (IsIncrease)
            {
                //SlotMenuController.Instance.LineBetPlus_Click();
                autoBtnClick.OnclickSetAuto(1);
            }
            else
            {
                //SlotMenuController.Instance.LineBetMinus_Click();
                autoBtnClick.OnclickSetAuto(-1);
            }
            count++;
            yield return new WaitForSeconds(timeInterval / count/10);
        }
    }



    

}
//>>>>>>>>>>>>>>>> end (8/4/2021)