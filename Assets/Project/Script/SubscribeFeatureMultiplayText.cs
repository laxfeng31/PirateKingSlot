using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
//>>>>>>>>>>>>>>>> (15/3/2021)
public class SubscribeFeatureMultiplayText : MonoBehaviour
{
    Text uiText;
    //>>>>>>>>>>>>>>>> (yb) (13/5/2021)

    //private void Start()
    //{
    //    uiText = GetComponent<Text>();
    //    SlotController.Instance.featureMultiplyText = uiText;
    //    uiText.text = SlotController.Instance.featureMultiply.ToString();
    //}

    private void OnEnable()
    {
        uiText = GetComponent<Text>();
        SlotController.Instance.featureMultiplyText = uiText;
        uiText.text = SlotController.Instance.featureMultiply.ToString();
    }

    private void OnDisable()
    {
        uiText = null;
        SlotController.Instance.featureMultiplyText = uiText;
    }
    //>>>>>>>>>>>>>>>> (yb) end (13/5/2021)
}
//>>>>>>>>>>>>>>>> end (15/3/2021)
