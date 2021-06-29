using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPage : MonoBehaviour
{
    public GameObject helpPagePrefab;
    public List<HelpPageGroup> helpGroup = new List<HelpPageGroup>();

}
[System.Serializable]
public class HelpPageGroup
{
    public string name;
    public List<HelpPageSetting> helpPageSettings = new List<HelpPageSetting>();
}

[System.Serializable]
public class HelpPageSetting
{
    public Sprite pageImage;
    public List<ButtonSetting> buttonsSetting = new List<ButtonSetting>();

}
[System.Serializable]
public class ButtonSetting
{
    public RectSetting rect = new RectSetting();
    public Sprite sprite;
    public HelpPageButtonType buttonAction;
    public int pageJumpId;
    public string openGroupName;
}

public enum HelpPageButtonType
{
    Close,
    NextPage,
    PreviousPage,

    OpenHelpGroup,
    JumpPage,
}

