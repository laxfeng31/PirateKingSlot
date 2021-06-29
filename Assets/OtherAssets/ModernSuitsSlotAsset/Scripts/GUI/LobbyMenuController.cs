using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

namespace Mkey
{
	public class LobbyMenuController : MonoBehaviour
    {
        [Space(8, order = 0)]
        [Header("Header menu objects: ", order = 1)]
        public Image levelSlider;
        public Text LevelNumberText;
        public Text BalanceSumText;
		public Text DealTimeText;
        public Text usernameText;


        [Space(8, order = 0)]
        [Header("Deal timer settings: ", order = 1)]
        public bool enableTimer;
        public bool createNewTimer;
        public int dealTimerDays = 0;
        public int dealTimerHours = 3;
        public int dealTimerMinutes = 0;
        public int dealTimerSeconds = 0;
        public string dealTimerName= "dealTimer";


		public static LobbyMenuController Instance;
        private Button[] buttons;
        private GlobalTimer gT;
        private SlotPlayer SP
        {
            get { return SlotPlayer.Instance; }
        }
		
#region regular
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            buttons = GetComponentsInChildren<Button>();
            gT = new GlobalTimer(dealTimerName, dealTimerDays, dealTimerHours, dealTimerMinutes,dealTimerSeconds, createNewTimer);
            gT.OnTickRestDaysHourMinSec = (d,h,m,s)=> {
              //  Debug.Log("rest: " + d + "d " + h + "h " + m + "m " + s + "s");
                if (DealTimeText) DealTimeText.text = String.Format("{0:00}:{1:00}:{2:00}", h, m, s);
            };
			
			// set player event handlers
			SP.ChangeCoinsEvent+=ChangeBalanceHandler;
			SP.ChangeLevelProgressEvent+=ChangeLevelProgressHandler;
			SP.ChangeLevelEvent+=ChangeLevelHandler;
            usernameText.text = PlayerPrefs.GetString("CurrentName");
            MasterAudio.PlaySound("ampyx-rise-nocopyrightmusic-gmzzkrke1-mmZDG3dG");
            Refresh();
        }

        void Update()
        {
            if (enableTimer)
            {
                //update timer
                gT.Update();
                if (gT.IsTimePassed)
                    gT.Restart();
            }
        }
		
		void OnDestroy()
		{
			if(SP)
			{
				// remove player event handlers
				SP.ChangeCoinsEvent-=ChangeBalanceHandler;
				SP.ChangeLevelProgressEvent-=ChangeLevelProgressHandler;
				SP.ChangeLevelEvent-=ChangeLevelHandler;
			}
		}
#endregion regular

        /// <summary>
        /// Set all buttons interactble = activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            if (buttons == null) return;
            foreach (Button b in buttons)
            {
              if(b)  b.interactable = activity;
            }
        }
        
        public void logout()
        {
            PlayerPrefs.SetInt("CurrentUID", 0);
            PlayerPrefs.SetString("CurrentName", null);
            PlayerPrefs.SetString("UserLoggedIn", null);
            
            SceneManager.LoadScene(0);
        }

        public void proceedToNextScene()
        {
            MasterAudio.StopAllOfSound("ampyx-rise-nocopyrightmusic-gmzzkrke1-mmZDG3dG");
        }

        public void goWebView()
        {
            SceneManager.LoadScene(3);
        }

        public void backFromWebView()
        {
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Refresh gui data : Balance,Level
        /// </summary>
        private void Refresh()
        {
            RefreshLevel();
            RefreshBalance();
        }

        /// <summary>
        /// Refresh gui level
        /// </summary>
        private void RefreshLevel()
        {
            if (SP)
            {
                if (levelSlider) levelSlider.fillAmount = SP.LevelProgress / 100f;
                if (LevelNumberText) LevelNumberText.text = SP.Level.ToString();
            }
        }

        /// <summary>
        /// Refresh gui balance
        /// </summary>
        private void RefreshBalance()
        {
            if (BalanceSumText && SP) BalanceSumText.text = SP.Coins.ToString("0.00");
        }

        public void ResetPlayer()
        {
            SlotPlayer.Instance.SetDefaultData();
        }

        #region header menu
        public void MainMenu_Click()
        {
            GuiController.Instance.ShowMainMenu();
        }

        public void Shop_Click()
        {
            GuiController.Instance.ShowShop();
        }

        public void GameIcon_Click(int level)
        {
            
        }

        public void FaceBook_Click()
        {
            /*if (FBholder.Instance)
            {
                FBholder.Instance.FBlogin();
            }*/
        }

        public void Deal_Click(int level)
        {
            GuiController.Instance.ShowMessageBigDeal("10000", "", () => { GuiController.Instance.ShowShop(); }, () => { }, null);
        }

        public void Level_Click()
        {
          if(SP)  GuiController.Instance.ShowLevelXP(SP.LevelProgress);
        }
        #endregion header menu
		
		#region event handlers
		private void ChangeBalanceHandler(double newBalance)
		{
			 if (this && BalanceSumText) BalanceSumText.text = newBalance.ToString("0.00");
		}
		
		private void ChangeLevelHandler(int newLevel, int reward, bool useLevelReward)
		{
			 if (this && LevelNumberText) LevelNumberText.text = newLevel.ToString();
		}
		
		private void ChangeLevelProgressHandler (float newProgress)
		{
			 if (this && levelSlider) levelSlider.fillAmount = newProgress / 100f;
		}
		#endregion event handlers

    }
}