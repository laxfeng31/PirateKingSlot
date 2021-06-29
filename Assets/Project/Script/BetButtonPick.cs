using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using UnityEngine.UI;
//(Updated)>>>>>>>>>>>>>>>> (4/5/2021)
public class BetButtonPick : MonoBehaviour
{
    public GameObject pickButtonPanel;
    public GameObject betCountText;
    public GameObject buttonImage;
    public Transform betText_before, betText_after;
    bool betClick = false;

    private void Start()
    {
        betCountText.transform.position = betText_before.transform.position;
        SlotMenuController.Instance.TotalBetSumText = betCountText.GetComponent<Text>();
        SlotMenuController.Instance.RefreshBetLines();
        betClick = false;
    }

    public void OnclickBet()
    {
        if (!betClick)
        {
            betCountText.transform.position = betText_after.transform.position;
            buttonImage.SetActive(true);
            pickButtonPanel.SetActive(!pickButtonPanel.activeInHierarchy);
            betClick = true;
        }
        else
        {
            betCountText.transform.position = betText_before.transform.position;
            buttonImage.SetActive(false);
            pickButtonPanel.SetActive(!pickButtonPanel.activeInHierarchy);
            betClick = false;
            UpdateBet();
        }
        
    }

    void UpdateBet()
    {
        SlotMenuController.Instance.RefreshBetLines();
    }

    public void betPlusClick()
    {
        SlotMenuController.Instance.LineBetPlus_Click();
    }
    public void betMinusClick()
    {
        SlotMenuController.Instance.LineBetMinus_Click();
    }
    public void betMaxClick()
    {
        SlotMenuController.Instance.MaxBet_Click();
    }
}
