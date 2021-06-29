using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateControl : MonoBehaviour, IStateControl
{
    [Header ("Sprite type")]
    
    public List<SpriteState> listOfSpriteState = new List<SpriteState>();
    public SpriteRenderer spriteRen;
    public Image uiImage;

    [Header("Gameobject type")]
    
    public List<GameObjectState> listOfGameObjState = new List<GameObjectState>();
 
    void Start()
    {
        StateControllerManager.SubcribeStateControl(this);
    }

    public void ChangingState(string idName)
    {

        foreach(GameObjectState gs in listOfGameObjState)
        {
            gs.ChangeState(idName);
        }

        
        foreach (SpriteState s in listOfSpriteState)
        {
            
            if (s.IdName == idName)
            {
                if(spriteRen != null)
                {
                    if (spriteRen.sprite == s.CurrentSprite)
                    {

                        spriteRen.sprite = s.ChangeToSprite;
                    }
                }

                if(uiImage != null)
                {
                    if (uiImage.sprite == s.CurrentSprite)
                    {

                        uiImage.sprite = s.ChangeToSprite;
                    }
                }
                
            }
        }
    }
}

[System.Serializable]
public class SpriteState
{
    [SerializeField]
    private string idName;
    [SerializeField]
    private Sprite currentSprite;
    [SerializeField]
    private Sprite changeToSprite;
   

    public string IdName { get => idName; set => idName = value; }
    public Sprite CurrentSprite { get => currentSprite; set => currentSprite = value; }
    public Sprite ChangeToSprite { get => changeToSprite; set => changeToSprite = value; }


}

[System.Serializable]
public class GameObjectState
{
    
    public string idName;
    public List<GameObject> listOfGameObjToEnable = new List<GameObject>();
    public List<GameObject> listOfGameobjToDisable = new List<GameObject>();

    public void ChangeState(string idName)
    {
        //Debug.Log(idName);
        if(this.idName == idName)
        {
            foreach (GameObject g in listOfGameObjToEnable)
            {
                if(g!= null)g.SetActive(true);
            }

            foreach(GameObject g in listOfGameobjToDisable)
            {
                if (g != null) g.SetActive(false);
            }
        }
        

    }
}
