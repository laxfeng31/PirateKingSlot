using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class SubscribeText : MonoBehaviour
{
    Text uiText;
    //>>>>>>>>>>>>>>>> (lax) (12/5/2021)
    // private void Start()
    // {
    //     uiText = GetComponent<Text>();
    //     SlotMenuController.Instance.FreeSpinCountText = uiText;
    //     uiText.text = SlotPlayer.Instance.FreeSpins.ToString();
    // }


    private void OnEnable()
    {
        uiText = GetComponent<Text>();
        SlotMenuController.Instance.FreeSpinCountText = uiText;
        uiText.text = SlotPlayer.Instance.FreeSpins.ToString();
    }

    private void OnDisable()
    {
        uiText = null;
        SlotMenuController.Instance.FreeSpinCountText = uiText;
    }
    //>>>>>>>>>>>>>>>> (lax) end (12/5/2021)

}
