using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
public interface IRunSlotsAsync
{
    IEnumerator EndSlotsAsyncRutine();

    IEnumerator OnWinSlotsAsyncRutine();
    IEnumerator OnLoseSlotsAsyncRutine();
}
//>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)