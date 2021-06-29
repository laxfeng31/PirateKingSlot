using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace Mkey
{
    public class SlotPlayer : MonoBehaviour
    {
        #region default data
        [Space(10, order = 0)]
        [Header("Default data", order = 1)]
        [Tooltip("Default coins at start")]
        [SerializeField]
        private int defCoinsCount = 500;

        [Tooltip("Default facebook coins")]
        [SerializeField]
        private int defFBCoinsCount = 100;

        [Tooltip("Default free spins at start")]
        [SerializeField]
        private int defFreeSpins = 0;

        [Tooltip("Default max line bet, min =1")]
        [SerializeField]
        private float maxLineBet = 0.2f;

        [Tooltip("Default line bet at start, min = 1")]
        [SerializeField]
        private float defLineBet = 0.01f;

        [Tooltip("Check if you want to add level up reward")]
        [SerializeField]
        private bool useLevelRevard = true;

        [Tooltip("Default level up reward")]
        [SerializeField]
        private int levelUpReward = 3000;

        [Tooltip("Max value of auto spins, min = 1")]
        [SerializeField]
        private int maxAutoSpins = 100;

        [Tooltip("Default auto spins count, min = 1")]
        [SerializeField]
        private int defAutoSpins = 1;

        [Tooltip("Check if you want to show big win congratulation")]
        [SerializeField]
        private bool useMegaWinCongratulation = true;

        [Tooltip("multiply total bet congratulation")]
        [SerializeField]
        public int megaWinMulti = 10;

        [Tooltip("Check if you want to show big win congratulation")]
        [SerializeField]
        private bool useUltraBigCongratulation = true;

        [Tooltip("multiply total bet congratulation")]
        [SerializeField]
        public int ultraBigWinMulti = 30;

        [Tooltip("Check if you want to show big win congratulation")]
        [SerializeField]
        private bool useKingWinCongratulation = true;

        [Tooltip("multiply total bet congratulation")]
        [SerializeField]
        public int kingWinMulti = 50;

        

        [Tooltip("Check if you want to play auto all free spins")]
        [SerializeField]
        private bool autoPlayFreeSpins = true;

        [Space(8)]
        [Header("Jackpot coins")]
        [Tooltip("Mini jackpot sum start value")]
        [SerializeField]
        private int miniStart = 10;

        [Tooltip("Maxi jackpot sum start value")]
        [SerializeField]
        private int maxiStart = 20;

        [Tooltip("Mega jackpot sum start value")]
        [SerializeField]
        private int megaStart = 1000;

        [Space(8)]
        [Tooltip("Check if you want to save coins, level, progress, facebook gift flag, sound settings")]
        [SerializeField]
        private bool saveData = false;

        public string urlWebSlot;
        public string isTPG;
        [SerializeField]
        private Script_Save save;

        [SerializeField]
        private Script_Load load;



        #endregion default data

        #region keys
        private string saveCoinsKey = "mk_slot_coins"; // current coins
        private string saveFbCoinsKey = "mk_slot_fbcoins"; // facebook coins
        private string saveLevelKey = "mk_slot_level"; // current level
        private string saveLevelProgressKey = "mk_slot_level_progress"; // progress to next level %
        private string saveAutoSpinsKey = "mk_slot_autospins"; // current auto spins
        private string saveMiniJackPotKey = "mk_slot_minijackpot"; // current  mini jackpot
        private string saveMaxiJackPotKey = "mk_slot_maxijackpot"; // current  maxi jackpot
        private string saveMegaJackPotKey = "mk_slot_megajackpot"; // current  mega jackpot
        #endregion keys

        #region events
        public Action<double> ChangeCoinsEvent;
        public Action<float> ChangeLevelProgressEvent;
        public Action<int, int, bool> ChangeLevelEvent;
        public Action<int> ChangeFreeSpinsEvent;
        public Action<int> ChangeAutoSpinsEvent;
        public Action<double> ChangeTotalBetEvent;
        public Action<double> ChangeLineBetEvent;
        public Action<int> ChangeSelectedLinesEvent;
        public Action<int> ChangeMiniJackPotEvent;
        public Action<int> ChangeMaxiJackPotEvent;
        public Action<int> ChangeMegaJackPotEvent;
        #endregion events

        #region properties
        public bool SaveData
        {
            get { return saveData; }
        }
        public int FreeSpins
        {
            get; private set;
        }
        public double LineBet
        {
            get; private set;
        }
        public bool HasFreeSpin
        {
            get { return FreeSpins > 0; }
        }
        public double TotalBet
        {
            get { return SlotController.Instance.useARCbetSystem ? LineBet: LineBet * SelectedLinesCount ; }
        }

        public int SelectedLinesCount
        {
            get; private set;
        }

        public bool AnyLineSelected
        {
            get { return SelectedLinesCount > 0; }
        }
        public bool HasMoneyForBet
        {
            get
            {
                double balance = 0d;
                if (SlotController.Instance.isUseDiamond())
                {
                    balance = Coins;
                }
                else
                {
                    balance = Coins;
                }
                return TotalBet <= balance;
            }
        }

        public int MegaWinMulti
        {
            get { return megaWinMulti; }
        }

        public int KingWinMulti
        {
            get { return kingWinMulti; }
        }

        public int MiniJackPotStart
        {
            get { return miniStart; }
        }
        public int MaxiJackPotStart
        {
            get { return maxiStart; }
        }
        public int MegaJackPotStart
        {
            get { return megaStart; }
        }

        public bool AutoPlayFreeSpins
        {
            get { return autoPlayFreeSpins; }
        }
        #endregion properties

        #region saved properties
        public double Coins
        {
            get; private set; 
        }

        public int Level
        {
            get; private set;
        }

        public float LevelProgress
        {
            get; private set;
        }

        public int AutoSpinCount
        {
            get; private set;
        }

        public int MiniJackPot
        {
            get; private set;
        }

        public int MaxiJackPot
        {
            get; private set;
        }

        public int MegaJackPot
        {
            get; private set;
        }
        #endregion saved properties

        public bool isBonus
        {
            get; private set;
        }

        public static SlotPlayer Instance;

        #region regular
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            Validate();
            LoadCoins();
            LoadLevel();
            LoadLevelProgress();
            //LoadFreeSpins();
            LoadLineBet();
            LoadAutoSpins();
            LoadMiniJackPot();
            LoadMaxiJackPot();
            LoadMegaJackPot();
            isBonus = false;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void Update()
        {
            if (GameObject.Find("SlotMenuController"))
            {
                if (!save) save = GameObject.Find("SlotMenuController").GetComponent<Script_Save>();
                if (!load) load = GameObject.Find("SlotMenuController").GetComponent<Script_Load>();
            }
            
        }

        private void OnValidate()
        {
            Validate();
        }

        private void Validate()
        {
            defCoinsCount = Math.Max(0, defCoinsCount);
            defFBCoinsCount = Math.Max(0, defFBCoinsCount);
            defFreeSpins = Math.Max(0, defFreeSpins);

            maxLineBet = Math.Max(0.01f, maxLineBet);
            //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
            //defLineBet = Math.Max(0.01f, defLineBet);
            defLineBet = Mathf.Min(defLineBet, maxLineBet);
            
            //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)

            levelUpReward = Math.Max(0, levelUpReward);

            maxAutoSpins = Math.Max(1, maxAutoSpins);
            defAutoSpins = Math.Max(1, defAutoSpins);
            defAutoSpins = Math.Min(defAutoSpins, maxAutoSpins);

            miniStart = Math.Max(0, miniStart);
            maxiStart = Math.Max(0, maxiStart);
            megaStart = Math.Max(0, megaStart);
        }

        #endregion regular

        #region coins
        /// <summary>
        /// Add coins and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddCoins(double count)
        {
            SetCoinsCount(Coins + count);
        }

        float firstFreeSpin;

        public void saveCoinsUpdate(double count) {

            string modeType = HasFreeSpin ? "FREE" : "NORMAL";
            modeType = isBonus ? "BONUS" : modeType;

            //Debug.Log(firstFreeSpin);
            double newpoint = Coins;
            double oldpoint;
            double activepoint;
            if (modeType == "FREE" )
            {

                if (SlotController.Instance.beforeFeatureGame)
                {
                    activepoint = count - TotalBet;
                    oldpoint = newpoint - count + TotalBet;
                   
                }
                else
                {
                    activepoint = count;
                    oldpoint = newpoint - count;
                }

            } else if (modeType == "BONUS")
            {
                
                activepoint = count;
                oldpoint = newpoint - count;
            }
            else
            {

                if (SlotController.Instance.lastFeatureGame)
                {

                    activepoint = count;
                    oldpoint = newpoint - count;
                } else
                {

                    activepoint = count - TotalBet;
                    oldpoint = newpoint - count + TotalBet;
                }


            }

            double confirmpoint = newpoint - oldpoint;
            //Debug.Log("result" + confirmpoint + " active" + activepoint);
            if (confirmpoint == activepoint)
            {
                if(modeType == "BONUS"){
                    save.saveMatchHistory(oldpoint, activepoint, newpoint, (double)TotalBet, SelectedLinesCount, SlotController.Instance.winController.GetWinLinesCount(), FreeSpins, modeType);
                } else {
                    save.saveMatchHistory(oldpoint, activepoint, newpoint, (double)TotalBet, SelectedLinesCount, SlotController.Instance.winController.GetWinLinesCount(), FreeSpins, modeType);
                }
            }
            else
            {

                //Debug.Log("Result Not Match");

            }

        }

        public void saveCoinsUpdate(double count, int freespincount)
        {
            string modeType = HasFreeSpin ? "FREE" : "NORMAL";
            modeType = isBonus ? "BONUS" : modeType;

            //Debug.Log(firstFreeSpin);
            double newpoint = Coins;
            double oldpoint;
            double activepoint;
            if (modeType == "FREE")
            {
                if (SlotController.Instance.beforeFeatureGame)
                {
                    activepoint = count - TotalBet;
                    oldpoint = newpoint - count + TotalBet;
                    
                }
                else
                {
                    activepoint = count;
                    oldpoint = newpoint - count;
                }

            }
            else if (modeType == "BONUS")
            {
                activepoint = count;
                oldpoint = newpoint - count;
            }
            else
            {
                if (SlotController.Instance.lastFeatureGame)
                {

                    activepoint = count;
                    oldpoint = newpoint - count;
                }
                else
                {

                    activepoint = count - TotalBet;
                    oldpoint = newpoint - count + TotalBet;
                }

            }

            double confirmpoint = newpoint - oldpoint;
            //Debug.Log("result" + confirmpoint + " active" + activepoint);
            if (confirmpoint == activepoint)
            {
                if(modeType == "BONUS"){
                    save.saveMatchHistory(oldpoint, activepoint, newpoint, (double)TotalBet, SelectedLinesCount, SlotController.Instance.winController.GetWinLinesCount(), freespincount, modeType);
                } else {
                    save.saveMatchHistory(oldpoint, activepoint, newpoint, (double)TotalBet, SelectedLinesCount, SlotController.Instance.winController.GetWinLinesCount(), freespincount, modeType);
                }
            }
            else
            {

                Debug.Log("Result Not Match");

            }
        }

        /// <summary>
        /// Set coins and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetCoinsCount(double count)
        {
            //count = Mathf.Max(0, count);
            bool changed = (Coins != count);
            Coins = count;
            if (SaveData && changed)
            {
                string key = saveCoinsKey;
                PlayerPrefs.SetInt(key, (int)Coins);
            }
            if (changed) RaiseEvent(ChangeCoinsEvent, Coins);
        }

        /// <summary>
        /// Add facebook gift (only once), and save flag.
        /// </summary>
        public void AddFbCoins()
        {
            if (!PlayerPrefs.HasKey(saveFbCoinsKey) || PlayerPrefs.GetInt(saveFbCoinsKey) == 0)
            {
                PlayerPrefs.SetInt(saveFbCoinsKey, 1);
                AddCoins(defFBCoinsCount);
            }
        }

        /// <summary>
        /// Load serialized coins or set defaults
        /// </summary>
        private void LoadCoins()
        {
            if (SaveData)
            {
                string key = saveCoinsKey;
                if (PlayerPrefs.HasKey(key)) SetCoinsCount(PlayerPrefs.GetInt(key));
                else SetCoinsCount(defCoinsCount);
            }
            else
            {
                SetCoinsCount(defCoinsCount);
            }
        }
        #endregion coins

        public double getCoins()
        {
            return Coins;
        }

        #region Level
        /// <summary>
        /// Change level and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddLevel(int count)
        {
            SetLevel(Level + count);
        }

        /// <summary>
        /// Set level and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetLevel(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (Level != count);
            int addLevels = count - Level;
            Level = count;
            if (SaveData && changed)
            {
                string key = saveLevelKey;
                PlayerPrefs.SetInt(key, Level);
            }
            if (changed) OnChangeLevelEvent(Level, Mathf.Max(0, addLevels * levelUpReward), useLevelRevard);
        }

        /// <summary>
        /// Load serialized level or set 0
        /// </summary>
        private void LoadLevel()
        {
            if (SaveData)
            {
                string key = saveLevelKey;
                if (PlayerPrefs.HasKey(key))
                {
                    Level = Mathf.Max(0, PlayerPrefs.GetInt(key));
                }
                else
                {
                    Level = 0;
                }
                PlayerPrefs.SetInt(key, Level);
                OnChangeLevelEvent(Level, 0, false);
            }
            else
            {
                Level = 0;
                OnChangeLevelEvent(Level, 0, false);
            }
        }
        #endregion Level

        #region LevelProgress
        /// <summary>
        /// Change level and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddLevelProgress(float count)
        {
            SetLevelProgress(LevelProgress + count);
        }

        /// <summary>
        /// Set level and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetLevelProgress(float count)
        {
            count = Mathf.Max(0, count);
            if (count >= 100)
            {
                int addLevels = (int)count / 100;
                AddLevel(addLevels);
                count = 0;
            }

            bool changed = (LevelProgress != count);
            LevelProgress = count;
            if (SaveData && changed)
            {
                string key = saveLevelProgressKey;
                PlayerPrefs.SetFloat(key, LevelProgress);
            }
            if (changed) RaiseEvent(ChangeLevelProgressEvent, LevelProgress);
        }

        /// <summary>
        /// Load serialized levelprogress or set 0
        /// </summary>
        private void LoadLevelProgress()
        {
            if (SaveData)
            {
                string key = saveLevelProgressKey;
                if (PlayerPrefs.HasKey(key)) SetLevelProgress(PlayerPrefs.GetFloat(key));
                else SetLevelProgress(0);
            }
            else
            {
                SetLevelProgress(0);
            }
        }
        #endregion LevelProgress

        #region FreeSpins
        /// <summary>
        /// Change free spins count and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddFreeSpins(int count)
        {
            SetFreeSpinsCount(FreeSpins + count);
        }

        /// <summary>
        /// Set Free spins count and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetFreeSpinsCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (FreeSpins != count);
            FreeSpins = count;
            if (changed) RaiseEvent(ChangeFreeSpinsEvent, FreeSpins);
        }

        /// <summary>
        /// Load default free spins count
        /// </summary>
        private void LoadFreeSpins()
        {
            SetFreeSpinsCount(defFreeSpins);
        }

        /// <summary>
        /// If has free spins, dec free spin and return true.
        /// </summary>
        /// <returns></returns>
        internal bool ApllyFreeSpin()
        {

            if (HasFreeSpin)
            {
                AddFreeSpins(-1);
                
                return true;
            }
            else
            {

                return false;
            }
        }
        #endregion FreeSpins

        #region LineBet
        /// <summary>
        /// Change line bet and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddLineBet(double count)
        {
            SetLineBet(LineBet + count);
        }

        

        /// <summary>
        /// Set line bet and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetLineBet(double count)
        {
            count = Math.Max(0.01, count);
            count = Math.Min(count, maxLineBet);
            bool changed = (LineBet != count);
            LineBet = count;
            if (changed)
            {
                RaiseEvent(ChangeLineBetEvent, LineBet);
                RaiseEvent(ChangeTotalBetEvent, TotalBet);
            }
        }

        /// <summary>
        /// Load default line bet
        /// </summary>
        private void LoadLineBet()
        {
            SetLineBet(defLineBet);
        }

        internal void SetMaxLineBet()
        {
            SetLineBet(maxLineBet);
        }
        #endregion LineBet

        #region AutoSpins
        /// <summary>
        /// Change auto spins cout and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddAutoSpins(int count)
        {
            SetAutoSpinsCount(AutoSpinCount + count);
        }

        /// <summary>
        /// Set level and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetAutoSpinsCount(int count)
        {
            count = Mathf.Max(1, count);
            count = Mathf.Min(count, maxAutoSpins);
            bool changed = (AutoSpinCount != count);
            AutoSpinCount = count;
            if (SaveData && changed)
            {
                string key = saveAutoSpinsKey;
                PlayerPrefs.SetInt(key, AutoSpinCount);
            }
            if (changed) RaiseEvent(ChangeAutoSpinsEvent, AutoSpinCount);
        }

        /// <summary>
        /// Load serialized auto spins count or set default auto spins count
        /// </summary>
        private void LoadAutoSpins()
        {
            if (SaveData)
            {
                string key = saveAutoSpinsKey;
                if (PlayerPrefs.HasKey(key)) SetAutoSpinsCount(PlayerPrefs.GetInt(key));
                else SetAutoSpinsCount(defAutoSpins);
            }
            else
            {
                SetAutoSpinsCount(defAutoSpins);
            }
        }
        #endregion AutoSpins

        #region mini jackpot
        /// <summary>
        /// Add mini jack pot and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddMiniJackPot(int count)
        {
            SetMiniJackPotCount(MiniJackPot + count);
        }

        /// <summary>
        /// Set mini jackpot and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetMiniJackPotCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (MiniJackPot != count);
            MiniJackPot = count;
            if (SaveData && changed)
            {
                string key = saveMiniJackPotKey;
                PlayerPrefs.SetInt(key, MiniJackPot);
            }
            if (changed) RaiseEvent(ChangeMiniJackPotEvent, MiniJackPot);
        }

        /// <summary>
        /// Load serialized mini jackpot or set defaults
        /// </summary>
        private void LoadMiniJackPot()
        {
            if (SaveData)
            {
                string key = saveMiniJackPotKey;
                if (PlayerPrefs.HasKey(key)) SetMiniJackPotCount(PlayerPrefs.GetInt(key));
                else SetMiniJackPotCount(miniStart);
            }
            else
            {
                SetMiniJackPotCount(miniStart);
            }
        }
        #endregion mini jackpot

        #region maxi jackpot
        /// <summary>
        /// Add maxi jack pot and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddMaxiJackPot(int count)
        {
            SetMaxiJackPotCount(MaxiJackPot + count);
        }

        /// <summary>
        /// Set maxi jackpot and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetMaxiJackPotCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (MaxiJackPot != count);
            MaxiJackPot = count;
            if (SaveData && changed)
            {
                string key = saveMaxiJackPotKey;
                PlayerPrefs.SetInt(key, MaxiJackPot);
            }
            if (changed) RaiseEvent(ChangeMaxiJackPotEvent, MaxiJackPot);
        }

        /// <summary>
        /// Load serialized maxi jackpot or set defaults
        /// </summary>
        private void LoadMaxiJackPot()
        {
            if (SaveData)
            {
                string key = saveMaxiJackPotKey;
                if (PlayerPrefs.HasKey(key)) SetMaxiJackPotCount(PlayerPrefs.GetInt(key));
                else SetMaxiJackPotCount(maxiStart);
            }
            else
            {
                SetMaxiJackPotCount(maxiStart);
            }
        }
        #endregion maxi jackpot

        #region mega jackpot
        /// <summary>
        /// Add mega jack pot and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddMegaJackPot(int count)
        {
            SetMegaJackPotCount(MegaJackPot + count);
        }

        /// <summary>
        /// Set mega jackpot and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetMegaJackPotCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (MegaJackPot != count);
            MegaJackPot = count;
            if (SaveData && changed)
            {
                string key = saveMegaJackPotKey;
                PlayerPrefs.SetInt(key, MegaJackPot);
            }
            if (changed) RaiseEvent(ChangeMegaJackPotEvent, MegaJackPot);
        }

        /// <summary>
        /// Load serialized mega jackpot or set defaults
        /// </summary>
        private void LoadMegaJackPot()
        {
            if (SaveData)
            {
                string key = saveMegaJackPotKey;
                if (PlayerPrefs.HasKey(key)) SetMegaJackPotCount(PlayerPrefs.GetInt(key));
                else SetMegaJackPotCount(megaStart);
            }
            else
            {
                SetMegaJackPotCount(megaStart);
            }
        }
        #endregion mega jackpot

        public void SetDefaultData()
        {
            SetCoinsCount(defCoinsCount);
            PlayerPrefs.SetInt(saveFbCoinsKey, 0); // reset facebook gift
            SetLineBet(defLineBet);
            SetLevel(0);
            SetLevelProgress(0);
            SetAutoSpinsCount(defAutoSpins);
            SetMiniJackPotCount(miniStart);
            SetMaxiJackPotCount(maxiStart);
            SetMegaJackPotCount(megaStart);
        }

        public void ResetPrevSession()
        {
            //LoadFreeSpins();
        }

        /// <summary>
        /// If has money for bet, dec money, and return true
        /// </summary>
        /// <returns></returns>
        internal bool ApplyBet()
        {
            if (HasMoneyForBet)
            {
                if (!HasFreeSpin) {
                    AddCoins(-TotalBet);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void SetSelectedLinesCount(int newSelectedLinesCount)
        {
            SelectedLinesCount = newSelectedLinesCount;
            OnChangeSelectedLinesEvent(newSelectedLinesCount);
            RaiseEvent(ChangeTotalBetEvent, TotalBet);
        }

        #region raise events
        private void RaiseEvent(Action eventAction)
        {
            eventAction?.Invoke();
        }

        private void RaiseEvent<T>(Action<T> eventAction, T p)
        {
            eventAction?.Invoke(p);
        }

        private void OnChangeLevelEvent(int level, int reward, bool useLevelRevard)
        {
            ChangeLevelEvent?.Invoke(level, reward, useLevelRevard);
        }

        private void OnChangeSelectedLinesEvent(int newLinesCount)
        {
            RaiseEvent(ChangeSelectedLinesEvent, newLinesCount);
        }
        #endregion raise events

        public void toggleIsBonus()
        {
            if (isBonus)
                isBonus = false;
            else
                isBonus = true;
        }
    }
}