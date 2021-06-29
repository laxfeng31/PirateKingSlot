using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
public class StateControllerOnclick : MonoBehaviour
{

    public void UpdateStateControl(string state)
    {
        StateControllerManager.ChangeState(state);
    }

}
//(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
