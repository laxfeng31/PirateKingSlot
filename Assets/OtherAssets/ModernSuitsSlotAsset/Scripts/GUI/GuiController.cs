using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
/*
100219
	fixed  private void PopUpCloseH(PopUpsController pUP)
	old  Destroy(pUP);
	new  Destroy(pUP.gameObject);
	
	fixed  internal bool HasNoPopUp
			old   get { return PopupsList.Count > 0; }
			new   get { return PopupsList.Count == 0; }
        
*/
namespace Mkey
{
    public class GuiController : MonoBehaviour
    {
        public Canvas popup;
        [Header("Popup prefabs", order = 1)]
        [SerializeField]
        private PopUpsController MainMenuWindowPrefab;
        [SerializeField]
        private PopUpsController MessageWindowPrefab;
        [SerializeField]
        private PopUpsController SettingsWindowPrefab;
        [SerializeField]
        private PopUpsController LevelXPWindowPrefab;
        [SerializeField]
        private PopUpsController LevelUpCongratulationPrefab;
        [SerializeField]
        private PopUpsController AboutPrefab;
        [SerializeField]
        private PopUpsController BigDealPrefab;
        [SerializeField]
        private PopUpsController BigWinPrefab;
        [SerializeField]
        private PopUpsController ShopWindowPrefab;
        [SerializeField]
        private PopUpsController GameInfoPrefab3x3;
        [SerializeField]
        private PopUpsController GameInfoPrefab3x5;
        [SerializeField]
        private PopUpsController GameInfoPrefab4x5;

        [SerializeField]
        private PopUpsController MiniJackpotPrefab;
        [SerializeField]
        private PopUpsController MaxiJackpotPrefab;
        [SerializeField]
        private PopUpsController MegaJackpotPrefab;

        [Space(8, order = 0)]
        [Header("Refresh handlers", order = 1)]
        public UnityEvent GUIrefreshers;

        [SerializeField]
        private List<PopUpsController> PopupsList;
        public static GuiController Instance;

        public GameObject HelpPage;
        bool isHelpShow;

        void Awake()
        {
           // Application.targetFrameRate = 35;
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            PopupsList = new List<PopUpsController>();
            isHelpShow = false;
        }

        void Update()
        {
            //if (HelpPage == null)
            //{
            //    //Debug.Log("LOL");
            //    HelpPage = GameObject.Find("HelpPage");
            //}
            //else
            //{
            //    if (!isHelpShow)
            //    {
                    
            //    }
            //    else
            //    {
            //        StateControllerManager.ChangeState("Help");
            //    }
            //}
        }

        private void PopUpOpenH(PopUpsController pUP)
        {
            if (PopupsList.IndexOf(pUP) == -1)
            {
                PopupsList.Add(pUP);
            }
        }

        private void PopUpCloseH(PopUpsController pUP)
        {
            if (PopupsList.IndexOf(pUP) != -1)
            {
                PopupsList.Remove(pUP);
                Destroy(pUP.gameObject);
            }
        }

        internal bool HasNoPopUp
        {
            get { return PopupsList.Count == 0; }
        }

        internal void RefreshGui()
        {
            GUIrefreshers.Invoke();
        }

        private PopUpsController ShowPopup(PopUpsController popup_prefab, Transform parent, Action closeCallBack)
        {
            return ShowPopup(popup_prefab,  parent, null, closeCallBack);
        }

        private PopUpsController ShowPopup(PopUpsController popup_prefab,  Transform parent, Action openCallBack, Action closeCallBack)
        {
            if (!popup_prefab) return null;
            PopUpsController pUp = CreateWindow(popup_prefab, parent);
            if (pUp)
            {
                pUp.PopUpInit(
                    (g) =>
                    {
                        PopUpOpenH(g); if (openCallBack != null) openCallBack();
                    }, (g) =>
                    {
                        PopUpCloseH(g);
                        if (closeCallBack != null) closeCallBack();
                    });
                pUp.ShowWindow();
            }
            return pUp;
        }

        private PopUpsController ShowPopup(PopUpsController popup_prefab, Transform parent, Vector3 position, Action openCallBack, Action closeCallBack)
        {
            if (!popup_prefab) return null;
            PopUpsController pUp = CreateWindow(popup_prefab, parent, position);
            if (pUp)
            {
                pUp.PopUpInit(
                    (g) =>
                    {
                        PopUpOpenH(g); if (openCallBack != null) openCallBack();
                    }, (g) =>
                    {
                        PopUpCloseH(g);
                        if (closeCallBack != null) closeCallBack();
                    });
                pUp.ShowWindow();
            }
            return pUp;
        }

