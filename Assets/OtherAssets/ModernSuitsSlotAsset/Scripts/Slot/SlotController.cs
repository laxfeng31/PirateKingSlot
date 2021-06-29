using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using static Mkey.SimpleTween;
using UnityEngine.UI;
using UnityEngine.Networking;
using DarkTonic.MasterAudio;
using System.Text;
using InchSky.WebRequest;
using InchSky;


/*
  22.03.18
    Add cycled
    Add Restart
 */

namespace Mkey
{
    public enum WinLineFlashing { All, Sequenced, None }
    public enum WinShowType { JumpSymbols, Zoom, LightFlashing }
    public enum JackPotType { None, Mini, Maxi, Mega }
    public enum JackPotIncType { Const, Percent } // add const value or percent of start value


    public class SlotController : MonoBehaviour
    {
        [SerializeField]
        bool testPattern = false;
        [SerializeField]
        string pattern;
        [SerializeField]
        bool useServerResult = false;
        [SerializeField]
        bool useDiamond = false;
        public bool isUseDiamond()
        {
            return useDiamond;
        }
        public bool getUseServerResult()
        {
            return useServerResult;
        }
        #region main reference
        private SlotMenuController menuController;
        #endregion main reference
        Result serverResult;
        #region icons
        public SlotIcon[] slotIcons;
        #endregion icons

        #region payTable
        public List<PayLine> payTable;
        [SerializeField]
        internal List<PayLine> payTableFull; // extended  if useWild
        #endregion payTable

        #region special major
        public int scatter_id;
        public int wild_id;
        public int wild_multiply = 1;
        public bool useWild;
        public bool useScatter;
        public bool scatterWildSub;
        public bool scatterFollowSeq;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public bool useSpecialIcon;
        public bool specialIconWildSub;
        public bool specialIconFollowSeq;
        public int specialIcon_id;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        #endregion special major

        #region scatter paytable
        public List<ScatterPay> scatterPayTable;

        #endregion scatter paytable

        #region wild paytable
        public List<WildPay> wildPayTable;

        #endregion wild paytable

        #region special Icon paytable
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public List<SpecialIconPay> specialIconPayTable;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        #endregion special Icon paytable

        #region feature mode
        public int featureMultiply = 3;
        public Text featureMultiplyText;
        public double lastWinBeforeBonus;
        #endregion feature mode

        #region prefabs
        public GameObject tilePrefab;
        public GameObject particlesStars;
        #endregion prefabs

        #region slotGroups
        public SlotGroupBehavior[] slotGroupsBeh;
        #endregion slotGroups

        #region tweenTargets
        public Transform bottomJumpTarget;
        public Transform topJumpTarget;
        #endregion tweenTargets

        #region spin options
        [SerializeField]
        private EaseAnim inRotType = EaseAnim.EaseLinear; // in rotation part
        [SerializeField]
        [Tooltip("Time in rotation part, 0-1 sec")]
        private float inRotTime = 0.3f;
        [SerializeField]
        [Tooltip("In rotation part angle, 0-10 deg")]
        private float inRotAngle = 7;

        [Space(16, order = 0)]
        [SerializeField]
        private EaseAnim outRotType = EaseAnim.EaseLinear;   // out rotation part
        [SerializeField]
        [Tooltip("Time out rotation part, 0-1 sec")]
        private float outRotTime = 0.3f;
        [SerializeField]
        [Tooltip("Out rotation part angle, 0-10 deg")]
        private float outRotAngle = 7;

        [Space(16, order = 0)]
        [SerializeField]
        private EaseAnim mainRotateType = EaseAnim.EaseLinear;   // main rotation part
        [SerializeField]
        [Tooltip("Time main rotation part, sec")]
        public float mainRotateTime = 4f;

        public int specialEffectTimeInterval = 3;
        public int autoSpinDelay = 1;
        #endregion spin options

        #region options
        //public WinShowType winShowType = WinShowType.JumpSymbols;
        public WinLineFlashing winLineFlashing = WinLineFlashing.Sequenced;
        public bool winSymbolParticles = true;
        public RNGType RandomGenerator = RNGType.Unity;
        [SerializeField]
        [Tooltip("Multiply win coins by bet multiplier")]
        public bool useLineBetMultiplier = true;
        [SerializeField]
        [Tooltip("Multiply win spins by bet multiplier")]
        public bool useLineBetFreeSpinMultiplier = true;
        [SerializeField]
        [Tooltip("Debug to console predicted symbols")]
        private bool debugPredictSymbols = false;
        #endregion options 

        #region jack pots test
        [Space(8)]
        public int jp_symbol_id = -1;
        public bool useMiniJacPot = false;
        [Tooltip("Count identical symbols on screen")]
        public int miniJackPotCount = 7;
        public bool useMaxiJacPot = false;
        [Tooltip("Count identical symbols on screen")]
        public int maxiJackPotCount = 9;
        public bool useMegaJacPot = false;
        [Tooltip("Count identical symbols on screen")]
        public int megaJackPotCount = 10;
        private JackPotIncType jackPotIncType = JackPotIncType.Const;
        public int jackPotIncValue = 1;
        #endregion jack pots 

        #region levelprogress
        [SerializeField]
        [Tooltip("Multiply level progress by bet multiplier")]
        public bool useLineBetProgressMultiplier = true;
        [SerializeField]
        [Tooltip("Player level progress for loose spin")]
        public float loseSpinLevelProgress = 0.5f;
        [SerializeField]
        [Tooltip("Player level progress for win spin per win line")]
        public float winSpinLevelProgress = 2.0f;
        #endregion level progress

        #region temp vars
        [SerializeField]
        private int slotTilesCount = 30;
        //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
        [SerializeField]
        private float slotRadius = 10.6f;
        //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)
        [HideInInspector]
        public WinController winController;
        private WaitForSeconds wfs1_0;
        private WaitForSeconds wfs0_2;
        private WaitForSeconds wfs0_1;
        private RNG rng; // random numbers generator
        private bool auto = false;
        private int spinCount = 0;
        private int autoCount = 1;
        private bool isSpinning = false;
        private bool playFreeSpins = false;
        private GameObject miniGame;

        private SlotPlayer SPlayer { get { return SlotPlayer.Instance; } }
        private GuiController SGUI { get { return GuiController.Instance; } }

        public bool IsSpinning { get => isSpinning; private set => isSpinning = value; }
        public void SetSlotsRunned(bool this_value)
        {
            isSpinning = this_value;
        }
        public bool PlayFreeSpins { get => playFreeSpins; private set => playFreeSpins = value; }
        public Result ServerResult { get => serverResult; set => serverResult = value; }

        public Script_Save save;
        public string cur_Scene;

        public double totalFeatureWin;
        public double tempGameWin;
        public bool lastFeatureGame;
        public bool beforeFeatureGame;

        public int totalScatterGet;
        public bool soundRunned;
        #endregion temp vars

        #region BigWin

        public Animation megaWinAnimation;
        public Animation ultraBigWinAnimation;
        public Animation kingWinAnimation;
        public Animation fireworkAnimation;
        public GameObject fireworkGO;
        public GameObject megaWinAnimationGO;
        public GameObject ultraBigWinAnimationGO;
        public GameObject kingWinAnimationGO;
        public float winAnimationLoop;
        #endregion BigWin


        //public SpriteState ss = new SpriteState.pressedSprite();
        public static SlotController Instance;
        public int serverFreespin = 0;
        #region regular

        [SerializeField]
        public float tileX = 1.95f;
        [SerializeField]
        public float tileY = 2.55f;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        //(Updated)>>>>>>>>>>>>>>>> (5/2/2021)
        public BonusGame bonusScript;
        //(Updated)>>>>>>>>>>>>>>>> (5/2/2021)

        public bool enableSpecialEffectDuringAuto = false;
        public bool IsWiningFreespin = false;
        public bool disableDisplayStandardWinOnFeature = false;
        public bool enableDisplayWinInfoOnFeature = false;
        public bool enableDisplayWinLineOnAuto = false;
        public bool onlyAddCoinOnEndFeature = false;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        List<LineBehavior> linesB = new List<LineBehavior>();
        //>>>>>>>>>>>>>>>> (lax) (17/5/2021)
        private List<IRunSlotsAsync> slotsAsyncList = new List<IRunSlotsAsync>();
        
        //>>>>>>>>>>>>>>>> (lax) end (17/5/2021)

        //>>>>>>>>>>>>>>>> (23/2/2021)
        public string decimalDisplay = "n2";
        //>>>>>>>>>>>>>>>> end (23/2/2021)

        //>>>>>>>>>>>>>>>> (Leong) (23/2/2021)
        public Sprite[] helpPagesSprite;
        public Vector2 helpPagesSize;

        //>>>>>>>>>>>>>>>> (Leong) end (23/2/2021)
        //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
        public Action OnClickSpin;
        //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)
        public bool useARCbetSystem = false;
        public void SubscribeLineB(LineBehavior line)
        {
            if (!linesB.Contains(line))
            {
                linesB.Add(line);
            }
        }

        public void SubcribeSlotsAsyncList(IRunSlotsAsync slotAsync)
        {
            if (!slotsAsyncList.Contains(slotAsync))
            {
                slotsAsyncList.Add(slotAsync);
            }
        }

        public void UnsubscribeSlotsAsyncList(IRunSlotsAsync slotsAsync)
        {
            if (slotsAsyncList.Contains(slotsAsync))
            {
                slotsAsyncList.Remove(slotsAsync);
            }
        }

        private void OnValidate()
        {
            Validate();
        }

        void Validate()
        {

            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);

            outRotTime = Mathf.Clamp(outRotTime, 0, 1f);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);
            jackPotIncValue = Mathf.Max(0, jackPotIncValue);

