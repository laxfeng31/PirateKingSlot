using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControllerManager : MonoBehaviour
{


    static List<IStateControl> listOfStateControl = new List<IStateControl>();

    

    public static void SubcribeStateControl(IStateControl stateConSprite)
    {
        if (!listOfStateControl.Contains(stateConSprite))
        {
            listOfStateControl.Add(stateConSprite);
        }
    }

    public static void UnSubcribeStateControl(IStateControl stateConSprite)
    {
        if (listOfStateControl.Contains(stateConSprite))
        {
            listOfStateControl.Remove(stateConSprite);
        }

    }

    public static void ChangeState(string idName)
    {
        
        foreach(IStateControl sc in listOfStateControl)
        {
            sc.ChangingState(idName);
            
        }
    }

    public void ChangeStateN (string idName)
    {
        foreach (IStateControl sc in listOfStateControl)
        {
            sc.ChangingState(idName);
        }
    }
    
}
