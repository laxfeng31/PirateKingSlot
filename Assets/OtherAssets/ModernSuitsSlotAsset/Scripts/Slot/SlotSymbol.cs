using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

namespace Mkey
{
    public class SlotSymbol : MonoBehaviour
    {
        public int iconID;
        [SerializeField]
        private int position;
        private GameObject particles;
        [SerializeField]
        private Sprite winLightSprite;
        private List<GameObject> winLightGO = new List<GameObject>();
        private SpriteRenderer sR;
        private float timeDisplayAnim = 2f;
        private SlotIcon.LineWinnigBox lineBoxSetting;
        private LineRenderer boxLine;
        
        private Animator anim;
        
        private GameObject effectWholeColumnGamePrefab;
        private GameObject effectWholeColumnObj;
        private string soundName;
        private Vector3 tileSize;
        public float tileX;
        public float tileY;
        private List<AnimatorSetGroup> animationWinGroupGO = new List<AnimatorSetGroup>();
        private Transform trans;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        private RuntimeAnimatorController animC;
        private RuntimeAnimatorController animCFeature;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        //>>>>>>>>>>>>>>>> (13/4/2021)
        private bool displayWinAnimWhenAppear = false;
        private string displayWinAnimWhenAppearSound = "";
        //>>>>>>>>>>>>>>>> end (13/4/2021)

        public int Position
        {
            get { return position; }
            set { position = value; }
        }
        private bool soundRunned;

        private void Start()
        {
            anim = GetComponent<Animator>();
            trans = transform;
            tileX = SlotController.Instance.tileX;
            tileY = SlotController.Instance.tileY;
            //Define tile size
            if (PlayerPrefs.GetString("current_Scene") == "GreatBlue")
            {
                tileSize = new Vector3(1.7f, 2.3f, 1f);
            }
            else if (PlayerPrefs.GetString("current_Scene") == "FruityTutti")
            {
                tileSize = new Vector3(1.75f, 2.25f, 1);
            }
            else if (PlayerPrefs.GetString("current_Scene") == "Sparta" || PlayerPrefs.GetString("current_Scene") == "CherryLove" || PlayerPrefs.GetString("current_Scene") == "IrishLuck")
            {
                tileSize = new Vector3(1.75f, 2.43f, 1);
            }
            else if (PlayerPrefs.GetString("current_Scene") == "FarmDays")
            {
                tileSize = new Vector3(1.39f, 1.83f, 1);
            }
            else if (PlayerPrefs.GetString("current_Scene") == "GreatChina")
            {
                tileSize = new Vector3(1.85f, 2.12f, 1);
            }
            else if (PlayerPrefs.GetString("current_Scene") == "HighwayKing")
            {
                tileSize = new Vector3(2.7f, 3.35f, 1);
            }
            else
            {
                tileSize = new Vector3(tileX, tileY, 1f);
            }
        }

        #region regular
        private void OnDestroy()
        {
            WinCancel();
            
            
        }

        private void OnDisable()
        {
            WinCancel();
            
            
        }

        #endregion regular
        SlotGroupBehavior slotGB;
        bool hasWinColumnEffect;
        //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
        GameObject groupGO;

