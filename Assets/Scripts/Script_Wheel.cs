using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Script_Wheel : MonoBehaviour
{
    private bool isSpinning;
    public bool Test;
    public AnimationCurve animationCurve;
    public GameObject wheelImg;

    // Start is called before the first frame update
    void Start()
    {
        isSpinning = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spinButton()
    {
        if (Test)
        {
            if (!isSpinning)
            {
                StartCoroutine(Spinning1());
            }
        }
        else
        {
            if (!isSpinning)
            {
                StartCoroutine(Spinning());
            }
        }
        
    }

    IEnumerator Spinning()
    {
        resetWheel();
        isSpinning = true;
        int rand = UnityEngine.Random.Range(1, 8);
        Sequence rotateSeq = DOTween.Sequence();
        switch (rand)
        {
            case 8:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4971f), 7f).SetEase(animationCurve);
                break;
            case 7:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4928f), 7f).SetEase(animationCurve);
                break;
            case 6:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4882f), 7f).SetEase(animationCurve);
                break;
            case 5:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4838f), 7f).SetEase(animationCurve);
                break;
            case 4:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4791f), 7f).SetEase(animationCurve);
                break;
            case 3:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4744f), 7f).SetEase(animationCurve);
                break;
            case 2:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4699f), 7f).SetEase(animationCurve);
                break;
            case 1:
                wheelImg.transform.DOLocalRotate(new Vector3(0, 0, -4658f), 7f).SetEase(animationCurve);
                break;
        }
        yield return new WaitForSeconds(7f);
        isSpinning = false;
    }

    IEnumerator Spinning1()
    {
        
        yield return new WaitForSeconds(2f);

    }

    public void resetWheel()
    {
        wheelImg.transform.localEulerAngles = new Vector3(0,0,0);
    }

}
