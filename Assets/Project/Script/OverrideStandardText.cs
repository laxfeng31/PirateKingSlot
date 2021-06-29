//>>>>>>>>>>>>>>>> (29/3/2021)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverrideStandardText : Text
{
    public Action<string> OnTextChange;
    public override string text { get => base.text; 
        set 
        {
            OnTextChange?.Invoke(value);
            //base.text = value; 
        } 
    }

    
}
//>>>>>>>>>>>>>>>> (29/3/2021)
