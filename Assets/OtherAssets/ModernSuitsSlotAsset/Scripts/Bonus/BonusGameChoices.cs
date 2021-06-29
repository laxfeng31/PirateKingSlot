//>>>>>>>>>>>>>>>> (5/2/2021)(ARC)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;
using UnityEngine.Events;
using DarkTonic.MasterAudio;

public class BonusGameChoices : BonusGame
{
    [Header("General Setting")]
    
    
    [SerializeField]
    private int numberOfPick = 4;
    [SerializeField]
    private int initialMultiplyWin = 1;
    [SerializeField]
    private int initialFreeSpinWin = 0;
    [SerializeField]
    private int initialMultiplyWinByBet = 0;
    [SerializeField]
    private GameObject resultGameObj;
    [Range(0f, 1f)]
    [SerializeField]
    float alphaOnUnpickedItem = 0.6f;
    [SerializeField]
    UnityEvent onFreeSpinEvent;

    [Space(5)]
    [Header("Ui Setting")]
    [SerializeField]
    Text totalBetText;
    [SerializeField]
    Text balanceText;
    [SerializeField]
    Text[] totalWinText;
    [SerializeField]
    Text[] totalFreeSpinText;
    [SerializeField]
    Text[] totalMultiplyWinText;
    [SerializeField]
    Text currentPickText;
    [SerializeField]
    string currentPickTextAdditionalWords = " Picks Remaining";
    [SerializeField]
    bool randomCoverSprite = true;
    [SerializeField]
    List<Button> listOfButtons = new List<Button>();
    [SerializeField]
    List<UnityEvent> buttonClick = new List<UnityEvent>();
    List<Image> listOfButtonImages = new List<Image>();

    List<Sprite> listOfCoverSprite = new List<Sprite>();
    [SerializeField]
    UnityEvent FinishChoiceTrigger;
    [Space(5)]
    [Header("Winning Setting")]
    [SerializeField]
    List<BonusGameChoicesType> listOfWinningType = new List<BonusGameChoicesType>();

    [Space(5)]
    [Header("Winning Text Animation")]
    [SerializeField]
    float timeAnimation = 5f;
    [SerializeField]
    EaseAnim animType;

    private double totalBet;
    private double totalWinCoin;
    private int totalMultiplyWin;
    private int totalWinFreeSpin;
    private int currentNumOfPick;
    private bool hasInit = false;
    private List<int> idButtonClicked = new List<int>();
    private TweenSeq tS;

    public GameObject hitEffect;
    public GameObject overlayBlock;
    [Header("Sound")]
    public string clickSound;
    public string bonusBackgroundSound;
    //public GameObject this_Set;

    private int coinsTweenId;
    private int winCoins;
    private int oldWinCoins;
    private float winCoinsTweenTime = 1f;


    [SerializeField]
    private Script_Save save;

    public double TotalBet {
        get => totalBet;
        private set
        {
            
            totalBet = value;
            if (totalBetText != null)
            {
                //>>>>>>>>>>>>>>>> (23/2/2021)
                totalBetText.text = totalBet.ToString(SlotController.Instance.decimalDisplay);
                //>>>>>>>>>>>>>>>> end (23/2/2021)
            }
        }
    }

    public double TotalWinCoin {
        get => totalWinCoin;
        private set
        {
            if (totalWinText.Length > 0)
            {
                if(totalWinCoin != value && value != 0)
                {
                    StartCoroutine(SlotMenuController.Instance.winCoinSound(2));
                   
                }
                foreach (Text t in totalWinText)
                {
                    //>>>>>>>>>>>>>>>> (23/2/2021)
                    TweenNumber.ShowAnimation(t, EaseAnim.EaseLinear, (float)totalWinCoin, (float)value, 2, SlotController.Instance.decimalDisplay);
                    //>>>>>>>>>>>>>>>> end (23/2/2021)


                }


            }
            totalWinCoin = value;

            
                
        }
    }

    private void Start()
    {
        SlotController.Instance.bonusScript = this;
    }

    public int CurrentNumOfPick {
        get => currentNumOfPick;
        private set
        {
            currentNumOfPick = value;
            if (currentPickText != null) currentPickText.text = currentNumOfPick.ToString() + currentPickTextAdditionalWords;
        }
    }

    public int TotalWinFreeSpin
    {
        get => totalWinFreeSpin;
        private set
        {
            totalWinFreeSpin = value;
            if (totalFreeSpinText.Length > 0)
            {
                foreach (Text t in totalFreeSpinText)
                {
                    t.text = totalWinFreeSpin.ToString();
                }

            }
        }
    }

    public int TotalMultiplyWin { 
        get => totalMultiplyWin;
        set { 
            
            totalMultiplyWin = value;
            foreach (Text t in totalMultiplyWinText)
            {
                t.text = totalMultiplyWin.ToString();
            }
        } 
    }
    
    void BonusSelect(int id)
    {
        TotalWinCoin += listOfWinningType[id].NumberOfMultiply * SlotPlayer.Instance.TotalBet;

        TotalWinFreeSpin += listOfWinningType[id].NumberOfFreeSpin;

        TotalMultiplyWin += listOfWinningType[id].WinMultiply;

        listOfButtonImages[id].sprite = listOfWinningType[id].SpriteImage;
        CurrentNumOfPick--;
    }


