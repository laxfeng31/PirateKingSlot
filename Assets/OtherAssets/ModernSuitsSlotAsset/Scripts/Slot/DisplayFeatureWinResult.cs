using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
using DarkTonic.MasterAudio;

public class DisplayFeatureWinResult : PopUpDisplay
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    Text bonusWinText;
    [SerializeField]
    Text featureTotalWin;
    [SerializeField]
    Text totalWinText;
    [SerializeField]
    Button backButton;
    //>>>>>>>>>>>>>>>> (7/4/2021)
    [SerializeField]
    bool stopAllAutoSpin = true;
    //>>>>>>>>>>>>>>>> end (7/4/2021)
    [SerializeField]
    float autoEndSec = 6;

    WinAnimation winAnim;
    double totalWinValue;

    //Jackpot
    [SerializeField]
    Sprite[] number;
    [SerializeField]
    Image[] field;
    [SerializeField]
    Image[] comma;
    [SerializeField]
    GameObject NewWinText;

    //>>>>>>>>>>>>>>>> (7/4/2021)
    bool endDisplay = false;
    //>>>>>>>>>>>>>>>> (7/4/2021)
    bool isClose;
    protected override void Start()
    {

        backButton?.onClick.AddListener(delegate {
            isClose = true;
            if (panel) panel.SetActive(false);
        });
        base.Start();
        if (panel) panel.SetActive(false);
    }

    public override IEnumerator EndSlotsAsyncRutine()
    {
        if (SlotController.Instance.PlayFreeSpins && !SlotPlayer.Instance.HasFreeSpin)
        {
            //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
            SlotController.Instance.OnClickSpin += EndDisplay;
            //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)
            //>>>>>>>>>>>>>>>> (7/4/2021)
            endDisplay = false;
            if (winAnim == null)
            {
                winAnim = FindObjectOfType<WinAnimation>();
                
            }
            if (winAnim != null)
            {
                yield return StartCoroutine(winAnim.RunWinAnimation((float)SlotController.Instance.totalFeatureWin));
                if (winAnim.HasWin())
                {
                    EndDisplay();
                    
                    yield break;
                }
            }

            if (SlotController.Instance.onlyAddCoinOnEndFeature)
            {
                SlotPlayer.Instance.AddCoins(SlotController.Instance.totalFeatureWin);
            }

            //>>>>>>>>>>>>>>>> end (7/4/2021)
            if (panel) panel.SetActive(true);
            //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
            if (bonusWinText) bonusWinText.text = SlotController.Instance.lastWinBeforeBonus.ToString(SlotController.Instance.decimalDisplay);

            if (featureTotalWin) featureTotalWin.text = ((float)SlotController.Instance.totalFeatureWin - (float)SlotController.Instance.lastWinBeforeBonus).ToString(SlotController.Instance.decimalDisplay);
            //>>>>>>>>>>>>>>>> end (23/2/2021)
            //(Updated)>>>>>>>>>>>>>>>> (7/2/2021)
            //>>>>>>>>>>>>>>>> (7/4/2021)
            if (stopAllAutoSpin)
            {
                SlotController.Instance.IsAutoSpin = false;
                SlotMenuController.Instance.spinauto = false;
            }
            //>>>>>>>>>>>>>>>> end (7/4/2021)

            StateControllerManager.ChangeState("normal");
            if (SlotMenuController.Instance.totalFeatureWinInfoText) SlotMenuController.Instance.totalFeatureWinInfoText.gameObject.SetActive(false);
            //>>>>>>>>>>>>>>>> end (7/2/2021)
            if (field.Length > 0)
            {
                TweenSeq tS = new TweenSeq();
                SlotMenuController.Instance.StartCoinSound();
                tS.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, (float)(SlotController.Instance.totalFeatureWin), 2)
                        .SetOnUpdate((float val) =>
                        {
                            SetValue(val);
                        })
                        .AddCompleteCallBack(() =>
                        {
                            if (callBack != null) callBack();
                            SlotMenuController.Instance.EndCoinSound();
                            //if (callback != null) callback();
                        }).SetEase(EaseAnim.EaseLinear);
                });
                tS.Start();
            }
            else
            {
                if (NewWinText) NewWinText.SetActive(false);
                //>>>>>>>>>>>>>>>> (23/2/2021)
                if (totalWinText) TweenNumber.ShowAnimation(totalWinText, EaseAnim.EaseLinear, 0, (float)SlotController.Instance.totalFeatureWin, 2, SlotController.Instance.decimalDisplay);
                //>>>>>>>>>>>>>>>> end (23/2/2021)
            }


            bool megaWin = (SlotController.Instance.totalFeatureWin >= (SlotPlayer.Instance.TotalBet * SlotPlayer.Instance.megaWinMulti));
            bool ultraBigWin = (SlotController.Instance.totalFeatureWin >= (SlotPlayer.Instance.TotalBet * SlotPlayer.Instance.ultraBigWinMulti));
            bool kingWin = (SlotController.Instance.totalFeatureWin >= (SlotPlayer.Instance.TotalBet * SlotPlayer.Instance.kingWinMulti));
            //>>>>>>>>>>>>>>>> (8/12/2020)
            if (kingWin)
            {

                if (SlotController.Instance.kingWinAnimationGO != null)
                {
                    Debug.Log("KingWin");
                    StartCoroutine(SlotController.Instance.kingWinningAnimationRun());
                    //>>>>>>>>>>>>>>>> (7/2/2021)
                    StartCoroutine(SlotController.Instance.StoreFireworkRutine);
                    //>>>>>>>>>>>>>>>> end (7/2/2021)

                }

            }
            else if (ultraBigWin)
            {
                if (SlotController.Instance.ultraBigWinAnimationGO != null)
                {
                    Debug.Log("ultraBigWin");
                    StartCoroutine(SlotController.Instance.ultraBigWinningAnimationRun());
                    //>>>>>>>>>>>>>>>> (7/2/2021)
                    StartCoroutine(SlotController.Instance.StoreFireworkRutine);
                    //>>>>>>>>>>>>>>>> end (7/2/2021)


                }
            }
            else if (megaWin)
            {
                if (SlotController.Instance.megaWinAnimationGO != null)
                {
                    Debug.Log("megaWin");
                    StartCoroutine(SlotController.Instance.megaWinningAnimationRun());
                    //>>>>>>>>>>>>>>>> (7/2/2021)
                    StartCoroutine(SlotController.Instance.StoreFireworkRutine);
                    //>>>>>>>>>>>>>>>> end (7/2/2021)


                }
            }


            //SlotController.Instance.StopAllFirework();
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            SlotController.Instance.totalFeatureWin = 0;
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
            //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
            if(autoEndSec > 0)
            {
                yield return StartCoroutine(AutoEnd());
            }
            if (!stopAllAutoSpin)
            {
                while (!endDisplay) yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

            //>>>>>>>>>>>>>>>> (7/2/2021)
            //SlotController.Instance.IsAutoSpin = false;
            //>>>>>>>>>>>>>>>> end (7/2/2021)
            //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
            //isClose = false;

            //if (!backButton)
            //{
            //    isClose = true;
            //}

            //while (!isClose) yield return null;

        }



    }

    //(Updated)>>>>>>>>>>>>>>>> (7/2/2021)
    //>>>>>>>>>>>>>>>> (7/4/2021)
    IEnumerator AutoEnd()
    {
        yield return new WaitForSeconds(autoEndSec);
        EndDisplay();
    }
    public void EndDisplay()
    {
        SlotController.Instance.OnClickSpin -= EndDisplay;
        SlotMenuController.Instance.SetWinInfo(0);
        SlotMenuController.Instance.ClearWinInfoText();
        StopFireworks();
        if (panel) panel.SetActive(false);
        endDisplay = true;
    }
    //>>>>>>>>>>>>>>>> end (7/4/2021)
    public void StopFireworks()
    {

        SlotController.Instance.StopAllFirework();

    }
    //>>>>>>>>>>>>>>>> end (7/2/2021)
    void Print(int activeObj, int scores, int field1)
    {
        field[field1].sprite = number[scores];
    }

    void SetValue(double scores)
    {
        int Convert = 1;
        scores = scores * 100;
        if (scores < 100000000)//100000
        {
            field[8].gameObject.SetActive(false);
            comma[1].gameObject.SetActive(false);
            if (scores < 10000000)//100000
            {
                field[7].gameObject.SetActive(false);
                if (scores < 1000000)//10000
                {
                    field[6].gameObject.SetActive(false);
                    if (scores < 100000)//1000
                    {
                        comma[0].gameObject.SetActive(false);
                        field[5].gameObject.SetActive(false);
                        if (scores < 10000)//100
                        {
                            field[4].gameObject.SetActive(false);
                            if (scores < 1000)//10
                            {
                                field[3].gameObject.SetActive(false);

                            }
                            else
                            {
                                field[3].gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            field[4].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        comma[0].gameObject.SetActive(true);
                        field[5].gameObject.SetActive(true);
                    }
                }
                else
                {
                    field[6].gameObject.SetActive(true);
                }
            }
            else
            {
                field[7].gameObject.SetActive(true);
            }
        }
        else
        {
            comma[1].gameObject.SetActive(true);
            field[8].gameObject.SetActive(true);
        }

        for (int i = 0; i < field.Length; i++)
        {
            int scoreConvert = ((int)scores / Convert) % 10;
            Print(i, scoreConvert, i);
            Convert *= 10;
        }
    }

    void update()
    {
        SetValue(totalWinValue);
    }
}
