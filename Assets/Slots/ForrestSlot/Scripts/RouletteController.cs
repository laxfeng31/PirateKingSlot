//>>>>>>>>>>>>>>>> (23/2/2021)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class RouletteController : BonusGame
{
    [System.Serializable]
    class ColorSector
    {
  
        public Text[] displayReward;
        public Animator GlowSector;
        public int spotStop;
        public string Reward;
        public string AmountReward;

        [Header("Amount of Reward setting")]
        [SerializeField]
        private int betMultiply = 0;
        [Header("Free spin setting")]
        [SerializeField]
        private int numberOfFreeSpin = 0;

        [Header("Multiply with bet setting")]
        [SerializeField]
        private int numberOfMultiply;

        [Header("Multiply win for feature")]
        [SerializeField]
        private int numberFeatureMultiply = 0;

        public int BetMultiply { get => betMultiply; set => betMultiply = value; }
        public int NumberOfFreeSpin { get => numberOfFreeSpin; set => numberOfFreeSpin = value; }
        public int NumberOfMultiply { get => numberOfMultiply; set => numberOfMultiply = value; }
        public int NumberOfFeatureMultiply { get => numberFeatureMultiply; set => numberFeatureMultiply = value; }
    }


    bool IsSpin;
    bool StartGame;
    bool ReadyStop;
    bool RouletteStop;

    [Header("General Setting")]
    [SerializeField]
    int numberOfSpin = 4;
    int remainSpinCount;
    [SerializeField]
    int minRotate = 4;
    [SerializeField]
    int maxRotate = 4;
    public float Speed;
    public float ConstantSpeed;
    public float lerpspeed;
    Quaternion tempRotate;
    //public float maxSpeed;
    public float GetReward;

    public float delayResult;

    [Header("UI Setting")]
    [SerializeField]
    Text[] numPickText;
    [SerializeField]
    Text[] currentBalanceText;
    [SerializeField]
    Text[] currentTotalBet;
    [SerializeField]
    Text[] totalWinText;
    [SerializeField]
    Text[] totalFreespinText;
    [SerializeField]
    Text[] totalFeatureMultiplyText;

    [Header("Sector Setting")]
    [SerializeField]
    List<ColorSector> ColorSec = new List<ColorSector>();

    [Header("Finish Pick Setting")]

    [SerializeField]
    GameObject displayPanel;
    List<int> collectResultId = new List<int>();
    int spinCount = 0;
    int currentResult;
    public double totalWinCoin;
    int totalFreespin;
    int totalFeatureMultiply = 1;
    public GameObject roulettePlate;
    public GameObject Pointer;
    public GameObject CollectBtn;
    public Animator Roulette;
    public Button rouletteBtn;
    public Text RewardText;

    public GameObject SoundGO;
    public WinAnimation winAnim;

    void Init()
    {
        StartGame = false;
        IsSpin = false;
        ReadyStop = false;
        RouletteStop = true;
        spinCount = 0;
        totalWinCoin = 0;
        totalFreespin = 0;
        totalFeatureMultiply = 1;
        if(CollectBtn)
        {
            CollectBtn.SetActive(false);
        }
        RewardText.text = "";
        UpdateUiText();
        
        UpdateTexts(totalWinText, totalWinCoin.ToString(SlotController.Instance.decimalDisplay));
        foreach(ColorSector sec in ColorSec)
        {
            Debug.Log(SlotPlayer.Instance.TotalBet);
            sec.AmountReward = (SlotPlayer.Instance.TotalBet*sec.BetMultiply).ToString(); 
            UpdateTexts(sec.displayReward,sec.Reward);
        }
        ResetRoulette();
    }
    void StartTrigger()
    {
        Init();
        GenerateResult();
    }

    void Update()
    {
        RotateRolette();
    }

    void RotateRolette()
    {
        if(StartGame)
        {
           if(ReadyStop)
            {
                SmoothRotate();
            }
            else
            {
                roulettePlate.transform.Rotate(0,0, -Speed * Time.deltaTime);
                if (!IsSpin)
                {
                    if(Speed>ConstantSpeed)
                    {
                        SlowDownRoulette();
                    }
                    if(Speed<=ConstantSpeed && !ReadyStop)
                    {
                        if(CheckDistance())
                        {
                            ReadyStop = true;
                            tempRotate = roulettePlate.transform.rotation; 
                        }
                    }
                }
                
            }
        }
        if(IsSpin)
        {
            rouletteBtn.GetComponent<Button>().enabled = false;
            rouletteBtn.gameObject.SetActive(false);
            IsSpin=false;
            SlowDownRoulette();
        }
    }
    public void StartRoulette()
    {
        Debug.Log("ok");
        if(spinCount<numberOfSpin)
        {
            RewardText.text = "";
            Roulette.SetBool("Spin",true);
            Speed = SpeedChange();
            currentResult = ColorSec[collectResultId[spinCount]].spotStop;
            Debug.Log(ColorSec[collectResultId[spinCount]].spotStop);
            ResetRoulette();
            StartGame=true;
            RouletteStop = false;
            IsSpin=true;
            spinCount++;
            
        }
        
    }
    public void SlowDownRoulette()
    {
        Speed--;
        if(Speed <= 0)
        {
            Pointer.GetComponent<BoxCollider>().enabled =true;
            Speed = 0;
            StartGame=false;
            rouletteBtn.gameObject.SetActive(true);
            rouletteBtn.GetComponent<Button>().enabled = true;
        }
    }
    public IEnumerator StopRoulette()
    {
        if(RouletteStop)
        {
            ConfirmResult(ColorSec[collectResultId[spinCount-1]]);
            IsSpin = false;
            Roulette.SetBool("Spin",false);
            rouletteBtn.gameObject.SetActive(true);
            rouletteBtn.GetComponent<Button>().enabled = true;
            StartGame = false;
            RouletteStop = false;
        }
        if(numberOfSpin==spinCount)
        {
            if(CollectBtn)
            {
                CollectBtn.SetActive(true);
                yield return new WaitForSeconds(delayResult);
                StartCoroutine(EndBonus());
            }
            else
            {
                rouletteBtn.gameObject.SetActive(false);
                yield return new WaitForSeconds(delayResult);
                StartCoroutine(EndBonus());
            }
        }
    }
    public void GenerateResult()
    {
        //if (SlotController.Instance.getUseServerResult())
        //{
        //    collectResultId = SlotController.Instance.ServerResult.bonusIds;
        //}else{
        collectResultId = RandomResult();
        //}
    }

    public List<int> RandomResult()
    {
        List<int> temp = new List<int>();
        for(int i = 0; i<numberOfSpin;i++)
        {
            int num = Random.Range(0,ColorSec.Count-1);
            temp.Add(num);
        }
        return temp;
    }

    int SpeedChange()
    {
        int speed = 60 * Random.Range(minRotate,maxRotate); 
        return speed;
    }
    bool CheckDistance()
    {
        bool near = false;
        if(roulettePlate.transform.rotation.eulerAngles.z>=currentResult)
        {
            float tempdata =roulettePlate.transform.rotation.eulerAngles.z - currentResult;
            if(tempdata>= 40 && tempdata<90)
            {
                near = true;
            }
        }
        return near;
    }
    void SmoothRotate()
    {
        float checkAngle = currentResult;
        if(checkAngle<0)
        {
            checkAngle = 360 - checkAngle;
        }
        roulettePlate.transform.rotation = Quaternion.RotateTowards(roulettePlate.transform.rotation, Quaternion.Euler(roulettePlate.transform.rotation.x,roulettePlate.transform.rotation.y,currentResult), lerpspeed * Time.deltaTime);
        if(roulettePlate.transform.localEulerAngles.z <= checkAngle + 0.1f)
        {
            RouletteStop = true;
            StartCoroutine(StopRoulette());
        }
    }
    public void ResetRoulette()
    {
        StartGame = false;
        IsSpin = false;
        ReadyStop = false;
        Pointer.GetComponent<BoxCollider>().enabled =false;
        rouletteBtn.gameObject.SetActive(true);
        rouletteBtn.GetComponent<Button>().enabled = true;
        foreach(ColorSector sec in ColorSec)
        {
            // sec.displayReward.text = sec.Reward;
            if(sec.GlowSector)
            {
                sec.GlowSector.gameObject.SetActive(false);
            }
        }
    }
    void ConfirmResult(ColorSector result)
    {
        if(result.GlowSector)
        {
            result.GlowSector.gameObject.SetActive(true);
        }
        // string newResult = string.Join("",result.Reward.Split('@', ',' ,'.' ,';', '\''));
        // GetReward = int.Parse(newResult);
        totalFreespin += result.NumberOfFreeSpin;
        //totalWinCoin += result.NumberOfMultiply * SlotPlayer.Instance.TotalBet;
        totalFeatureMultiply = totalFeatureMultiply * result.NumberOfFeatureMultiply;
        totalWinCoin += double.Parse(result.AmountReward);
        Debug.Log(totalWinCoin + "" );
        UpdateRewardText(result);
        UpdateUiText();
        
    }
    void UpdateRewardText(ColorSector result)
    {
        if(result.BetMultiply>0)
        {
            RewardText.text = result.AmountReward;
        }
        else if(result.NumberOfFreeSpin>0)
        {
            RewardText.text = result.NumberOfFreeSpin + " Free Games";
        } 
        else if(result.NumberOfMultiply>0)
        {
            //RewardText.text = result.NumberOfFreeSpin + " Free Games";
        }
        else if(result.NumberOfFeatureMultiply>0)
        {
            //RewardText.text = result.NumberOfFreeSpin + " Free Games";
        }
        else
        {
            RewardText.text = " ";
        }
    }
    void UpdateUiText()
    {
        UpdateTexts(numPickText, (numberOfSpin - spinCount).ToString());
        UpdateTexts(currentBalanceText, SlotPlayer.Instance.Coins.ToString(SlotController.Instance.decimalDisplay));
        UpdateTexts(currentTotalBet, SlotPlayer.Instance.TotalBet.ToString(SlotController.Instance.decimalDisplay));
        
        UpdateTexts(totalFreespinText, totalFreespin.ToString());
        UpdateTexts(totalFeatureMultiplyText, totalFeatureMultiply.ToString());
    }

    void UpdateTexts(Text[] textArray,string content)
    {
        foreach(Text t in textArray)
        {
            if (t) t.text = content;
        }
    }

    public void EndBonusTrigger()
    {
        StartCoroutine(EndBonus());
    }

    public IEnumerator EndBonus()
    {
        if(SoundGO) SoundGO.SetActive(true);
        
        if (winAnim == null)
        {
            winAnim = FindObjectOfType<WinAnimation>();
            
        }
        if (winAnim != null)
        {
            yield return StartCoroutine(winAnim.RunWinAnimation((float)totalWinCoin));
            if (winAnim.HasWin())
            {
                //EndDisplay();
                if (displayPanel) 
                {
                    displayPanel.SetActive(true);
                    OnClickEndBonus();
                }
                    
                yield break;
            }
            else
            {
                displayPanel?.SetActive(true);
                TweenCoin(totalWinText, 0, (float)totalWinCoin);
            }
        }
        // displayPanel?.SetActive(true);
        // TweenCoin(totalWinText, 0, (float)totalWinCoin); 
        SaveResult();
    }
    void TweenCoin(Text[] text, float startValue,float endValue)
    {
        foreach(Text t in text)
        {
            TweenNumber.ShowAnimation(t, EaseAnim.EaseLinear, startValue, endValue, 2, SlotController.Instance.decimalDisplay);
        }
       
    }
    void SaveResult()
    {
        SlotPlayer.Instance.toggleIsBonus();
        SlotPlayer.Instance.AddFreeSpins(totalFreespin);
        if(!SlotController.Instance.onlyAddCoinOnEndFeature)SlotPlayer.Instance.AddCoins(totalWinCoin);
        SlotController.Instance.totalFeatureWin += totalWinCoin;

        SlotController.Instance.featureMultiply = totalFeatureMultiply;
        SlotPlayer.Instance.toggleIsBonus();
       
        SlotPlayer.Instance.saveCoinsUpdate(totalWinCoin, totalFreespin);
    }
    public override void OnTrigger()
    {
        base.OnTrigger();
        StartTrigger();
    }

    public void OnClickEndBonus()
    {
        hasEndedBonus = true;
        foreach (ColorSector pick in ColorSec)
        {
            if(pick.GlowSector)
            {
                pick.GlowSector.gameObject.SetActive(false);
            }
        }
        displayPanel?.SetActive(false);
    }
}
//>>>>>>>>>>>>>>>> end (23/2/2021)