    private void StartTrigger()
    {

        
        MasterAudio.PlaySound("SFX_ToBonusFeature");

        
        if (!hasInit) Init();

        if (!string.IsNullOrEmpty(bonusBackgroundSound)) {
            if (!MasterAudio.IsSoundGroupPlaying(bonusBackgroundSound))
            {

                MasterAudio.PlaySound(bonusBackgroundSound);

            }
        }
        idButtonClicked.Clear();
        
        if (randomCoverSprite)
        {
            ShuffleList<Sprite> shuffleCoverSprite = new ShuffleList<Sprite>();
            listOfCoverSprite = shuffleCoverSprite.Shuffle(listOfCoverSprite);
            for (int i = 0; i < listOfButtonImages.Count; i++)
            {
                listOfButtonImages[i].sprite = listOfCoverSprite[i];
            }
        }

        //shuffle result

        ShuffleList<BonusGameChoicesType> shuffleResult = new ShuffleList<BonusGameChoicesType>();
        if (SlotController.Instance.getUseServerResult())
        {
            listOfWinningType = shuffleResult.Shuffle(listOfWinningType);
        }
        


        ClearButtonsEvent();

        //assign result to buttons
        for (int i = 0; i < listOfButtons.Count; i++)
        {
            int id = i;
            Animator anim = listOfButtons[id].GetComponent<Animator>();
            anim.SetBool("isWin", false);
            anim.SetBool("isClicked", false);
            listOfButtons[i].onClick.AddListener(delegate
            {
                if(buttonClick.Count > id)
                {
                    if (buttonClick[id] != null)
                        buttonClick[id].Invoke();
                }

                BonusSelect(id);

                listOfButtonImages[id].color = new Color(255, 255, 255, 1);
                idButtonClicked.Add(id);
                
                EndBonusGame();
                listOfButtons[id].onClick.RemoveAllListeners();
                anim.SetBool("isClicked", true);
                GameObject particles = Instantiate(hitEffect, listOfButtonImages[id].transform);
                
                StartCoroutine(wfs3(anim, particles));
            });
        }
        
        GetAllData();
    }

    IEnumerator wfs3(Animator anim, GameObject particles)
    {
        if(!string.IsNullOrEmpty(clickSound))
            MasterAudio.PlaySound(clickSound);
        
        overlayBlock.SetActive(true);
        yield return new WaitForSeconds(3f);
        overlayBlock.SetActive(false);
        anim.SetBool("isWin", true);
        Destroy(particles);
    }

    void Init()
    {
        foreach(Button b in listOfButtons)
        {
            Image i = b.GetComponent<Image>();
            if (i != null)
            {
                listOfButtonImages.Add(i);
                listOfCoverSprite.Add(i.sprite);
            }
            
        }
        hasInit = true;
        
    }

    void GetAllData()
    {
        TotalBet = SlotPlayer.Instance.TotalBet;
        //>>>>>>>>>>>>>>>> (23/2/2021)(ARC)
        if (balanceText!=null)
            balanceText.text = SlotPlayer.Instance.Coins.ToString(SlotController.Instance.decimalDisplay);
        //>>>>>>>>>>>>>>>> end (23/2/2021)(ARC)
        TotalWinCoin = initialMultiplyWinByBet * SlotPlayer.Instance.TotalBet;
        TotalWinFreeSpin = initialFreeSpinWin;
        TotalMultiplyWin = initialMultiplyWin;
        CurrentNumOfPick = numberOfPick;
        
        SlotController.Instance.IsOnBonus = true;
    }

    void ClearButtonsEvent()
    {
        //save.saveMatchHistory(SlotPlayer.Instance.TotalBet, TotalWinCoin, "BONUS");
        foreach (Button b in listOfButtons)
        {
            b.onClick.RemoveAllListeners();
        }
    }
    
    void EndBonusGame()
    {
        if (CurrentNumOfPick <= 0)
        {
            SlotPlayer.Instance.toggleIsBonus();
            SlotPlayer.Instance.AddFreeSpins(TotalWinFreeSpin);
            if (!SlotController.Instance.onlyAddCoinOnEndFeature) SlotPlayer.Instance.AddCoins(TotalWinCoin);
            SlotController.Instance.totalFeatureWin += TotalWinCoin;
            
            //SlotMenuController.Instance.SetBonusWinInfo(TotalWinCoin,totalWinText[1]);
            SlotController.Instance.featureMultiply = TotalMultiplyWin;
            SlotPlayer.Instance.toggleIsBonus();
            ClearButtonsEvent();
            

            DisplayAllButtons();
            resultGameObj.SetActive(true);
            SlotPlayer.Instance.saveCoinsUpdate(TotalWinCoin, TotalWinFreeSpin);

        }
    }
    bool coinSoundIsPlaying = false;
    public void ShowAnimation()
    {
        if(TotalWinCoin > 0)
        {
            SlotMenuController.Instance.StartCoinSound();
            coinSoundIsPlaying = true;
        }
       
        for (int i = 0; i<totalWinText.Length; i++)
        {
            //>>>>>>>>>>>>>>>> (23/2/2021)
            if (i == 0)
            {
                TweenNumber.ShowAnimation(totalWinText[i], animType, 0, (float)TotalWinCoin, timeAnimation,SlotController.Instance.decimalDisplay,()=> 
                {
                    StopCoinSound();
                });
            }
            else
            {
                TweenNumber.ShowAnimation(totalWinText[i], animType, 0, (float)TotalWinCoin, timeAnimation,SlotController.Instance.decimalDisplay);
            }
            //>>>>>>>>>>>>>>>> end (23/2/2021)

        }
    }