        //>>>>>>>>>>>>>>>> (13/4/2021)
        internal void SetIcon(SlotIcon icon, int iconID, GameObject groupGO, bool displayWinOnAppear = false)
        {
        //>>>>>>>>>>>>>>>> end (14/4/2021)   
            if (this.groupGO!= null)
            {
                Destroy(this.groupGO);
            }
            this.groupGO = groupGO;
            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            hasWinColumnEffect = false;
            
            if (anim == null)
            {
                anim = GetComponent<Animator>();
            }
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            if (icon.animC != null)
            {
                anim.runtimeAnimatorController = icon.animC;
                animC = icon.animC;
            }
            else
            {
                animC = null;
            }
            //>>>>>>>>>>>>>>>> (13/4/2021)
            displayWinAnimWhenAppear = displayWinOnAppear;
            displayWinAnimWhenAppearSound = icon.displayWinAnimWhenAppearSound;
            //>>>>>>>>>>>>>>>> end (13/4/2021)
            if (icon.animCFeature != null)
            {
                if(SlotController.Instance.PlayFreeSpins) anim.runtimeAnimatorController = icon.animCFeature;
                animCFeature = icon.animCFeature;
            }
            else
            {
                animCFeature = null;
            }
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            if (icon.winningBox != null)
            {
                lineBoxSetting = icon.winningBox;
            }
            else
            {
                lineBoxSetting = null;
            }


            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            timeDisplayAnim = Mathf.Max(2f, icon.timeDisplayAnim);
            

            if (icon.slotGB != null)
            {
                slotGB = icon.slotGB;
            }

            if (!string.IsNullOrEmpty(icon.soundName))
            {
                soundName = icon.soundName;
            }
            else
            {
                soundName = "";
            }


            hasWinColumnEffect = icon.hasColumnWinEffect;

            if (sR == null)
            {
                sR = GetComponent<SpriteRenderer>();
            }

            if(animationWinGroupGO.Count != icon.animationWinGroup.Count)
            {
                if(animationWinGroupGO.Count < icon.animationWinGroup.Count)
                {
                    List<AnimatorSetGroup> tempList = new List<AnimatorSetGroup>();
                    for (int i = animationWinGroupGO.Count; i < icon.animationWinGroup.Count; i++)
                    {
                        if(trans == null)
                        {
                            trans = transform;
                        }
                        GameObject gObj = Creator.CreateSpriteAtPosition(trans, null, trans.position, sR.sortingLayerID, sR.sortingOrder).gameObject;
                        Animator animS = gObj.AddComponent<Animator>();

                        tempList.Add(new AnimatorSetGroup(gObj, animS));
                    }

                    foreach (AnimatorSetGroup a in tempList)
                    {
                        animationWinGroupGO.Add(a);
                    }
                }

            }

            for (int i = 0; i < animationWinGroupGO.Count; i++)
            {
                if(i < icon.animationWinGroup.Count)
                {
                    animationWinGroupGO[i].GameObj.SetActive(true);
                    animationWinGroupGO[i].AnimG.runtimeAnimatorController = icon.animationWinGroup[i].AnimC;
                }
                else
                {
                    animationWinGroupGO[i].GameObj.SetActive(false);
                }
            }


            this.iconID = iconID;
            
            sR.sprite = icon.iconSprite;
            //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
            if (icon.iconSpriteFeature != null && SlotController.Instance.PlayFreeSpins)
            {
                sR.sprite = icon.iconSpriteFeature;
            }
            //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            winLightSprite = icon.addIconSprite;

        }
        //>>>>>>>>>>>>>>>> (13/4/2021)
        public void ShowWinWhileSpin()
        {
            if (displayWinAnimWhenAppear)
            {
                IsWin(false,string.IsNullOrEmpty(displayWinAnimWhenAppearSound)? "" : displayWinAnimWhenAppearSound);
            }
        }
        public void StopShowWinWhileSpin()
        {
            if (displayWinAnimWhenAppear)
            {
                DisableIsWin();
            }
        }
        //>>>>>>>>>>>>>>>> end (13/4/2021)
        internal void ShowParticles(bool activity, GameObject particlesPrefab)
        {
            if (activity)
            {
                if (particlesPrefab)
                {
                    if (particles == null)
                    {
                        particles = (GameObject)Instantiate(particlesPrefab, transform.position, transform.rotation);
                        particles.transform.parent = transform.parent;
                        particles.transform.localScale = transform.localScale;
                    }
                }
            }
            else
            {
                if (particles) { Destroy(particles); }
            }
        }
        //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
        public void SetAnim(string nameKey, bool condition, bool isLineWiningBox)
        {
            //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
            if (anim)
            {
                //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
                if(animC != null)
                {
                    anim.runtimeAnimatorController = animC;
                }
                if(animCFeature != null && SlotController.Instance.PlayFreeSpins)
                {
                    anim.runtimeAnimatorController = animCFeature;
                }
                
                if (anim.runtimeAnimatorController != null)
                    anim.SetBool(nameKey, condition);
                //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
            }
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            if (!isLineWiningBox)
            {
                foreach (AnimatorSetGroup a in animationWinGroupGO)
                {
                    if (a.GameObj.activeInHierarchy)
                    {

                        a.AnimG.SetBool(nameKey, condition);

                    }
                }
                if(lineBoxSetting != null)
                {
                    if (lineBoxSetting.enableWinnigLineBox)
                    {
                        if (condition && nameKey == "IsWin")
                        {
                            if (boxLine == null)
                            {
                                GameObject lineObj = new GameObject("WinningBoxLine");
                                boxLine = lineObj.AddComponent<LineRenderer>();
                                SpriteLine spLine = lineObj.AddComponent<SpriteLine>();
                                spLine.textureMode = LineTextureMode.Tile;

                                lineObj.transform.SetParent(trans);
                                lineObj.transform.localPosition = new Vector3(0, 0, 0);

                            }
                            boxLine.material = lineBoxSetting.lineMaterial;
                            boxLine.colorGradient = GetGradient( lineBoxSetting.lineColor,1);
                            boxLine.sortingLayerID = lineBoxSetting.sortingLayerID;
                            boxLine.sortingOrder = lineBoxSetting.sortingOrder;
                            boxLine.widthMultiplier = lineBoxSetting.lineRendererWidth;
                            boxLine.loop = true;
                            CreateWinningBox(boxLine, lineBoxSetting, boxLine.transform.position);
                            boxLine.enabled = condition;
                            boxWinnigLineOn = condition;
                            if (condition) StartCoroutine(StartWinningBoxFlashing(lineBoxSetting));
                        }
                        else
                        {
                            boxWinnigLineOn = false;
                            if (boxLine != null) boxLine.enabled = false;
                        }
                        
                        
                    }
                }
            }

            if (groupGO)
            {
                Animator groupAnim = groupGO.GetComponent<Animator>();
                if (groupAnim)
                {
                    groupAnim.SetBool(nameKey, condition);
                }
            }

            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
        }
        //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end