        private PopUpsController CreateWindow(PopUpsController prefab, Transform parent)
        {
            if (!prefab || !parent) return null;
            GameObject gP = (GameObject)Instantiate(prefab.gameObject, parent);
            RectTransform mainRT = gP.GetComponent<RectTransform>();
            mainRT.SetParent(parent);
            WindowOpions winOptions = gP.GetComponent<GuiFader_v2>().winOptions;
            Vector3[] vC = new Vector3[4];
            mainRT.GetWorldCorners(vC);

            RectTransform rt = gP.GetComponent<GuiFader_v2>().guiPanel;
            Vector3[] vC1 = new Vector3[4];
            rt.GetWorldCorners(vC1);
            float height = (vC1[2] - vC1[0]).y;
            float width = (vC1[2] - vC1[0]).x;

            switch (winOptions.instantiatePosition)
            {
                case Position.LeftMiddleOut:
                    rt.position = new Vector3(vC[0].x - width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleOut:
                    rt.position = new Vector3(vC[2].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.MiddleBottomOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y - height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[2].y + height / 2f, rt.position.z);
                    break;
                case Position.LeftMiddleIn:
                    rt.position = new Vector3(vC[0].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleIn:
                    rt.position = new Vector3(vC[2].x - width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.MiddleBottomIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y + height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[2].y - height / 2f, rt.position.z);
                    break;
                case Position.CustomPosition:
                    rt.position = winOptions.position;
                    break;
                case Position.AsIs:
                    break;
                case Position.Center:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
            }
            PopUpsController pUp = gP.GetComponent<PopUpsController>();
            if (pUp)
            {
                pUp.SetControlActivity(false);
            }
            return pUp;
        }

        private PopUpsController CreateWindow(PopUpsController prefab, Transform parent, Vector3 position)
        {
            if (!prefab || !parent) return null;
            GameObject gP = (GameObject)Instantiate(prefab.gameObject, parent);
            RectTransform mainRT = gP.GetComponent<RectTransform>();
            mainRT.SetParent(parent);
            Vector2 sD;
            WindowOpions winOptions = gP.GetComponent<GuiFader_v2>().winOptions;

            Vector3[] vC = new Vector3[4];
            mainRT.GetWorldCorners(vC);

            RectTransform rt = gP.GetComponent<GuiFader_v2>().guiPanel;
            Vector3[] vC1 = new Vector3[4];
            rt.GetWorldCorners(vC1);
            float height = (vC1[2]-vC1[0]).y;
            float width = (vC1[2] - vC1[0]).x;
            if (winOptions == null) winOptions = new WindowOpions();
            winOptions.position =  position;

            switch (winOptions.instantiatePosition)
            {
                case Position.LeftMiddleOut:
                    rt.position = new Vector3(vC[0].x - width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleOut:
                    rt.position = new Vector3(vC[2].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.MiddleBottomOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y - height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x)/2f,  vC[2].y + height / 2f, rt.position.z);
                    break;
                case Position.LeftMiddleIn:
                    rt.position = new Vector3(vC[0].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleIn:
                    rt.position = new Vector3(vC[2].x - width / 2f, (vC[0].y + vC[2].y)/ 2f, rt.position.z);
                    break;
                case Position.MiddleBottomIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y + height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[2].y - height / 2f, rt.position.z);
                    break;
                case Position.CustomPosition:
                    rt.position = winOptions.position;
                    break;
                case Position.AsIs:
                    break;
                case Position.Center:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
            }
            PopUpsController pUp = gP.GetComponent<PopUpsController>();
            if (pUp)
            {
                pUp.SetControlActivity(false);
            }
            return pUp;
        }

        /// <summary>
        /// Set children buttons interactable = activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        #region menus
        public void ShowPopUp(PopUpsController popup_prefab)
        {
            PopUpsController mm = ShowPopup(popup_prefab, popup.transform, null, null);
        }

        public void ShowMainMenu()
        {
            PopUpsController mm = ShowPopup(MainMenuWindowPrefab, popup.transform,  null , null );
        }

        public void ShowSettings()
        {
            PopUpsController mm = ShowPopup(SettingsWindowPrefab,  popup.transform, null);
        }

        public void ShowShop()
        {
            PopUpsController mm = ShowPopup(ShopWindowPrefab,  popup.transform, null);
        }

        public void clickHelps()
        {
            isHelpShow = true;
        }

        public void hideHelps()
        {
            isHelpShow = false;
        }

        internal void ShowRateUs()
        {

        }

        internal void ShowLevelXP(float xp)
        {
            PopUpsController mm = ShowPopup(LevelXPWindowPrefab, popup.transform, null);
            LevelXPController lXP = mm.GetComponent<LevelXPController>();
            if (lXP)
            {
                lXP.XPValue.text = xp.ToString("00.0");
            }
        }
        #endregion menus

        #region messages
        internal void ShowMessageWithCloseButton(string caption, string message, Action cancelCallBack)
        {
            ShowMessageWithYesNoCloseButton(caption, message, null, cancelCallBack, null);
        }

        internal void ShowMessageWithYesCloseButton(string caption, string message, Action yesCallBack, Action cancelCallBack)
        {
            ShowMessageWithYesNoCloseButton(caption, message, yesCallBack, cancelCallBack, null);
        }

        internal void ShowMessageWithYesNoButton(string caption, string message, Action yesCallBack, Action noCallBack)
        {
            ShowMessageWithYesNoCloseButton(caption, message, yesCallBack, null, noCallBack);
        }

        internal void ShowMessageWithYesNoCloseButton(string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            WarningMessController wMC = CreateMessage(MessageWindowPrefab, caption, message, yesCallBack, cancelCallBack, noCallBack);
        }

        internal void ShowMessageLevelUpCongratulation(string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            WarningMessController wMC = CreateMessage(LevelUpCongratulationPrefab, caption, message, yesCallBack,  cancelCallBack, noCallBack);
        }

        internal void ShowMessageAbout(string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            WarningMessController wMC = CreateMessage(AboutPrefab, caption, message, yesCallBack, cancelCallBack, noCallBack);
        }

        internal void ShowMessageBigDeal(string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            WarningMessController wMC = CreateMessage(BigDealPrefab, caption, message, yesCallBack, cancelCallBack, noCallBack);
        }

        internal void ShowMessageBigWin(string caption, string message, Action completeCallBack)
        {
            WarningMessController wMC = CreateMessage(BigWinPrefab, caption, message, null, null, null);
            SimpleTween.Value(gameObject, 0, 1, 3.0f).AddCompleteCallBack(() => { if (completeCallBack != null) completeCallBack(); wMC.CloseWindow(); });
        }

        /// <summary>
        /// Autoclosed message if time >0
        /// </summary>
        /// <param name="time"></param>
        /// <param name="message"></param>
        /// <param name="jackPotType"></param>
        /// <param name="completeCallBack"></param>
        internal void ShowMessageJackpot(float time, string message, JackPotType jackPotType, Action completeCallBack)
        {
            switch (jackPotType)
            {
                case JackPotType.None:
                    completeCallBack?.Invoke();
                    break;
                case JackPotType.Mini:
                    ShowMessageMiniJackpot(time, message, completeCallBack);
                    break;
                case JackPotType.Maxi:
                    ShowMessageMaxiJackpot(time, message, completeCallBack);
                    break;
                case JackPotType.Mega:
                    ShowMessageMegaJackpot(time, message, completeCallBack);
                    break;
            }
        }

        internal void ShowMessageMiniJackpot(float time, string message, Action completeCallBack)
        {
            WarningMessController wMC = CreateMessage(MiniJackpotPrefab, "", message, null, null, null);
            if (time > 0) SimpleTween.Value(gameObject, 0, 1, time).AddCompleteCallBack(() => { if (completeCallBack != null) completeCallBack(); if (wMC) wMC.CloseWindow(); });
        }

        internal void ShowMessageMaxiJackpot(float time, string message, Action completeCallBack)
        {
            WarningMessController wMC = CreateMessage(MaxiJackpotPrefab, "", message, null, null, null);
            if (time > 0) SimpleTween.Value(gameObject, 0, 1, time).AddCompleteCallBack(() => { if (completeCallBack != null) completeCallBack(); if (wMC) wMC.CloseWindow(); });
        }

        internal void ShowMessageMegaJackpot(float time, string message, Action completeCallBack)
        {
            WarningMessController wMC = CreateMessage(MegaJackpotPrefab, "", message, null, null, null);
            if (time > 0) SimpleTween.Value(gameObject, 0, 1, time).AddCompleteCallBack(() => { if (completeCallBack != null) completeCallBack(); if (wMC) wMC.CloseWindow(); });
        }

        public void ShowMessage(string caption, string message, float showTime, Action completeCallBack)
        {
            StartCoroutine(ShowMessageC(caption, message, showTime, completeCallBack));
        }

        private IEnumerator ShowMessageC(string caption, string message, float showTime, Action completeCallBack)
        {
            WarningMessController pUp = CreateMessage(MessageWindowPrefab, caption, message, null, null, null);
            yield return new WaitForSeconds(showTime);
            if (completeCallBack != null) completeCallBack();
            if (pUp)pUp.CloseWindow();
        }
       
        private WarningMessController CreateMessage(PopUpsController prefab, string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            PopUpsController p = CreateWindow(prefab, popup.transform);
            if (!p) return null;
            WarningMessController pUp = p.GetComponent<WarningMessController>();

            pUp.SetControlActivity(false);
            pUp.PopUpInit(new Action<PopUpsController>(PopUpOpenH), (g) =>
            {
                PopUpCloseH(g);
                switch (pUp.Answer)
                {
                    case MessageAnswer.Yes:
                        if (yesCallBack != null) yesCallBack();
                        break;
                    case MessageAnswer.Cancel:
                        if (cancelCallBack != null) cancelCallBack();
                        break;
                    case MessageAnswer.No:
                        if (noCallBack != null) noCallBack();
                        break;
                }
            });
            pUp.SetMessage(caption, message, yesCallBack != null, cancelCallBack != null, noCallBack != null);
            p.ShowWindow();
            return pUp;
        }
        #endregion messages


        [SerializeField]
        private Text newGUIText;
        [SerializeField]
        private GameObject newGUIMessageBox;
        //NEW POPUP
        public void newGUIShowMessage(string content)
        {
            if(newGUIMessageBox) newGUIMessageBox.SetActive(true);
            if (newGUIText) newGUIText.text = content;
        }

    }
    
}

 
