//>>>>>>>>>>>>>>>> (29/3/2021)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpriteText : TextSubscribe
{
    [Header("List of Sprite")]
    public List<Sprite> LetterSprite;
    public List<Sprite> NumberSprite;
    public List<Sprite> OperatorSprite;
    public List<Sprite> PunctuationSprite;

    List<SpriteRenderer> listofText = new List<SpriteRenderer>();
   
    public bool enablePunct = false;
    [Header("Other Component")]
    public GameObject textPrefab;

    [Range(0.1f, 10.0f)]
    public float spriteScale;
    public float punctScale;
    public int Layer;

    
    public string currentText;
    // Start is called before the first frame update
    protected override void Start()
    {
        text= this.GetComponent<OverrideStandardText>();
        if(text == null)
        {
            text = gameObject.AddComponent<OverrideStandardText>();
        }
        text.OnTextChange = (newString) =>
        {
            TextLengthManager(newString);
            
        };
        base.Start();

    }

    Sprite GenerateText(char text)
    {
        Sprite temp = null;
        if (Char.IsLetter(text))
        {
            string alpha = "abcdefghijklmnopqrstuvwxyz";
            if(Char.IsUpper(text))
            {
                alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            for(int j = 0; j<alpha.Length; j++)
            {
                if(text == alpha[j])
                {
                    temp = LetterSprite[j]; 
                    break;
                }
            }
        }
        if(Char.IsNumber(text))
        {
            temp = NumberSprite[int.Parse(text.ToString())];
        }
        if(Char.IsSymbol(text))
        {
            string oper = "+-";
            for(int j = 0; j<oper.Length; j++)
            {
                if(text == oper[j])
                {
                    temp = OperatorSprite[j];
                    break;
                }
            }
        }
        if(Char.IsPunctuation(text))
        {
            string punc = ".,!?";
            for(int j = 0; j<punc.Length; j++)
            {
                if(text == punc[j])
                {
                    temp = PunctuationSprite[j];
                    break;
                }
            }
        }

        return temp;
    }

    void TextLengthManager (string newString)
    {
        foreach (SpriteRenderer item in listofText)
        {
            item.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
            item.gameObject.SetActive(false);
        }
        if (listofText.Count -1 < newString.Length)
        {
            for (int i = 0; i <= newString.Length - listofText.Count; i++)
            {
                GameObject empty = Instantiate(textPrefab);
                empty.transform.SetParent(gameObject.transform);
                empty.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
                SpriteRenderer sR = empty.GetComponent<SpriteRenderer>();
                sR.sortingOrder = Layer;
                empty.SetActive(false);
                listofText.Add(sR);
            }
        }
        
        if(!enablePunct)
        {
            newString = newString.Replace(",","");
            newString = newString.Replace(".","");
            newString = newString.Replace("?","");
            newString = newString.Replace("!","");
        }
        for(int i = 0; i< newString.Length; i++)
        {
            listofText[i].sprite = GenerateText(newString[i]);
            listofText[i].gameObject.SetActive(true);
            // if(enablePunct && Char.IsPunctuation(newString[i]))
            // {
            //     listofText[i].gameObject.transform.localScale = new Vector3(punctScale,punctScale,punctScale);
            // }
        }
        
       
    }

    
}

//>>>>>>>>>>>>>>>> (29/3/2021)