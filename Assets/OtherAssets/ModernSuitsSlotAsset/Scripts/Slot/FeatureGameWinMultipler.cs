using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Mkey;

public class FeatureGameWinMultipler : MonoBehaviour , IRunSlotsAsync
{
    [SerializeField]
    Text displayTextMultipler;
    [SerializeField]
    Text displayTotalAdditionalWin;
    [SerializeField]
    List<Condition> listOfCondition = new List<Condition>();
    SlotController slotController;
    SlotPlayer slotP;
    double totalFeatureWin;
    double totalAddWinResult;
    bool isPausing;
    bool skip;
    bool isActive;
    void Start()
    {
        slotController = SlotController.Instance;
        slotP = SlotPlayer.Instance;
        slotController.SubcribeSlotsAsyncList(this);
        skip = true;
        isActive = false;
    }
    
    public IEnumerator EndSlotsAsyncRutine()
    {
        
        if (slotP.HasFreeSpin)
        {
            isActive = true;
            if (skip)
            {
                skip = false;
            }
            else
            {
                
                totalFeatureWin += slotController.tempGameWin;
            }
            Debug.Log("Total Win: " + totalFeatureWin);
            
        }

        if (isActive && !slotP.HasFreeSpin)
        {
            
            bool hasWin = false;
            isActive = false;
            totalAddWinResult = 0;
            isPausing = true;
            skip = true;
            foreach (Condition ir in listOfCondition)
            {
                double valueOfCompareLine = slotP.LineBet * ir.multipler;
                double valueOfCompareTotal = slotP.TotalBet * ir.multipler;
                switch (ir.totalWinIs)
                {
                    case ConditionType.MoreThan:

                        if (ir.whichBet == TypeOfBet.LineBet)
                        {

                            if (totalFeatureWin > valueOfCompareLine)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.LineBet;
                                hasWin = true;
                                

                            }
                        }
                        else if (ir.whichBet == TypeOfBet.TotalBet)
                        {
                            if (totalFeatureWin > valueOfCompareTotal)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.TotalBet;
                                
                                hasWin = true;
                            }
                        }


                        break;
                    case ConditionType.MoreAndEqual:
                        if (ir.whichBet == TypeOfBet.LineBet)
                        {

                            if (totalFeatureWin >= valueOfCompareLine)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.LineBet;
                                hasWin = true;
                            }
                        }
                        else if (ir.whichBet == TypeOfBet.TotalBet)
                        {
                            if (totalFeatureWin >= valueOfCompareTotal)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.TotalBet;
                                hasWin = true;
                            }
                        }
                        break;
                    case ConditionType.LessThan:
                        if (ir.whichBet == TypeOfBet.LineBet)
                        {

                            if (totalFeatureWin < valueOfCompareLine)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.LineBet;
                                hasWin = true;

                            }
                        }
                        else if (ir.whichBet == TypeOfBet.TotalBet)
                        {
                            if (totalFeatureWin < valueOfCompareTotal)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.TotalBet;
                                hasWin = true;
                            }
                        }
                        break;
                    case ConditionType.LessAndEqual:
                        if (ir.whichBet == TypeOfBet.LineBet)
                        {

                            if (totalFeatureWin <= valueOfCompareLine)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.LineBet;
                                hasWin = true;
                            }
                        }
                        else if (ir.whichBet == TypeOfBet.TotalBet)
                        {
                            if (totalFeatureWin <= valueOfCompareTotal)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.TotalBet;
                                hasWin = true;
                            }
                        }
                        break;
                    case ConditionType.Equal:
                        if (ir.whichBet == TypeOfBet.LineBet)
                        {

                            if (totalFeatureWin == valueOfCompareLine)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.LineBet;
                                hasWin = true;
                            }
                        }
                        else if (ir.whichBet == TypeOfBet.TotalBet)
                        {
                            if (totalFeatureWin == valueOfCompareTotal)
                            {
                                totalAddWinResult += ir.finalResultMultipler * slotP.TotalBet;
                                hasWin = true;
                            }
                        }
                        break;
                }
                if (hasWin)
                {
                    if(displayTextMultipler != null)
                        displayTextMultipler.text = ir.finalResultMultipler.ToString();
                    if(displayTotalAdditionalWin != null)
                        displayTotalAdditionalWin.text = totalAddWinResult.ToString();
                    slotP.AddCoins(totalAddWinResult);
                    totalFeatureWin = 0;
                    ir.buttonToContinuePaused?.onClick.RemoveAllListeners();
                    ir.buttonToContinuePaused?.onClick.AddListener(() => { isPausing = false; });
                    
                    foreach (UnityEvent ac in ir.eventTrigger)
                    {
                        ac?.Invoke();
                    }
                    if (ir.enablePause)
                    {
                        while (isPausing) yield return new WaitForEndOfFrame();

                        
                    }
                    break;
                }

            }
        }
        yield return null;
    }

    public double GetTotalExtraWin()
    {
        return totalAddWinResult;
    }
    //>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
    public IEnumerator OnWinSlotsAsyncRutine()
    {
        yield return null;
    }

    public IEnumerator OnLoseSlotsAsyncRutine()
    {
        yield return null;
    }
    //>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)
}

public enum TypeOfBet
{
    LineBet,
    TotalBet,
}

public enum ConditionType
{
    MoreThan,
    MoreAndEqual,
    LessThan,
    LessAndEqual,
    Equal,
}


[System.Serializable]
public class Condition
{
    [Header("Condition")]
    public ConditionType totalWinIs;
    public TypeOfBet whichBet;
    public int multipler = 1;
    [Header("Condition Result")]
    public int finalResultMultipler;
    public List<UnityEvent> eventTrigger;
    public bool enablePause;
    public Button buttonToContinuePaused;


}