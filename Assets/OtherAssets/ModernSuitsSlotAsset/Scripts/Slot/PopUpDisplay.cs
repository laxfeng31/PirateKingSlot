using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
public class PopUpDisplay : MonoBehaviour, IRunSlotsAsync
{

    protected virtual void Start()
    {
        SlotController.Instance.SubcribeSlotsAsyncList(this);
    }
    //>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
    public virtual IEnumerator EndSlotsAsyncRutine()
    {



        yield return null;
    }

    public virtual IEnumerator OnWinSlotsAsyncRutine()
    {
        yield return null;
    }

    public virtual IEnumerator OnLoseSlotsAsyncRutine()
    {
        yield return null;
    }
    //>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)
}
