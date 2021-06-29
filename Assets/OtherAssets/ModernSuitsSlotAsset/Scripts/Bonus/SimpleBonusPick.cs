//>>>>>>>>>>>>>>>> (5/2/2021)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
using DarkTonic.MasterAudio;
using UnityEngine.Events;

public class SimpleBonusPick : BonusGame
{

    [System.Serializable]
    protected class SimplePick
    {

        [Header("Free spin setting")]
        [SerializeField]
        private int numberOfFreeSpin = 0;

        [Header("Multiply with bet setting")]
        [SerializeField]
        private int numberOfMultiply;

        [Header("Multiply win for feature")]
        [SerializeField]
        private int numberFeatureMultiply = 0;
        //>>>>>>>> (Chan Ming) (13/3/21)(ARC)
        [SerializeField]
        private bool addOnMultiply = false;

        //>>>>>>>> end (Chan Ming) (13/3/21)(ARC)

        [Header("Diplay Text")]
        [SerializeField]
        string someText;

        [Header("Animation Parameter")]
        [SerializeField]
        string parameterName;

        [Header("TypeOfButton")]
        [SerializeField]
        bool EndGameButton;

        public int NumberOfFreeSpin { get => numberOfFreeSpin; set => numberOfFreeSpin = value; }
        public int NumberOfMultiply { get => numberOfMultiply; set => numberOfMultiply = value; }
        public int NumberOfFeatureMultiply { get => numberFeatureMultiply; set => numberFeatureMultiply = value; }
        //>>>>>>>> (Chan Ming) (13/3/21)(ARC)
        public bool AddOnMultiply { get => addOnMultiply; set => addOnMultiply = value; }

        //>>>>>>>> end (Chan Ming) (13/3/21)(ARC)


        public string SomeText { get => someText; set => someText = value; }
        public string ParameterName { get => parameterName; set => parameterName = value; }
        public bool EndGame { get => EndGameButton; set => EndGameButton = value; }
    }

    [System.Serializable]
    protected class UiPick
    {
  
        public Button pickButton;
        public Animator buttonAnim;
        //>>>>>>>> (Lax) (30/3/21)(ARC)
        public Animator SpriteTextAnim;

        //>>>>>>>> end (Lax) (30/3/21)(ARC)
        public Text displayText;
       

        bool isPicked = false;

        public bool IsPicked { get => isPicked; private set => isPicked = value; }

        void Init()
        {
            IsPicked = false;
            pickButton.enabled = true;
            
        }

        public void UpdateText(string content)
        {
            if (displayText)
            {
                displayText.text = content;
            }
        }

        public void PlayAnimationPicked()
        {
            if (buttonAnim)
            {
                
                buttonAnim.SetBool("IsPicked",true);
                buttonAnim.SetBool("IsNotPicked", false);
            }
            //>>>>>>>> (Lax) (30/3/21)(ARC)
            if (SpriteTextAnim)
            {
                SpriteTextAnim.SetBool("IsPicked",true);
                SpriteTextAnim.SetBool("IsNotPicked", false);
            }
            //>>>>>>>> end (Lax) (30/3/21)(ARC)
        }

        public void PlayAnimationNotPicked()
        {
            if (buttonAnim)
            {
                buttonAnim.SetBool("IsPicked",false);
                buttonAnim.SetBool("IsNotPicked", true);
            }
            //>>>>>>>> (Lax) (30/3/21)(ARC)
            if (SpriteTextAnim)
            {
                SpriteTextAnim.SetBool("IsPicked",false);
                SpriteTextAnim.SetBool("IsNotPicked", true);
            }
            //>>>>>>>> end (Lax) (30/3/21)(ARC)
        }

        public void AnimationReset()
        {
            if (buttonAnim)
            {
                buttonAnim.SetTrigger("Reset");
            }
            //>>>>>>>> (Lax) (30/3/21)(ARC)
            if (SpriteTextAnim)
            {
                SpriteTextAnim.SetTrigger("Reset");
            }
            //>>>>>>>> end (Lax) (30/3/21)(ARC)
        }
        public void ResetText()
        {
            if (displayText)
            {
                displayText.text = "";
            }
        }
        public void SetTrueAnimation(string idName)
        {
            if (buttonAnim)
            {

                buttonAnim.SetBool(idName, true);
            }
            //>>>>>>>> (Lax) (30/3/21)(ARC)
            if (SpriteTextAnim)
            {
                SpriteTextAnim.SetBool(idName, true);
            }
            //>>>>>>>> end (Lax) (30/3/21)(ARC)
        }