            miniJackPotCount = Mathf.Max(7, miniJackPotCount);
            maxiJackPotCount = Mathf.Max(9, maxiJackPotCount);
            megaJackPotCount = Mathf.Max(10, megaJackPotCount);
            if (scatterPayTable != null)
            {
                foreach (var item in scatterPayTable)
                {
                    if (item != null)
                    {
                        item.payMult = Mathf.Max(1, item.payMult);
                    }
                }
            }
        }

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            wfs1_0 = new WaitForSeconds(1.0f);
            wfs0_2 = new WaitForSeconds(0.2f);
            wfs0_1 = new WaitForSeconds(0.1f);
        }

        private int[] tryOut;

        [Serializable]
        class UserData
        {
            public float freespin;
            public double bet;
            public int totalLine;
            public int totalFeatureMultiply;
        }

        [Serializable]
        class ServerPayline
        {
            public int[] line;
            public int pay;
            public int totalBetMult;
            public int freespin;
            public bool reverseWin;
            public bool onlyWinOneWay;
        }

        [Serializable]
        class ServerScatterPaytable
        {
            public int scattersCount;
            public int pay;
            public int freeSpins;
            public int payMult;
            public int totalBetMult;
            public bool enableFeatureMode;
            public int featurePay;
            public int featureFreeSpins;
            public int featurePayMult;
            public int bonusTrigger;
            
        }
        [Serializable]
        class ServerWildPaytable
        {
            public int wildCount;
            public int pay;
            public int freeSpins;
            public int payMult;
            public int totalBetMult;
            public bool enableFeatureMode;
            public int featurePay;
            public int featureFreeSpins;
            public int featurePayMult;
            public int bonusTrigger;

        }

        [Serializable]
        class LoadPaytable
        {
            public List<ServerPayline> listOfPayline;
            public List<ServerScatterPaytable> listOfScatter;
            public List<ServerWildPaytable> listOfWild;
        }

        public void Load()
        {
            if(PlayerPrefs.GetInt("IsDiamong") == 1)
            {
                useDiamond = true;

            }
            else
            {
                useDiamond = false;
            }
            if (useServerResult)
            {



                JSONObject j = new JSONObject();
                j.AddField("gameId", PlayerPrefs.GetInt("CurrenPlayGameId"));
                SetInputActivity(false);

                


            }
        }

        void ErrorConnection ()
        {
            //InchSky.PopUpMessageManager.Instance.WarningMessage("Error", "Error Connection", 3, () => { });
            SlotMenuController.Instance.Lobby_Click();
        }

        IEnumerator Start()
        {
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            if(bonusScript == null)
            {
                bonusScript = FindObjectOfType<BonusGameChoices>();
            }
            // for old asset
            if(bonusScript == null)
            {
                bool foundBonusScript = false;
                foreach(ScatterPay p in scatterPayTable)
                {
                    if(p.WinEventBonusTrigger != null)
                    {
                        bonusScript = p.WinEventBonusTrigger;
                        foundBonusScript = true;
                        break;
                    }
                }
                if (!foundBonusScript)
                {
                    foreach(WildPay p in wildPayTable)
                    {
                        if (p.WinEventBonusTrigger != null)
                        {
                            bonusScript = p.WinEventBonusTrigger;
                            foundBonusScript = true;
                            break;
                        }
                    }
                }
            }
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            //Connect.Instance.ConnectionErrorCallback += ErrorConnection;
            menuController = SlotMenuController.Instance;
            //InchSky.toggleMarquee.Instance.showMarquee();
            if (!useServerResult)
            {
                
                payTableFull = new List<PayLine>();
                for (int j = 0; j < payTable.Count; j++)
                {
                    payTable[j].onlyWinOneWay = true;
                    payTableFull.Add(payTable[j]);
                    if (useWild)
                    {
                        PayLine p = payTable[j];
                        List<PayLine> tempWildLine = new List<PayLine>();
                        tempWildLine = p.GetWildLines(this);
                        for (int l = 0; l < tempWildLine.Count; l++)
                        {
                            tempWildLine[l].reverseWin = payTable[j].reverseWin;
                        }
                        payTableFull.AddRange(tempWildLine);
                    }
                }
                List<PayLine> tempReverseLine = new List<PayLine>();
                for (int k = 0; k < payTableFull.Count; k++)
                {
                    if (payTableFull[k].reverseWin)
                    {
                        PayLine pay = new PayLine(payTableFull[k]);

                        pay.reverseWin = true;
                        pay.onlyWinOneWay = false;
                        payTableFull[k].reverseWin = false;
                        payTableFull[k].onlyWinOneWay = false;
                        Array.Reverse(pay.line);

                        tempReverseLine.Add(pay);
                    }


                }
                payTableFull.AddRange(tempReverseLine);
                winController = new WinController(this, FindObjectsOfType<LineBehavior>());
            }
            

            yield return null;

            // create slots
            int slotsGrCount = slotGroupsBeh.Length;
            checkForceStop = new CheckAbleToForceStop(slotGroupsBeh);
            ReelData[] reelsData = new ReelData[slotsGrCount];
            ReelData reelData;
            int i = 0;
            foreach (SlotGroupBehavior sGB in slotGroupsBeh)
            {
                reelData = new ReelData(sGB.symbOrder);
                reelsData[i++] = reelData;
                //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
                sGB.CreateSlotCylinder(slotIcons, slotTilesCount, tilePrefab, slotRadius);
                //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)
            }

            
            rng = new RNG(RNGType.Unity, reelsData);
            SPlayer?.ResetPrevSession();
            SetInputActivity(true);
            lastFeatureGame = false;
            beforeFeatureGame = false;
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            StoreFireworkRutine = FireworkAnimationRun();
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        }



        void Update()
        {
            if (rng == null) return;
            rng.Update();


        }

        private void OnDestroy()
        {
            //Connect.Instance.ConnectionErrorCallback -= ErrorConnection;
            if (winController != null)
            {

                winController.WinCancel();
            }
        }

        private void OnDisable()
        {
            if (winController != null)
            {

                winController.WinCancel();
            }
        }
        #endregion regular

        /// <summary>
        /// Run slots when you press the button
        /// </summary>
        internal void SpinPress()
        {

            MasterAudio.PlaySound("SpinAutoPress");
            autoCount = SPlayer.AutoSpinCount;
            //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
            
            OnClickSpin?.Invoke();
            //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)
            RunSlots();
        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public float forceStopTime = 0.1f;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public CheckAbleToForceStop checkForceStop;
        public bool isStopping = false;
        public bool hasSpecialEffect;
        public bool IsAutoSpin;
        public bool IsOnBonus;
        bool pendingStop = false;
        //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
        public bool turboMode = false;
        //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
        IEnumerator StopSpinQ()
        {
            StateControllerManager.ChangeState("Spin-Disable");
            while (!(checkForceStop.CheckIsReady() || hasSpecialEffect))
            {
                yield return null;
            }
            while (!isStopping)
            {
                foreach (SimpleTweenObject s in SimpleTween.tweenObjects)
                {
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                    if (s.tweenTime > forceStopTime)
                    {
                        s.tweenTime = forceStopTime;
                    }
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                }
                yield return null;
            }
            
            pendingStop = false;
            checkForceStop.ResetIsReady();
        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        bool pendingSpin = false;
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        private void RunSlots()
        {
            soundRunned = false;

            
            if (IsSpinning)
            {
                
                if (!pendingStop)
                {
                    pendingStop = true;
                    StartCoroutine(StopSpinQ());
                }
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                if (isStopping)
                {
                    pendingSpin = true;
                }
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                return;
            }
            //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
            if (turboMode)
            {
                StartCoroutine(StopSpinQ());
            }
            //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
            if (IsOnBonus) return;

            winController.WinEffectsShow(false, false);
            winController.WinCancel();

            winController.ResetLineWinning();

            if (!SPlayer.AnyLineSelected)
            {
                SGUI.ShowMessage(null, "Please select a any line.", 1.5f, null);
                menuController.ResetAuto();
                return;
            }
            if (!SPlayer.ApllyFreeSpin() && !SPlayer.ApplyBet())
            {
                SGUI.newGUIShowMessage("You have no money.");
                menuController.ResetAuto();
                return;
            }

            if (kingWinAnimationGO) { kingWinAnimationGO.SetActive(false); }
            if (ultraBigWinAnimationGO) { ultraBigWinAnimationGO.SetActive(false); }
            if (megaWinAnimationGO) { megaWinAnimationGO.SetActive(false); }

            StartCoroutine(RunSlotsAsync());
        }

        private void StopAllWinningSound()
        {
            //-------------reset sound-------------
            MasterAudio.StopAllOfSound("WinSound");
            MasterAudio.StopAllOfSound("WinFreeSpinSound");
            MasterAudio.StopAllOfSound("MiniJackport");
            MasterAudio.StopAllOfSound("MaxiJackpot");
            MasterAudio.StopAllOfSound("MegaJackport");
        }

        private void OnStartSpinningCallback()
        {
            IsSpinning = true;
            StateControllerManager.ChangeState("Stop");
            if (!IsAutoSpin) StateControllerManager.ChangeState("AutoSpin-Disable");

            menuController.totalLineWinCount.text = "";
            menuController.resultWinText.text = "";
            //>>>>>>>>>>>>>>>> (11/3/2021)(ARC)
            if(menuController.standardLineWin)menuController.standardLineWin.text = "";
            //>>>>>>>>>>>>>>>> end (11/3/2021)(ARC)
            menuController.SetWinInfo(0);

            //if (!PlayFreeSpins)
            //{
            //    if(menuController.totalFeatureWinInfoText) menuController.totalFeatureWinInfoText.gameObject.SetActive(false);
            //    //menuController.SetTotalWinInfo(0);
            //}
            //else
            //{
            //    if (menuController.totalFeatureWinInfoText) menuController.totalFeatureWinInfoText.gameObject.SetActive(true);
            //}

            autoCount--;


            //1 ---------------start preparation-------------------------------
            SetInputActivity(false);
            winController.HideAllLines();

            //1a ------------- sound -----------------------------------------

            MasterAudio.PlaySound("SpinSound");

            StopAllWinningSound();
        }

        private void OnFinishedSpinningCallback()
        {

            MasterAudio.StopAllOfSound("SpinSound");
            StateControllerManager.ChangeState("Spin");
            if (!IsAutoSpin)
            {
                StateControllerManager.ChangeState("AutoSpin");
            }
            if (PlayFreeSpins)
            {
                StateControllerManager.ChangeState("freeSpin");

                MasterAudio.PlaySound("FeatureGameBG");

            }
            else
            {

                StateControllerManager.ChangeState("normal");
                MasterAudio.StopAllOfSound("FeatureGameBG");
            }
            isStopping = true;
        }

        private void OnWinningCallback()
        {
            //3a ---- show particles, line flasing  -----------
            winController.WinEffectsShow(winLineFlashing == WinLineFlashing.All, winSymbolParticles);
            //MasterAudio.PlaySound("WinSound");
        }

        private void OnWinningCoinCallback()
        {


        }

        private void OnWinningFreeSpinCallback()
        {
            //MasterAudio.PlaySound("WinFreeSpinSound");
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            IsWiningFreespin = true;
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
            MasterAudio.PlaySound("SFX_ToBonusFeature");
        }

        private void OnEndWinningCallback()
        {
            soundRunned = true;

        }

        private IEnumerator RunSlotsAsync()
        {
            
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            pendingSpin = false;
            IsWiningFreespin = false;
            StopAllFirework();
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            OnStartSpinningCallback();
            
            //2 --------start rotating ----------------------------------------
            bool fullRotated = false;

            yield return StartCoroutine(RotateSlotsWithStoreResult(() => { OnFinishedSpinningCallback(); fullRotated = true; }));

            while (!fullRotated) yield return wfs0_2;  // wait 


            //3 --------check result-------------------------------------------
            winController.SearchWinSymbols();

            bool hasLineWin = false;
            bool hasScatterWin = false;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            bool hasSpecialIconWin = false;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            bool hasWildWin = false;
            bool megaWin = false;
            bool ultraBigWin = false;
            bool kingWin = false;


            JackPotType jackPotType = JackPotType.None;
            double winCoins = 0;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            if (winController.HasAnyWinn(ref hasLineWin, ref hasScatterWin, ref hasWildWin,ref hasSpecialIconWin, ref jackPotType))
            {
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                OnWinningCallback();

                //3b --------- check Jack pot -------------
                int jackPotCoins = 0;
                while (!SGUI.HasNoPopUp) yield return wfs0_1;


                switch (jackPotType)
                {
                    case JackPotType.None:
                        break;

                    case JackPotType.Mini:
                        jackPotCoins = SPlayer.MiniJackPot;
                        if (jackPotCoins > 0)
                        {
                            Debug.Log("JP mini: " + jackPotCoins);
                            SPlayer.AddCoins(jackPotCoins);
                            SPlayer.SetMiniJackPotCount(0);

                            MasterAudio.PlaySound("MiniJackport");

                            menuController.SetWinInfo(jackPotCoins);
                            if (SPlayer.HasFreeSpin || auto)
                            {
                                // autoclose after 5 sec
                                SGUI.ShowMessageJackpot(5, jackPotCoins.ToString(), JackPotType.Mini, null);
                                yield return new WaitForSeconds(5.0f); // delay
                            }
                            else
                            {
                                SGUI.ShowMessageJackpot(0, jackPotCoins.ToString(), JackPotType.Mini, null);
                                yield return new WaitForSeconds(1.5f); // delay
                            }

                        }
                        break;
                    case JackPotType.Maxi:
                        jackPotCoins = SPlayer.MaxiJackPot;
                        if (jackPotCoins > 0)
                        {
                            Debug.Log("JP maxi: " + jackPotCoins);
                            SPlayer.AddCoins(jackPotCoins);
                            SPlayer.SetMaxiJackPotCount(0);

                            MasterAudio.PlaySound("MaxiJackpot");

                            menuController.SetWinInfo(jackPotCoins);
                            if (SPlayer.HasFreeSpin || auto)
                            {
                                // autoclose after 5 sec
                                SGUI.ShowMessageJackpot(5, jackPotCoins.ToString(), JackPotType.Maxi, null);
                                yield return new WaitForSeconds(5.0f); // delay
                            }
                            else
                            {
                                SGUI.ShowMessageJackpot(0, jackPotCoins.ToString(), JackPotType.Maxi, null);
                                yield return new WaitForSeconds(2.0f);// delay
                            }

                        }
                        break;
                    case JackPotType.Mega:
                        jackPotCoins = SPlayer.MegaJackPot;
                        if (jackPotCoins > 0)
                        {
                            Debug.Log("JP mega: " + jackPotCoins);
                            SPlayer.AddCoins(jackPotCoins);
                            SPlayer.SetMegaJackPotCount(0);

                            MasterAudio.PlaySound("MegaJackport");

                            menuController.SetWinInfo(jackPotCoins);
                            if (SPlayer.HasFreeSpin || auto)
                            {
                                // autoclose after 5 sec
                                SGUI.ShowMessageJackpot(5, jackPotCoins.ToString(), JackPotType.Mega, null);
                                yield return new WaitForSeconds(5.0f); // delay
                            }
                            else
                            {
                                SGUI.ShowMessageJackpot(0, jackPotCoins.ToString(), JackPotType.Mega, null);
                                yield return new WaitForSeconds(3.0f);// delay
                            }
                        }
                        break;
                }

                //3c0 -----------------calc coins -------------------
                winCoins = winController.GetWinCoins();

                //if (useLineBetMultiplier) winCoins *= SPlayer.LineBet;
                winCoins += winController.GetWinTotalBetMulti();
                tempGameWin = winCoins;

                //3c1 ----------- calc free spins ----------------
                float winSpins = winController.GetWinSpins();
                int winLinesCount = winController.GetWinLinesCount();

                //SPlayer.AddFreeSpins(winSpins);
                

                if (PlayFreeSpins)
                {

                    
                    totalFeatureWin += winCoins * featureMultiply;
                    if (!onlyAddCoinOnEndFeature)
                    {
                        SPlayer.AddCoins(winCoins * featureMultiply);
                    }
                    
                    menuController.SetTotalFeatureWinInfo(totalFeatureWin);
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                    if (enableDisplayWinInfoOnFeature)
                    {
                        if(!disableDisplayStandardWinOnFeature)
                            menuController.SetWinInfo(winCoins * featureMultiply);
                       //(Updated)>>>>>>>>>>>>>>>> (25/1/2021)
                        if (SlotMenuController.Instance.totalFeatureWinInfoText) SlotMenuController.Instance.totalFeatureWinInfoText.gameObject.SetActive(true);
                        //(Updated)>>>>>>>>>>>>>>>> end (25/1/2021)
                    }
                    //(Updated)>>>>>>>>>>>>>>>> end (8/12/2020) 
                   
                     
                }
                else
                {
                    //if (SlotMenuController.Instance.InfoText) SlotMenuController.Instance.InfoText.gameObject.SetActive(true);
                    menuController.SetWinInfo(jackPotCoins + winCoins);
                    if (SlotMenuController.Instance.totalFeatureWinInfoText) SlotMenuController.Instance.totalFeatureWinInfoText.gameObject.SetActive(false);
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                    SlotMenuController.Instance.previousFeatureWin = 0;
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
                    totalFeatureWin = winCoins;
                    lastWinBeforeBonus = winCoins;
                    SPlayer.AddCoins(winCoins);
                    
                    
                    
                }


                

                if (winCoins > 0)
                {
                    OnWinningCoinCallback();
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                    menuController.totalLineWinCount.text = winController.GetWinLinesCount() > 0 ? "You won on " + winController.GetWinLinesCount() + " lines":"";
                    //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
                    megaWin = (winCoins >= (SPlayer.TotalBet * SPlayer.megaWinMulti));
                    ultraBigWin = (winCoins >= (SPlayer.TotalBet * SPlayer.ultraBigWinMulti));
                    kingWin = (winCoins >= (SPlayer.TotalBet * SPlayer.kingWinMulti));

                }

               


                
                if (winSpins > 0)
                {
                    OnWinningFreeSpinCallback();

                }



                SPlayer.AddFreeSpins((int)winSpins);

                
                

                // 3d ----- increase jackpots ----
                if (useMiniJacPot) SPlayer.AddMiniJackPot((jackPotIncType == JackPotIncType.Const) ?
                    jackPotIncValue : (int)((float)SPlayer.MiniJackPotStart * (float)jackPotIncValue / 100f));
                if (useMaxiJacPot) SPlayer.AddMaxiJackPot((jackPotIncType == JackPotIncType.Const) ?
                      jackPotIncValue : (int)((float)SPlayer.MaxiJackPotStart * (float)jackPotIncValue / 100f));
                if (useMegaJacPot) SPlayer.AddMegaJackPot((jackPotIncType == JackPotIncType.Const) ?
                      jackPotIncValue : (int)((float)SPlayer.MegaJackPotStart * (float)jackPotIncValue / 100f));

                // 3d1 -------- add levelprogress --------------
                while (!SGUI.HasNoPopUp) yield return wfs0_1; // wait for the prev popup to close
                SPlayer.AddLevelProgress((useLineBetProgressMultiplier) ? (float)(winSpinLevelProgress * winLinesCount * SPlayer.LineBet) : winSpinLevelProgress * winLinesCount); // for each win line

                // 3d2 ------------ start line events ----------
                winController.StartLineEvents();


                //3f ----------- show line effects, events can be interrupted by player----------------
                bool showEnd = false;
                //>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
                foreach (IRunSlotsAsync sA in slotsAsyncList)
                {
                    yield return StartCoroutine(sA.OnWinSlotsAsyncRutine());
                }
                //>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)
                //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                winController.WinSymbolShowContinuous((IsAutoSpin || PlayFreeSpins),
                                    (windata) => //linewin
                                    {
                                        //event can be interrupted by player
                                        if (windata != null)
                                        {
                                            //menuController.resultWinText.text = "You Won " + windata.Pay + " on Line " + windata.LineBehaviourNum;
                                            //Debug.Log("lineWin : " + windata.ToString());
                                        }
                                    },
                                    () => //scatter win
                                    {
                                        //event can be interrupted by player
                                    },
                                    () => //special Icon win
                                    {
                                        //event can be interrupted by player
                                    },
                                    () => //jack pot 
                                    {
                                        //event can be interrupted by player
                                    },
                                    () => //wild win
                                    {
                                        //event can be interrupted by player
                                    },
                                    () =>
                                    {
                                        OnEndWinningCallback();
                                        showEnd = true;
                                    }
                );
                //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                if (kingWin)
                {

                    if (kingWinAnimationGO != null)
                    {
                        if (fireworkGO) { StartCoroutine(StoreFireworkRutine); }

                        StartCoroutine(kingWinningAnimationRun());
                    }

                }
                else if (ultraBigWin)
                {
                    if (ultraBigWinAnimationGO != null)
                    {
                        if (fireworkGO) { StartCoroutine(StoreFireworkRutine); };


                        StartCoroutine(ultraBigWinningAnimationRun());
                    }
                }
                else if (megaWin)
                {
                    if (megaWinAnimationGO != null)
                    {
                        if (fireworkGO) { StartCoroutine(StoreFireworkRutine); };


                        StartCoroutine(megaWinningAnimationRun());
                    }
                }
  

                while (((IsAutoSpin || PlayFreeSpins) && (!showEnd || fireworkIsPlaying)) && !pendingSpin)
                {
                    
                    yield return wfs0_2;    // wait for show end
                }
                

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)



                
                //>>>>>>>>>>>>>>>> (25/3/2021)
                if (useServerResult)
                {

                }
                else
                {
                    if (winController.GetFeatureMultiply() > 0)
                    {
                        featureMultiply = winController.GetFeatureMultiply();
                    }
                }
                // ----- invoke scatter win event -----------
                if (winController.scatterWin != null)
                {

                    StopAllWinningSound();
                    string bonusSound = winController.scatterWin.ScatterP.soundBonusTrigger;
                    if (!string.IsNullOrEmpty(bonusSound))
                    {
                        if (!IsOnBonus)
                        {
                            MasterAudio.PlaySound(bonusSound);

                        }
                    }
                    if (winController.scatterWin.ScatterP.hasBonus)
                    {
                        //>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
                        yield return new WaitForSeconds(3);
                        bonusScript.OnTrigger();

                        if (fireworkIsPlaying) StopAllFirework();
                        winController.WinCancel();
                        //>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)
                        while (!bonusScript.HasFinished() &&
                           winController.scatterWin.ScatterP.enablePauseAutoPlay) yield return wfs0_2;
                        StopWinEffect();
                    }
                }
                //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                // ----- invoke special icons win event -----------
                if (winController.specialIconWin != null)
                {

                    StopAllWinningSound();
                    string bonusSound = winController.specialIconWin.SpecialIconP.soundBonusTrigger;
                    if (!string.IsNullOrEmpty(bonusSound))
                    {
                        if (!IsOnBonus)
                        {
                            MasterAudio.PlaySound(bonusSound);

                        }
                    }
                    if (winController.specialIconWin.SpecialIconP.hasBonus)
                    {
                        //>>>>>>>>>>>>>>>> (29/3/2021)
                        yield return new WaitForSeconds(3);
                        bonusScript.OnTrigger();

                        if (fireworkIsPlaying) StopAllFirework();
                        winController.WinCancel();
                        //>>>>>>>>>>>>>>>> end (29/3/2021)
                        while (!bonusScript.HasFinished() &&
                           winController.specialIconWin.SpecialIconP.enablePauseAutoPlay) yield return wfs0_2;
                        StopWinEffect();
                    }
                }
                //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                // ----- invoke wild win event -----------
                if (winController.wildWin != null)
                {
                    StopAllWinningSound();
                    string bonusSound = winController.wildWin.WildP.soundBonusTrigger;
                    if (!string.IsNullOrEmpty(bonusSound))
                    {
                        if (!IsOnBonus)
                        {
                            MasterAudio.PlaySound(bonusSound);

                        }
                    }
                    if (winController.wildWin.WildP.hasBonus)
                    {
                        //>>>>>>>>>>>>>>>> (29/3/2021)

                        yield return new WaitForSeconds(3);
                        bonusScript.OnTrigger();

                        if (fireworkIsPlaying) StopAllFirework();
                        winController.WinCancel();
                        //>>>>>>>>>>>>>>>> end (29/3/2021)
                        while (!bonusScript.HasFinished() &&
                            winController.wildWin.WildP.enablePauseAutoPlay) yield return wfs0_2;
                        StopWinEffect();
                    }
                }
                //>>>>>>>>>>>>>>>> end (25/3/2021)

            } // end win
            else // lose
            {
                tempGameWin = 0;
                SPlayer.saveCoinsUpdate(0);
                // 3d ----- increase jackpots ----
                if (useMiniJacPot) SPlayer.AddMiniJackPot((jackPotIncType == JackPotIncType.Const) ?
                    jackPotIncValue : (int)((float)SPlayer.MiniJackPotStart * (float)jackPotIncValue / 100f));
                if (useMaxiJacPot) SPlayer.AddMaxiJackPot((jackPotIncType == JackPotIncType.Const) ?
                      jackPotIncValue : (int)((float)SPlayer.MaxiJackPotStart * (float)jackPotIncValue / 100f));
                if (useMegaJacPot) SPlayer.AddMegaJackPot((jackPotIncType == JackPotIncType.Const) ?
                      jackPotIncValue : (int)((float)SPlayer.MegaJackPotStart * (float)jackPotIncValue / 100f));

                MasterAudio.PlaySound("Loose");

                SPlayer.AddLevelProgress(loseSpinLevelProgress);
                //>>>>>>>>>>>>>>>> (29/3/2021)(ARC)
                foreach (IRunSlotsAsync sA in slotsAsyncList)
                {
                    yield return StartCoroutine(sA.OnLoseSlotsAsyncRutine());
                }
                //>>>>>>>>>>>>>>>> end (29/3/2021)(ARC)
            }

            if (playFreeSpins && !SPlayer.HasFreeSpin)
            {

                lastFeatureGame = true;
                
            }
            else if(!playFreeSpins && SPlayer.HasFreeSpin)
            {
                beforeFeatureGame = true;
            }
            else
            {
                lastFeatureGame = false;
                beforeFeatureGame = false;
            }
            
            foreach (IRunSlotsAsync sA in slotsAsyncList)
            {
                yield return StartCoroutine(sA.EndSlotsAsyncRutine());
            }

            


            hasSpecialEffect = false;
            checkForceStop.ResetIsReady();
            isStopping = false;
            IsSpinning = false;
            if (!IsAutoSpin && !SPlayer.HasFreeSpin)
            {
                SetInputActivity(true);
            }
            else
            {
                SetInputActivity(false);
            }



            if (SPlayer.HasFreeSpin)
            {
                if (!PlayFreeSpins)
                {
                    PlayFreeSpins = true;
                }
                if (featureMultiplyText != null) featureMultiplyText.text = featureMultiply.ToString("0");
                StateControllerManager.ChangeState("freeSpin");
                MasterAudio.PlaySound("FeatureGameBG");
            }
            else
            {
                if (PlayFreeSpins)
                {
                    PlayFreeSpins = false;
                }
                if (SlotMenuController.Instance.totalFeatureWinInfoText) SlotMenuController.Instance.totalFeatureWinInfoText.gameObject.SetActive(false);
                StateControllerManager.ChangeState("normal");
                MasterAudio.StopAllOfSound("FeatureGameBG");
            }
            
            //>>>>>>>>>>>>>>>> (25/3/2021)(ARC)
            if (!PlayFreeSpins)
            {
                if (SlotMenuController.Instance.totalAutoSpinLeft >= 1)
                {
                    SlotMenuController.Instance.totalAutoSpinLeft--;
                    if (SlotMenuController.Instance.totalAutoSpinLeft == 0)
                    {
                        SlotMenuController.Instance.spinauto = false;
                        IsAutoSpin = false;
                        //(Updated)>>>>>>>>>>>>>>>> (4/5/2020)
                        turboMode = false;
                        //(Updated)>>>>>>>>>>>>>>>> end (4/5/2020)
                    }
                    else
                    {
                        SlotMenuController.Instance.spinauto = true;
                        IsAutoSpin = true;
                    }
                }
                SlotMenuController.Instance.UpdateAutoSpinText();
            }
            
            //>>>>>>>>>>>>>>>> end (25/3/2021)(ARC)
            
            if (PlayFreeSpins || IsAutoSpin)
            {
                yield return new WaitForSeconds(autoSpinDelay);
                RunSlots();
            }
        }
        //>>>>>>>>>>>>>>>> (30/3/2021)

        void StopWinEffect()
        {
            if (menuController.totalLineWinCount) menuController.totalLineWinCount.text = "";
            if (menuController.resultWinText) menuController.resultWinText.text = "";
            
            if (menuController.standardLineWin) menuController.standardLineWin.text = "";
            winController.ResetLineWinning();
            winController.WinEffectsShow(false, false);
        }
        [HideInInspector]
        public bool fireworkIsPlaying = false;
        public void StopAllFirework()
        {
            if (fireworkGO == null) return;
            fireworkIsPlaying = false;
            fireworkGO.SetActive(false);
            kingWinAnimationGO.SetActive(false);
            megaWinAnimationGO.SetActive(false);
            ultraBigWinAnimationGO.SetActive(false);
            MasterAudio.StopAllOfSound("MegaWin");
            MasterAudio.StopAllOfSound("KingWin");
            MasterAudio.StopAllOfSound("UltraBigWin");
            MasterAudio.StopAllOfSound("SFX_Firework");
            StopCoroutine(StoreFireworkRutine);
            StoreFireworkRutine = FireworkAnimationRun();
        }
        public IEnumerator StoreFireworkRutine;
        public IEnumerator FireworkAnimationRun()
        {
            if (fireworkGO == null) yield return null;
            fireworkGO.SetActive(true);
            MasterAudio.PlaySound("SFX_Firework");
            fireworkAnimation.Play();
            fireworkIsPlaying = true;
            yield return new WaitForSeconds(fireworkAnimation.clip.length * winAnimationLoop);
            StopAllFirework();
        }



        public IEnumerator kingWinningAnimationRun()
        {
            if (kingWinAnimationGO == null) yield return null;
            kingWinAnimationGO.SetActive(true);
            MasterAudio.PlaySound("KingWin");
            kingWinAnimation.Play();
            yield return null;


        }

        public IEnumerator megaWinningAnimationRun()
        {
            if (megaWinAnimationGO == null) yield return null;
            megaWinAnimationGO.SetActive(true);
            MasterAudio.PlaySound("MegaWin");
            megaWinAnimation.Play();
            yield return null;

        }

        public IEnumerator ultraBigWinningAnimationRun()
        {
            if (ultraBigWinAnimationGO == null) yield return null;
            ultraBigWinAnimationGO.SetActive(true);
            MasterAudio.PlaySound("UltraBigWin");
            ultraBigWinAnimation.Play();
            yield return null;

        }



        //>>>>>>>>>>>>>>>> (30/3/2021)
        private void RotateSlots(Action rotCallBack)
        {
            ParallelTween pT = new ParallelTween();
            int[] rands = rng.GetRandSymbols(); //next symbols for reel (bottom raycaster)

            

            #region prediction visible symbols on reels
            if (debugPredictSymbols)
                for (int i = 0; i < rands.Length; i++)
                {
                    //Debug.Log("------- Reel: " + i + " ------- (down up)");
                    for (int r = 0; r < slotGroupsBeh[i].RayCasters.Length; r++)
                    {
                        int sO = (int)Mathf.Repeat(rands[i] + r, slotGroupsBeh[i].symbOrder.Count);
                        int sID = slotGroupsBeh[i].symbOrder[sO];
                        string sName = slotIcons[sID].iconSprite.name;
                        //Debug.Log("NextSymb ID: " + sID + " ;name : " + sName);
                    }
                }
            #endregion prediction

            for (int i = 0; i < slotGroupsBeh.Length; i++)
            {
                int n = i;
                int r = rands[i];
                pT.Add((callBack) =>
                {
                    slotGroupsBeh[n].NextRotateCylinderEase(mainRotateType, inRotType, outRotType,
                        mainRotateTime,
                        inRotTime, outRotTime, inRotAngle, outRotAngle,
                        r, callBack);
                });
            }

            pT.Start(rotCallBack);
        }

        IEnumerator RotateSlotsWithStoreResult(Action rotCallBack)
        {


            Result res = new Result();


            #region start spinning

            ParallelTween startSpin = new ParallelTween();
            bool finishStartSpin = false;
            for (int i = 0; i < slotGroupsBeh.Length; i++)
            {
                int n = i;

                startSpin.Add((callBack) =>
                {
                    
                    slotGroupsBeh[n].StartSpin(mainRotateType, inRotType, inRotTime, outRotAngle, this, callBack);
                });
            }

            startSpin.Start(delegate { finishStartSpin = true; });

            #endregion start spin
            if (useServerResult)
            {
                //string platformUID = PlayerPrefs.GetString("CurrentPUID");
                //string portal_md5 = WebRequestManager.Md5Sum("KINGSLOT");
                double playpoint = 0;
                //int modetype = 0;
                //if (SPlayer.HasFreeSpin)
                //{
                //    modetype = 1;
                //}
                //else if (IsOnBonus)
                //{
                //    modetype = 2;
                //}
                //if (modetype > 0)
                //{
                //    playpoint = 0;

                //}
                //else
                //{
                    playpoint = SPlayer.LineBet;
                Debug.Log("playpoint :"+playpoint);
                //}
                //double oldpoint = SPlayer.getCoins() + playpoint;
                //string sign = platformUID + SPlayer.TotalBet + oldpoint + save.game_ID +
                //    SPlayer.SelectedLinesCount + SPlayer.LineBet + SPlayer.FreeSpins + "GETPATTERN" + "i8n8c8hAsNky" + portal_md5;
                //string sign_sha1 = WebRequestManager.Sha1Sum(sign);
                ////Debug.Log(sign);
                JSONObject j = new JSONObject();
                //j.AddField("id", platformUID);
                //j.AddField("bet", playpoint.ToString());
                //j.AddField("oldpoint", oldpoint.ToString());
                //j.AddField("gameid", save.game_ID);
                //j.AddField("lineplay", SPlayer.SelectedLinesCount);
                //j.AddField("linepayout", SPlayer.LineBet);
                //j.AddField("freespin", SPlayer.FreeSpins);
                //j.AddField("modetype", modetype);
                //j.AddField("method", "GETPATTERN");
                //j.AddField("sign", sign_sha1);
                
                while (!finishStartSpin) yield return null;
                bool finishContSpin = true;
                yield return StartCoroutine(Connect.Instance.EmitGetPattern(j , (result)=> 
                {
                    string serverRawJsonResult = result.ToString();

                    Debug.Log(serverRawJsonResult);

                    res = JsonUtility.FromJson<Result>(serverRawJsonResult);
                },()=> 
                {

                    if (finishContSpin)
                    {

                        finishContSpin = false;
                        ParallelTween contSpin = new ParallelTween();

                        for (int i = 0; i < slotGroupsBeh.Length; i++)
                        {
                            int n = i;


                            contSpin.Add((callBack) =>
                            {
                                slotGroupsBeh[n].ContSpin(mainRotateType, mainRotateTime, inRotAngle, outRotAngle, callBack);
                            });
                        }

                        contSpin.Start(delegate { finishContSpin = true; });
                    }
                }));
                //yield return StartCoroutine(WebRequestManager.PostWaitResult("http://inchsky.com/KingSlot/API/HighwayKing/getPattern.php", j.ToString(),
                //    (result) =>
                //    {
                //        string serverRawJsonResult = result;

                //        Debug.Log(serverRawJsonResult);

                //        res = JsonUtility.FromJson<Result>(serverRawJsonResult);


                //    },
                //    () =>
                //    {

                //        if (finishContSpin)
                //        {

                //            finishContSpin = false;
                //            ParallelTween contSpin = new ParallelTween();

                //            for (int i = 0; i < slotGroupsBeh.Length; i++)
                //            {
                //                int n = i;


                //                contSpin.Add((callBack) =>
                //                {
                //                    slotGroupsBeh[n].ContSpin(mainRotateType, mainRotateTime, inRotAngle, outRotAngle, callBack);
                //                });
                //            }

                //            contSpin.Start(delegate { finishContSpin = true; });
                //        }
                //    }));
                while (!finishContSpin) yield return null;


            }

            #region prediction visible symbols on reels


            SpinningStopEffect spinningEffect = new SpinningStopEffect(winController, payTable, slotIcons.ToList(), wild_id, playFreeSpins);
            int[] rands = rng.GetRandSymbols(); //next symbols for reel (bottom raycaster)
            int counter = 0;
            List<List<int>> sIDIconsGroup = new List<List<int>>();
            List<List<int>> sIDResultGroup = new List<List<int>>();
            for (int i = 0; i < slotGroupsBeh.Length; i++)
            {
                List<int> sIDIcons = new List<int>();
                List<int> sIDResult = new List<int>();
                if (useServerResult)
                {
                    
                    List<int> colIdIcons = new List<int>();
                    List<int> colIdReuslt = new List<int>();
                    for (int j = 0; j < slotGroupsBeh[i].RayCasters.Length; j++)
                    {
                        if (res.pattern.Length > counter)
                        {
                            colIdIcons.Add(res.pattern[counter]);
                            
                            counter++;
                        }
                        
                    }
                    
                    sIDIcons = colIdIcons;
                }
                else if (testPattern)
                {
                    
                    res = JsonUtility.FromJson<Result>("{\"pattern\":"+pattern+"}");
                    List<int> colIds = new List<int>();

                    for (int j = 0; j < slotGroupsBeh[i].RayCasters.Length; j++)
                    {
                        if (res.pattern.Length > counter)
                        {
                            colIds.Add(res.pattern[counter]);

                            counter++;
                        }

                    }

                    sIDIcons = colIds;
                }
                else
                {
                    //Debug.Log("------- Reel: " + i + " ------- (down up)");
                    
                    for (int r = 0; r < slotGroupsBeh[i].RayCasters.Length; r++)
                    {
                        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                        List<int> pickedSymb = slotGroupsBeh[i].symbOrder;
                        if (slotGroupsBeh[i].symbOrderFeature != null)
                        {
                            if (slotGroupsBeh[i].symbOrderFeature.Count > 0)
                            {
                                pickedSymb = PlayFreeSpins ? slotGroupsBeh[i].symbOrderFeature: slotGroupsBeh[i].symbOrder;
                            }
                        }
                        int sO = (int)Mathf.Repeat(rands[i] + r, pickedSymb.Count);
                        
                        int sID = pickedSymb[sO];
                        sID = pickedSymb[sO];
                        
                        
                        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                        sIDIcons.Add(sID);
                        string sName = slotIcons[sID].iconSprite.name;
                        //Debug.Log("NextSymb ID: " + sID + " ;name : " + sName);

                    }

                    sIDIcons.Reverse();
                    
                }
                
            
                spinningEffect.AddResult(sIDIcons.ToArray(), slotGroupsBeh[i]);

            }

            #endregion prediction

            ParallelTween pT = new ParallelTween();

            spinningEffect.CalculateResult();
            int effectTimeInterval = 0;
            int effectCounter = 0;
            for (int i = 0; i < slotGroupsBeh.Length; i++)
            {
                int n = i;

                bool hasEffect = spinningEffect.HasEffect(i);
                if (hasEffect)
                {
                    effectCounter++;
                    effectTimeInterval = specialEffectTimeInterval * effectCounter;
                }
                StoreSpinResult tempS = new StoreSpinResult(slotGroupsBeh[i], spinningEffect.SlotGBResult(slotGroupsBeh[i],true), spinningEffect.SlotIconResult(slotGroupsBeh[i]), slotGroupsBeh[i].RayCasters.ToList<RayCaster>(), hasEffect, effectTimeInterval);

                pT.Add((callBack) =>
                {
                    
                    slotGroupsBeh[n].NextRotateCylinderEase(mainRotateType, outRotType,
                        mainRotateTime,
                        inRotTime, outRotTime, inRotAngle, outRotAngle,
                        tempS, callBack);
                });
            }



            pT.Start(() => {
                //>>>>>>>>>>>>>>>> (13/4/2021)
                foreach (SlotGroupBehavior s in slotGroupsBeh)
                {
                    s.CallbackOnCompleteAllSpin();
                }
                //>>>>>>>>>>>>>>>> end (13/4/2021)
                rotCallBack();

            });

        }

        bool inputEnable = true;
        public void toggleInput()
        {
            if (inputEnable)
            {
                SetInputActivity(true);
                inputEnable = false;
            }
            else
            {
                SetInputActivity(false);
                inputEnable = true;
            }
        }

        /// <summary>
        /// Set touch activity for game and gui elements of slot scene
        /// </summary>
        private void SetInputActivity(bool activity)
        {
            if (activity)
            {
                if (SlotPlayer.Instance.HasFreeSpin)
                {
                    TouchManager.SetTouchActivity(false); // preserve line selecting  if free spin available
                    menuController.SetControlActivity(false, true); // preserve bet change if free spin available
                }
                else
                {
                    TouchManager.SetTouchActivity(activity);
                    menuController.SetControlActivity(activity);
                }
            }
            else
            {
                TouchManager.SetTouchActivity(activity);
                menuController.SetControlActivity(activity, auto); // spin button set active for auto spin
            }
        }

        /// <summary>
        /// Set auto play
        /// </summary>
        /// <param name="auto"></param>
        public void SetAutoPlay(bool auto)
        {
            this.auto = auto;
        }

        /// <summary>
        /// Calculate propabilities
        /// </summary>
        /// <returns></returns>
        public string[,] CreatePropabilityTable()
        {
            List<string> rowList = new List<string>();
            string[] iconNames = GetIconNames(false);
            int length = slotGroupsBeh.Length;
            string[,] table = new string[length + 1, iconNames.Length + 1];

            rowList.Add("reel / icon");
            rowList.AddRange(iconNames);
            SetRow(table, rowList, 0, 0);

            for (int i = 1; i <= length; i++)
            {
                table[i, 0] = "reel #" + i.ToString();
                SetRow(table, new List<float>(slotGroupsBeh[i - 1].GetReelSymbHitPropabilities(slotIcons)), 1, i);
            }
            return table;
        }

        /// <summary>
        /// Calculate propabilities
        /// </summary>
        /// <returns></returns>
        public string[,] CreatePayTable(out float sumPayOut, out float sumPayoutFreeSpins)
        {

            List<string> row = new List<string>();
            List<float[]> reelSymbHitPropabilities = new List<float[]>();
            string[] iconNames = GetIconNames(false);

            sumPayOut = 0;
            payTableFull = new List<PayLine>();
            //for (int j = 0; j < payTable.Count; j++)
            //{
            //    payTableFull.Add(payTable[j]);
            //    ;p;    if (useWild) payTableFull.AddRange(payTable[j].GetWildLines(this));
            //}

            for (int j = 0; j < payTable.Count; j++)
            {

                payTableFull.Add(new PayLine( payTable[j]));
                if (useWild)
                {
                    PayLine p = payTable[j];
                    List<PayLine> tempWildLine = new List<PayLine>();
                    tempWildLine = new List<PayLine>( p.GetWildLines(this));
                    for (int l = 0; l < tempWildLine.Count; l++)
                    {
                        tempWildLine[l].reverseWin = payTable[j].reverseWin;
                    }
                    payTableFull.AddRange(tempWildLine);
                }
            }
            List<PayLine> tempReverseLine = new List<PayLine>();
            for (int k = 0; k < payTableFull.Count; k++)
            {
                if (payTableFull[k].reverseWin)
                {
                    PayLine pay = new PayLine(payTableFull[k]);

                    pay.reverseWin = true;
                    payTableFull[k].reverseWin = false;
                    Array.Reverse(pay.line);

                    tempReverseLine.Add(pay);
                }


            }
            payTableFull.AddRange(tempReverseLine);


            int rCount = payTableFull.Count + 1;
            int cCount = slotGroupsBeh.Length + 3;
            string[,] table = new string[rCount, cCount];
            row.Add("PayLine / reel");
            for (int i = 0; i < slotGroupsBeh.Length; i++)
            {
                row.Add("reel #" + (i + 1).ToString());
            }
            row.Add("Payout");
            row.Add("Payout, %");
            SetRow(table, row, 0, 0);

            PayLine pL;
            List<PayLine> freeSpinsPL = new List<PayLine>();  // paylines with free spins

            for (int i = 0; i < payTableFull.Count; i++)
            {
                pL = payTableFull[i];
                table[i + 1, 0] = "Payline #" + (i + 1).ToString();
                table[i + 1, cCount - 2] = pL.pay.ToString();
                float pOut = pL.GetPayOutProb(this);
                sumPayOut += pOut;
                table[i + 1, cCount - 1] = pOut.ToString("F6");
                SetRow(table, new List<string>(pL.Names(slotIcons)), 1, i + 1);
                if (pL.freeSpins > 0) freeSpinsPL.Add(pL);
            }

            //Debug.Log("sum (without free spins) % = " + sumPayOut);

            sumPayoutFreeSpins = sumPayOut;
            foreach (var item in freeSpinsPL)
            {
                sumPayoutFreeSpins += sumPayOut * item.GetProbability(this) * item.freeSpins;
            }
            //Debug.Log("sum (with free spins) % = " + sumPayoutFreeSpins);

            return table;
        }

        private void SetRow<T>(string[,] table, List<T> row, int beginColumn, int rowNumber)
        {
            if (rowNumber >= table.GetLongLength(0)) return;

            for (int i = 0; i < row.Count; i++)
            {
                if (i + beginColumn < table.GetLongLength(1)) table[rowNumber, i + beginColumn] = row[i].ToString();
            }
        }

        public string[] GetIconNames(bool addAny)
        {
            if (slotIcons == null || slotIcons.Length == 0) return null;
            int length = (addAny) ? slotIcons.Length + 1 : slotIcons.Length;
            string[] sName = new string[length];
            if (addAny) sName[0] = "any";
            int addN = (addAny) ? 1 : 0;
            for (int i = addN; i < length; i++)
            {
                if (slotIcons[i - addN] != null && slotIcons[i - addN].iconSprite != null)
                {
                    sName[i] = slotIcons[i - addN].iconSprite.name;
                }
                else
                {
                    sName[i] = (i - addN).ToString();
                }
            }
            return sName;
        }
    }

    public class CheckAbleToForceStop
    {
        SlotGroupBehavior[] slotGBArray;
        List<bool> slotGroupIsReady = new List<bool>();

        public CheckAbleToForceStop(SlotGroupBehavior[] slotGBArray)
        {
            this.slotGBArray = slotGBArray;
            for (int i = 0; i < slotGBArray.Length; i++)
            {
                slotGroupIsReady.Add(false);
            }
        }

        public void SubscribeAsReady(SlotGroupBehavior slotGB)
        {
            for (int i = 0; i < slotGBArray.Length; i++)
            {
                if (slotGBArray[i] == slotGB)
                {
                    slotGroupIsReady[i] = true;
                }
            }
        }

        public void UnSubscribeAsReady(SlotGroupBehavior slotGB)
        {
            for (int i = 0; i < slotGBArray.Length; i++)
            {
                if (slotGBArray[i] == slotGB)
                {
                    slotGroupIsReady[i] = false;
                }
            }
        }

        public bool CheckIsReady()
        {
            foreach (bool b in slotGroupIsReady)
            {
                if (!b)
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetIsReady()
        {
            for (int i = 0; i < slotGroupIsReady.Count; i++)
            {
                slotGroupIsReady[i] = false;
            }

        }
    }

    public class SpinningStopEffect
    {
        List<LineBehavior> selectedLine = new List<LineBehavior>();
        List<PayLine> payLineWithEffect = new List<PayLine>();
        List<ResultWithRaycaster> resultsId = new List<ResultWithRaycaster>();
        List<SlotGroupBehavior> listOfSlotGB = new List<SlotGroupBehavior>();
        List<SlotIcon> slotI = new List<SlotIcon>();
        bool isFeatureMode = false;
        int wildId = -1;
        List<StoreEffectedCol> storeEffectedCol = new List<StoreEffectedCol>();
        bool hasEffectingOther = false;
        SlotIcon effectedSlotIcon = null;
        int effectedSlotId = -1;
        WinController winC;

        public SpinningStopEffect(WinController winC, List<PayLine> pay, List<SlotIcon> slotI, int wildId, bool isFeatureMode)
        {
            this.winC = winC;
            foreach (LineBehavior l in winC.LineBehL)
            {
                if (l.IsSelected)
                {
                    selectedLine.Add(l);
                }
            }
            foreach (PayLine p in pay)
            {
                if (p.specialEffect)
                {
                    payLineWithEffect.Add(p);
                }
            }

            this.slotI = slotI;
            this.wildId = wildId;
            this.isFeatureMode = isFeatureMode;

        }

        List<List<int>> resultForCol = new List<List<int>>();
        List<int> resultCol = new List<int>();
        public void AddResult(int[] result, SlotGroupBehavior slotGb)
        {
            resultCol = new List<int>();
            for (int i = 0; i < slotGb.RayCasters.Length; i++)
            {
                resultsId.Add(new ResultWithRaycaster(slotGb.RayCasters[i], result[i]));
                resultCol.Add(result[i]);
            }

            resultForCol.Add(resultCol);

            listOfSlotGB.Add(slotGb);


        }

        public List<int> SlotGBResult(SlotGroupBehavior slotGb, bool withEffectedIconResult)
        {
            List<int> result = new List<int>();
            List<ResultWithRaycaster> hasEffectingIconResult = new List<ResultWithRaycaster>();
            if (hasEffectingOther && withEffectedIconResult )
            {
                foreach (StoreEffectedCol s in storeEffectedCol)
                {
                    
                    if (s.slot == slotGb)
                    {
                        hasEffectingIconResult = s.idResults;
                    }
                }
            }
            
            foreach (RayCaster r in slotGb.RayCasters)
            {
                
                if (hasEffectingOther && withEffectedIconResult && hasEffectingIconResult.Count > 0)
                {
                    for(int i = 0; i< hasEffectingIconResult.Count; i++)
                    {
                        if (hasEffectingIconResult[i].Ray == r)
                        {
                            result.Add(hasEffectingIconResult[i].Result);
                        }
                    }
                    



                }
                else
                {
                    for (int i = 0; i < resultsId.Count; i++)
                    {
                        if (resultsId[i].Ray == r)
                        {



                            result.Add(resultsId[i].Result);

                        }
                    }

                }
            }
            result.Reverse();
            string st = "[";
            foreach (int i in result)
            {
                st += i + ",";
            }
            st += "]";
            //Debug.Log(st);
            return result;
        }

        public List<SlotIcon> SlotIconResult(SlotGroupBehavior slotGb)
        {
            List<int> ids = new List<int>();
            List<int> idsWithEffectedIconResult = new List<int>();
            List<SlotIcon> tempSlotIcons = new List<SlotIcon>();
            ids = SlotGBResult(slotGb, false);
            idsWithEffectedIconResult = SlotGBResult(slotGb, true);
            
            

            for (int i = 0; i < ids.Count; i++)
            {   
                if(idsWithEffectedIconResult[i] == ids[i])
                {
                    
                    tempSlotIcons.Add(slotI[ids[i]]);
                }
                else
                {
                    
                    SlotIcon standardIcon = slotI[ids[i]];
                    SlotIcon tempEffectedSlotIcon = new SlotIcon(effectedSlotIcon);
                    tempEffectedSlotIcon.iconSprite = standardIcon.iconSprite;
                    tempEffectedSlotIcon.animC = standardIcon.animC;
                    //>>>>>>>>>>>>>>>> (13/4/2021)
                    tempEffectedSlotIcon.displayWinAnimWhenAppear = standardIcon.displayWinAnimWhenAppear;
                    tempEffectedSlotIcon.displayWinAnimWhenAppearSound = standardIcon.displayWinAnimWhenAppearSound;
                    //>>>>>>>>>>>>>>>> end (13/4/2021)
                    //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                    tempEffectedSlotIcon.iconSpriteFeature = standardIcon.iconSpriteFeature;
                    tempEffectedSlotIcon.animCFeature = standardIcon.animCFeature;
                    //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                    //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
                    tempEffectedSlotIcon.winningBox = standardIcon.winningBox;
                    //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
                    tempSlotIcons.Add(tempEffectedSlotIcon);
                }
                
            }

            return tempSlotIcons;

        }

        public List<SlotIcon> SlotIconResult(SlotGroupBehavior slotGb, List<int> serverReuslt)
        {
            List<int> ids = new List<int>();
            List<int> idsWithEffectedIconResult = new List<int>();
            List<SlotIcon> tempSlotIcons = new List<SlotIcon>();
            ids = SlotGBResult(slotGb, false);

            idsWithEffectedIconResult = SlotGBResult(slotGb, true);

            for (int i = 0; i < ids.Count; i++)
            {
                if (idsWithEffectedIconResult[i] == ids[i])
                {

                    tempSlotIcons.Add(slotI[ids[i]]);
                }
                else
                {

                    SlotIcon standardIcon = slotI[ids[i]];
                    SlotIcon tempEffectedSlotIcon = new SlotIcon(effectedSlotIcon);
                    tempEffectedSlotIcon.iconSprite = standardIcon.iconSprite;
                    tempEffectedSlotIcon.animC = standardIcon.animC;
                    //>>>>>>>>>>>>>>>> (13/4/2021)
                    tempEffectedSlotIcon.displayWinAnimWhenAppear = standardIcon.displayWinAnimWhenAppear;
                    tempEffectedSlotIcon.displayWinAnimWhenAppearSound = standardIcon.displayWinAnimWhenAppearSound;
                    //>>>>>>>>>>>>>>>> end (13/4/2021)
                    //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                    tempEffectedSlotIcon.iconSpriteFeature = standardIcon.iconSpriteFeature;
                    tempEffectedSlotIcon.animCFeature = standardIcon.animCFeature;
                    //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                    //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
                    tempEffectedSlotIcon.winningBox = standardIcon.winningBox;
                    //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
                    tempSlotIcons.Add(tempEffectedSlotIcon);
                }

            }

            return tempSlotIcons;

        }

        List<int[]> finalWinningResult = new List<int[]>();


        public void CalculateResult()
        {
            foreach (LineBehavior l in selectedLine)
            {
                List<int> lineResult = new List<int>();
                for (int i = 0; i < l.rayCasters.Length; i++)
                {
                    for (int j = 0; j < resultsId.Count; j++)
                    {
                        if (l.rayCasters[i] == resultsId[j].Ray)
                        {

                            lineResult.Add(resultsId[j].Result);
                        }
                    }
                }
                finalWinningResult.Add(lineResult.ToArray());
            }
            storeEffectedCol.Clear();
            hasEffectingOther = false;
            effectedSlotIcon = null;
            effectedSlotId = -1;
            if (isFeatureMode)
            {
                
                CalculateOnEffectingOther();
            }
        }
        
        void CalculateOnEffectingOther()
        {
            int totalEffected = 0;
            for (int i = 0; i < resultForCol.Count; i++)
            {
                if (!hasEffectingOther)
                {
                    for (int k = 0; k < resultForCol[i].Count; k++)
                    {
                        SlotIcon s = slotI[resultForCol[i][k]];
                        if (s.effectingOtherCol)
                        {
                            for(int l = 0; l< s.listOfPercentageHit.Count; l++)
                            {
                                int ran = UnityEngine.Random.Range(0, 100);

                                if (ran <= s.listOfPercentageHit[l].percentageHit)
                                {   
                                    
                                    totalEffected = s.listOfPercentageHit[l].quantitySplit;
                                    effectedSlotIcon = s;
                                    effectedSlotId = resultForCol[i][k];
                                    hasEffectingOther = true;
                                }
                            }
                            


                            break;

                        }
                    }
                }

                List<ResultWithRaycaster> temp = new List<ResultWithRaycaster>();
                foreach(RayCaster r in listOfSlotGB[i].RayCasters)
                {
                    for(int j = 0; j< resultsId.Count; j++)
                    {
                        if(resultsId[j].Ray == r)
                        {
                            temp.Add(new ResultWithRaycaster(resultsId[j]));
                        }
                    }
                }

                storeEffectedCol.Add(new StoreEffectedCol(listOfSlotGB[i], temp));
            }
            if (hasEffectingOther)
            {
                List<StoreEffectedCol> tempStore = new List<StoreEffectedCol>();
                for(int i = 0; i< listOfSlotGB.Count; i++)
                {
                    if (!effectedSlotIcon.colExcluded.Contains(i))
                    {
                        tempStore.Add(storeEffectedCol[i]);
                    }

                }

                storeEffectedCol = new List<StoreEffectedCol>(tempStore);

                for (int i = 1; i <= totalEffected; i++)
                {
                    int ranCol = UnityEngine.Random.Range(0, tempStore.Count);

                    int ranResult = UnityEngine.Random.Range(0, tempStore[ranCol].idResults.Count);

                    storeEffectedCol[ranCol].idResults[ranResult].ChangeResult(effectedSlotId);
                   
                    tempStore.Remove(storeEffectedCol[ranCol]);
                    
                }
            }

        }

        public bool HasEffect(int slotGBPos)
        {
            bool hasEffect = false;

            int counter = 0;


            int slotPos = 0;
            int scatterCounter = 0;
            
            int wildCOunter = 0;
            bool hasWildWin = false;
            bool scatterHasLose = false;
            bool skipScatter = false;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            int specialIconCounter = 0;
            bool specialIconHasLose = false;
            bool skipSpecialIcon = false;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            foreach (var sid in resultForCol)
            {
                if (slotGBPos >= slotPos)
                {
                    slotPos++;
                    hasWildWin = false;
                    foreach (var resultSid in sid)
                    {

                        if (resultSid == winC.Wild_id)
                        {
                            hasWildWin = true;
                            wildCOunter++;

                            if (slotPos <= slotGBPos)
                            {
                                foreach (var wildP in winC.WildPayTable)
                                {
                                    if (wildP.wildCount > 1 && (wildP.wildCount - 1) <= wildCOunter && wildP.useSpecialEffect)
                                    {
                                        hasEffect = true;

                                        return hasEffect;
                                    }
                                }
                            }
                        }

                        if (winC.ScatterWildSub)
                        {
                            scatterCounter += wildCOunter;
                        }
                        if (!skipScatter)
                        {
                            if (resultSid == winC.Scatter_id || (winC.ScatterWildSub && hasWildWin))
                            {
                                if (resultSid == winC.Scatter_id)
                                {
                                    scatterCounter++;
                                    scatterHasLose = false;
                                }


                                if (slotPos <= slotGBPos)
                                {
                                    foreach (var scatterP in winC.ScatterPayTable)
                                    {
                                        if (scatterP.scattersCount > 1 && (scatterP.scattersCount - 1) <= scatterCounter && scatterP.useSpecialEffect)
                                        {
                                            hasEffect = true;

                                            return hasEffect;
                                        }
                                    }
                                }
                                if (winC.ScatterFollowSeq)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (winC.ScatterFollowSeq)
                                {
                                    if (winC.ScatterWildSub && !hasWildWin)
                                    {

                                        scatterHasLose = true;

                                    }

                                }
                            }
                        }

                        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                        if (winC.SpecialIconWildSub)
                        {
                            specialIconCounter += wildCOunter;
                        }
                        if (!skipSpecialIcon)
                        {
                            if (resultSid == winC.SpecialIcon_id || (winC.SpecialIconWildSub && hasWildWin))
                            {
                                if (resultSid == winC.SpecialIcon_id)
                                {
                                    specialIconCounter++;
                                    specialIconHasLose = false;
                                }


                                if (slotPos <= slotGBPos)
                                {
                                    foreach (var specialIconP in winC.SpecialIconPayTable)
                                    {
                                        if (specialIconP.specialIconsCount > 1 && (specialIconP.specialIconsCount - 1) <= specialIconCounter && specialIconP.useSpecialEffect)
                                        {
                                            hasEffect = true;

                                            return hasEffect;
                                        }
                                    }
                                }
                                if (winC.SpecialIconFollowSeq)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (winC.SpecialIconFollowSeq)
                                {
                                    if (winC.SpecialIconWildSub && !hasWildWin)
                                    {

                                        specialIconHasLose = true;

                                    }

                                }
                            }
                        }
                        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)

                    }

                    if (winC.ScatterFollowSeq && scatterHasLose)
                    {
                        skipScatter = true;
                    }

                    //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                    if (winC.SpecialIconFollowSeq && specialIconHasLose)
                    {
                        skipSpecialIcon = true;
                    }
                    //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                }

            }

            if (!hasEffect)
            {
                for (int i = 0; i < payLineWithEffect.Count; i++)
                {

                    for (int j = 0; j < finalWinningResult.Count; j++)
                    {
                        counter = 0;
                        int winningTargetId = -1;
                        //match both results
                        for (int k = 0; k < payLineWithEffect[i].line.Length; k++)
                        {
                            winningTargetId = payLineWithEffect[i].line[0];


                            if ((finalWinningResult[j][k] == payLineWithEffect[i].line[k] || (finalWinningResult[j][k] == wildId && slotI[winningTargetId].useWildSubstitute)) || payLineWithEffect[i].line[k] < 0)
                            {



                                if (payLineWithEffect[i].line[k] < 0)
                                {

                                    if (slotGBPos >= counter - 1)
                                    {

                                        return !hasEffect;
                                    }
                                }
                                counter++;
                            }
                            else
                            {
                                if ((k + 1) < payLineWithEffect[i].line.Length)
                                {
                                    if (payLineWithEffect[i].line[k + 1] < 0)
                                    {
                                        if (slotGBPos >= counter)
                                        {

                                            return !hasEffect;
                                        }
                                    }
                                }
                                break;
                            }

                        }
                    }
                }

            }





            return hasEffect;
        }

        internal void AddResult(object v, SlotGroupBehavior slotGroupBehavior)
        {
            throw new NotImplementedException();
        }
    }

    public class StoreEffectedCol
    {
        public SlotGroupBehavior slot;
        public List<ResultWithRaycaster> idResults;
        public StoreEffectedCol(SlotGroupBehavior slot, List<ResultWithRaycaster> idResults)
        {
            this.slot = slot;
            this.idResults = idResults;
        }

    }

    public class ResultWithRaycaster
    {
        RayCaster ray;
        int result;

        public ResultWithRaycaster(RayCaster r, int result)
        {
            this.Ray = r;
            this.Result = result;

        }

        public ResultWithRaycaster(ResultWithRaycaster r)
        {
            this.Ray = r.Ray;
            this.Result = r.Result;
        }

        public RayCaster Ray { get => ray; private set => ray = value; }
        public int Result { get => result; private set => result = value; }

        public void ChangeResult(int id)
        {
            result = id;
        }
    }



    public class StoreSpinResult
    {
        SlotGroupBehavior slotGroupB;
        List<int> symbolIds = new List<int>();
        List<RayCaster> listOfRaycaster = new List<RayCaster>();
        bool specialEffect = false;
        List<SlotIcon> sIcons = new List<SlotIcon>();
        int additionalEffectTiem;
        public StoreSpinResult(SlotGroupBehavior sGB, List<int> symbolIds, List<SlotIcon> sIcons, List<RayCaster> listOfRaycaster, bool specialEffect, int additionalEffectTIme)
        {

            SlotGroupB = sGB;
            this.SymbolIds = symbolIds;
            this.SIcons = sIcons;
            this.specialEffect = specialEffect;
            this.ListOfRaycaster = listOfRaycaster;
            AdditionalEffectTiem = additionalEffectTIme;
        }

        public SlotGroupBehavior SlotGroupB { get => slotGroupB; private set => slotGroupB = value; }
        public List<int> SymbolIds { get => symbolIds; set => symbolIds = value; }
        public bool SpecialEffect { get => specialEffect; private set => specialEffect = value; }
        public List<RayCaster> ListOfRaycaster { get => listOfRaycaster; private set => listOfRaycaster = value; }
        public List<SlotIcon> SIcons { get => sIcons; set => sIcons = value; }
        public int AdditionalEffectTiem { get => additionalEffectTiem; set => additionalEffectTiem = value; }
    }
    [Serializable]
    public class numberOfSlotIconEffecting
    {
        public int quantitySplit;
        public int percentageHit;

    }
    [Serializable]
    public class SlotIcon
    {
        public Sprite iconSprite;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public Sprite iconSpriteFeature;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        [Header("Winning Setting")]
        public Sprite addIconSprite;
        public RuntimeAnimatorController animC;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public RuntimeAnimatorController animCFeature;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        [Range(2,5)]
        public int timeDisplayAnim = 2;

        //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
        [Serializable]
        public class LineWinnigBox
        {
            
            public bool enableWinnigLineBox;
            public Material lineMaterial;
            public Color lineColor;
            public Vector2 lineSize;
            public int sortingLayerID;
            public int sortingOrder;
            [Range(0.5f, 3f)]
            public float lineFlashingSpeed = 1f;
            [Range(0.1f, 100f)]
            public float lineRendererWidth = 0.2f;
        }

        [Header("Winnig line Box")]
        public LineWinnigBox winningBox;

        [Serializable]
        public class GroupIcon
        {
            public bool useGroupIcon = false;
            public int groupCount;
            public float posY;
            public GameObject groupIconPrefab;
        }

        [Header("Group Icon Setting")]
        public GroupIcon groupIconSetting;

        //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)

        [Header("Sound Setting")]
        public string soundName;
        [Header("Other Setting")]
        public bool useWildSubstitute = true;

        [Header("Replace all with same")]
        public bool effectWholeColumn = false;
        public GameObject effectWholeColumnGamePrefab;
        
        [Header("On Feature Mode")]
        public bool effectingOtherCol = false;
        public List<numberOfSlotIconEffecting> listOfPercentageHit;
        public List<int> colExcluded;



        [Header("Sticky Slot Icon")]
        public bool enableStickyIcon = false;

        [Header("Animation group")]
        public List<SlotIconAnimGroup> animationWinGroup = new List<SlotIconAnimGroup>();
        [HideInInspector]
        public SlotGroupBehavior slotGB;
        [HideInInspector]
        public bool hasColumnWinEffect;
        private GameObject winPrefab;

        //>>>>>>>>>>>>>>>> (13/4/2021)
        public bool displayWinAnimWhenAppear = false;
        public string displayWinAnimWhenAppearSound = "";
        //>>>>>>>>>>>>>>>> end (13/4/2021)

        public SlotIcon(SlotIcon s)
        {
            this.iconSprite = s.iconSprite;
            this.addIconSprite = s.addIconSprite;
            this.animC = s.animC;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            this.animCFeature = s.animCFeature;
            this.iconSpriteFeature = s.iconSpriteFeature;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            this.timeDisplayAnim = s.timeDisplayAnim;
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            this.winningBox = s.winningBox;
            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            //>>>>>>>>>>>>>>>> (13/4/2021)
            this.displayWinAnimWhenAppear = s.displayWinAnimWhenAppear;
            this.displayWinAnimWhenAppearSound = s.displayWinAnimWhenAppearSound;
            //>>>>>>>>>>>>>>>> end (13/4/2021)
            this.soundName = s.soundName;
            this.useWildSubstitute = s.useWildSubstitute;

            this.effectWholeColumn = s.effectWholeColumn;
            this.effectWholeColumnGamePrefab = s.effectWholeColumnGamePrefab;
            
            this.effectingOtherCol = s.effectingOtherCol;

            this.listOfPercentageHit = s.listOfPercentageHit;
            this.colExcluded = s.colExcluded;


            this.enableStickyIcon = s.enableStickyIcon;

            this.animationWinGroup = s.animationWinGroup;
            this.slotGB = s.slotGB;
            this.hasColumnWinEffect = s.hasColumnWinEffect;
            this.winPrefab = s.winPrefab;
        }
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public SlotIcon(Sprite iconSprite, Sprite iconSpriteFeature, Sprite addIconSprite, RuntimeAnimatorController animC, RuntimeAnimatorController animCFeature, string soundName, LineWinnigBox winningBox,bool displayWinAnimWhenAppear, string displayWinAnimWhenAppearSound)
        {
            this.iconSprite = iconSprite;
            this.addIconSprite = addIconSprite;
            this.animC = animC;
            //>>>>>>>>>>>>>>>> (13/4/2021)
            this.displayWinAnimWhenAppear = displayWinAnimWhenAppear;
            this.displayWinAnimWhenAppearSound = displayWinAnimWhenAppearSound;
            //>>>>>>>>>>>>>>>> end (13/4/2021)
            this.iconSpriteFeature = iconSpriteFeature;
            this.animCFeature = animCFeature;
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            this.winningBox = winningBox;
            
            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            this.soundName = soundName;

        }
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)

        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public SlotIcon(Sprite iconSprite, Sprite iconSpriteFeature, Sprite addIconSprite, RuntimeAnimatorController animC, RuntimeAnimatorController animCFeature, GameObject winPrefab, bool useWildSubstitute, LineWinnigBox winningBox, bool displayWinAnimWhenAppear, string displayWinAnimWhenAppearSound)
        {
            this.iconSprite = iconSprite;
            this.addIconSprite = addIconSprite;
            this.winPrefab = winPrefab;
            this.animC = animC;
            this.animCFeature = animCFeature;
            this.iconSpriteFeature = iconSpriteFeature;
            //>>>>>>>>>>>>>>>> (13/4/2021)
            this.displayWinAnimWhenAppear = displayWinAnimWhenAppear;
            this.displayWinAnimWhenAppearSound = displayWinAnimWhenAppearSound;
            //>>>>>>>>>>>>>>>> end (13/4/2021
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            this.winningBox = winningBox;
            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            this.useWildSubstitute = useWildSubstitute;
        }
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
    }

    [Serializable]
    public class SlotIconAnimGroup
    {
        [SerializeField]
        string name;
        [SerializeField]
        RuntimeAnimatorController animC;

        public SlotIconAnimGroup(string name, RuntimeAnimatorController animC)
        {
            this.name = name;
            this.animC = animC;
        }

        public string Name { get => name; private set => name = value; }
        public RuntimeAnimatorController AnimC { get => animC; set => animC = value; }



    }
    // Helper for winning symbols check
    public class WinController
    {
        private List<LineBehavior> lineBehL;
        private List<PayLine> payTable;
        private List<ScatterPay> scatterPayTable;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        private List<SpecialIconPay> specialIconPayTable;
        private List<SlotSymbol> specialIconWinSymbols;
        private bool useSpecialIcon;
        private bool specialIconWildSub;
        private bool specialIconFollowSeq;
        private int specialIcon_id;
        public WinData specialIconWin;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        private List<WildPay> wildPayTable;
        private List<SlotSymbol> scatterWinSymbols;
       
        private List<SlotSymbol> wildWinSymbols;
        private List<SlotSymbol> jackPotWinSymbols;
        public WinData scatterWin;
        public WinData wildWin;
        private SlotGroupBehavior[] slotGroupsBeh;
        private int scatter_id;
        
        private int wild_id;
        private bool useScatter;
        private bool scatterWildSub;
        private bool scatterFollowSeq;
        private GameObject particlesPrefab;
        private Transform topJumpTarget;
        private Transform bottomJumpTarget;
        private int megaJackPotCount;
        private int maxiJackPotCount;
        private int miniJackPotCount;
        private bool useMegaJackPot;
        private bool useMaxiJackPot;
        private bool useMiniJackPot;
        private int jp_symb_id;
       
        private SlotController sC;
        private int contID;
        private TweenSeq contTS;
        
        public List<LineBehavior> LineBehL { get => lineBehL; private set => lineBehL = value; }
        public List<PayLine> PayTable { get => payTable; private set => payTable = value; }
        public int Scatter_id { get => scatter_id; private set => scatter_id = value; }
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public int SpecialIcon_id { get => specialIcon_id; private set => specialIcon_id = value; }
        public List<SpecialIconPay> SpecialIconPayTable { get => specialIconPayTable; private set => specialIconPayTable = value; }
        public bool SpecialIconFollowSeq { get => specialIconFollowSeq; private set => specialIconFollowSeq = value; }
        public bool SpecialIconWildSub { get => specialIconWildSub; private set => specialIconWildSub = value; }
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)

        public List<ScatterPay> ScatterPayTable { get => scatterPayTable; private set => scatterPayTable = value; }
        public List<WildPay> WildPayTable { get => wildPayTable; private set => wildPayTable = value; }
        public int Wild_id { get => wild_id; set => wild_id = value; }
        public bool ScatterFollowSeq { get => scatterFollowSeq; private set => scatterFollowSeq = value; }
        public bool ScatterWildSub { get => scatterWildSub; private set => scatterWildSub = value; }

        public WinController(SlotController sC, LineBehavior[] lineBeh)
        {
            LineBehL = new List<LineBehavior>(lineBeh);
            PayTable = sC.payTableFull;
            slotGroupsBeh = sC.slotGroupsBeh;
            Scatter_id = sC.scatter_id;
            Wild_id = sC.wild_id;
            useScatter = sC.useScatter;
            ScatterWildSub = sC.scatterWildSub;
            ScatterFollowSeq = sC.scatterFollowSeq;
            particlesPrefab = sC.particlesStars;
            topJumpTarget = sC.topJumpTarget;
            bottomJumpTarget = sC.bottomJumpTarget;
            megaJackPotCount = sC.megaJackPotCount;
            maxiJackPotCount = sC.maxiJackPotCount;
            miniJackPotCount = sC.miniJackPotCount;
            ScatterPayTable = sC.scatterPayTable;
            WildPayTable = sC.wildPayTable;
            useMegaJackPot = sC.useMegaJacPot;
            useMaxiJackPot = sC.useMaxiJacPot;
            useMiniJackPot = sC.useMiniJacPot;
            jp_symb_id = sC.jp_symbol_id;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            SpecialIcon_id = sC.specialIcon_id;
            useSpecialIcon = sC.useSpecialIcon;
            SpecialIconWildSub = sC.specialIconWildSub;
            SpecialIconFollowSeq = sC.specialIconFollowSeq;
            SpecialIconPayTable = sC.specialIconPayTable;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            this.sC = sC;

        }

        /// <summary>
        /// Return true if slot has any winning
        /// </summary>
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        internal bool HasAnyWinn(ref bool hasLineWin, ref bool hasScatterWin, ref bool hasWildWin, ref bool hasSpecialIconWin, ref JackPotType jackPotType)
        {
            hasLineWin = false;
            hasScatterWin = false;
            
            hasSpecialIconWin = false;
            
            hasWildWin = false;
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsWinningLine)
                {
                    hasLineWin = true;
                    break;
                }
            }
            if (useScatter && HasScatterWin())
            {
                hasScatterWin = true;
            }

            if (useSpecialIcon && HasSpecialIconWin())
            {
                hasScatterWin = true;
            }

            if (HasWildWin())
            {
                hasWildWin = true;
            }

            jackPotType = GetJackPotWin();
            return (hasLineWin || hasScatterWin || hasWildWin || hasSpecialIconWin || jackPotType != JackPotType.None);
        }
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        /// <summary>
        /// Search win symbols (paylines, scatter)
        /// </summary>
        internal void SearchWinSymbols()
        {
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsSelected)
                {
                    lB.FindWin(PayTable);
                }
            }

            // search scatters
            scatterWinSymbols = new List<SlotSymbol>();

            List<SlotSymbol> scatterSymbolsTemp = new List<SlotSymbol>();
            scatterWin = null;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            specialIconWinSymbols = new List<SlotSymbol>();
            
            List<SlotSymbol> specialIconSymbolsTemp = new List<SlotSymbol>();
            specialIconWin = null;
            bool isSpecialIconWild = false;
            bool disableSpecialIconWin = false;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            wildWinSymbols = new List<SlotSymbol>();

            List<SlotSymbol> wildSymbolsTemp = new List<SlotSymbol>();
            wildWin = null;

            bool disableScatterWin = false;
            bool isScatterWild = false;
            foreach (var item in slotGroupsBeh)
            {
                bool hasWild = false;
                if (!item.HasSymbolInAnyRayCaster(Wild_id, ref wildSymbolsTemp))
                {

                }
                else
                {
                    wildWinSymbols.AddRange(wildSymbolsTemp);
                    hasWild = true;
                    if (ScatterWildSub) isScatterWild = true;
                    //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                    if (SpecialIconWildSub) isSpecialIconWild = true;
                    if (SpecialIconWildSub && !disableSpecialIconWin)
                    {
                        if (SpecialIconFollowSeq)
                        {

                            specialIconWinSymbols.Add(wildSymbolsTemp[0]);
                        }
                        else
                        {
                            specialIconWinSymbols.AddRange(wildSymbolsTemp);
                        }

                    }
                    //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                    if (ScatterWildSub && !disableScatterWin)
                    {
                        if (ScatterFollowSeq)
                        {

                            scatterWinSymbols.Add(wildSymbolsTemp[0]);
                        }
                        else
                        {
                            scatterWinSymbols.AddRange(wildSymbolsTemp);
                        }

                    }
                }
                //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                if (!item.HasSymbolInAnyRayCaster(SpecialIcon_id, ref specialIconSymbolsTemp))
                {
                    if (hasWild && SpecialIconWildSub)
                    {

                    }
                    else
                    {

                        if (SpecialIconFollowSeq)
                        {

                            disableSpecialIconWin = true;
                        }
                    }

                }
                else
                {
                    if (!disableSpecialIconWin)
                    {
                        if (SpecialIconFollowSeq)
                        {



                            specialIconWinSymbols.Add(specialIconSymbolsTemp[0]);
                        }
                        else
                        {
                            specialIconWinSymbols.AddRange(specialIconSymbolsTemp);
                        }

                    }


                }
                //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
                if (!item.HasSymbolInAnyRayCaster(Scatter_id, ref scatterSymbolsTemp))
                {
                    if(hasWild && ScatterWildSub)
                    {

                    }
                    else
                    {
                        
                        if (ScatterFollowSeq)
                        {
                            
                            disableScatterWin = true;
                        }
                    }
                    
                }
                else
                {
                    if (!disableScatterWin)
                    {
                        if (ScatterFollowSeq)
                        {
                            
                            
                            
                            scatterWinSymbols.Add(scatterSymbolsTemp[0]);
                        }
                        else
                        {
                            scatterWinSymbols.AddRange(scatterSymbolsTemp);
                        }
                        
                    }
                    

                }


               
            }
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            if (useSpecialIcon)
                foreach (var item in SpecialIconPayTable)
                {
                    if (item.specialIconsCount > 0 && item.specialIconsCount == specialIconWinSymbols.Count)
                    {
                        specialIconWin = new WinData(specialIconWinSymbols, item, isSpecialIconWild);
                    }
                }
            if (specialIconWin == null) specialIconWinSymbols = new List<SlotSymbol>();
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            if (useScatter)
                foreach (var item in ScatterPayTable)
                {
                    if (item.scattersCount > 0 && item.scattersCount == scatterWinSymbols.Count)
                    {
                        scatterWin = new WinData(scatterWinSymbols, item, isScatterWild);
                    }
                }
            if (scatterWin == null) scatterWinSymbols = new List<SlotSymbol>();



            foreach (var item in WildPayTable)
            {
                if (item.wildCount > 0 && item.wildCount == wildWinSymbols.Count)
                {
                    wildWin = new WinData(wildWinSymbols, item);
                }
            }
            if (wildWin == null) wildWinSymbols = new List<SlotSymbol>();

        }

        #region winshow
        /// <summary>
        /// Show symbols particles and lines glowing
        /// </summary>
        internal void WinEffectsShow(bool flashingLines, bool showSymbolParticles)
        {
            
            HideAllLines();
            LineBehL.ForEach((lB) =>
            {
                if (lB.IsWinningLine)
                {
                    lB.SetLineVisible(flashingLines);
                    lB.LineFlashing(flashingLines);
                }
                //lB.ShowWinSymbolsParticles(showSymbolParticles);
            });

            if (useScatter && scatterWinSymbols != null && scatterWinSymbols.Count > 0)
            {
                foreach (var item in scatterWinSymbols)
                {
                    //item.ShowParticles(showSymbolParticles, sC.particlesStars);
                }
            }

            if (jackPotWinSymbols != null && jackPotWinSymbols.Count > 0)
            {
                foreach (var item in jackPotWinSymbols)
                {
                    //item.ShowParticles(showSymbolParticles, sC.particlesStars);
                }
            }

        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        /// <summary>
        /// Show win symbols jumps
        /// </summary>
        internal void WinSymbolShow(bool isAutoSpin, double lineBet, double totalLineBet, Action<WinData> lineWinCallBack, Action scatterWinCallBack, Action specialIconWinCallBack, Action jackPotWinCallBack, Action wildWinCallback, Action completeCallBack)
        {
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
            TweenSeq ts = new TweenSeq();
            int autoLineWinCounter = 0;

            //show jackPot win
            if (jackPotWinSymbols != null && jackPotWinSymbols.Count > 0)
            {
                ParallelTween pT;
                pT = new ParallelTween();
                pT.Add((callBack) =>
                {

                    foreach (var item in jackPotWinSymbols)
                    {
                        item.WinTrigger(callBack, 1);
                    }

                });

                ts.Add((callBack) =>
                {
                    pT.Start(() =>
                    {
                        jackPotWinCallBack?.Invoke();
                        callBack?.Invoke();
                    });
                });
            }
            //>>>>>>>>>>>>>>>> (15/3/2021)
            if (useSpecialIcon && specialIconWinSymbols != null && specialIconWinSymbols.Count > 0)
            {
                ParallelTween pT;
                pT = new ParallelTween();

                pT.Add((callBack) =>
                {
                    if (specialIconWin != null)
                    {
                        
                        double totalSpecialIconWin = 0;
                        
                        totalSpecialIconWin = (specialIconWin.SpecialIconP.pay * lineBet) + (specialIconWin.SpecialIconP.totalBetMult * totalLineBet);
                        
                        if (SlotController.Instance.PlayFreeSpins)
                        {
                            totalSpecialIconWin = totalSpecialIconWin * SlotController.Instance.featureMultiply;
                        }
                        
                        SlotMenuController.Instance.resultWinText.text = "You Won " + totalSpecialIconWin.ToString(SlotController.Instance.decimalDisplay) + " on extra";
                        
                        if (SlotMenuController.Instance.standardLineWin) SlotMenuController.Instance.standardLineWin.text = totalSpecialIconWin > 0? totalSpecialIconWin.ToString(SlotController.Instance.decimalDisplay):"";
                        

                    }

                    foreach (var item in specialIconWinSymbols)
                    {
                        
                        item.WinTrigger(callBack, 1, false);
                        

                    }

                });

                ts.Add((callBack) =>
                {

                    pT.Start(() =>
                    {
                        specialIconWinCallBack?.Invoke();
                        callBack?.Invoke();

                    });
                });

            }
            //>>>>>>>>>>>>>>>> end (15/3/2021)
            //show scatterwin
            if (useScatter && scatterWinSymbols != null && scatterWinSymbols.Count > 0)
            {
                ParallelTween pT;
                pT = new ParallelTween();

                pT.Add((callBack) =>
                {
                    if (scatterWin != null)
                    {
                        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                        double totalScatterWin = 0;
                        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                        totalScatterWin = (scatterWin.ScatterP.pay * lineBet) + (scatterWin.ScatterP.totalBetMult * totalLineBet);
                        //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
                        if (SlotController.Instance.PlayFreeSpins)
                        {
                            totalScatterWin = totalScatterWin * SlotController.Instance.featureMultiply;
                        }
                        //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                        SlotMenuController.Instance.resultWinText.text = "You Won " + totalScatterWin.ToString(SlotController.Instance.decimalDisplay) + " on extra";
                        //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)

                        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  (ARC)
                        //>>>>>>>>>>>>>>>> (11/3/2021)
                        if (SlotMenuController.Instance.standardLineWin) SlotMenuController.Instance.standardLineWin.text = totalScatterWin >0 ?totalScatterWin.ToString(SlotController.Instance.decimalDisplay): "";
                        //>>>>>>>>>>>>>>>> end (11/3/2021)(ARC)

                    }

                    foreach (var item in scatterWinSymbols)
                    {
                        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                        item.WinTrigger(callBack, 1, false);
                        //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)

                    }

                });

                ts.Add((callBack) =>
                {

                    pT.Start(() =>
                    {
                        scatterWinCallBack?.Invoke();
                        callBack?.Invoke();
                       
                    });
                });

            }

            //show wildWin
            if (wildWinSymbols != null && wildWinSymbols.Count > 0)
            {
                ParallelTween pT;
                pT = new ParallelTween();
                pT.Add((callBack) =>
                {
                    if (wildWin != null)
                    {
                        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                        double totalWildWin = 0;
                        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                        totalWildWin = (wildWin.WildP.pay * lineBet) + (wildWin.WildP.totalBetMult * totalLineBet);
                        //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
                        if (SlotController.Instance.PlayFreeSpins)
                        {
                            totalWildWin = totalWildWin * SlotController.Instance.featureMultiply;
                        }
                        //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                        SlotMenuController.Instance.resultWinText.text = "You Won " + totalWildWin.ToString(SlotController.Instance.decimalDisplay) + " on extra";
                        //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
                        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  (ARC)
                        //>>>>>>>>>>>>>>>> (11/3/2021)
                        if (SlotMenuController.Instance.standardLineWin) SlotMenuController.Instance.standardLineWin.text = totalWildWin>0? totalWildWin.ToString(SlotController.Instance.decimalDisplay):"";
                        //>>>>>>>>>>>>>>>> end (11/3/2021)
                    }
                    foreach (var item in wildWinSymbols)
                    {
                        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                        item.WinTrigger(callBack, 1, false);
                        //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
                    }

                });
                

                ts.Add((callBack) =>
                {
                    pT.Start(() =>
                    {
                        wildWinCallback?.Invoke();
                        callBack?.Invoke();
                       
                    });
                });
                

            }

            if (isAutoSpin)
            {
                ts.Add((callBack) =>
                {

                    foreach (LineBehavior lB in LineBehL)
                    {
                        if (lB.IsWinningLine)
                        {
                            autoLineWinCounter++;
                            lB.LineFlashing(true);
                            lB.SetLineVisible(true);
                            lB.LineWin(1,
                                (windata) =>
                                {
                                    foreach (LineBehavior l in LineBehL)
                                    {
                                        l.SetLineVisible(false);
                                        l.LineFlashing(false);

                                    }
                                    lineWinCallBack?.Invoke(windata);
                                    callBack();

                                });
                        }
                        

                    }
                    if(autoLineWinCounter == 0)
                    {
                        callBack();
                    }
                });
                 
            }
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
            if (!isAutoSpin || (SlotController.Instance.enableDisplayWinLineOnAuto && !SlotController.Instance.turboMode))
            {
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                foreach (LineBehavior lB in LineBehL)
                {
                    if (lB.IsWinningLine)
                    {
                       
                        ts.Add((callBack) =>
                        {
                            double totalLineWin = 0;
                            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                            if (lB.win.WinPay > 0)
                            {
                                totalLineWin += lB.win.WinPay * lineBet;
                            }
                            if (lB.win.WinTotalBetMulti > 0)
                            {
                                totalLineWin += lB.win.WinTotalBetMulti;
                            }
                            
                            if (SlotController.Instance.PlayFreeSpins)
                            {
                                //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                                SlotMenuController.Instance.resultWinText.text = "You Won " + (totalLineWin * SlotController.Instance.featureMultiply ).ToString(SlotController.Instance.decimalDisplay)  + " on Line " + (lB.number + 1);
                                //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
                                //>>>>>>>>>>>>>>>> (11/3/2021)
                                if(SlotMenuController.Instance.standardLineWin)SlotMenuController.Instance.standardLineWin.text =
                                    (totalLineWin * SlotController.Instance.featureMultiply) > 0 ?(totalLineWin * SlotController.Instance.featureMultiply).ToString(SlotController.Instance.decimalDisplay):"";
                                //>>>>>>>>>>>>>>>> end (11/3/2021)
                            }
                            else
                            {
                                //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
                                SlotMenuController.Instance.resultWinText.text = "You Won " + totalLineWin.ToString(SlotController.Instance.decimalDisplay) + " on Line " + (lB.number + 1);
                                //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
                                //>>>>>>>>>>>>>>>> (11/3/2021)
                                if (SlotMenuController.Instance.standardLineWin) SlotMenuController.Instance.standardLineWin.text = totalLineWin > 0? totalLineWin.ToString(SlotController.Instance.decimalDisplay):"";
                                //>>>>>>>>>>>>>>>> end (11/3/2021)
                            }
                            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
                            lB.LineFlashing(true);
                            lB.SetLineVisible(true);
                            lB.LineWin( 1,
                                (windata) =>
                                {
                                    lB.SetLineVisible(false);
                                    lB.LineFlashing(false);
                                    lB.LineWinCancel();

                                    lineWinCallBack?.Invoke(windata);
                                    callBack();
                                });
                        });

                    }
                }
            }
            
            ts.Add((callBack) => { completeCallBack?.Invoke(); callBack(); });
            ts.Start();
        }






        /// <summary>
        /// Show win symbols blin continuous
        /// </summary>
        //>>>>>>>>>>>>>>>> (30/3/2021)
        internal void WinSymbolShowContinuous(bool isAutoSpin, Action<WinData> lineWinCallBack, Action scatterWinCallBack, Action specialIconWinCallBack, Action jackPotWinCallBack, Action wildWinCallBack, Action completeCallBack)
        {
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            
            contTS = new TweenSeq();
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            double LineBet = SlotPlayer.Instance.LineBet;
            double totalLineBet = SlotPlayer.Instance.TotalBet;
            contTS.Add((callBack) => {
                //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                WinSymbolShow(isAutoSpin, LineBet, totalLineBet, lineWinCallBack, scatterWinCallBack, specialIconWinCallBack, jackPotWinCallBack, wildWinCallBack, 
                    () =>
                    {

                        completeCallBack?.Invoke();
                        callBack();
                    });
                //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            });
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            if (isAutoSpin)
            {
                contTS.Start();
            }
            else
            {
                contTS.StartCycle();
            }


        }

        //>>>>>>>>>>>>>>>> end (30/3/2021)
        /// <summary>
        /// Show selected lines with flashing or without
        /// </summary>
        internal void ShowSelectedLines(bool flashing)
        {
            LineBehL.ForEach((lB) =>
            {
                if (lB.IsSelected)
                {
                    lB.SetLineVisible(true);
                }
                lB.LineFlashing(flashing);
            });
        }

        /// <summary>
        /// Hide selected lines
        /// </summary>
        internal void HideAllLines()
        {
            LineBehL.ForEach((lB) =>
            {
                lB.LineFlashing(false);
                lB.LineBurn(false, 0, null);
            });
        }

        /// <summary>
        /// Reset winning line data
        /// </summary>
        internal void ResetLineWinning()
        {
            foreach (LineBehavior lb in LineBehL)
            {
                lb.ResetLineWinning();
            }

            scatterWinSymbols = null;
            wildWinSymbols = null;
            jackPotWinSymbols = null;
            scatterWin = null;
            wildWin = null;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            specialIconWinSymbols = null;
            specialIconWin = null;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        }

        internal void WinCancel()
        {
            if (contTS != null) contTS.Break();

            foreach (LineBehavior lb in LineBehL)
            {
                lb.LineWinCancel();
            }
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            if (useSpecialIcon && specialIconWinSymbols != null)
                foreach (var item in specialIconWinSymbols)
                {
                    item.WinCancel();
                }
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            if (useScatter && scatterWinSymbols != null)
                foreach (var item in scatterWinSymbols)
                {
                    item.WinCancel();
                }

            if (wildWinSymbols != null)
                foreach (var item in wildWinSymbols)
                {
                    item.WinCancel();
                }

            if (jackPotWinSymbols != null)
                foreach (var item in jackPotWinSymbols)
                {
                    item.WinCancel();
                }
            SimpleTween.Cancel(contID, false);
        }

        

        

        #endregion winshow

        private bool HasScatterWin()
        {
            return scatterWin != null;
        }
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        private bool HasSpecialIconWin()
        {
            return specialIconWin != null;
        }
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)

        private bool HasWildWin()
        {
            return wildWin != null;
        }

        private JackPotType GetJackPotWin()
        {
            if (!useMiniJackPot && !useMaxiJackPot && !useMegaJackPot) return JackPotType.None;

            jackPotWinSymbols = null;
            Dictionary<int, List<SlotSymbol>> sD = new Dictionary<int, List<SlotSymbol>>();

            // create symbols dictionary
            foreach (var item in slotGroupsBeh)
            {
                RayCaster[] rCs = item.RayCasters;
                foreach (var rc in rCs)
                {
                    SlotSymbol s = rc.GetSymbol();
                    int sID = s.iconID;
                    if (sD.ContainsKey(sID))
                    {
                        sD[sID].Add(s);
                    }
                    else
                    {
                        sD.Add(sID, new List<SlotSymbol> { s });
                    }
                }
            }

            // search jackPot id if symbol is any
            if (jp_symb_id == -1)
            {
                int sCount = 0;
                int id = -1;
                foreach (var item in sD)
                {
                    if (item.Value.Count > sCount)
                    {
                        sCount = item.Value.Count;
                        id = item.Key;
                    }
                }

                if (sD.ContainsKey(id))
                {
                    if (useMegaJackPot && sCount >= megaJackPotCount && megaJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mega;
                    }
                    if (useMaxiJackPot && sCount >= maxiJackPotCount && maxiJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Maxi;
                    }
                    if (useMiniJackPot && sCount >= miniJackPotCount && miniJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mini;
                    }
                }
            }
            else
            {
                int id = jp_symb_id;
                if (sD.ContainsKey(id))
                {
                    if (useMegaJackPot && sD[id].Count >= megaJackPotCount && megaJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mega;
                    }
                    if (useMaxiJackPot && sD[id].Count >= maxiJackPotCount && maxiJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Maxi;
                    }
                    if (useMiniJackPot && sD[id].Count >= miniJackPotCount && miniJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mini;
                    }
                }
            }
            return JackPotType.None;
        }
        //>>>>>>>>>>>>>>>> (25/3/2021)
        public int GetFeatureMultiply()
        {
            int res = 0;
            if (specialIconWin != null)
            {

                if (specialIconWin.SpecialIconP.featureMultiply > 0) res = specialIconWin.SpecialIconP.featureMultiply;


            }
            

            if (scatterWin != null)
            {

                if(scatterWin.ScatterP.featureMultiply > 0) res = scatterWin.ScatterP.featureMultiply;

            }


            if (wildWin != null)
            {
                
                if (wildWin.WildP.featureMultiply > 0) res = wildWin.WildP.featureMultiply;
            }
            return res;
        }
        //>>>>>>>>>>>>>>>> end (25/3/2021)
        /// <summary>
        /// Return line win coins + sctater win coins, without jackpot
        /// </summary>
        /// <returns></returns>
        public double GetWinCoins()
        {
            double res = 0;
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsWinningLine)
                {
                    res += lB.win.Pay;
                }
            }
            //>>>>>>>>>>>>>>>> (25/3/2021)
            if (specialIconWin != null)
            {

                if (specialIconWin.IsWildWin)
                {
                    
                    res += specialIconWin.Pay * sC.wild_multiply;
                    
                    if (specialIconWin.SpecialIconP.totalBetMult > 0) res += (specialIconWin.SpecialIconP.totalBetMult * SlotPlayer.Instance.TotalBet * sC.wild_multiply);


                }
                else
                {
                    
                    res += specialIconWin.Pay;
                    
                    if (specialIconWin.SpecialIconP.totalBetMult > 0) res += (specialIconWin.SpecialIconP.totalBetMult * SlotPlayer.Instance.TotalBet);
                }
                

            }
            //>>>>>>>>>>>>>>>> end (25/3/2021)

            if (scatterWin != null)
            {
                
                if (scatterWin.IsWildWin)
                {
                    //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                    res += scatterWin.Pay * sC.wild_multiply;
                    //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
                    if (scatterWin.ScatterP.totalBetMult > 0) res += (scatterWin.ScatterP.totalBetMult * SlotPlayer.Instance.TotalBet * sC.wild_multiply);


                }
                else
                {
                    //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                    res += scatterWin.Pay;
                    //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
                    if (scatterWin.ScatterP.totalBetMult > 0) res += (scatterWin.ScatterP.totalBetMult * SlotPlayer.Instance.TotalBet);
                }

            }


            if (wildWin != null)
            {
                //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                res += wildWin.Pay;
                //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
                if (wildWin.WildP.totalBetMult > 0) res += (wildWin.WildP.totalBetMult * SlotPlayer.Instance.TotalBet);
            }
            return res;
        }

        public double GetWinTotalBetMulti()
        {
            double res = 0;
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsWinningLine)
                {
                    res += lB.win.WinTotalBetMulti;
                }
            }
            return res;
        }

        /// <summary>
        /// Return line win spins + sctater win spins
        /// </summary>
        /// <returns></returns>
        public int GetWinSpins()
        {
            int res = 0;
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsWinningLine)
                {
                    res += lB.win.FreeSpins;
                }
            }

            if (scatterWin != null) res += scatterWin.FreeSpins;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            if (specialIconWin != null) res += specialIconWin.FreeSpins;
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            if (wildWin != null) res += wildWin.FreeSpins;
            return res;
        }

        
        

        /// <summary>
        /// Return product of lines payMultiplier, sctater payMultiplier
        /// </summary>
        /// <returns></returns>
        

        public int GetWinLinesCount()
        {
            int res = 0;
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsWinningLine)
                {
                    res++;
                }
            }
            return res;
        }

        public void StartLineEvents()
        {
            foreach (LineBehavior lB in LineBehL)
            {
                if (lB.IsWinningLine)
                {
                    lB.win?.WinEvent?.Invoke();
                }
            }
        }
    }

    // GetInterface method for gameobject
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns all monobehaviours (casted to T)
        /// </summary>
        /// <typeparam name="T">interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfaces<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            //  var mObjs = gObj.GetComponents<MonoBehaviour>();
            var mObjs = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        /// <summary>
        /// Returns the first monobehaviour that is of the interface type (casted to T)
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterface<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfaces<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first instance of the monobehaviour that is of the interface type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterfaceInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all monobehaviours in children that implement the interface of type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

            var mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }
    }

    public enum RNGType { Unity, MersenneTwister }
    public class RNG
    {
        private int[] randSymb;
        private RNGType rngType;
        private Action UpdateRNGAction;
        private ReelData[] reelsData;
        private RandomMT randomMT;

        public RNG(RNGType rngType, ReelData[] reelsData)
        {
            randSymb = new int[reelsData.Length];
            this.rngType = rngType;
            this.reelsData = reelsData;
            switch (rngType)
            {
                case RNGType.Unity:
                    UpdateRNGAction = UnityRNGUpdate;
                    break;
                case RNGType.MersenneTwister:
                    randomMT = new RandomMT();
                    UpdateRNGAction = MTRNGUpdate;
                    break;
                default:
                    UpdateRNGAction = UnityRNGUpdate;
                    break;
            }
        }

        public void Update()
        {
            UpdateRNGAction();

        }

        public int[] GetRandSymbols()
        {
            return randSymb;
        }

        int rand;
        private void UnityRNGUpdate()
        {
            for (int i = 0; i < randSymb.Length; i++)
            {
                rand = UnityEngine.Random.Range(0, reelsData[i].Length);
                randSymb[i] = rand;
            }
        }

        private void MTRNGUpdate()
        {
            for (int i = 0; i < randSymb.Length; i++)
            {
                rand = randomMT.RandomRange(0, reelsData[i].Length - 1);
                randSymb[i] = rand;
            }
        }
    }

    [Serializable]
    public class ReelData
    {
        public List<int> symbOrder;
        public ReelData(List<int> symbOrder)
        {
            this.symbOrder = symbOrder;
        }
        public int Length
        {
            get { return (symbOrder == null) ? 0 : symbOrder.Count; }
        }
        public int GetSymbolAtPos(int position)
        {
            return (symbOrder == null || position >= symbOrder.Count) ? 0 : symbOrder.Count;
        }
    }

    /// <summary>
	/// Summary description for RandomMT.https://www.codeproject.com/Articles/5147/A-C-Mersenne-Twister-class
	/// </summary>
	public class RandomMT
    {
        private const int N = 624;
        private const int M = 397;
        private const uint K = 0x9908B0DFU;
        private const uint DEFAULT_SEED = 4357;

        private ulong[] state = new ulong[N + 1];
        private int next = 0;
        private ulong seedValue;


        public RandomMT()
        {
            SeedMT(DEFAULT_SEED);
        }
        public RandomMT(ulong _seed)
        {
            seedValue = _seed;
            SeedMT(seedValue);
        }

        public ulong RandomInt()
        {
            ulong y;

            if ((next + 1) > N)
                return (ReloadMT());

            y = state[next++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9D2C5680U;
            y ^= (y << 15) & 0xEFC60000U;
            return (y ^ (y >> 18));
        }

        private void SeedMT(ulong _seed)
        {
            ulong x = (_seed | 1U) & 0xFFFFFFFFU;
            int j = N;

            for (j = N; j >= 0; j--)
            {
                state[j] = (x *= 69069U) & 0xFFFFFFFFU;
            }
            next = 0;
        }

        public int RandomRange(int lo, int hi)
        {
            return (Math.Abs((int)RandomInt() % (hi - lo + 1)) + lo);
        }

        public int RollDice(int face, int number_of_dice)
        {
            int roll = 0;
            for (int loop = 0; loop < number_of_dice; loop++)
            {
                roll += (RandomRange(1, face));
            }
            return roll;
        }

        public int HeadsOrTails() { return ((int)(RandomInt()) % 2); }

        public int D6(int die_count) { return RollDice(6, die_count); }
        public int D8(int die_count) { return RollDice(8, die_count); }
        public int D10(int die_count) { return RollDice(10, die_count); }
        public int D12(int die_count) { return RollDice(12, die_count); }
        public int D20(int die_count) { return RollDice(20, die_count); }
        public int D25(int die_count) { return RollDice(25, die_count); }


        private ulong ReloadMT()
        {
            ulong[] p0 = state;
            int p0pos = 0;
            ulong[] p2 = state;
            int p2pos = 2;
            ulong[] pM = state;
            int pMpos = M;
            ulong s0;
            ulong s1;

            int j;

            if ((next + 1) > N)
                SeedMT(seedValue);

            for (s0 = state[0], s1 = state[1], j = N - M + 1; --j > 0; s0 = s1, s1 = p2[p2pos++])
                p0[p0pos++] = pM[pMpos++] ^ (mixBits(s0, s1) >> 1) ^ (loBit(s1) != 0 ? K : 0U);


            for (pM[0] = state[0], pMpos = 0, j = M; --j > 0; s0 = s1, s1 = p2[p2pos++])
                p0[p0pos++] = pM[pMpos++] ^ (mixBits(s0, s1) >> 1) ^ (loBit(s1) != 0 ? K : 0U);


            s1 = state[0];
            p0[p0pos] = pM[pMpos] ^ (mixBits(s0, s1) >> 1) ^ (loBit(s1) != 0 ? K : 0U);
            s1 ^= (s1 >> 11);
            s1 ^= (s1 << 7) & 0x9D2C5680U;
            s1 ^= (s1 << 15) & 0xEFC60000U;
            return (s1 ^ (s1 >> 18));
        }

        private ulong hiBit(ulong _u)
        {
            return ((_u) & 0x80000000U);
        }
        private ulong loBit(ulong _u)
        {
            return ((_u) & 0x00000001U);
        }
        private ulong loBits(ulong _u)
        {
            return ((_u) & 0x7FFFFFFFU);
        }
        private ulong mixBits(ulong _u, ulong _v)
        {
            return (hiBit(_u) | loBits(_v));

        }
    }

    [Serializable]
    //Helper class for creating pay table
    public class PayTable
    {
        public int reelsCount;
        public List<PayLine> payLines;
    }

    [Serializable]
    public class PayLine
    {
        public int[] line;
        public bool reverseWin = false;
        public bool wildMulti = true;
        public int pay;
        public int freeSpins;
        public bool showEvent = false;
        public bool specialEffect = false;
        public bool onlyWinOneWay = true;
        public UnityEvent LineEvent;
        [Tooltip("Payouts multiplier, default value = 1")]
        public int payMult = 1; // payout multiplier
        public float totalBetMult = 0; // totalbet multiplier
        public string winStateName;

        public PayLine(ServerPaytable p)
        {
            int[] lineIds = { int.Parse(p.e1), int.Parse(p.e2), int.Parse(p.e3), int.Parse(p.e4), int.Parse(p.e5) };
            line = lineIds;
            reverseWin = (p.revwin == "1");
            wildMulti = (p.wildmult == "1");
            pay = int.Parse(p.pay);
            freeSpins = int.Parse(p.freespin);
            payMult = int.Parse(p.paymult);
            totalBetMult = int.Parse(p.totalbetmult);
            
        }

        public PayLine()
        {
            line = new int[5];
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = -1;
            }
        }

        public PayLine(int reelsCount)
        {
            line = new int[reelsCount];
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = -1;
            }
        }

        public PayLine(PayLine pLine)
        {
            if (pLine.line != null)
            {
                line = new int[pLine.line.Length];
                for (int i = 0; i < line.Length; i++)
                {
                    line[i] = pLine.line[i];
                }
                pay = pLine.pay;
                freeSpins = pLine.freeSpins;
                LineEvent = pLine.LineEvent;
                payMult = pLine.payMult;
                totalBetMult = pLine.totalBetMult;
                reverseWin = pLine.reverseWin;
                onlyWinOneWay = true;
            }
            else
            {
                line = new int[5];
                for (int i = 0; i < line.Length; i++)
                {
                    line[i] = -1;
                }
            }
        }

        public PayLine(int[] line, int pay, int freeSpins)
        {
            if (line != null)
            {
                this.line = line;
                this.pay = pay;
                this.freeSpins = freeSpins;
            }
        }

        public string ToString(Sprite[] sprites)
        {
            string res = "";
            if (line == null) return res;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] >= 0)
                    res += sprites[line[i]].name;
                else
                {
                    res += "any";
                }
                if (i < line.Length - 1) res += ";";
            }
            return res;
        }

        public string[] Names(SlotIcon[] sprites)
        {
            if (line == null) return null;
            string[] res = new string[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] >= 0)
                    res[i] = (sprites[line[i]] != null && sprites[line[i]].iconSprite != null) ? sprites[line[i]].iconSprite.name : "failed";
                else
                {
                    res[i] = "any";
                }
            }
            return res;
        }

        public float GetPayOutProb(SlotController sC)
        {
            return GetProbability(sC) * 100f * pay;
        }

        public float GetProbability(SlotController sC)
        {
            float res = 0;
            if (!sC) return res;
            if (line == null || sC.slotGroupsBeh == null || sC.slotGroupsBeh.Length != line.Length) return res;
            float[] rP = sC.slotGroupsBeh[0].GetReelSymbHitPropabilities(sC.slotIcons);
            
            //avoid "any" symbol error in first position
            res = (line[0] >= 0) ? rP[line[0]] : 1; //  res = rP[line[0]];

            for (int i = 1; i < line.Length; i++)
            {
                if (line[i] >= 0) // any.ID = -1
                {
                    rP = sC.slotGroupsBeh[i].GetReelSymbHitPropabilities(sC.slotIcons);
                    res *= rP[line[i]];
                }
                else
                {
                    // break;
                }
            }
            return res;
        }

        public bool IsMatch2()
        {
            if (line.Length == 2 && line[0] >= 0 && line[1] >= 0)
            {
                return true;
            }
            else if (line.Length > 2 && line[0] >= 0 && line[1] >= 0 && line[2] < 0) // line[2] - any
            {
                return true;
            }
            return false;
        }

        public bool IsMatch3()
        {
            if (line.Length == 3 && line[0] >= 0 && line[1] >= 0 && line[2] >= 0)
            {
                return true;
            }
            else if (line.Length > 3 && line[0] >= 0 && line[1] >= 0 && line[2] >= 0 && line[3] < 0) // line[3] - any
            {
                return true;
            }
            return false;
        }

        public bool IsMatch4()
        {
            if (line.Length == 4 && line[0] >= 0 && line[1] >= 0 && line[2] >= 0 && line[3] >= 0)
            {
                return true;
            }
            else if (line.Length > 4 && line[0] >= 0 && line[1] >= 0 && line[2] >= 0 && line[3] >= 0 && line[4] < 0) // line[4] - any
            {
                return true;
            }
            return false;
        }

        public bool IsMatch5()
        {
            if (line.Length == 5 && line[0] >= 0 && line[1] >= 0 && line[2] >= 0 && line[3] >= 0 && line[4] >= 0)
            {
                return true;
            }
            else if (line.Length > 5 && line[0] >= 0 && line[1] >= 0 && line[2] >= 0 && line[3] >= 0 && line[4] >= 0 && line[5] < 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create and return additional lines for this line with wild symbol,  only if symbol can be substitute with wild
        /// </summary>
        /// <returns></returns>
        public List<PayLine> GetWildLines(SlotController sC)
        {
            List<PayLine> res = new List<PayLine>();
            if (!sC) return res; // return empty list
            if (!sC.useWild) return res; //// return empty list

            int wild_id = sC.wild_id;
            int first = line[0];
            if (first < 0) return res; // first any - return empty list
            if (first == wild_id) return res; // return empty list

            if (IsMatch2())
            {
                //1)     F W ? ?
                PayLine pl1 = new PayLine(this);
                if (pl1.line[1] == 11)
                {
                    Debug.Log(sC.slotIcons[pl1.line[1]].iconSprite.name);
                    Debug.Log(sC.slotIcons[pl1.line[1]].useWildSubstitute);
                }
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1 - )     W F ? ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 

            }
            else if (IsMatch3())
            {
                //1)     F W W ? ?
                PayLine pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8 / 12 / 2020)
                //2 - )     W F W ? ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
                
                //(Updated)>>>>>>>>>>>>>>>> (8 / 12 / 2020)
                //2 - )     W W F ? ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 

                //2)     F F W ? ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //2)     F W F ? ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //2 - )     W F F ? ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 

                
            }
            else if (IsMatch4())
            {
                #region W 3 of 4
                //1)     F W W W ?
                PayLine pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W F W W ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W W F W ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W W W F ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                #endregion W 3 of 4

                #region W 2 of 4
                //2)     F F W W ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //3)     F W F W ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //3)     W F F W ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //4)     F W W F ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //4)     W F W F ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //4)     W W F F ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                #endregion W 2 of 4

                #region W 1 of 4
                //5)     F F F W ? 
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //6)     F F W F ? 
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //7)     F W F F ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //7)     W F F F ?
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                #endregion W 1 of 4

            }
            else if (IsMatch5())
            {

                #region W 4 of 5
                //1)     F W W W W
                PayLine pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W F W W W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W W F W W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W W W F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //1)     W W W W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                #endregion W 4 of 5

                #region W 3 of 5
                //2)     F F W W W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //3)     F W F W W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //3)     W F F W W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //4)     F W W F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //4)     W F W F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //4)     W W F F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //5)     F W W W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //5)     W F W W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //5)     W W F W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //5)     W W W F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                #endregion  W 3 of 5

                #region W 2 of 5
                //6)     F F F W W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //7)     F F W F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //7 - )     F W F F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //7 - )     W F F F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  

                //8)     F F W W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1); 

                //9)     F W F W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //10)     F W W F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //10)     W F W F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //10)     W W F F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                #endregion W 2 of 5

                #region W 1 of 5
                //10)     F F F F W
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[4]].useWildSubstitute) pl1.line[4] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //11)     F F F W F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[3]].useWildSubstitute) pl1.line[3] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //12)     F F W F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[2]].useWildSubstitute) pl1.line[2] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //13)     F W F F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[1]].useWildSubstitute) pl1.line[1] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);

                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                //13)     W F F F F
                pl1 = new PayLine(this);
                if (sC.slotIcons[pl1.line[0]].useWildSubstitute) pl1.line[0] = wild_id;
                if (!pl1.IsEqual(this) && !ContainEqualLine(res, pl1)) res.Add(pl1);
                //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)

                #endregion W 1 of 5
            }
            return res;
        }

        
        private bool IsEqual(PayLine pLine)
        {
            if (pLine == null) return false;
            if (pLine.line == null) return false;
            if (line.Length != pLine.line.Length) return false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != pLine.line[i]) return false;
            }
            return true;
        }

        private bool ContainEqualLine(List<PayLine> pList, PayLine pLine)
        {
            if (pList == null) return false;
            if (pLine == null) return false;
            if (pLine.line == null) return false;

            foreach (var item in pList)
            {
                if (item.IsEqual(pLine)) return true;
            }
            return false;
        }
    }
    //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
    [Serializable]
    public class SpecialIconPay
    {
        public int specialIconsCount;
        public int pay;
        public int freeSpins;
        public int payMult = 1;
        public float totalBetMult = 0;
        //>>>>>>>>>>>>>>>> (25/3/2021)
        public int featureMultiply = 0;
       
        //>>>>>>>>>>>>>>>> end (25/3/2021)
        public bool useSpecialEffect;

        [Header("Trigger On Feature Mode")]
        public bool enableFeatureMode = false;
        public int featurePay;
        public int featureFreeSpins;
        
        public int featurePayMult = 1;


        [Header("Winning Bonus Trigger")]
        public string soundBonusTrigger;
        public bool hasBonus;
        public BonusGameChoices WinEventBonusTrigger;

        public bool enablePauseAutoPlay;

        public SpecialIconPay()
        {
            payMult = 1;
            specialIconsCount = 3;
            pay = 0;
            freeSpins = 0;
            totalBetMult = 0;
            //>>>>>>>>>>>>>>>> (25/3/2021)
            featureMultiply = 0;
            //>>>>>>>>>>>>>>>> end (25/3/2021)
            useSpecialEffect = true;

        }

        public SpecialIconPay(SpecialIconPay s)
        {
            specialIconsCount = s.specialIconsCount;
            pay = s.pay;
            freeSpins = s.freeSpins;
            payMult = s.payMult;
            totalBetMult = s.totalBetMult;
            //>>>>>>>>>>>>>>>> (25/3/2021)
            featureMultiply = s.featureMultiply;
           
            //>>>>>>>>>>>>>>>> end (25/3/2021)

            enableFeatureMode = s.enableFeatureMode;
            featurePay = s.featurePay;
            featureFreeSpins = s.featureFreeSpins;
            featurePayMult = s.featurePayMult;
            WinEventBonusTrigger = s.WinEventBonusTrigger;
            hasBonus = s.hasBonus;
        }
    }
    //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)

    [Serializable]
    public class ScatterPay
    {
        public int scattersCount;
        public int pay;
        public int freeSpins;
        public int payMult = 1;
        public float totalBetMult = 0;
        //>>>>>>>>>>>>>>>> (25/3/2021)
        public int featureMultiply = 0;
        //>>>>>>>>>>>>>>>> end (25/3/2021)
        public bool useSpecialEffect;

        [Header("Trigger On Feature Mode")]
        public bool enableFeatureMode = false;
        public int featurePay;
        public int featureFreeSpins;
        public int featurePayMult = 1;
        


        [Header("Winning Bonus Trigger")]
        public string soundBonusTrigger;
        public bool hasBonus;
        public BonusGameChoices WinEventBonusTrigger;

        public bool enablePauseAutoPlay;

        public ScatterPay()
        {
            payMult = 1;
            scattersCount = 3;
            pay = 0;
            freeSpins = 0;
            totalBetMult = 0;
            //>>>>>>>>>>>>>>>> (25/3/2021)
            featureMultiply = 0;
            //>>>>>>>>>>>>>>>> end (25/3/2021)
            useSpecialEffect = true;

        }

        public ScatterPay(ScatterPay s)
        {
            scattersCount = s.scattersCount;
            pay = s.pay;
            freeSpins = s.freeSpins ;
            payMult = s.payMult;
            totalBetMult = s.totalBetMult;

            //>>>>>>>>>>>>>>>> (25/3/2021)
            featureMultiply = s.featureMultiply;
          
            //>>>>>>>>>>>>>>>> end (25/3/2021)

            enableFeatureMode = s.enableFeatureMode; 
            featurePay = s.featurePay;
            featureFreeSpins = s.featureFreeSpins;
            featurePayMult = s.featurePayMult;
            WinEventBonusTrigger = s.WinEventBonusTrigger;
            hasBonus = s.hasBonus;
        }
    }

    [Serializable]
    public class Result
    {
        public int[] pattern;
        public ServerPayDetial totalwon;
       
    }

    [Serializable]
    public class ServerPayDetial
    {
        public double pay;
        public int freespin;

    }

    [Serializable]
    public class ListOfServerPaytable
    {
        public List<ServerPaytable> list;

      
    }

    [Serializable]
    public class ServerPaytable
    {
        public string e1;
        public string e2;
        public string e3;
        public string e4;
        public string e5;
        public string revwin;
        public string wildmult;
        public string pay;
        public string freespin;
        public string paymult;
        public string totalbetmult;

    }

    [Serializable]
    public class WildPay
    {
        public int wildCount;
        public int pay;
        public int freeSpins;
        public int payMult = 1;
        public float totalBetMult = 0;
        //>>>>>>>>>>>>>>>> (25/3/2021)
        public int featureMultiply = 0;
        
        //>>>>>>>>>>>>>>>> end (25/3/2021)
        public bool useSpecialEffect;

        [Header("Trigger On Feature Mode")]
        public bool enableFeatureMode = false;
        public int featurePay;
        public int featureFreeSpins;
        
        public int featurePayMult = 1;


        [Header("Winning Bonus Trigger")]
        public string soundBonusTrigger;
        public bool hasBonus;
        public BonusGameChoices WinEventBonusTrigger;
        public bool enablePauseAutoPlay;
        public WildPay()
        {
            payMult = 1;
            wildCount = 3;
            pay = 0;
            totalBetMult = 0;
            freeSpins = 0;
            //>>>>>>>>>>>>>>>> (25/3/2021)
            featureMultiply = 0;
  
            //>>>>>>>>>>>>>>>> end (25/3/2021)
            useSpecialEffect = true;
        }
    }

    static class ClassExt
    {
        public enum FieldAllign { Left, Right, Center }

        /// <summary>
        /// Return formatted string; (F2, N5, e, r, p, X, D12, C)
        /// </summary>
        /// <param name="fNumber"></param>
        /// <param name="format"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string ToString(this float fNumber, string format, int field)
        {
            string form = "{0," + field.ToString() + ":" + format + "}";
            string res = String.Format(form, fNumber);
            return res;
        }

        /// <summary>
        /// Return formatted string; (F2, N5, e, r, p, X, D12, C)
        /// </summary>
        /// <param name="fNumber"></param>
        /// <param name="format"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string ToString(this string s, int field)
        {
            string form = "{0," + field.ToString() + "}";
            string res = String.Format(form, s);
            return res;
        }

        /// <summary>
        /// Return formatted string; (F2, N5, e, r, p, X, D12, C)
        /// </summary>
        /// <param name="fNumber"></param>
        /// <param name="format"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string ToString(this string s, int field, FieldAllign fAllign)
        {
            int length = s.Length;
            if (length >= field)
            {
                string form = "{0," + field.ToString() + "}";
                return String.Format(form, s);
            }
            else
            {
                if (fAllign == FieldAllign.Center)
                {
                    int lCount = (field - length) / 2;
                    int rCount = field - length - lCount;
                    string lSp = new string('*', lCount);
                    string rSp = new string('*', rCount);
                    return (lSp + s + rSp);
                }
                else if (fAllign == FieldAllign.Left)
                {
                    int lCount = (field - length);
                    string lSp = new string('*', lCount);
                    return (s + lSp);
                }
                else
                {
                    string form = "{0," + field.ToString() + "}";
                    return String.Format(form, s);
                }
            }
        }

        private static string ToStrings<T>(T[] a)
        {
            string res = "";
            for (int i = 0; i < a.Length; i++)
            {
                res += a[i].ToString();
                res += " ";
            }
            return res;
        }

        private static string ToStrings(float[] a, string format, int field)
        {
            string res = "";
            for (int i = 0; i < a.Length; i++)
            {
                res += a[i].ToString(format, field);
                res += " ";
            }
            return res;
        }

        private static string ToStrings(string[] a, int field, ClassExt.FieldAllign allign)
        {
            string res = "";
            for (int i = 0; i < a.Length; i++)
            {
                res += a[i].ToString(field, allign);
                res += " ";
            }
            return res;
        }

        private static float[] Mul(float[] a, float[] b)
        {
            if (a.Length != b.Length) return null;
            float[] res = new float[a.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = a[i] * b[i];
            }
            return res;
        }
    }
}