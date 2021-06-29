//>>>>>>>>>>>>>>>> (5/2/2021)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
using DarkTonic.MasterAudio;


public class ClimberBonusPick : SimpleBonusPick
{   

    [SerializeField]
    float waitEffectTime = 1.0f;
    protected override void Init()
    {
        //>>>>>>>>>>>>>>>> (lax) (15/6/2021)
        foreach(Text t in totalWinText)
        {
            t.transform.parent.gameObject.SetActive(true);
        }
        foreach(Text t in totalFreespinText)
        {
            t.transform.parent.gameObject.SetActive(true);
        }
        //>>>>>>>>>>>>>>>> (lax) end (15/6/2021)
        base.Init();
        
        
    }

    protected override IEnumerator EndBonus()
    {

        foreach (UiPick pick in picks)
        {
            if (!pick.IsPicked)
            {
                pick.pickButton.enabled = false;
               
            }
        }
        yield return new WaitForSeconds(delayBeforeDisplayAll);
        foreach (UiPick pick in picks)
        {
            if (!pick.IsPicked)
            {
                pick.PlayAnimationNotPicked();
                if (collectResultId.Count > pickCount)
                {

                    pick.UpdateText(resultPicks[collectResultId[pickCount]].SomeText);
                    pick.SetTrueAnimation(resultPicks[collectResultId[pickCount]].ParameterName);
                    pickCount++;
                }
                else
                {
                    int random = Random.Range(0, resultPicks.Count);
                    pick.UpdateText(resultPicks[random].SomeText);
                    pick.SetTrueAnimation(resultPicks[random].ParameterName);
                }
            }
        }

        //>>>>>>>>>>>>>>>> (lax) (15/6/2021)
        if(totalWinCoin<=0)
        {
            foreach(Text t in totalWinText)
            {
                t.transform.parent.gameObject.SetActive(false);
            }
        }
        if(totalFreespin<=0)
        {
            foreach(Text t in totalFreespinText)
            {
                t.transform.parent.gameObject.SetActive(false);
            }

        }
        //>>>>>>>>>>>>>>>> (lax) end (15/6/2021)
        // if(SoundGO) SoundGO.SetActive(true);
        
        if (winAnim == null)
        {
            winAnim = FindObjectOfType<WinAnimation>();
            
        }
        if (winAnim != null)
        {
            yield return StartCoroutine(winAnim.RunWinAnimation((float)totalWinCoin));
            if (winAnim.HasWin())
            {
                //EndDisplay();
                if (displayPanel) 
                {
                    displayPanel.SetActive(false);
                    OnClickEndBonus();
                }
                    
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(waitBeforeDiaplayDetail);
                displayPanel?.SetActive(true);
                TweenCoin(totalWinText, 0, (float)totalWinCoin);
            }
        }
       
        // displayPanel?.SetActive(true);
        // TweenCoin(totalWinText, 0, (float)totalWinCoin);
        SaveResult();

    }

    protected override void BonusSelect(UiPick uP)
    {

        SimplePick selectedResult = resultPicks[collectResultId[pickCount]];

        uP.SetTrueAnimation(selectedResult.ParameterName);
        uP.PlayAnimationPicked();

        uP.UpdateText(selectedResult.SomeText);

        StartCoroutine(PlayEffectSound());
        
        pickCount++;
        ConfirmResult(selectedResult);

        if (pickCount >= numberOfPick || selectedResult.EndGame==true)
        {
            StartCoroutine(EndBonus());
        }
    }

    protected IEnumerator PlayEffectSound()
    {
        yield return new WaitForSeconds(waitEffectTime);
        MasterAudio.PlaySound("GemBreaking");
    }

    


}
//>>>>>>>>>>>>>>>> end (5/2/2021)