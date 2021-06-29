﻿using UnityEngine;

namespace Mkey
{
    public class MainMenuController : PopUpsController
    {
        public string ANDROID_RATE_URL;
        public string IOS_RATE_URL;
        public string SUPPORT_URL;

        public void RateUsButton_Click()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#elif UNITY_IOS
            if (!string.IsNullOrEmpty(IOS_RATE_URL)) Application.OpenURL(IOS_RATE_URL);
#else
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#endif
        }

        public void SettingsButton_Click()
        {
            GuiController.Instance.ShowSettings();
        }

        public void HelpButton_Click()
        {
            GuiController.Instance.clickHelps();
            StateControllerManager.ChangeState("Help");
            CloseButton_click();
        }

        public void AboutButton_Click()
        {
            if (GuiController.Instance) GuiController.Instance.ShowMessageAbout("DEVELOPED BY MASTER KEY", "Need Help??", () =>
            {
                Debug.Log("Support"); if (!string.IsNullOrEmpty(SUPPORT_URL)) Application.OpenURL(SUPPORT_URL);
            }, null, null);

           // if (!string.IsNullOrEmpty(SUPPORT_URL)) GuiController.Instance.ShowMessageAbout("DEVELOPED BY MASTER KEY", "Need Help??", () => { Debug.Log("Support"); Application.OpenURL(SUPPORT_URL); }, null, null);
        }
    }
}