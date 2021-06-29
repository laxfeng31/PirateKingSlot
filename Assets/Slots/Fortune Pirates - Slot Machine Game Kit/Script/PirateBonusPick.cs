//>>>>>>>>>>>>>>>> (5/2/2021)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
using DarkTonic.MasterAudio;


public class PirateBonusPick : SimpleBonusPick
{

    

    
    [SerializeField]
    Text[] TotalBetRewardText;
   
    int currentNumMultiply=0;

    

    protected override void Init()
    {
        currentNumMultiply = 0;
        base.Init();

        //>>>>>>>>>>>>>>>> (yb) (22/6/2021)
        foreach (Text i in TotalBetRewardText)
        {
            i.text = "0";
        }
        //>>>>>>>>>>>>>>>> (yb) (22/6/2021)
    }


    //protected override List<int> SuffleResult()
    //{
    //    List<int> temp = new List<int>();
    //    List<int> endGameIds = new List<int>();
    //    bool addEndGame = false;
    //    for(int i = 0; i< picks.Count; i++)
    //    {
    //        int j = Random.Range(0,resultPicks.Count);
    //        Debug.Log(j);
    //        if (resultPicks[j].EndGame && !addEndGame)
    //        {
    //            endGameIds.Add(j);
    //            addEndGame = true;
    //        }
    //        else
    //        {
    //            temp.Add(0);
    //        }

    //    }
    //    ShuffleList<int> shuffleList = new ShuffleList<int>();
    //    endGameIds = shuffleList.Shuffle(endGameIds);
    //    temp = shuffleList.Shuffle(temp);

    //    for(int i = 0; i< endGameIds.Count; i++)
    //    {
    //        temp.Insert(Random.Range(minPick, temp.Count), endGameIds[i]);
    //    }



    //    return temp;
    //}

    int CheckExistNum(List<int> list)
    {
        int i =0;
        if(list!=null)
        {
            do
            {
                i = Random.Range(1,20);
            }while(list.Contains(i));
        }
        else
        {
            i = Random.Range(1,20);
        }
        return i;
    }

    
    protected override void BonusSelect(UiPick uP)
    {
        SimplePick selectedResult = new SimplePick();
        selectedResult = resultPicks[collectResultId[pickCount]];

        uP.SetTrueAnimation(selectedResult.ParameterName);
        uP.PlayAnimationPicked();

        if(selectedResult.EndGame)
        {
            currentNumMultiply = 0;
            selectedResult.SomeText = "GameOver";
            MasterAudio.PlaySound("SkullAudio");
        }
        else
        {
            
            
            selectedResult.SomeText = "X "+ resultPicks[collectResultId[pickCount]].NumberOfMultiply;
            currentNumMultiply = resultPicks[collectResultId[pickCount]].NumberOfMultiply;
            MasterAudio.PlaySound("ChestAudio");
        }
        uP.UpdateText(selectedResult.SomeText);
        UpdateTexts(TotalBetRewardText, (SlotPlayer.Instance.TotalBet*currentNumMultiply).ToString());
        
        pickCount++;
        ConfirmResult(selectedResult);

        if (pickCount >= numberOfPick || selectedResult.EndGame==true)
        {
            StartCoroutine(EndBonus());
        }
    }

    


    protected override IEnumerator EndBonus()
    {
        //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
        bool GameOver = false;
        //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)
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
                if (resultPicks[collectResultId[pickCount]].EndGame)
                {
                    pick.UpdateText("GameOver");
                }
                else
                {
                    pick.UpdateText("X " + resultPicks[collectResultId[pickCount]].NumberOfMultiply);
                }
                
                pick.SetTrueAnimation(resultPicks[collectResultId[pickCount]].ParameterName);
                pickCount++;
                //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
                GameOver = true;
                //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)
            }
        }
        //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
        if(GameOver) yield return new WaitForSeconds(waitBeforeDiaplayDetail);

        if(SoundGO) SoundGO.SetActive(true);
        
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
                displayPanel?.SetActive(true);
                TweenCoin(totalWinText, 0, (float)totalWinCoin);
            }
        }

        //displayPanel?.SetActive(true);
        //TweenCoin(totalWinText, 0, (float)totalWinCoin);
        //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)
        SaveResult();

    }


}
//>>>>>>>>>>>>>>>> end (5/2/2021)