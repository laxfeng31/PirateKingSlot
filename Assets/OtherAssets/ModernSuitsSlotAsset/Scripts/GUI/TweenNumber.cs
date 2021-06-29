using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TweenNumber 
{
    public static void ShowAnimation(Text text, EaseAnim typeAnim,  float startNumber, float endNumber, float time, string stringType = "n2", Action callback = null)
    {
        TweenSeq tS = new TweenSeq();


        tS.Add((callBack) => 
        {
            SimpleTween.Value(text.gameObject, startNumber, endNumber, time)
                              .SetOnUpdate((float val) =>
                              {
                                  text.text = (val).ToString(stringType);
                                  

                              })
                              .AddCompleteCallBack(() =>
                              {
                                  if (callBack != null) callBack();
                                  if (callback != null) callback();
                              }).SetEase(typeAnim);
        });
        tS.Start();
    }


}
