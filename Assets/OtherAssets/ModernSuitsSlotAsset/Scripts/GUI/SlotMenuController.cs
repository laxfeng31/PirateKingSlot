using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using System.Collections.Generic;

namespace Mkey
{
    public class SlotMenuController : MonoBehaviour
    {
        [Header("UI Text")]

        public Text BalanceSumText;
        public Text BalanceDiamondSumText;
        public Text TotalBetSumText;
        public Text LinesCountText;
        public Text LineBetSumText;

        public Text FreeSpinCountText;
        public Text WinCoinText;
        
        public Text ErrorInfo;
        public Text MiniJackpotSumText;
        public Text MaxiJackpotSumText;
        public Text MegaJackpotSumText;
        public Text resultWinText;
        //>>>>>>>>>>>>>>>> (11/3/2021)(ARC)
        public Text standardLineWin;
        //>>>>>>>>>>>>>>>> end (11/3/2021)(ARC)
        public Text totalLineWinCount;
        public Text LineBetSumWithWordsText;
        public Text LinesCountWithWordsText;
        public Text totalFeatureWinInfoText;
        //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
        public Text displayTotalAutospinText;
        public int totalAutoSpinLeft = 0;
        //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
        [Header("UI Button")]

        public Button spinButton;

        public Button autoButton;

        //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
        public List<Button> buttonsCanClickWhileSpin = new List<Button>();

        //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)

        [Header("Others")]

        
        [SerializeField]
        private LinesController linesController;
        [SerializeField]
        [Tooltip("Tween time per 100 coins")]
        private float winCoinsTweenTime = 1f; // per 100 coins

        public bool isSlotRunned;
        public bool enableSpin;
        public bool spinauto;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public bool usePresetBet = false;
        public bool enableDisplayZeroFreespin = false;
        public List<double> presetBetIncrease = new List<double>();
        public int currentSelectedPresetId = 0;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
        //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
        public bool enableStartPresetBet = false;
        //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
        public static SlotMenuController Instance;

        #region temp
        private double winCoins;
        private double oldWinCoins;
        private int coinsTweenId;
        private Button[] buttons;

        private SlotPlayer SPlayer
        {
            get { return SlotPlayer.Instance; }
        }

        #endregion temp


        
        #region regular

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            spinauto = false;
            isSlotRunned = false;
            enableSpin = true;
            Script_Load load = gameObject.GetComponent<Script_Load>();
            // set start button delegates
            if (spinButton)
            {
                spinButton.GetComponent<StartButtonBehavior>().ClickDelegate = () =>
                {
                    
                   // if (enableSpin)
                    //{

                        SlotController.Instance.SpinPress();
                    //}

                };
            }

            if (autoButton)
            {
                autoButton.GetComponent<StartButtonBehavior>().ClickDelegate = () =>
                {
                    //load.checkUserPoint();
                    //if(enableSpin)
                    // {
                    Auto_Click();
                    // }
                };
            }
            buttons = GetComponentsInChildren<Button>();

            // set player event handlers
            SPlayer.ChangeCoinsEvent += ChangeBalanceHandler;

            SPlayer.ChangeLevelEvent += ChangeLevelHandler;
            SPlayer.ChangeFreeSpinsEvent += ChangeFreeSpinsHandler;
            SPlayer.ChangeAutoSpinsEvent += ChangeAutoSpinsHandler;
            SPlayer.ChangeTotalBetEvent += ChangeTotalBetHandler;
            SPlayer.ChangeLineBetEvent += ChangeLineBetHandler;
            SPlayer.ChangeSelectedLinesEvent += ChangeSelectedLinesHandler;

            SPlayer.ChangeMiniJackPotEvent += ChangeMiniJackPotHandler;
            SPlayer.ChangeMaxiJackPotEvent += ChangeMaxiJackPotHandler;
            SPlayer.ChangeMegaJackPotEvent += ChangeMegaJackPotHandler;
            //(Updated)>>>>>>>>>>>>>>>> (30/1/2021)
            if (SlotMenuController.Instance.enableStartPresetBet)
            {
                SPlayer.SetLineBet(presetBetIncrease[currentSelectedPresetId]);
                
            }
            //(Updated)>>>>>>>>>>>>>>>> end (30/1/2021)
            Refresh();
        }