        //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
        bool boxWinnigLineOn = false;
        IEnumerator StartWinningBoxFlashing(SlotIcon.LineWinnigBox lineSetting)
        {
            if(boxLine != null)
            {
                bool isOn = false;
                while (boxWinnigLineOn)
                {
                    boxLine.enabled = !isOn;
                    yield return new WaitForSeconds(lineBoxSetting.lineFlashingSpeed);
                }
            }
            
        }
        Gradient GetGradient(Color setLineColor, float alpha)
        {
            Gradient g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] { new GradientColorKey(setLineColor, 0.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f) }
                );

            return g;
        }
        private void CreateWinningBox(LineRenderer lr, SlotIcon.LineWinnigBox lineBoxSetting, Vector3 pos)
        {

            lr.transform.position = pos;

            lr.positionCount = 5;
            lr.SetPositions(
                new[]
                {
                        new Vector3(pos.x , pos.y + lineBoxSetting.lineSize.y / 2, pos.z),
                        new Vector3(pos.x + lineBoxSetting.lineSize.x/2, pos.y + lineBoxSetting.lineSize.y / 2, pos.z),
                        new Vector3(pos.x + lineBoxSetting.lineSize.x/2, pos.y - lineBoxSetting.lineSize.y / 2, pos.z),
                        new Vector3(pos.x - lineBoxSetting.lineSize.x/2, pos.y - lineBoxSetting.lineSize.y / 2, pos.z),
                        new Vector3(pos.x - lineBoxSetting.lineSize.x/2, pos.y + lineBoxSetting.lineSize.y / 2, pos.z),
                }
                );
        }
        //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021) 

        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)

        //>>>>>>>>>>>>>>>> (13/4/2021)
        public float IsWin(bool isLineWiningBox, string winSound ="" )
        {
        //>>>>>>>>>>>>>>>> end (13/4/2021)
            //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)

            soundRunned = SlotController.Instance.soundRunned;
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
            
            //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
            SetAnim("IsWin", true, isLineWiningBox);
            //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)

            //if (anim)
            //{
            //    if(anim.runtimeAnimatorController != null)
            //        anim.SetBool("IsWin", true);

            //}

            //foreach(AnimatorSetGroup a in animationWinGroupGO)
            //{
            //    if (a.GameObj.activeInHierarchy)
            //    {
            //        a.AnimG.SetBool("IsWin", true);
            //    }
            //}
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
            //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
            if (winLightSprite!= null && !isLineWiningBox)
            {
                winLightGO.Add(Creator.CreateSpriteAtPosition(trans, winLightSprite, trans.position, sR.sortingLayerID, sR.sortingOrder+1).gameObject);
            }
            //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)

            if (hasWinColumnEffect)
            {
                if(slotGB != null)
                {
                    
                    if(slotGB.ColumnWinningEffect != null)
                    {
                        
                        slotGB.ColumnWinningEffect.SetActive(true);
                    }
                        
                }
               
            }
            if (!string.IsNullOrEmpty(soundName) || !string.IsNullOrEmpty(winSound))
            {

                if (!SlotController.Instance.IsOnBonus && !soundRunned)
                {
                    
                    MasterAudio.PlaySound(string.IsNullOrEmpty(winSound) ? soundName : winSound);
                }

            }

            return timeDisplayAnim;
        }

        public void DisableIsWin()
        {
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
            //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
            SetAnim("IsWin",false, false);
            //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
            //if (anim)
            //{
            //    if (anim.runtimeAnimatorController != null)
            //    {
            //        anim.SetBool("IsWin", false);
            //    }
            //}

            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
            if (winLightGO.Count>0)
            {
                foreach(GameObject g in winLightGO)
                    Destroy(g);
            }
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
            //foreach (AnimatorSetGroup a in animationWinGroupGO)
            //{
            //    if (a.GameObj.activeInHierarchy)
            //    {
            //        a.AnimG.SetBool("IsWin", false);
            //    }
            //}
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
            if (!string.IsNullOrEmpty(soundName))
            {
                MasterAudio.StopAllOfSound(soundName);
            }
        }

        GameObject tweenClone;
        
        
        
        TweenSeq blinkTS;
        //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
        internal void WinTrigger(Action completeCallBack, int count, bool isLineWiningBox = true)
        {
        //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
            blinkTS = new TweenSeq();
            localScale = transform.localScale;
            localScale = tileSize;
            float time = 0;
            string cur_Scene = PlayerPrefs.GetString("current_Scene");
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
            //String kName = transform.gameObject.GetComponent<SpriteRenderer>().sprite.name;
            //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
            
            //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
            time = IsWin(isLineWiningBox);
            
            
            for (int i = 0; i < count; i++)
            {
                
                blinkTS.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0f, 1f, time).SetOnUpdate((float val) =>
                    {
                        
                    }).AddCompleteCallBack(() => { callBack(); SetAnim("IsWin", false, false); });
                });

                
            }

            blinkTS.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, 0f, 0f, 0.5f).SetOnUpdate((float val) =>
                {

                    
                }).AddCompleteCallBack(() => { if (completeCallBack != null) completeCallBack(); callBack(); });
            });

            //3 
            //blinkTS.Add((callBack) =>
            //{
            //    if (this) transform.localScale = tileSize;
            //    if (completeCallBack != null) completeCallBack();
            //    callBack();

            //});
            //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
            blinkTS.Start();
        }

        
        private Vector3 localScale;
        SpriteRenderer srL;

        

        internal void WinCancel()
        {
            //  Debug.Log("Blink cancel : " + iconID);
            if (blinkTS != null) blinkTS.Break();
            if (gameObject) SimpleTween.Cancel(gameObject, false, 1f);
            transform.localScale = tileSize;

            
            DisableIsWin();
        }

        

    }

    [Serializable]
    public class AnimatorSetGroup
    {
        [SerializeField]
        GameObject gameObj;
        [SerializeField]
        Animator animG;

        public GameObject GameObj { get => gameObj; private set => gameObj = value; }
        public Animator AnimG { get => animG; private set => animG = value; }

        public AnimatorSetGroup(GameObject gameObj, Animator animG)
        {
            this.gameObj = gameObj;
            this.animG = animG;
        }
    }
}

