using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using DarkTonic.MasterAudio;
public class FeatureGamePause : MonoBehaviour, IRunSlotsAsync
{
    
    [SerializeField]
    List<GameObject> gameObjToTrigger = new List<GameObject>();
    
    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
    [SerializeField]
    bool enablePauseOnFreespin = false;
    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
    bool hasDone = false;
    

    public IEnumerator EndSlotsAsyncRutine()
    {
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        if (SlotController.Instance.IsWiningFreespin && (!SlotController.Instance.PlayFreeSpins || enablePauseOnFreespin))
        {
            //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
            SlotController.Instance.OnClickSpin += TriggerDone;
            //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)
            foreach (GameObject g in gameObjToTrigger)
            {
                g.SetActive(true);
            }
            gameObject.SetActive(true);
            while (!hasDone) yield return new WaitForEndOfFrame();
            hasDone = false;
            foreach (GameObject g in gameObjToTrigger)
            {
                g.SetActive(false);
            }
        }
    }

    
    

    //>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
    public IEnumerator OnLoseSlotsAsyncRutine()
    {
        yield return null;
    }

    public IEnumerator OnWinSlotsAsyncRutine()
    {
        yield return null;
    }
    //>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)
    public void TriggerDone()
    {
        //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
        SlotController.Instance.OnClickSpin -= TriggerDone;
        //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)
        hasDone = true;
    }

    private void Start()
    {
        SlotController.Instance.SubcribeSlotsAsyncList(this);

    }
   
}
