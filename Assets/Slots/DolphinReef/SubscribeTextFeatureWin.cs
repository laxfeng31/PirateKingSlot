using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class SubscribeTextFeatureWin : MonoBehaviour
{
    //>>>>>>>> (Chan Ming) (26/1/21)

    Text uiText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
        SlotMenuController.Instance.totalFeatureWinInfoText = uiText;
        uiText.text = SlotPlayer.Instance.Coins.ToString();

    }
    //>>>>>>>> end (Chan Ming) (26/1/21)
    

}