    void StopCoinSound()
    {
        if (coinSoundIsPlaying)
        {
            SlotMenuController.Instance.EndCoinSound();
            coinSoundIsPlaying = false;
        }
    }

    void DisplayAllButtons()
    {
        for(int i = 0; i<listOfButtons.Count; i++)
        {
            int id = i;
            if (!idButtonClicked.Contains(i))
            {
                StartCoroutine(wfs3(id));
                
            }
        }
        FinishChoiceTrigger.Invoke();
    }

    IEnumerator wfs3(int id)
    {
        yield return new WaitForSeconds(3f);
        listOfButtons[id].animator.SetBool("isShowOthers", true);
        listOfButtonImages[id].sprite = listOfWinningType[id].SpriteImage;
        var tempColor = listOfButtonImages[id].color;
        tempColor.a = alphaOnUnpickedItem;
        listOfButtonImages[id].color = tempColor;
    }

    public void CloseBonus()
    {
        for (int i = 0; i < listOfButtons.Count; i++)
        {
            listOfButtonImages[i].sprite = listOfWinningType[i].SpriteImage;
            var tempColor = listOfButtonImages[i].color;
            if (PlayerPrefs.GetString("current_Scene") == "WongChoy" || PlayerPrefs.GetString("current_Scene") == "GoldenLotus" || PlayerPrefs.GetString("current_Scene") == "BigProsperity")
            {
                tempColor.a = 0.0f;
            } else
            {
                tempColor.a = 1.0f;
            }
            listOfButtonImages[i].color = tempColor;
        }
        resultGameObj.SetActive(false);
        
        
        
        if (!string.IsNullOrEmpty(bonusBackgroundSound))
            MasterAudio.StopAllOfSound(bonusBackgroundSound);
        if (TotalWinFreeSpin > 0)
        {
            MasterAudio.StopEverything();
            MasterAudio.PlaySound("FeatureGameBG");
            onFreeSpinEvent.Invoke();
        }
        StopCoinSound();
        Debug.Log("isEnter");
        if (SlotMenuController.Instance.totalFeatureWinInfoText) SlotMenuController.Instance.totalFeatureWinInfoText.gameObject.SetActive(true);
        SlotMenuController.Instance.SetTotalFeatureWinInfo(SlotController.Instance.totalFeatureWin);
        SlotController.Instance.IsOnBonus = false;
        hasEndedBonus = true;
        gameObject.SetActive(false);
    }

    public override void OnTrigger()
    {
        //MasterAudio.StopAllOfSound("WildWin");
        base.OnTrigger();

        StartTrigger();
    }

}

public class ShuffleList<T>
{
    public List<T> Shuffle(List<T> listToShuffle)
    {

        for (int i = 0; i < listToShuffle.Count; i++)
        {
            T temp = listToShuffle[i];
            int randomIndex = UnityEngine.Random.Range(i, listToShuffle.Count);
            listToShuffle[i] = listToShuffle[randomIndex];
            listToShuffle[randomIndex] = temp;
        }

        return listToShuffle;
    }
}

[System.Serializable]
public class BonusGameChoicesType
{
    [Header("Free spin setting")]
    [SerializeField]
    private int numberOfFreeSpin;

    [Header("Multiply with bet setting")]
    [SerializeField]
    private int numberOfMultiply;

    [Header("Multiply win for feature")]
    [SerializeField]
    private int winMultiply = 0;

    [Header("General setting")]
    [SerializeField]
    private Sprite spriteImage;

    //>>>>>>>>>>>>>>>> (15/3/2021)
    [SerializeField]
    private bool triggerEndGame = false;

    [SerializeField]
    private bool addFeatureMultiply = false;
    //>>>>>>>>>>>>>>>> end (15/3/2021)

    public int NumberOfFreeSpin { get => numberOfFreeSpin;  set => numberOfFreeSpin = value; }
    public int NumberOfMultiply { get => numberOfMultiply;  set => numberOfMultiply = value; }
    public Sprite SpriteImage { get => spriteImage; private set => spriteImage = value; }
    public int WinMultiply { get => winMultiply; set => winMultiply = value; }
    public bool TriggerEndGame { get => triggerEndGame; set => triggerEndGame = value; }
    public bool AddFeatureMultiply { get => addFeatureMultiply; set => addFeatureMultiply = value; }
}
//>>>>>>>>>>>>>>>> end (5/2/2021)