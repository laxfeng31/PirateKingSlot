using Mkey;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiGeneralSetting : MonoBehaviour
{
    public UiImage uiImageSetting;
    public UiGameobject uiGameobjectSetting;
    public UiText uiTextSetting;
    public UiButtons uiButtonSetting;
    public UiLineSetting uiLineSetting;
    public SlotGroupBehaviourSetting slotGroupSetting;
}

[System.Serializable]
public class SlotGroupBehaviourSetting
{
    public Transform SlotGroupParentTrans;
    public GameObject slotGroupPrefab;
}
[System.Serializable]
public class UiLineSetting
{
    public Transform UiLineButtonSceneTrans;
    public Transform lineSceneTrans;
    public GameObject linePrefab;
    public List<LineSetting> listOfLineSetting = new List<LineSetting>();
}

[System.Serializable]
public class LineSetting
{
    public bool toggleOn = false;
    public List<SlotRayCastId> slotGroupBehavior = new List<SlotRayCastId>() { new SlotRayCastId(), new SlotRayCastId(), new SlotRayCastId(), new SlotRayCastId(), new SlotRayCastId()};


    public Material lineMaterial;

    public Color lineColor = Color.white;
    
    public float lineFlashingSpeed = 1f;

    public float lineRendererWidth = 0.2f;
    public float lineSpeed = 100f;

    public WinningBoxSequenceType typeOfBoxWinning = WinningBoxSequenceType.Always_Display;
    public float boxSizeWidth = 4.1f;
    public float boxSizeHeight = 3.13f;
    public UILineButtonBehavior uiButton;
    public UiButtonSetting uiButtonSetting = new UiButtonSetting();
    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
    public UILineButtonBehavior uiButtonSecond;
    public UiButtonSetting uiButtonSecondSetting = new UiButtonSetting();
    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
    public bool enableButton;

    //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
    [System.Serializable]
    public class AnimationWinLine : TransfomSetting
    {
        public bool enableAnimationLine;
        public RuntimeAnimatorController animC;
        public int sortingOrder = 16;
        
    }

    public AnimationWinLine animationWinLine;
    //>>>>>>>>>>>>>>>> (18/2/2021)(ARC)
    public LineSetting()
    {

    }
   
    public LineSetting(List<SlotRayCastId> slotGroupBehavior, Material lineMaterial, Color lineColor, float lineFlashingSpeed, float lineRendererWidth, float lineSpeed, WinningBoxSequenceType typeOfBoxWinning, float boxSizeWidth, float boxSizeHeight)
    {
        this.slotGroupBehavior = slotGroupBehavior;
        this.lineMaterial = lineMaterial;
        this.lineColor = lineColor;
        this.lineFlashingSpeed = lineFlashingSpeed;
        this.lineRendererWidth = lineRendererWidth;
        this.lineSpeed = lineSpeed;
        this.typeOfBoxWinning = typeOfBoxWinning;
        this.boxSizeWidth = boxSizeWidth;
        this.boxSizeHeight = boxSizeHeight;
        this.uiButtonSetting.scale = new Vector3(0, 0, 0);
        this.uiButtonSecondSetting.scale = new Vector3(0, 0, 0);
    }
    //>>>>>>>>>>>>>>>> end (18/2/2021)(ARC)
    //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
}
[System.Serializable]
public class SlotRayCastId
{
    public List<int> ids = new List<int>() ;
    public void Reset()
    {
        for(int i = 0; i < ids.Count; i++)
        {
            ids[i] = -1;
        }
    }
    public SlotRayCastId()
    {

    }
    //>>>>>>>>>>>>>>>> (18/2/2021)(ARC)
    public SlotRayCastId(int groupId, int totalCol)
    {
        ids = new List<int>();
        for (int i = 0; i < totalCol; i++)
        {
            if (i == groupId)
            {
                ids.Add(1);
            }
            else
            {
                ids.Add(-1);
            }

        }

    }
    //>>>>>>>>>>>>>>>> end (18/2/2021)(ARC)
}

[System.Serializable]
public class UiButtons
{
    public Transform UiButtonSceneTrans;
    public UiButtonSetting 
        LinePlus, LineMinus, BetPlus, BetMinus, Back, Menu, AutoSpin, Spin;

}

[System.Serializable]
public class UiText
{
    public Transform UiTextSceneTrans;
    public List<UiTextSetting> listOfUiTextSetting = new List<UiTextSetting>();
}

[System.Serializable]
public class UiButtonSetting: ImageSetting
{

    public Sprite pressedSprite;
    public UnityEvent onClickEvent;
    public bool enableLongPress;
    public float longPressStartTime = 1f;
    public float longPressIntervalTime = 0.5f;

}

public enum TypeOfUiButton
{
    LinePlus,
    LineMinus,
    BetPlus,
    BetMinus,
    Back,
    Menu,
    AutoSpin,
    Spin,
}

[System.Serializable]
public class UiTextSetting: RectSetting
{
    public string name;
    public string content;
    public UiTextSettingType uiTextType;
    public Font font;
    public int fontSize;
    public TextAnchor textAnchor;
    public bool bestFit;
    public int textSizeMax;
    public int textSizeMin;
    public Color textColor = Color.white;
    public bool enableOutline;
    public Color outlineColor;
    public Vector2 outlineDistance;
    public bool enableSorting;
    public int sortingOrder;
    public bool enableGradient;
    public Color topColor;
    public Color bottomColor;
}

public enum UiTextSettingType
{
    standard,
    BalanceSumText,
    TotalBetSumText,
    LinesCountText,
    LineBetSumText,
    FreeSpinCountText,
    WinCoinText,
    ErrorInfo,
    MiniJackpotSumText,
    MaxiJackpotSumText,
    MegaJackpotSumText,
    resultWinText,
    totalLineWinCount,
    LineBetSumWithWordsText,
    LinesCountWithWordsText,
    totalFeatureWinInfoText,
    //>>>>>>>>>>>>>>>> (11/3/2021)(ARC)
    standardLineWin,
    BalanceDiamondSumText,
    //>>>>>>>>>>>>>>>> end (11/3/2021)(ARC)
    //>>>>>>>>>>>>>>>> (31/3/2021)(ARC)
    featuremultiplyText,
    //>>>>>>>>>>>>>>>> end (31/3/2021)(ARC)
}

[System.Serializable]
public class UiGameobject
{
    public Transform UiGameObjectSceneTrans;
    public List<UiGameobjectSetting> listOfUiGameobjSetting = new List<UiGameobjectSetting>();
}

[System.Serializable]
public class UiGameobjectSetting: RectSetting
{
    public GameObject gameObjectPrefab;
}

[System.Serializable]
public class UiImage
{

    public Transform uiImageSceneTrans;
    public List<ImageSetting> listOfImagesSetting = new List<ImageSetting>();
}
[System.Serializable]
public class ImageSetting: RectSetting
{
    public string name;
    public Sprite sprite;
    public bool enableSorting;
    public int sortingOrder;

    public List<SpriteState> listOfSpriteState = new List<SpriteState>();


}
[System.Serializable]
public class RectSetting
{
    public Vector2 position = new Vector3(0, 0);
    public Vector2 size = new Vector2(100, 100);
    public Vector3 scale = new Vector3(1, 1, 1);

}
[System.Serializable]
public class TransfomSetting
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}