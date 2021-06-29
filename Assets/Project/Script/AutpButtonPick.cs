using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using UnityEngine.UI;
//(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
public class AutpButtonPick : MonoBehaviour
{
    public GameObject pickButtonPanel;
    //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
    UnityEngine.UI.SpriteState AutoButtonState;
    //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
    public List<ButtonLimit> listButtons = new List<ButtonLimit>();
    public Text spinCountText;
    public GameObject infinityImageGameObj;
    public Image autoButtonImage;
    public Sprite autoButtonSpriteIdle;
    public Sprite autoButtonWithLimit;
    //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
    public Sprite autoButtonSpritePressed;
    public Sprite autoButtonSpritePressedWithLimit;
    public Sprite turboButtonSpriteWithLimit;
    public Sprite TurboButtonSpritePressed;

    [Header("Panel Setting")]
    int autoCount = 0 ;
    public Text autoCountText;
    public GameObject infinityIcon;
    public int infinityCount = 0;
    bool turboing = false;

    [System.Serializable]
    public class ButtonLimit
    {
        public Button button;
        public bool isInfinity = false;
        public int limit;
    }
    //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
    private void Start()
    {
        //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
        AutoButtonState = this.GetComponent<Button>().spriteState;
        //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
        foreach (ButtonLimit bL in listButtons)
        {
            SlotMenuController.Instance.buttonsCanClickWhileSpin.Add(bL.button);
            if (bL.isInfinity)
            {
                bL.button.onClick.AddListener(() =>
                {
                    OnclickAutoInfinity();
                    pickButtonPanel.SetActive(false);
                });
            }
            else
            {
                bL.button.onClick.AddListener(() =>
                {
                    OnclickAutoWithLimit(bL.limit);
                    pickButtonPanel.SetActive(false);
                    
                });
            }
        }
        SlotMenuController.Instance.displayTotalAutospinText = spinCountText;
        SlotMenuController.Instance.buttonsCanClickWhileSpin.Add(GetComponent<Button>());
        //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
        autoCountText.text = autoCount.ToString();
        pickButtonPanel.SetActive(false);
        //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
    }

    public void OnclickAuto()
    {
        SlotMenuController.Instance.AutoSpin_Clicking();
        if (SlotController.Instance.IsAutoSpin)
        {
            autoCount = 0;
            SlotMenuController.Instance.totalAutoSpinLeft = 0;
            SlotController.Instance.turboMode = false;
            UpdateAuto();
        }
        else
        {
            pickButtonPanel.SetActive(!pickButtonPanel.activeInHierarchy);
            //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
            if (autoCount>0 && autoCount < infinityCount)
            {
                OnclickAutoWithLimit(autoCount);
            }
            else if(autoCount == infinityCount)
            {
                OnclickAutoInfinity();
            }
            //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
        }
    }

    public void OnclickAutoInfinity()
    {
        infinityImageGameObj.SetActive(true);
        spinCountText.gameObject.SetActive(false);
        SlotMenuController.Instance.totalAutoSpinLeft = 0;
        UpdateAuto();
    }

    public void OnclickAutoWithLimit(int spinCount)
    {
        infinityImageGameObj.SetActive(false);
        spinCountText.gameObject.SetActive(true);
        SlotMenuController.Instance.totalAutoSpinLeft = spinCount;
        UpdateAuto();
    }

    void UpdateAuto()
    {
        SlotMenuController.Instance.Auto_Click();
        SlotMenuController.Instance.UpdateAutoSpinText();
       
    }
    //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
    public void OnclickSetAuto(int count)
    {
        autoCount += count;
        if(autoCount < 0)
        {
            autoCount = 0;
        }

        autoCountText.text = autoCount.ToString();
        
        if(autoCount >= infinityCount)
        {
            autoCount = infinityCount;
            autoCountText.gameObject.SetActive(false);
            infinityIcon.SetActive(true);
        }
        else
        {
            autoCountText.gameObject.SetActive(true);
            infinityIcon.SetActive(false);
        }

    }
    public void SetTurbo()
    {
        if(!turboing)
        {
            SlotController.Instance.turboMode = true;
            AutoButtonState.pressedSprite = TurboButtonSpritePressed;
            
        }
        else
        {
            SlotController.Instance.turboMode = false;
            AutoButtonState.pressedSprite = autoButtonSpritePressed;
        }
    }
    //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
    private void Update()
    {
        if (SlotMenuController.Instance.totalAutoSpinLeft >= 1)
        {
            //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
            if (!SlotController.Instance.turboMode)
            {
                autoButtonImage.sprite = autoButtonWithLimit;
            }
            else
            {
                autoButtonImage.sprite = turboButtonSpriteWithLimit;
            }
            //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
        }
        else
        {
            infinityImageGameObj.SetActive(false);
            spinCountText.gameObject.SetActive(false);
            //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
            if (!SlotController.Instance.turboMode)
            {
                autoButtonImage.sprite = autoButtonSpriteIdle;
            }
            else
            {
                autoButtonImage.sprite = turboButtonSpriteWithLimit;
            }
            //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
        }
    }
}
//(Updated)>>>>>>>>>>>>>>>> (29/1/2021)