        void OnDestroy()
        {
            if (SPlayer)
            {
                // remove player event handlers
                SPlayer.ChangeCoinsEvent -= ChangeBalanceHandler;

                SPlayer.ChangeLevelEvent -= ChangeLevelHandler;
                SPlayer.ChangeFreeSpinsEvent -= ChangeFreeSpinsHandler;
                SPlayer.ChangeAutoSpinsEvent -= ChangeAutoSpinsHandler;
                SPlayer.ChangeTotalBetEvent -= ChangeTotalBetHandler;
                SPlayer.ChangeLineBetEvent -= ChangeLineBetHandler;
                SPlayer.ChangeSelectedLinesEvent -= ChangeSelectedLinesHandler;

                SPlayer.ChangeMiniJackPotEvent -= ChangeMiniJackPotHandler;
                SPlayer.ChangeMaxiJackPotEvent -= ChangeMaxiJackPotHandler;
                SPlayer.ChangeMegaJackPotEvent -= ChangeMegaJackPotHandler;
            }
        }

        private void OnValidate()
        {
            winCoinsTweenTime = Mathf.Max(0, winCoinsTweenTime);
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
                if (b) b.interactable = activity;
            }
            //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
            foreach (Button b in buttonsCanClickWhileSpin)
            {
                b.interactable = true;
            }
            //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
        }

        /// <summary>
        /// Set all buttons interactble = activity, but startButton = startButtonAcivity
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="startButtonAcivity"></param>
        public void SetControlActivity(bool activity, bool startButtonAcivity)
        {
            if (buttons != null)
            {
                foreach (Button b in buttons)
                {
                    if (b) b.interactable = activity;
                }
            }
            if (spinButton) spinButton.interactable = startButtonAcivity;
            if (autoButton) autoButton.interactable = startButtonAcivity;
            //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
            foreach(Button b in buttonsCanClickWhileSpin)
            {
                b.interactable = true;
            }
            //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
        }

        /// <summary>
        /// Refresh gui data : Balance,  BetCount, freeSpin
        /// </summary>
        private void Refresh()
        {
            #region header refresh
            RefreshBalance();
            RefreshJackPots();
            #endregion header refresh

            #region footer refresh
            RefreshBetLines();
            RefreshSpins();
            RefreshInfo();
            #endregion footer refresh
        }

        public void SetWinInfo(double coins)
        {
            winCoinsTweenTime = Mathf.Max(0, winCoinsTweenTime);
            SimpleTween.Cancel(coinsTweenId, false);
            winCoins = coins;
            double tT = coins / 100f * winCoinsTweenTime;
            tT = Mathf.Min(3, (float)tT);
            if (winCoins > 0)
            {
        
                StartCoroutine(winCoinSound(winCoinsTweenTime));
                coinsTweenId = SimpleTween.Value(gameObject, (float)oldWinCoins, (float)winCoins, winCoinsTweenTime).SetOnUpdate((float val) =>
                {
                    oldWinCoins = val;
                    //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                    if (WinCoinText) WinCoinText.text = oldWinCoins.ToString(SlotController.Instance.decimalDisplay);
                    //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
                }).ID;
            }
            else
                oldWinCoins = 0;
            RefreshInfo();
        }
        public void ClearWinInfoText()
        {
            if (WinCoinText) WinCoinText.text = "";
        }

        //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
        public void UpdateAutoSpinText()
        {
            if (displayTotalAutospinText) displayTotalAutospinText.text = totalAutoSpinLeft == 0? "" : totalAutoSpinLeft.ToString();
        }
        //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
        public void SetWinInfo(string info)
        {
            if (ErrorInfo) ErrorInfo.text = info;
            RefreshInfo();
        }

        public IEnumerator winCoinSound(float winCoinsTweenTime)
        {
            StartCoinSound();
            yield return new WaitForSeconds(winCoinsTweenTime);
            EndCoinSound();
        }
        public void StartCoinSound()
        {
            MasterAudio.PlaySound("WinCoinSound");
        }

        public void EndCoinSound()
        {
            MasterAudio.StopAllOfSound("WinCoinSound");
            MasterAudio.PlaySound("WinCoinEnd");
        }


        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public double previousFeatureWin;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
        public void SetTotalFeatureWinInfo(double featureWin)
        {

            double coins = featureWin;
            //Debug.Log("previousFeatureWin: " + previousFeatureWin + " coins: " + coins);
            if (coins == 0)
            {

                if (totalFeatureWinInfoText) totalFeatureWinInfoText.text = " ";
            }
            else
            {
                if ((coins - previousFeatureWin) > 0)
                {
                    if (totalFeatureWinInfoText)
                    {
                        //Debug.Log("IS counting : " + winCoinsTweenTime);
                        StartCoroutine(winCoinSound(winCoinsTweenTime));
                        //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                        TweenNumber.ShowAnimation(totalFeatureWinInfoText, EaseAnim.EaseLinear, (float)previousFeatureWin, (float)coins, winCoinsTweenTime,SlotController.Instance.decimalDisplay);
                        //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)

                    }
                }

            }
            previousFeatureWin = coins;
            RefreshInfo();
        }



        /// <summary>
        /// Refresh gui balance
        /// </summary>
        private void RefreshBalance()
        {
            if (SPlayer)
            {
                //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                if (BalanceSumText) BalanceSumText.text = SPlayer.Coins.ToString(SlotController.Instance.decimalDisplay);
                //if (BalanceDiamondSumText) BalanceDiamondSumText.text = SPlayer.Diamands.ToString(SlotController.Instance.decimalDisplay);
                //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
            }
        }

        /// <summary>
        /// Refresh gui lines, bet
        /// </summary>
        public void RefreshBetLines()
        {
            if (SPlayer)
            {
                //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                if (LineBetSumText) LineBetSumText.text = SPlayer.LineBet.ToString(SlotController.Instance.decimalDisplay);
                if (LineBetSumWithWordsText) LineBetSumWithWordsText.text = SPlayer.LineBet.ToString(SlotController.Instance.decimalDisplay) + " Bet per line";
                if (TotalBetSumText) TotalBetSumText.text = SPlayer.TotalBet.ToString(SlotController.Instance.decimalDisplay);
                //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
                if (LinesCountText) LinesCountText.text = SPlayer.SelectedLinesCount.ToString();
                if (LinesCountWithWordsText) LinesCountWithWordsText.text = SPlayer.SelectedLinesCount.ToString() + " Lines selected";
            }
        }

        /// <summary>
        /// Refresh gui spins
        /// </summary>
        private void RefreshSpins()
        {
            if (SPlayer)
            {

                if (FreeSpinCountText)
                {
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                    if (!enableDisplayZeroFreespin)
                    {
                        FreeSpinCountText.text = (SPlayer.FreeSpins > 0) ? SPlayer.FreeSpins.ToString() : "";
                    }
                    else
                    {
                        FreeSpinCountText.text =  SPlayer.FreeSpins.ToString();
                    }
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                }

            }
        }

        private void RefreshInfo()
        {
            if (SPlayer)
            {
                if (winCoins == 0)
                {
                    if (WinCoinText)
                    {
                        WinCoinText.text = (SPlayer.TotalBet > 0) ? " " : "Select any slot line!";
                    }
                }
                else
                {
                    //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                    if (WinCoinText)
                        WinCoinText.text = winCoins.ToString(SlotController.Instance.decimalDisplay);
                    //>>>>>>>>>>>>>>>> end (23/2/2021)
                }
            }
        }

        private void RefreshJackPots()
        {
            //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
            if (this && MiniJackpotSumText) MiniJackpotSumText.text = SPlayer.MiniJackPot.ToString(SlotController.Instance.decimalDisplay);
            if (this && MaxiJackpotSumText) MaxiJackpotSumText.text = SPlayer.MaxiJackPot.ToString(SlotController.Instance.decimalDisplay);
            if (this && MegaJackpotSumText) MegaJackpotSumText.text = SPlayer.MegaJackPot.ToString(SlotController.Instance.decimalDisplay);
            //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
        }

        /// <summary>
        /// Set play state to single rotation
        /// </summary>
        public void ResetAuto()
        {
            spinButton.GetComponent<StartButtonBehavior>().ResetAuto();
            //autoButton.GetComponent<StartButtonBehavior>().ResetAuto();
        }

        #region header menu
        public void MainMenu_Click()
        {
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            
            MasterAudio.PlaySound("MainMenu_Click");
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            if (PlayerPrefs.GetString("current_Scene") == "HighwayKing" || PlayerPrefs.GetString("current_Scene") == "SilverBullet" || PlayerPrefs.GetString("current_Scene") == "SafariZone" || PlayerPrefs.GetString("current_Scene") == "CaptainTreasure")
            {
                MasterAudio.PlaySound("12ButtonClick");
            }
            else
            {
                MasterAudio.PlaySound("OnPointerDown");
            }
            GuiController.Instance.ShowMainMenu();
        }

        public void GameInfo_Click(PopUpsController pU)
        {
            GuiController.Instance.ShowPopUp(pU);
        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
        public void Spin_Clicking()
        {
            
            MasterAudio.PlaySound("Spin_Clicking");
            
        }
        
        public void AutoSpin_Clicking()
        {
            
            MasterAudio.PlaySound("AutoSpin_Clicking");
            
        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public void Lobby_Click()
        {
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
            MasterAudio.PlaySound("Lobby_Click");
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            if (PlayerPrefs.GetString("current_Scene") == "HighwayKing" || PlayerPrefs.GetString("current_Scene") == "SilverBullet" || PlayerPrefs.GetString("current_Scene") == "SafariZone" || PlayerPrefs.GetString("current_Scene") == "CaptainTreasure")
            {
                MasterAudio.PlaySound("12ButtonClick");
            }
            else
            {
                MasterAudio.PlaySound("OnPointerDown");
            }
            PlayerPrefs.SetString("current_Scene", "Lobby");
            SceneManager.LoadScene(1);
        }

        public void Level_Click()
        {
            if (SPlayer) GuiController.Instance.ShowLevelXP(SPlayer.LevelProgress);
        }

        
        #endregion header menu

        #region footer menu
        public void LinesPlus_Click()
        {
            linesController.IncSelectedLines();
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
            MasterAudio.PlaySound("LinesPlus_Click");
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            if (PlayerPrefs.GetString("current_Scene") == "HighwayKing" || PlayerPrefs.GetString("current_Scene") == "SilverBullet" || PlayerPrefs.GetString("current_Scene") == "SafariZone" || PlayerPrefs.GetString("current_Scene") == "CaptainTreasure")
            {
                MasterAudio.PlaySound("12ButtonClick");
            }
            else
            {
                MasterAudio.PlaySound("OnPointerDown");
            }
        }

        public void LinesMinus_Click()
        {
            linesController.DecSelectedLines();
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
            MasterAudio.PlaySound("LinesMinus_Click");

            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

            if (PlayerPrefs.GetString("current_Scene") == "HighwayKing" || PlayerPrefs.GetString("current_Scene") == "SilverBullet" || PlayerPrefs.GetString("current_Scene") == "SafariZone" || PlayerPrefs.GetString("current_Scene") == "CaptainTreasure")
            {
                MasterAudio.PlaySound("12ButtonClick");
            }
            else
            {
                MasterAudio.PlaySound("OnPointerDown");
            }
        }
        private int betPhase = 1;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
        public void AddPresetLineBet(int count)
        {
            if ((currentSelectedPresetId + count) < 0)
            {
                return;
            }
            if (presetBetIncrease.Count > (currentSelectedPresetId + count))
            {
                currentSelectedPresetId = currentSelectedPresetId + count;
                SPlayer.SetLineBet(presetBetIncrease[currentSelectedPresetId]);
            }

        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public void LineBetPlus_Click()
        {
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            
            MasterAudio.PlaySound("LineBetPlus_Click");
            if (usePresetBet)
            {
                AddPresetLineBet(1);
            }
            else
            {
                if (PlayerPrefs.GetString("current_Scene") == "HighwayKing" ||
                PlayerPrefs.GetString("current_Scene") == "SilverBullet" ||
                PlayerPrefs.GetString("current_Scene") == "SafariZone" ||
                PlayerPrefs.GetString("current_Scene") == "CaptainTreasure" ||
                PlayerPrefs.GetString("current_Scene") == "BbqKing" ||
                PlayerPrefs.GetString("current_Scene") == "WukongJourney" ||
                PlayerPrefs.GetString("current_Scene") == "KingdomQuarrel" ||
                PlayerPrefs.GetString("current_Scene") == "CasinoNight")
                {
                    if (betPhase < 8 && betPhase >= 1)
                    {
                        betPhase++;
                        switch (betPhase)
                        {
                            case 1:
                                SPlayer.SetLineBet(0.01);
                                break;
                            case 2:
                                SPlayer.SetLineBet(0.05);
                                break;
                            case 3:
                                SPlayer.SetLineBet(0.1);
                                break;
                            case 4:
                                SPlayer.SetLineBet(0.25);
                                break;
                            case 5:
                                SPlayer.SetLineBet(0.50);
                                break;
                            case 6:
                                SPlayer.SetLineBet(1);
                                break;
                            case 7:
                                SPlayer.SetLineBet(5);
                                break;
                            case 8:
                                SPlayer.SetLineBet(10);
                                break;
                        }
                    }
                    MasterAudio.PlaySound("12ButtonClick");
                }
                else if (PlayerPrefs.GetString("current_Scene") == "EgyptDream" ||
                    PlayerPrefs.GetString("current_Scene") == "WongChoy" ||
                    PlayerPrefs.GetString("current_Scene") == "BigProsperity" ||
                    PlayerPrefs.GetString("current_Scene") == "GoldenLotus" ||
                    PlayerPrefs.GetString("current_Scene") == "Rome" ||
                    PlayerPrefs.GetString("current_Scene") == "GoldKing" ||
                    PlayerPrefs.GetString("current_Scene") == "HikingTrip" ||
                    PlayerPrefs.GetString("current_Scene") == "Magic" ||
                    PlayerPrefs.GetString("current_Scene") == "Legend" ||
                    PlayerPrefs.GetString("current_Scene") == "TheDiscovery")
                { 
                    if (betPhase < 14 && betPhase >= 1)
                    {
                        betPhase++;
                        switch (betPhase)
                        {
                            case 1:
                                SPlayer.SetLineBet(0.01);
                                break;
                            case 2:
                                SPlayer.SetLineBet(0.02);
                                break;
                            case 3:
                                SPlayer.SetLineBet(0.03);
                                break;
                            case 4:
                                SPlayer.SetLineBet(0.04);
                                break;
                            case 5:
                                SPlayer.SetLineBet(0.05);
                                break;
                            case 6:
                                SPlayer.SetLineBet(0.06);
                                break;
                            case 7:
                                SPlayer.SetLineBet(0.07);
                                break;
                            case 8:
                                SPlayer.SetLineBet(0.08);
                                break;
                            case 9:
                                SPlayer.SetLineBet(0.09);
                                break;
                            case 10:
                                SPlayer.SetLineBet(0.1);
                                break;
                            case 11:
                                SPlayer.SetLineBet(0.25);
                                break;
                            case 12:
                                SPlayer.SetLineBet(0.5);
                                break;
                            case 13:
                                SPlayer.SetLineBet(1);
                                break;
                            case 14:
                                SPlayer.SetLineBet(2);
                                break;
                        }
                    }
                    MasterAudio.PlaySound("OnPointerDown");
                }
                else
                {
                    if (betPhase < 8 && betPhase >= 1)
                    {
                        betPhase++;
                        switch (betPhase)
                        {
                            case 1:
                                SPlayer.SetLineBet(0.01);
                                break;
                            case 2:
                                SPlayer.SetLineBet(0.02);
                                break;
                            case 3:
                                SPlayer.SetLineBet(0.05);
                                break;
                            case 4:
                                SPlayer.SetLineBet(0.1);
                                break;
                            case 5:
                                SPlayer.SetLineBet(0.25);
                                break;
                            case 6:
                                SPlayer.SetLineBet(0.5);
                                break;
                            case 7:
                                SPlayer.SetLineBet(1);
                                break;
                            case 8:
                                SPlayer.SetLineBet(2);
                                break;
                        }
                    }
                    MasterAudio.PlaySound("OnPointerDown");
                }
            }
            
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
            
        }

        public void LineBetMinus_Click()
        {
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            MasterAudio.PlaySound("LineBetMinus_Click");

           
            if (usePresetBet)
            {
                AddPresetLineBet(-1);
            }
            else
            {
                if (PlayerPrefs.GetString("current_Scene") == "HighwayKing" ||
                PlayerPrefs.GetString("current_Scene") == "SilverBullet" ||
                PlayerPrefs.GetString("current_Scene") == "SafariZone" ||
                PlayerPrefs.GetString("current_Scene") == "CaptainTreasure" ||
                PlayerPrefs.GetString("current_Scene") == "BbqKing" ||
                PlayerPrefs.GetString("current_Scene") == "WukongJourney" ||
                PlayerPrefs.GetString("current_Scene") == "KingdomQuarrel" ||
                PlayerPrefs.GetString("current_Scene") == "CasinoNight")
                {
                    if (betPhase <= 8 && betPhase > 1)
                    {
                        betPhase--;
                        switch (betPhase)
                        {
                            case 1:
                                SPlayer.SetLineBet(0.01);
                                break;
                            case 2:
                                SPlayer.SetLineBet(0.05);
                                break;
                            case 3:
                                SPlayer.SetLineBet(0.1);
                                break;
                            case 4:
                                SPlayer.SetLineBet(0.25);
                                break;
                            case 5:
                                SPlayer.SetLineBet(0.50);
                                break;
                            case 6:
                                SPlayer.SetLineBet(1);
                                break;
                            case 7:
                                SPlayer.SetLineBet(5);
                                break;
                            case 8:
                                SPlayer.SetLineBet(10);
                                break;
                        }
                    }
                    MasterAudio.PlaySound("12ButtonClick");
                }
                else if (PlayerPrefs.GetString("current_Scene") == "EgyptDream" ||
                    PlayerPrefs.GetString("current_Scene") == "WongChoy" ||
                    PlayerPrefs.GetString("current_Scene") == "BigProsperity" ||
                    PlayerPrefs.GetString("current_Scene") == "GoldenLotus" ||
                    PlayerPrefs.GetString("current_Scene") == "Rome" ||
                    PlayerPrefs.GetString("current_Scene") == "GoldKing" ||
                    PlayerPrefs.GetString("current_Scene") == "HikingTrip" ||
                    PlayerPrefs.GetString("current_Scene") == "Magic" ||
                    PlayerPrefs.GetString("current_Scene") == "Legend" ||
                    PlayerPrefs.GetString("current_Scene") == "TheDiscovery")
                {
                    if (betPhase <= 14 && betPhase > 1)
                    {
                        betPhase--;
                        switch (betPhase)
                        {
                            case 1:
                                SPlayer.SetLineBet(0.01);
                                break;
                            case 2:
                                SPlayer.SetLineBet(0.02);
                                break;
                            case 3:
                                SPlayer.SetLineBet(0.03);
                                break;
                            case 4:
                                SPlayer.SetLineBet(0.04);
                                break;
                            case 5:
                                SPlayer.SetLineBet(0.05);
                                break;
                            case 6:
                                SPlayer.SetLineBet(0.06);
                                break;
                            case 7:
                                SPlayer.SetLineBet(0.07);
                                break;
                            case 8:
                                SPlayer.SetLineBet(0.08);
                                break;
                            case 9:
                                SPlayer.SetLineBet(0.09);
                                break;
                            case 10:
                                SPlayer.SetLineBet(0.1);
                                break;
                            case 11:
                                SPlayer.SetLineBet(0.25);
                                break;
                            case 12:
                                SPlayer.SetLineBet(0.5);
                                break;
                            case 13:
                                SPlayer.SetLineBet(1);
                                break;
                            case 14:
                                SPlayer.SetLineBet(2);
                                break;
                        }
                    }
                    MasterAudio.PlaySound("OnPointerDown");
                }
                else
                {
                    if (betPhase <= 8 && betPhase > 1)
                    {
                        betPhase--;
                        switch (betPhase)
                        {
                            case 1:
                                SPlayer.SetLineBet(0.01);
                                break;
                            case 2:
                                SPlayer.SetLineBet(0.02);
                                break;
                            case 3:
                                SPlayer.SetLineBet(0.05);
                                break;
                            case 4:
                                SPlayer.SetLineBet(0.1);
                                break;
                            case 5:
                                SPlayer.SetLineBet(0.25);
                                break;
                            case 6:
                                SPlayer.SetLineBet(0.5);
                                break;
                            case 7:
                                SPlayer.SetLineBet(1);
                                break;
                            case 8:
                                SPlayer.SetLineBet(2);
                                break;
                        }
                    }
                    MasterAudio.PlaySound("OnPointerDown");
                }
            }
            
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  

        }

        public void AutoSpinPlus_Click()
        {
            SPlayer.AddAutoSpins(1);
        }

        public void AutoSpinMinus_Click()
        {
            SPlayer.AddAutoSpins(-1);
        }

        public void MaxBet_Click()
        {
            linesController.SelectAllLines(true);

            //(Updated)>>>>>>>>>>>>>> (4/5/2021)
            SPlayer.SetLineBet(presetBetIncrease[presetBetIncrease.Count - 1]);
            currentSelectedPresetId = presetBetIncrease.Count - 1;
            //SPlayer.SetMaxLineBet();
            //(Updated)>>>>>>>>>>>>>> end (4/5/2021)
        }

        public void Spin_Click()
        {
            //MasterAudio.PlaySound("SpinAutoPress");
            SlotController.Instance.SpinPress();
        }

        public void Auto_Click()
        {
            //MasterAudio.PlaySound("SpinAutoPress");
            
            if (!spinauto)
            {
                StateControllerManager.ChangeState("AutoStop");
                
                if (!SlotController.Instance.IsSpinning)
                {
                    SlotController.Instance.SpinPress();
                }
                spinauto = true;
            }
            else
            {
                //StateControllerManager.ChangeState("AutoSpin");
                spinauto = false;
            }
            SlotController.Instance.IsAutoSpin = spinauto;
        }

        void Update()
        {

        }

        #endregion footer menu

        private string GetMoneyName(int count)
        {
            if (count > 1) return "coins";
            else return "coin";
        }

        #region event handlers
        private void ChangeBalanceHandler(double newBalance)
        {
            //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
            if (this && BalanceSumText) BalanceSumText.text = newBalance.ToString(SlotController.Instance.decimalDisplay);
            //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
            //save.savePressed();
        }

        private void ChangeLevelHandler(int newLevel, int reward, bool useLevelReward)
        {


            if (useLevelReward && reward > 0) GuiController.Instance.ShowMessageLevelUpCongratulation(reward.ToString(), newLevel.ToString(), () => { SPlayer.AddCoins(reward); }, null, null);
        }

		
		private void ChangeFreeSpinsHandler(int newFreeSpinsCount)
		{
			if (this)
			{   
				//if (FreeSpinText) FreeSpinText.gameObject.SetActive(newFreeSpinsCount > 0);
                
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                if (!enableDisplayZeroFreespin)
                {
                    if (FreeSpinCountText) FreeSpinCountText.text = (newFreeSpinsCount > 0) ? newFreeSpinsCount.ToString() : "";
                }
                else
                {
                    if (FreeSpinCountText) FreeSpinCountText.text = newFreeSpinsCount.ToString();
                }
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            }
        }

        private void ChangeAutoSpinsHandler(int newAutoSpinsCount)
        {
            //if (this && AutoSpinsCountText) AutoSpinsCountText.text = newAutoSpinsCount.ToString();
        }

        private void ChangeTotalBetHandler(double newTotalBet)
        {
            //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
            if (this && TotalBetSumText) TotalBetSumText.text = newTotalBet.ToString(SlotController.Instance.decimalDisplay);
            //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
        }

        private void ChangeLineBetHandler(double newLineBet)
        {
            //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
            if (this && LineBetSumText) LineBetSumText.text = newLineBet.ToString(SlotController.Instance.decimalDisplay);
            if (this && LineBetSumWithWordsText) LineBetSumWithWordsText.text = newLineBet.ToString(SlotController.Instance.decimalDisplay) + " Bet per line";
            //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)

        }

        private void ChangeSelectedLinesHandler(int newCount)
        {
            if (this && LinesCountText) LinesCountText.text = newCount.ToString();
            if (this && LinesCountWithWordsText) LinesCountWithWordsText.text = newCount.ToString() + " Lines selected";

        }

        private void ChangeMiniJackPotHandler(int newCount)
        {
            if (this && MiniJackpotSumText) MiniJackpotSumText.text = newCount.ToString();
        }

        private void ChangeMaxiJackPotHandler(int newCount)
        {
            if (this && MaxiJackpotSumText) MaxiJackpotSumText.text = newCount.ToString();
        }

        private void ChangeMegaJackPotHandler(int newCount)
        {
            if (this && MegaJackpotSumText) MegaJackpotSumText.text = newCount.ToString();
        }
        #endregion event handlers
    }
}