using UnityEngine;
using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;

namespace Mkey
{
    public class SlotGroupBehavior : MonoBehaviour
    {
        public List<int> symbOrder;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public List<int> symbOrderFeature;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        [SerializeField]
        [Tooltip("Symbol windows, from top to bottom")]
        private RayCaster[] rayCasters;
        //>>>>>>>>>>>>>>>> (2/4/2021)
        public RayCaster[] RayCasters { get { return rayCasters; } set { rayCasters = value; } }
        //>>>>>>>>>>>>>>>> end (2/4/2021)
        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("sec, additional rotation")]
        //>>>>>>>>>>>>>>>> (16/4/2021)
        public int addRotate = 0;
        //>>>>>>>>>>>>>>>> end (16/4/2021)

        [SerializeField]
        private int spinSpeedMultiplier = 1;

        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("If true - reel set to random position at start")]
        //>>>>>>>>>>>>>>>> (13/4/2021)
        public bool randomStartPosition = false;
        //>>>>>>>>>>>>>>>> end (13/4/2021)
        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("Tile size by Y")]
        private float tileSizeY = 3.13f;
        [SerializeField]
        [Tooltip("Additional space between tiles")]
        private float gapY = 0.35f; // additional space 
        [SerializeField]
        [Tooltip("Link to base (bottom raycaster)")]
        private bool baseLink = false;
        [SerializeField]
        GameObject spinningEffectObj;
        private int tileCount;
        private SlotSymbol[] slotSymbols;
        private SlotIcon[] sprites;

        private int nextReelSymbolToChange = 0;
        private int nextReelSymbolToSet = 0;
        private int currOrderPosition = 0;
        private float anglePerTileRad = 0;
        private float anglePerTileDeg = 0;
        private TweenSeq tS;
        private Transform TilesGroup;

        private Vector3 tileSize;

        [SerializeField]
        private float spinningEffectTime;
        public float tileX;
        public float tileY;
        private GameObject columnWinningEffect;

        #region simulate
        [SerializeField]
        private bool simulate = false;
        [SerializeField]
        public int simPos = 0;
        #endregion simulate

        [Tooltip("ReelSymbols source")]
        public SlotGroupBehavior CopyFrom;
        
        public int NextOrderPosition
        {
            get; set;
        }

        #region regular
        private void OnValidate()
        {
            
            spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
            addRotate = Mathf.Max(0, addRotate);
        }

        private void OnDestroy()
        {
            CancelRotation();
        }

        private void OnDisable()
        {
            CancelRotation();
        }
        #endregion regular

        public float[] SymbProbabilities
        {
            get; private set;
        }
        public GameObject ColumnWinningEffect { get => columnWinningEffect; set => columnWinningEffect = value; }

        /// <summary>
        /// Instantiate slot tiles 
        /// </summary>
        //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
        internal void CreateSlotCylinder(SlotIcon[] sprites, int tileCount, GameObject tilePrefab, float slotRadius)
        //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)
        {
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
                tileSize = new Vector3(SlotController.Instance.tileX, SlotController.Instance.tileY, 1);
            }


            this.sprites = sprites;
            this.tileCount = tileCount;
            slotSymbols = new SlotSymbol[tileCount];

            // create Reel transform
            TilesGroup = (new GameObject()).transform;
            TilesGroup.localScale = transform.lossyScale;
            TilesGroup.parent = transform;
            TilesGroup.localPosition = Vector3.zero;
            TilesGroup.name = "Reel(" + name + ")";

            // calculate reel geometry
            float distTileY = tileSizeY + gapY; //old float distTileY = 3.48f;

            anglePerTileDeg = 360.0f / (float)tileCount;
            anglePerTileRad = anglePerTileDeg * Mathf.Deg2Rad;
            float radius = (distTileY / 2f) / Mathf.Tan(anglePerTileRad / 2.0f); //old float radius = ((tileCount + 1) * distTileY) / (2.0f * Mathf.PI);

            bool isEvenRayCastersCount = (rayCasters.Length % 2 == 0);
            int dCount = (isEvenRayCastersCount) ? rayCasters.Length / 2 - 1 : rayCasters.Length / 2;
            float addAnglePerTileDeg = (isEvenRayCastersCount) ? -anglePerTileDeg * dCount - anglePerTileDeg / 2f : -anglePerTileDeg;
            float addAnglePerTileRad = (isEvenRayCastersCount) ? -anglePerTileRad * dCount - anglePerTileRad / 2f : -anglePerTileRad;
            
            //>>>>>>>>>>>>>>>> (lax) (19/5/2021)
            TilesGroup.localPosition = new Vector3(TilesGroup.localPosition.x, TilesGroup.localPosition.y, slotRadius); // offset reel position by z-coordinat
            //>>>>>>>>>>>>>>>> (lax) end (19/5/2021)

            // orient to base rc
            RayCaster baseRC = rayCasters[rayCasters.Length - 1]; // bottom raycaster
            float brcY = baseRC.transform.localPosition.y;
            float dArad = 0f;
            if (brcY > -radius && brcY < radius && baseLink)
            {
                float dY = brcY - TilesGroup.localPosition.y;
                dArad = Mathf.Asin(dY / radius);
                //    Debug.Log("dY: "+ dY + " ;dArad: " + dArad  + " ;deg: " + dArad* Mathf.Rad2Deg);
                addAnglePerTileRad = dArad;
                addAnglePerTileDeg = dArad * Mathf.Rad2Deg;
            }
            else if (baseLink)
            {
                Debug.Log("Base Rc position out of reel radius");
            }
            
            //create reel tiles
            for (int i = 0; i < tileCount; i++)
            {
                float n = (float)i;
                float tileAngleRad = n * anglePerTileRad + addAnglePerTileRad; // '- anglePerTileRad' -  symborder corresponds to visible symbols on reel before first spin 
                float tileAngleDeg = n * anglePerTileDeg + addAnglePerTileDeg;

                slotSymbols[i] = Instantiate(tilePrefab, transform.position, Quaternion.identity).GetComponent<SlotSymbol>();
                slotSymbols[i].transform.parent = TilesGroup;
                slotSymbols[i].transform.localPosition = new Vector3(0, radius * Mathf.Sin(tileAngleRad), -radius * Mathf.Cos(tileAngleRad));
                slotSymbols[i].transform.localScale = tileSize;
                slotSymbols[i].transform.localEulerAngles = new Vector3(tileAngleDeg, 0, 0);
                slotSymbols[i].name = "SlotSymbol: " + String.Format("{0:00}", i);
            }

            if (randomStartPosition)
            {
                next = UnityEngine.Random.Range(0, symbOrder.Count);
            }
            //set symbols
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            
            SlotIcon.GroupIcon tempGroupIcon = null;

            int tempGroupIconCounter = 0;
            GameObject groupGO = null;
            
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                int symNumber = symbOrder[GetNextSymb()];
                
                if (sprites[symNumber].groupIconSetting.useGroupIcon)
                {
                    tempGroupIcon = sprites[symNumber].groupIconSetting;
                    if(tempGroupIconCounter == 1)
                    {
                        groupGO = Instantiate(tempGroupIcon.groupIconPrefab, slotSymbols[i].transform);
                        
                        groupGO.transform.localPosition = new Vector3(0, 0, 0);

                    }
                    if(tempGroupIconCounter > tempGroupIcon.groupCount)
                    {
                        tempGroupIconCounter = 0;
                        tempGroupIcon = null;
                        groupGO = null;
                    }
                    else
                    {
                        tempGroupIconCounter++;
                    }
                    
                }

                slotSymbols[i].SetIcon(sprites[symNumber], symNumber, groupGO);
                slotSymbols[i].Position = i;

            }
            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            nextReelSymbolToSet = slotSymbols.Length;

            SymbProbabilities = GetReelSymbHitPropabilities(sprites);
            currOrderPosition = 0; // offset  '- anglePerTileRad' - 

            // set random start position
            //if (randomStartPosition)
            //{
            //    NextOrderPosition = UnityEngine.Random.Range(0, symbOrder.Count);
            //    float angleX = GetAngleToNextSymb(NextOrderPosition);
            //    TilesGroup.Rotate(-angleX, 0, 0);
            //    currOrderPosition = NextOrderPosition;
            //    ChangeIcon();
            //}

        }

        /// <summary>
        /// Async rotate cylinder
        /// </summary>
        internal void NextRotateCylinderEase(EaseAnim mainRotType, EaseAnim inRotType, EaseAnim outRotType,
                                        float mainRotTime, 
                                        float inRotTime, float outRotTime,
                                        float inRotAngle, float outRotAngle,
                                        int nextOrderPosition, Action rotCallBack)

        {
            NextOrderPosition = (!simulate) ? nextOrderPosition : simPos;

            // check range before start
            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);

            outRotTime = Mathf.Clamp(outRotTime, 0, 1f);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);

            // create reel rotation sequence - 4 parts  in - (continuous) - main - out
            float oldVal = 0f;
            tS = new TweenSeq();
            float angleX = 0;

            tS.Add((callBack) => // in rotation part
            {
                SimpleTween.Value(gameObject, 0f, inRotAngle, inRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (callBack != null) callBack();
                                  }).SetEase(inRotType);
            });

            if (NextOrderPosition == -1)
                tS.Add((callBack) => // continuous rotation
                {
                    RecurRotation(mainRotTime / 1.0f, callBack);
                });

            tS.Add((callBack) =>  // main rotation part
            {
                oldVal = 0f;

                //spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
                angleX = GetAngleToNextSymb(NextOrderPosition) + anglePerTileDeg * symbOrder.Count * spinSpeedMultiplier;

                SimpleTween.Value(gameObject, 0, -(angleX + outRotAngle + inRotAngle), mainRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                      ChangeIcon();
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (callBack != null) callBack();
                                  }).SetEase(mainRotType);
            });

            tS.Add((callBack) =>  // out rotation part
            {
                oldVal = 0f;
                SimpleTween.Value(gameObject, 0, outRotAngle, outRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      currOrderPosition = NextOrderPosition;
                                      if (rotCallBack != null) rotCallBack();
                                      if (callBack != null) callBack();
                                      MasterAudio.PlaySound("SpinStop");
                                  }).SetEase(outRotType);
            });

            tS.Start();
        }

        GameObject InstantiateStickyIcon(SlotController slotC, SlotIcon result, int symbolIds, SlotSymbol slotSymbols)
        {
            GameObject stickyObj = Instantiate(slotC.tilePrefab, transform);
            //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
            stickyObj.GetComponent<SlotSymbol>().SetIcon(result, symbolIds, null);
            //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)
            SpriteRenderer[] sRChild = stickyObj.GetComponents<SpriteRenderer>();
            foreach (SpriteRenderer s in sRChild)
            {
                s.sortingOrder = s.sortingOrder + 200;
                s.enabled = false;
            }

            stickyObj.transform.localScale = slotSymbols.gameObject.transform.localScale;
            stickyObj.transform.position = slotSymbols.gameObject.transform.position;


            return stickyObj;
        }
        //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
        void UpdateIconAnimation(string nameKey, bool condition)
        {
            for(int i = 0; i< slotSymbols.Length; i++)
            {
                //(Updated)>>>>>>>>>>>>>>>> (13/1/2021)
                slotSymbols[i].SetAnim(nameKey, condition, false);
                //(Updated)>>>>>>>>>>>>>>>> end (13/1/2021)
            }
        }
        //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
        int initAngle = 45;
        public bool alwaysTriggerWholeCol = false;
        internal void StartSpin(EaseAnim mainRotType, EaseAnim inRotType, float inRotTime,float inRotAngle,  SlotController slotC, Action rotCallBack)
        {
            this.slotC = slotC;
            alwaysTriggerWholeCol = false;
            if (columnWinningEffect != null)
            {

                ColumnWinningEffect.SetActive(false);
            }
            if(spinningEffectObj) spinningEffectObj.SetActive(false);
            //check the result for effector whole column

            // check range before start
            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);


            // create reel rotation sequence - 4 parts  in - (continuous) - main - out
            float oldVal = 0f;
            tS = new TweenSeq();

            //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
            //if (symbOrderFeature != null)
            //{
            //    if (symbOrderFeature.Count > 0)
            //    {
            //        ChangeIcon(SlotController.Instance.PlayFreeSpins ? symbOrderFeature : symbOrder);
            //    }
            //    else
            //    {
            //        ChangeIcon(symbOrder);
            //    }

            //}
            //else
            //{
            //    ChangeIcon(symbOrder);
            //}

            //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)

            tS.Add((callBack) => // in rotation part
            {
                //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
                UpdateIconAnimation("IsSpin", true);
                //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
                SimpleTween.Value(gameObject, 0f, inRotAngle, inRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (rotCallBack != null) rotCallBack();
                                      if (callBack != null) callBack();
                                      
                                  }).SetEase(inRotType);
            });

            

            tS.Start();
        }

        internal void ContSpin(EaseAnim mainRotType,float mainRotTime, float inRotAngle, float outRotAngle, Action rotCallBack)
        {

            float angleX = 0f;
            float oldVal = 0f;
            tS = new TweenSeq();

            
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);



            spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
            angleX = 360 * (int)spinSpeedMultiplier;
            angleX = angleX + (360 ) + outRotAngle + inRotAngle;
            mainRotTime = mainRotTime * (360 / angleX);

            tS.Add((callBack) =>  // main rotation part
            {
                oldVal = 0f;



                SimpleTween.Value(gameObject, 0, -(360), mainRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      
                                      //ChangeRandonIcon();

                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;

                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (rotCallBack != null) rotCallBack();
                                      if (callBack != null) callBack();
                                      
                                  }).SetEase(mainRotType);
            });

            tS.Start();
        }

        SlotController slotC;
        List<GameObject> stickyIconStorage = new List<GameObject>();
        List<int> stickySlotIconIdStorage = new List<int>();
        internal void NextRotateCylinderEase(EaseAnim mainRotType, EaseAnim outRotType,
                                        float mainRotTime, 
                                        float inRotTime, float outRotTime,
                                        float inRotAngle, float outRotAngle,
                                        StoreSpinResult result, Action rotCallBack)

        {

            bool hasChangeIcon = false;
            int effectWholeColumnSlotId = -1;

            bool hasEffectorColumn = false;
            string tempSoundName = "";
            for (int i = 0; i < result.SymbolIds.Count; i++)
            {

                if (result.SIcons[i].effectWholeColumn)
                {

                    effectWholeColumnSlotId = result.SymbolIds[i];

                    if (ColumnWinningEffect == null)
                    {

                        ColumnWinningEffect = Instantiate(result.SIcons[i].effectWholeColumnGamePrefab, transform);
                        ColumnWinningEffect.SetActive(false);
                    }

                    alwaysTriggerWholeCol = true;
                    tempSoundName = result.SIcons[i].soundName;
                    //>>>>>>>>>>>>>>>> (13/4/2021)
                    SlotIcon temp = new SlotIcon(result.SIcons[i].iconSprite, result.SIcons[i].iconSpriteFeature, result.SIcons[i].addIconSprite, null, null, tempSoundName, result.SIcons[i].winningBox, result.SIcons[i].displayWinAnimWhenAppear, result.SIcons[i].displayWinAnimWhenAppearSound);
                    //>>>>>>>>>>>>>>>> end (13/4/2021)
                    result.SIcons[i] = temp;
                    result.SIcons[i].hasColumnWinEffect = true;
                    result.SIcons[i].slotGB = this;
                    hasEffectorColumn = true;

                }
            }

            if (hasEffectorColumn)
            {

                for (int i = 0; i < result.SymbolIds.Count; i++)
                {
                    if (!result.SIcons[i].effectWholeColumn)
                    {
                        //>>>>>>>>>>>>>>>> (13/4/2021)
                        SlotIcon temp = new SlotIcon(result.SIcons[i].iconSprite, result.SIcons[i].iconSpriteFeature, result.SIcons[i].addIconSprite, null, null, tempSoundName, result.SIcons[i].winningBox, result.SIcons[i].displayWinAnimWhenAppear, result.SIcons[i].displayWinAnimWhenAppearSound);
                        //>>>>>>>>>>>>>>>> end (13/4/2021)
                        result.SIcons[i] = temp;
                        result.SymbolIds[i] = effectWholeColumnSlotId;
                        result.SIcons[i].hasColumnWinEffect = true;
                        result.SIcons[i].slotGB = this;

                    }

                }
            }


            setWinResultCounter = 0;


            if (slotC.PlayFreeSpins)
            {


                for (int i = 0; i < result.SymbolIds.Count; i++)
                {
                    if (result.SIcons[i].enableStickyIcon)
                    {



                        if (stickyIconStorage.Count < i + 1)
                        {
                            stickyIconStorage.Add(InstantiateStickyIcon(slotC, result.SIcons[i], result.SymbolIds[i], slotSymbols[i]));

                            stickySlotIconIdStorage.Add(result.SymbolIds[i]);


                        }
                        else
                        {
                            if (stickyIconStorage[i] == null)
                            {
                                stickyIconStorage[i] = InstantiateStickyIcon(slotC, result.SIcons[i], result.SymbolIds[i], slotSymbols[i]);

                                stickySlotIconIdStorage[i] = result.SymbolIds[i];


                            }
                        }
                    }
                    else
                    {
                        if (stickyIconStorage.Count < i + 1)
                        {
                            stickyIconStorage.Add(null);
                            stickySlotIconIdStorage.Add(-1);
                        }
                        else if (stickyIconStorage[i] != null)
                        {
                            //>>>>>>>>>>>>>>>> (13/4/2021)
                            SlotIcon temp = new SlotIcon(result.SIcons[i].iconSprite, result.SIcons[i].iconSpriteFeature, result.SIcons[i].addIconSprite, null, null,tempSoundName, result.SIcons[i].winningBox, result.SIcons[i].displayWinAnimWhenAppear, result.SIcons[i].displayWinAnimWhenAppearSound);
                            //>>>>>>>>>>>>>>>> end (13/4/2021)
                            result.SymbolIds[i] = stickySlotIconIdStorage[i];
                            result.SIcons[i] = temp;
                            result.SIcons[i].slotGB = this;
                        }

                    }
                }



            }
            else
            {
                if (stickyIconStorage.Count > 0)
                {
                    foreach (GameObject gObj in stickyIconStorage)
                    {
                        Destroy(gObj);
                    }

                    stickyIconStorage.Clear();
                    stickySlotIconIdStorage.Clear();
                }
            }

            // check range before start
            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);

            outRotTime = Mathf.Clamp(outRotTime, 0, 1f);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);

            // create reel rotation sequence - 4 parts  in - (continuous) - main - out
            float oldVal = 0f;
            tS = new TweenSeq();
            float angleX = 0;

            addRotate = Mathf.Max(0, addRotate);
            
            
            spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
            angleX = 360 * (int)spinSpeedMultiplier;
            
            angleX = angleX + (360) + outRotAngle + inRotAngle;
            float additionalRotTime = mainRotTime * (initAngle * (addRotate + 1) / angleX) ;

            if (!slotC.isStopping)
            {
                slotC.checkForceStop.SubscribeAsReady(this);

            }

            tS.Add((callBack) =>
            {
                oldVal = 0f;





                SimpleTween.Value(gameObject, 0, (initAngle * (addRotate + 1)), 0)
                                  .SetOnUpdate((float val) =>
                                  {

                                      //>>>>>>>>>>>>>>>> (Leong) (21/5/2021)
                                      if (!hasChangeIcon)
                                      {
                                          if (symbOrderFeature != null)
                                          {
                                              if (symbOrderFeature.Count > 0)
                                              {
                                                  ChangeIcon(SlotController.Instance.PlayFreeSpins ? symbOrderFeature : symbOrder);
                                              }
                                              else
                                              {
                                                  ChangeIcon(symbOrder);
                                              }

                                          }
                                          else
                                          {
                                              ChangeIcon(symbOrder);
                                          }
                                          ChangeToWinIcon(result);
                                          hasChangeIcon = true;
                                      }


                                      //>>>>>>>>>>>>>>>> (Leong) end (21/5/2021)
                                      

                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;


                                  })
                                  .AddCompleteCallBack(() =>
                                  {


                                      if (callBack != null) callBack();
                                  }).SetEase(mainRotType);
            });

            tS.Add((callBack) =>  // main rotation part
            {
                oldVal = 0f;



                
                SimpleTween.Value(gameObject, 0, -(angleX), mainRotTime)
                                  .SetOnUpdate((float val) =>
                                  {


                                      //ChangeRandonIcon();
                                      
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;

                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (callBack != null) callBack();
                                  }).SetEase(mainRotType);
            });

            tS.Add((callBack) =>  // additional rotation part
            {
                oldVal = 0f;



                SimpleTween.Value(gameObject, 0, -(initAngle * (addRotate + 1)), additionalRotTime)
                                  .SetOnUpdate((float val) =>
                                  {


                                     // ChangeRandonIcon();

                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;

                                  })
                                  .AddCompleteCallBack(() =>
                                  {

                                      //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
                                      if ((result.SpecialEffect && !slotC.IsAutoSpin && !slotC.PlayFreeSpins) || (result.SpecialEffect && slotC.enableSpecialEffectDuringAuto))
                                      {
                                          //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)  
                                          slotC.hasSpecialEffect = true;
                                          spinningEffectObj.SetActive(true);
                                          MasterAudio.PlaySound("ScatterEffect");
                                      }
                                      if (callBack != null) callBack();

                                  }).SetEase(mainRotType);
            });
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            if ((result.SpecialEffect && !slotC.IsAutoSpin && !slotC.PlayFreeSpins) || (result.SpecialEffect && slotC.enableSpecialEffectDuringAuto))
            {
             //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)   
                tS.Add((callBack) =>  // effect rotation
                {
                    oldVal = 0f;
                    

                    SimpleTween.Value(gameObject, 0, -(36000 ), result.AdditionalEffectTiem)
                                      .SetOnUpdate((float val) =>
                                      {

                                          
                                          
                                          

                                          TilesGroup.Rotate(val - oldVal, 0, 0);
                                          oldVal = val;
                                          

                                      })
                                      .AddCompleteCallBack(() =>
                                      {
                                          if (callBack != null) callBack();
                                      }).SetEase(mainRotType);
                });
            }
            

            tS.Add((callBack) =>  // out rotation part
            {
                oldVal = 0f;
                SimpleTween.Value(gameObject, 0, outRotAngle, outRotTime )
                                  .SetOnUpdate((float val) =>
                                  {

                                      spinningEffectObj.SetActive(false);
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020)
                                      UpdateIconAnimation("IsSpin", false);
                                      //(Updated)>>>>>>>>>>>>>>>>>> (25/11/2020) end
                                      //>>>>>>>>>>>>>>>> (13/4/2021)
                                      UpdateDisplayWinWhileSpin();
                                      //>>>>>>>>>>>>>>>> end (13/4/2021)
                                      if (slotC.PlayFreeSpins)
                                      {
                                          foreach (GameObject o in stickyIconStorage)
                                          {
                                              if (o != null)
                                              {

                                                  o.GetComponent<Animator>().SetBool("IsWin", true);
                                                  SpriteRenderer[] sRChild = o.GetComponents<SpriteRenderer>();
                                                  foreach (SpriteRenderer s in sRChild)
                                                  {

                                                      s.enabled = true;
                                                  }
                                              }

                                          }
                                      }
                                      
                                      if (rotCallBack != null) rotCallBack();
                                      if (callBack != null) callBack();
                                      MasterAudio.StopAllOfSound("ScatterEffect");
                                      MasterAudio.PlaySound("SpinStop");
                                  }).SetEase(outRotType);
            });

            tS.Start();


        }
        private void RecurRotation(float rotTime, Action completeCallBack)
        {
            float newAngle = -anglePerTileDeg * symbOrder.Count;
            float oldVal = 0;
            SimpleTween.Value(gameObject, 0, newAngle, rotTime)
                                .SetOnUpdate((float val) =>
                                {
                                    if (this)
                                    {
                                        TilesGroup.Rotate(val - oldVal, 0, 0);
                                        oldVal = val;
                                        ChangeIcon();
                                    }
                                })
                                .AddCompleteCallBack(() =>
                                {
                                    if (NextOrderPosition == -1) RecurRotation(rotTime, completeCallBack);
                                    else { if (completeCallBack != null) completeCallBack(); }
                                   
                                }).SetEase(EaseAnim.EaseLinear);
        }


        Vector3 pos;
        /// <summary>
        /// Change icon on reel appropriate to symbOrder
        /// </summary>
        private void ChangeIcon()
        {
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                pos = slotSymbols[i].transform.position - TilesGroup.position;

                int posOnReel = slotSymbols[i].Position;
                if (pos.z > -10 && posOnReel == nextReelSymbolToChange) // back side 
                {
                    slotSymbols[i].Position = nextReelSymbolToSet;
                    int symNumber = symbOrder[GetNextSymb()];
                    //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
                    slotSymbols[i].SetIcon(sprites[symNumber], symNumber, null);
                    //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)


                    nextReelSymbolToSet++;
                    nextReelSymbolToChange++;
                    return;
                }
            }
        }
        //>>>>>>>>>>>>>>>> (13/4/2021)
        public void CallbackOnCompleteAllSpin()
        {
            if (alwaysTriggerWholeCol)
            {
                if (ColumnWinningEffect != null)
                {
                    ColumnWinningEffect.SetActive(true);
                }
            }

            StopDisplayWinWhileSpin();
        }
        void UpdateDisplayWinWhileSpin()
        {
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                slotSymbols[i].ShowWinWhileSpin();
                
            }
        }
        void StopDisplayWinWhileSpin()
        {
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                slotSymbols[i].StopShowWinWhileSpin();

            }
        }
        //>>>>>>>>>>>>>>>> end (13/4/2021)
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        private void ChangeIcon(List<int> newSymOrder)
        {
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                

                int posOnReel = slotSymbols[i].Position;
                if (posOnReel == nextReelSymbolToChange) // back side 
                {
                    slotSymbols[i].Position = nextReelSymbolToSet;
                    int symNumber = newSymOrder[GetNextSymb()];
                    
                    slotSymbols[i].SetIcon(sprites[symNumber], symNumber, null);
                    


                    nextReelSymbolToSet++;
                    nextReelSymbolToChange++;
                    
                }
            }
        }
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        private void ChangeRandonIcon()
        {
            
            for (int i = rayCasters.Length - 1; i < slotSymbols.Length; i++)
            {
                pos = slotSymbols[i].transform.position - TilesGroup.position;
                
                int posOnReel = slotSymbols[i].Position;
                if (pos.z > -10 && posOnReel == nextReelSymbolToChange) // back side 
                {
                    
                    slotSymbols[i].Position = nextReelSymbolToSet;
                    int symNumber = symbOrder[UnityEngine.Random.Range(0, symbOrder.Count)];
                    //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)
                    slotSymbols[i].SetIcon(sprites[symNumber], symNumber, null);
                    //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)


                    nextReelSymbolToSet++;
                    nextReelSymbolToChange++;

                    return;
                }
            }
        }

        int setWinResultCounter = 0;
        //(Updated)>>>>>>>>>>>>>>>> (14/1/2021)

        private void ChangeToWinIcon(StoreSpinResult result)
        {
            if (slotC == null) return;
            if (!slotC.IsSpinning) return;
            SlotIcon.GroupIcon tempGroupIcon = null;
            int iconId = -1;
            int counterGroup = 0;
            GameObject groupGO = null;
            for (int i = 0; i < result.SymbolIds.Count; i++)
            {
                //>>>>>>>>>>>>>>>> (7/3/2021)(ARC)
                if (result.SIcons[i].groupIconSetting != null)
                {
                    if (result.SIcons[i].groupIconSetting.useGroupIcon)
                    {
                        if (iconId < 0 || iconId == result.SymbolIds[i])
                        {
                            iconId = result.SymbolIds[i];
                            if (counterGroup == 0)
                            {
                                tempGroupIcon = result.SIcons[i].groupIconSetting;
                                groupGO = Instantiate(tempGroupIcon.groupIconPrefab, slotSymbols[i].transform);
                            }
                            else
                            {
                                if (i == counterGroup)
                                    groupGO.transform.SetParent(slotSymbols[i].transform);

                            }
                            if (i == counterGroup)
                            {
                                groupGO.transform.localPosition = new Vector3(0, -tempGroupIcon.posY, 0);
                            }
                            else
                            {
                                groupGO.transform.localPosition = new Vector3(0, tempGroupIcon.posY, 0);
                            }

                            counterGroup++;
                        }
                        else
                        {
                            counterGroup = 0;
                            groupGO = null;
                        }

                    }
                    else
                    {
                        counterGroup = 0;
                        groupGO = null;
                    }
                }
                //>>>>>>>>>>>>>>>> end (7/3/2021)(ARC)
                //>>>>>>>>>>>>>>>> (13/4/2021)
                slotSymbols[i].SetIcon(result.SIcons[i], result.SymbolIds[i], groupGO, result.SIcons[i].displayWinAnimWhenAppear);
                //>>>>>>>>>>>>>>>> end (13/4/2021)
            }

        }
        //(Updated)>>>>>>>>>>>>>>>> end (14/1/2021)

        int next = 0;
        /// <summary>
        /// Return next symb position  in symbOrder array
        /// </summary>
        /// <returns></returns>
        private int GetNextSymb()
        {
            return (int)Mathf.Repeat(next++, symbOrder.Count);
        }

        /// <summary>
        /// Return angle in degree to next symbol position in symbOrder array
        /// </summary>
        /// <param name="nextOrderPosition"></param>
        /// <returns></returns>
        private float GetAngleToNextSymb(int nextOrderPosition)
        {
            if (currOrderPosition < nextOrderPosition)
            {
                return (nextOrderPosition - currOrderPosition) * anglePerTileDeg;
            }
            return (symbOrder.Count - currOrderPosition + nextOrderPosition) * anglePerTileDeg;
        }

        /// <summary>
        /// Return probabilties for eac symbol according to symbOrder array 
        /// </summary>
        /// <returns></returns>
        internal float[] GetReelSymbHitPropabilities(SlotIcon[] symSprites)
        {
            if (symSprites == null || symSprites.Length == 0) return null;
            float[] probs = new float[symSprites.Length];
            int length = symbOrder.Count;
            for (int i = 0; i < length; i++)
            {
                int n = symbOrder[i];
                probs[n]++;
            }
            for (int i = 0; i < probs.Length; i++)
            {
                float effectWholeColumnChanges = 1f;
                if (symSprites[i].effectWholeColumn)
                {
                    effectWholeColumnChanges = RayCasters.Length;
                }
                probs[i] = probs[i] * effectWholeColumnChanges / (float)length;
            }
            return probs;
        }

        /// <summary>
        /// Return true if top, middle or bottom raycaster has symbol with ID == symbID
        /// </summary>
        /// <param name="symbID"></param>
        /// <returns></returns>
        public bool HasSymbolInAnyRayCaster(int symbID, ref List<SlotSymbol> slotSymbols)
        {
            slotSymbols = new List<SlotSymbol>();
            bool res = false;
            SlotSymbol sS;

            for (int i = 0; i < rayCasters.Length; i++)
            {
                sS = rayCasters[i].GetSymbol();
                if (sS.iconID == symbID)
                {
                    res = true;
                    slotSymbols.Add(sS);
                }
            }

            return res;
        }

        /// <summary>
        /// Set next reel order while continuous rotation
        /// </summary>
        /// <param name="r"></param>
        internal void SetNextOrder(int r)
        {
            if (NextOrderPosition == -1)
                NextOrderPosition = r;
        }

        internal void CancelRotation()
        {
            SimpleTween.Cancel(gameObject, false);
            if (tS != null) tS.Break();
        }


    }
}
