//>>>>>>>>>>>>>>>> (23/3/2021)(ARC)
using DarkTonic.MasterAudio;
using Mkey;
using UnityEngine;

public abstract class BonusGame: MonoBehaviour
{
    [SerializeField]
    protected bool disableBonusOnFeature = false;


    protected bool hasEndedBonus = false;

    public virtual void OnTrigger() 
    {
        
        if (disableBonusOnFeature)
        {
            if (SlotController.Instance.PlayFreeSpins)
            {
                SlotController.Instance.IsOnBonus = false;

                hasEndedBonus = true;
                return;
            }
        }
        SlotController.Instance.IsOnBonus = true;
        MasterAudio.StopEverything();
        hasEndedBonus = false;
        gameObject.SetActive(true);
    }

    public virtual bool HasFinished()
    {
        
        if (hasEndedBonus)
        {
            SlotController.Instance.IsOnBonus = false;
            gameObject.SetActive(false);
        }
        return hasEndedBonus;
    }
}
//>>>>>>>>>>>>>>>> end (23/3/2021)(ARC)