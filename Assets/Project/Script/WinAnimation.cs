using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using UnityEngine.UI;
//>>>>>>>>>>>>>>>> (29/3/2021)
public class WinAnimation : PopUpDisplay
{
    [System.Serializable]
    class WinType
    {

        public GameObject winGameObj;

        public float multiplyTrigger = 5;
    }

    //>>>>>>>> (Chan Ming) (27/4/20) (ARC)
    [SerializeField]
    GameObject currentWinBackground;

    [SerializeField]
    GameObject EndButton;

    //>>>>>>>> end (Chan Ming) (27/4/20) (ARC)

    [SerializeField]
    List<WinType> listOfWin;
    [SerializeField]
    float timeOfDisplay;
    [SerializeField]
    float delayClose = 3;
    [SerializeField]
    Text coinTextDisplay;

    GameObject currentActiveWin;
    //>>>>>>>>>>>>>>>> (lax) (24/6/2021)
    [SerializeField]
    GameObject targetAnim;
    //>>>>>>>>>>>>>>>> (lax) end (24/6/2021)
    bool isEnd = false;
    bool endNow = false;
    bool hasWin = false;
    public override IEnumerator OnWinSlotsAsyncRutine()
    {
        isEnd = false;
        float win = (float)SlotController.Instance.tempGameWin;

        yield return StartCoroutine(RunWinAnimation(win));

    }

    public IEnumerator RunWinAnimation(float winAmount)
    {
        hasWin = false;
        float totalBet = (float)SlotPlayer.Instance.TotalBet;
        float totalMultiply = winAmount / totalBet;
        bool hasBigWin = false;
        float coinCounter = 0;
        endNow = false;
        foreach (WinType wT in listOfWin)
        {
            if (wT.multiplyTrigger <= totalMultiply)
            {

                hasBigWin = true;
                break;
            }

        }
        //>>>>>>>>>>>>>>>> (lax) (24/6/2021)
        foreach(WinType wT in listOfWin)
        {
            if(totalMultiply>=wT.multiplyTrigger)
            {
                targetAnim = wT.winGameObj;
            }
        }
        //>>>>>>>>>>>>>>>> (lax) end (24/6/2021)
        if (!hasBigWin)
        {
            
            ResetAll();
            yield return null;
        }
        else
        {
            hasWin = true;
            SimpleTween.Value(gameObject, 0, winAmount, timeOfDisplay).SetOnUpdate((float val) =>
            {
                if (listOfWin.Count == 0) return;
                coinCounter = val;

                if (currentActiveWin == null)
                {
                    //>>>>>>>>>>>>>>>> (lax) (24/6/2021)
                    currentActiveWin = targetAnim;
                    //>>>>>>>>>>>>>>>> (lax) end (24/6/2021)
                    if (currentWinBackground != null)
                    {
                        currentWinBackground.SetActive(false);
                        currentWinBackground.SetActive(true);

                    }
                    currentActiveWin.SetActive(true);
                }
                foreach (WinType wT in listOfWin)
                {

                    float diff = (wT.multiplyTrigger - (coinCounter / totalBet));
                    if (diff >= -0.3f && diff <= 0.3f)
                    {

                        //>>>>>>>> (Chan Ming) (20/4/20) (ARC)
                        if (currentWinBackground != null)
                        {
                            currentWinBackground.SetActive(false);
                            currentWinBackground.SetActive(true);

                        }
                        //>>>>>>>> end (Chan Ming) (20/4/20) (ARC)

                        currentActiveWin?.SetActive(false);
                        //>>>>>>>>>>>>>>>> (lax) (24/6/2021)
                        currentActiveWin = targetAnim;
                        //>>>>>>>>>>>>>>>> (lax) end (24/6/2021)
                        currentActiveWin.SetActive(true);
                    }
                }
                if (currentActiveWin != null)
                {
                    if (coinTextDisplay) coinTextDisplay.text = coinCounter.ToString(SlotController.Instance.decimalDisplay);
                    //>>>>>>>> (Chan Ming) (27/4/20) (ARC)
                    if (EndButton != null)
                    {
                        EndButton.SetActive(true);
                    }
                    //>>>>>>>> end (Chan Ming) (27/4/20) (ARC)

                }

                if (coinCounter >= winAmount)
                {

                    EndShow();
                }
            });
            while (!isEnd) yield return new WaitForEndOfFrame();
            if (!endNow)
            {
                yield return new WaitForSeconds(delayClose);
            }

            ResetAll();
            yield return null;
        }
    }

    public bool HasWin()
    {
        return hasWin;
    }
    public void EndShow()
    {

        isEnd = true;

    }

    public void EndNow()
    {
        isEnd = true;
        endNow = true;
        //>>>>>>>> (Chan Ming) (27/4/20) (ARC)

        ResetAll();
        //>>>>>>>> end (Chan Ming) (27/4/20) (ARC)

    }

    void ResetAll()
    {
        //>>>>>>>> (Chan Ming) (27/4/20) (ARC)
        if (currentWinBackground != null)
            currentWinBackground.SetActive(false);

        if (coinTextDisplay != null)
            coinTextDisplay.text = "";

        if (EndButton != null)
            EndButton.SetActive(false);


        //>>>>>>>> end (Chan Ming) (27/4/20) (ARC)

        currentActiveWin?.SetActive(false);
        currentActiveWin = null;
        coinTextDisplay.text = "";
    }
}
//>>>>>>>>>>>>>>>> end (29/3/2021)