        public void AssignOnclick(UnityAction<UiPick> call)
        {
            Init();
      
            pickButton.onClick.RemoveAllListeners();
            pickButton.onClick.AddListener(() =>
            {
                IsPicked = true;
    
                call.Invoke(this);
                pickButton.enabled = false;
            });
        }

    }

    [Header("General Setting")]
    [SerializeField]
    protected int numberOfPick = 4;
    [SerializeField]
    protected int minPick = 1;
    [SerializeField]
    protected float delayBeforeDisplayAll = 1;
    [SerializeField]
    protected float waitBeforeDiaplayDetail = 5;

    [Header("UI Setting")]
    [SerializeField]
    protected Text[] numPickText;
    [SerializeField]
    protected Text[] currentBalanceText;
    [SerializeField]
    protected Text[] currentTotalBet;
    [SerializeField]
    protected Text[] totalWinText;
    [SerializeField]
    protected Text[] totalFreespinText;
    [SerializeField]
    protected Text[] totalFeatureMultiplyText;

    //>>>>>>>> (Chan Ming) (15/3/21)(ARC)
    [Header("InitialBonus")]
    [SerializeField]
    protected int initialWinCoin;
    [SerializeField]
    protected int initialFreespin;
    [SerializeField]
    protected int initialFeatureMultiply = 1;
    //>>>>>>>> end (Chan Ming) (15/3/21)(ARC)

    [Header("Pick Ui Setting")]
    [SerializeField]
    protected List<UiPick> picks = new List<UiPick>();
    

    [Header("Result Setting")]
    [SerializeField]
    protected List<SimplePick> resultPicks = new List<SimplePick>();

    [Header("Finish Pick Setting")]

    [SerializeField]
    protected GameObject displayPanel;

    protected List<int> collectResultId = new List<int>();
    protected int pickCount = 0;

    protected double totalWinCoin;
    protected int totalFreespin;
    protected int totalFeatureMultiply = 1;
    
    //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
    protected WinAnimation winAnim;
    public GameObject SoundGO;
    //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)

    protected virtual void Init()
    {
        //>>>>>>>> (Chan Ming) (15/3/21)(ARC)
        pickCount = 0;
        totalWinCoin = initialWinCoin;
        totalFreespin = initialFreespin;
        totalFeatureMultiply = initialFeatureMultiply;
        //>>>>>>>> end (Chan Ming) (15/3/21)(ARC)
        UpdateUiText();
        //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
        UpdateTexts(totalWinText, totalWinCoin.ToString(SlotController.Instance.decimalDisplay));
        //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
        foreach (UiPick uP in picks)
        {
            //>>>>>>>>>>>>>>>> (29/3/2021)
            //uP.AnimationReset();
            //>>>>>>>>>>>>>>>> (29/3/2021)
            uP.ResetText();
            uP.AssignOnclick(BonusSelect);
        }
    }

    protected void StartTrigger()
    {
        Init();
        GenerateResult();
    }

    protected void GenerateResult()
    {
        //if (SlotController.Instance.getUseServerResult())
        //{
        //    collectResultId = GetServerResult();
        //}else{
        collectResultId = SuffleResult();
        
        //}
    }
    //List<int> GetServerResult()
    //{
    //    List<int> temp = new List<int>(SlotController.Instance.ServerResult.bonusIds);
    //    List<int> suffle = SuffleResult();

    //    for(int i = 0; i< temp.Count; i++)
    //    {
    //        suffle.Remove(temp[i]);
    //    }
    //    temp.AddRange(suffle);


    //    return temp;
    //}
    protected virtual List<int> SuffleResult()
    {
        List<int> temp = new List<int>();
        List<int> endGameIds = new List<int>();
        for(int i = 0; i< resultPicks.Count; i++)
        {
            if (resultPicks[i].EndGame)
            {
                endGameIds.Add(i);
            }
            else
            {
                temp.Add(i);
            }
            
        }

        ShuffleList<int> shuffleList = new ShuffleList<int>();
        endGameIds = shuffleList.Shuffle(endGameIds);
        temp = shuffleList.Shuffle(temp);
        
        for (int i = 0; i< endGameIds.Count; i++)
        {
            temp.Insert(Random.Range(minPick, temp.Count), endGameIds[i]);
        }
        
        return temp;
    }

