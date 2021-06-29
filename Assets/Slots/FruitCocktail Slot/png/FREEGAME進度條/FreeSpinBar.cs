using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//>>>>>>>>>>>>>>>> (yb) (14/6/2021)
public class FreeSpinBar : MonoBehaviour
{
    public Text subscribeText;
    public Text freespinTotalAmount;
    public Image FreespinBar;

    public List<Sprite> bar10;
    public List<Sprite> bar15;
    public List<Sprite> bar20;

    //public Sprite[] barImage;

    public List<Sprite> barImage = new List<Sprite>();
    int imageNum = 0;
    private void OnEnable()
    {
        freespinTotalAmount.text = subscribeText.text;

        if (freespinTotalAmount.text == "10")
        {
            for (int i = 0; i < bar10.Count; i++)
            {
                barImage.Add(bar10[i]);
            }
        }
        else if (freespinTotalAmount.text == "15")
        {
            for (int i = 0; i < bar15.Count; i++)
            {
                barImage.Add(bar15[i]);
            }
        }
        else if (freespinTotalAmount.text == "20")
        {
            for (int i = 0; i < bar20.Count; i++)
            {
                barImage.Add(bar20[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        imageNum = int.Parse(subscribeText.text);
        FreespinBar.sprite = barImage[imageNum];
    }

    private void OnDisable()
    {
        imageNum = 0;
        barImage.Clear();
    }
}
//>>>>>>>>>>>>>>>> (yb) end (14/6/2021)