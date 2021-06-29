using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;

public class SlotGroupEditorHelper : MonoBehaviour
{
    public Transform raycastParentTrans;
    public SlotGroupBehavior slotGB;
    public Transform maskTrans;
    public Transform specialEffectTrans;
    List<RayCaster> listOfRaycaster = new List<RayCaster>();
    public void AddRaycast(Vector2 pos)
    {
        if(slotGB.RayCasters != null)
        {
            listOfRaycaster = new List<RayCaster>(slotGB.RayCasters);
        }
        else
        {
            listOfRaycaster = new List<RayCaster>();
        }
        
        GameObject newRay = new GameObject("Raycaster " + listOfRaycaster.Count);
        newRay.transform.SetParent(raycastParentTrans);
        newRay.transform.localScale = new Vector3(1, 1, 1);
        newRay.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        RayCaster rayC = newRay.AddComponent<RayCaster>();
        listOfRaycaster.Add(rayC);
        slotGB.RayCasters = listOfRaycaster.ToArray();
    }

    public void RemoveAllRaycasters()
    {
        listOfRaycaster = new List<RayCaster>(slotGB.RayCasters);
        foreach(RayCaster rC in listOfRaycaster)
        {
            GameObject.DestroyImmediate(rC.gameObject);
        }
        listOfRaycaster.Clear();


        slotGB.RayCasters = null;
    }
    
}