    protected void UpdateUiText()
    {
        UpdateTexts(numPickText, (numberOfPick - pickCount).ToString());
        //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
        UpdateTexts(currentBalanceText, SlotPlayer.Instance.Coins.ToString(SlotController.Instance.decimalDisplay));
        UpdateTexts(currentTotalBet, SlotPlayer.Instance.TotalBet.ToString(SlotController.Instance.decimalDisplay));
        //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
        UpdateTexts(totalFreespinText, totalFreespin.ToString());
        UpdateTexts(totalFeatureMultiplyText, totalFeatureMultiply.ToString());
    }

    protected void TweenCoin(Text[] text, float startValue,float endValue)
    {
        foreach(Text t in text)
        {
            //>>>>>>>>>>>>>>>> (23/2/2021)
            TweenNumber.ShowAnimation(t, EaseAnim.EaseLinear, startValue, endValue, 2,SlotController.Instance.decimalDisplay);
            //>>>>>>>>>>>>>>>> end (23/2/2021)
        }

    }

    protected void UpdateTexts(Text[] textArray,string content)
    {
        foreach(Text t in textArray)
        {
            if (t) t.text = content;
        }
    }


    protected virtual void BonusSelect(UiPick uP)
    {

        SimplePick selectedResult = resultPicks[collectResultId[pickCount]];

        uP.SetTrueAnimation(selectedResult.ParameterName);
        uP.PlayAnimationPicked();

        uP.UpdateText(selectedResult.SomeText);
        
        pickCount++;
        ConfirmResult(selectedResult);

        if (pickCount >= numberOfPick || selectedResult.EndGame==true)
        {
            StartCoroutine(EndBonus());
        }
    }

    protected virtual IEnumerator EndBonus()
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
        yield return new WaitForSeconds(waitBeforeDiaplayDetail);
        displayPanel?.SetActive(true);
        TweenCoin(totalWinText, 0, (float)totalWinCoin);
        SaveResult();

    }

    protected void SaveResult()
    {
        SlotPlayer.Instance.toggleIsBonus();
        SlotPlayer.Instance.AddFreeSpins(totalFreespin);
        if (!SlotController.Instance.onlyAddCoinOnEndFeature || SlotPlayer.Instance.FreeSpins == 0) SlotPlayer.Instance.AddCoins(totalWinCoin);
        SlotController.Instance.totalFeatureWin += totalWinCoin;

        SlotController.Instance.featureMultiply = totalFeatureMultiply;
        SlotPlayer.Instance.toggleIsBonus();
       
        SlotPlayer.Instance.saveCoinsUpdate(totalWinCoin, totalFreespin);
    }

    public void OnClickEndBonus()
    {
        hasEndedBonus = true;
        foreach (UiPick pick in picks)
        {
            pick.AnimationReset();
        }
        displayPanel?.SetActive(false);
        //>>>>>>>>>>>>>>>> (yb) (21/6/2021)
        if(SoundGO) SoundGO.SetActive(false);
        //>>>>>>>>>>>>>>>> (yb) end (21/6/2021)
        SlotMenuController.Instance.SetWinInfo(0);
        SlotMenuController.Instance.ClearWinInfoText();
    }

    protected void ConfirmResult(SimplePick selectedResult)
    {
        float oldWin = (float)totalWinCoin;

        totalFreespin += selectedResult.NumberOfFreeSpin;
        totalWinCoin += selectedResult.NumberOfMultiply * SlotPlayer.Instance.TotalBet;
        //>>>>>>>> (Chan Ming) (13/3/21)(ARC)
        if (selectedResult.AddOnMultiply)
        {
            totalFeatureMultiply = totalFeatureMultiply + selectedResult.NumberOfFeatureMultiply;

        }
        else
        {
            totalFeatureMultiply = totalFeatureMultiply * selectedResult.NumberOfFeatureMultiply;

        }
        //>>>>>>>> end (Chan Ming) (13/3/21)(ARC)

        TweenCoin(totalWinText, oldWin, (float)totalWinCoin);
        UpdateUiText();
    }

    public override void OnTrigger()
    {
        base.OnTrigger();
        StartTrigger();
    }

}
//>>>>>>>>>>>>>>>> end (5/2/2021)