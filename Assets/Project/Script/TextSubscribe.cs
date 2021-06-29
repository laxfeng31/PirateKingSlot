//>>>>>>>>>>>>>>>> (29/3/2021)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using UnityEngine.UI;

public class TextSubscribe : MonoBehaviour
{
    [SerializeField]
    UiTextSettingType textType;
	protected OverrideStandardText text;
    protected virtual void Start()
    {
		Subscribe(text);
	}

    void Subscribe(Text text)
    {
		SlotMenuController slotMenu = SlotMenuController.Instance;
		SlotController slotControl = SlotController.Instance;
		switch (textType)
		{

			case UiTextSettingType.BalanceSumText:
				if (slotMenu)
				{
					slotMenu.BalanceSumText = text;
				}
				break;
			case UiTextSettingType.BalanceDiamondSumText:
				if (slotMenu)
				{
					slotMenu.BalanceDiamondSumText = text;
				}
				break;
			case UiTextSettingType.TotalBetSumText:
				if (slotMenu)
				{
					slotMenu.TotalBetSumText = text;
				}
				break;
			case UiTextSettingType.LinesCountText:
				if (slotMenu)
				{
					slotMenu.LinesCountText = text;
				}
				break;
			case UiTextSettingType.LineBetSumText:
				if (slotMenu)
				{
					slotMenu.LineBetSumText = text;
				}
				break;
			case UiTextSettingType.FreeSpinCountText:
				if (slotMenu)
				{
					slotMenu.FreeSpinCountText = text;
					text.text = SlotPlayer.Instance.FreeSpins.ToString();
				}
				break;
			case UiTextSettingType.WinCoinText:
				if (slotMenu)
				{
					slotMenu.WinCoinText = text;
				}
				break;
			case UiTextSettingType.ErrorInfo:
				if (slotMenu)
				{
					slotMenu.ErrorInfo = text;
				}
				break;
			case UiTextSettingType.MiniJackpotSumText:
				if (slotMenu)
				{
					slotMenu.MiniJackpotSumText = text;
				}
				break;
			case UiTextSettingType.MaxiJackpotSumText:
				if (slotMenu)
				{
					slotMenu.MaxiJackpotSumText = text;
				}
				break;
			case UiTextSettingType.MegaJackpotSumText:
				if (slotMenu)
				{
					slotMenu.MegaJackpotSumText = text;
				}
				break;
			case UiTextSettingType.resultWinText:
				if (slotMenu)
				{
					slotMenu.resultWinText = text;
				}
				break;
			case UiTextSettingType.totalLineWinCount:
				if (slotMenu)
				{
					slotMenu.totalLineWinCount = text;
				}
				break;
			case UiTextSettingType.LineBetSumWithWordsText:
				if (slotMenu)
				{
					slotMenu.LineBetSumWithWordsText = text;
				}
				break;
			case UiTextSettingType.LinesCountWithWordsText:
				if (slotMenu)
				{
					slotMenu.LinesCountWithWordsText = text;
				}
				break;
			case UiTextSettingType.totalFeatureWinInfoText:
				if (slotMenu)
				{
					slotMenu.totalFeatureWinInfoText = text;
				}
				break;
			case UiTextSettingType.standardLineWin:
				if (slotMenu)
				{
					slotMenu.standardLineWin = text;
				}
			
				break;
			case UiTextSettingType.featuremultiplyText:
				if (slotControl)
				{
					slotControl.featureMultiplyText = text;
					text.text = SlotController.Instance.featureMultiply.ToString();
				}
			
				break;
		}
	}

}
//>>>>>>>>>>>>>>>> (29/3/2021)