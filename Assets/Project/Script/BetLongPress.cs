using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mkey;
//>>>>>>>>>>>>>>>> (8/4/2021)
public class BetLongPress : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool IsIncrease;
    public float timeStart = 1f;
    public float timeInterval = 0.5f;

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
    }

    IEnumerator StartLongPressRutine()
    {
        yield return new WaitForSeconds(timeStart);
        while (isPressed)
        {
            if (IsIncrease)
            {
                SlotMenuController.Instance.LineBetPlus_Click();
            }
            else
            {
                SlotMenuController.Instance.LineBetMinus_Click();
            }

            yield return new WaitForSeconds(timeInterval);
        }
    }

    

}
//>>>>>>>>>>>>>>>> end (8/4/2021)