using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using System;
using UnityEngine.Events;

namespace MkeyFW // mkey fortune wheel
{
    public class WheelController : MonoBehaviour
    {
        [Header("Main references")]
        [Space(16, order = 0)]
        [SerializeField]
        private Transform Reel;
        [SerializeField]
        private Animator pointerAnimator;
        [SerializeField]
        private LampsController lampsController;
        [SerializeField]
        private SpinButton spinButton;
        [SerializeField]
        private ArrowBeviour arrowBeviour;
        [SerializeField]
        private SpriteRenderer sectorLight;

        [Header("Spin options")]
        [Space(16, order = 0)]
        [SerializeField]
        private float inRotTime = 0.2f;
        [SerializeField]
        private float inRotAngle = 5;
        [SerializeField]
        private float mainRotTime = 1.0f;
        [SerializeField]
        private EaseAnim mainRotEase = EaseAnim.EaseLinear;
        [SerializeField]
        private float outRotTime = 0.2f;
        [SerializeField]
        private float outRotAngle = 5;
        [SerializeField]
        private float spinStartDelay = 0;
        [SerializeField]
        private int spinSpeedMultiplier = 1;

   
        [Header("Lamps control")]
        [Space(16, order = 0)]
        [Tooltip("Before spin")]
        [SerializeField]
        private LampsFlash lampsFlashAtStart = LampsFlash.Random;
        [Tooltip("During spin")]
        [SerializeField]
        private LampsFlash lampsFlashDuringSpin = LampsFlash.Sequence;
        [Tooltip("After spin")]
        [SerializeField]
        private LampsFlash lampsFlashEnd = LampsFlash.All;
      
        [Header("Additional options")]
        [Space(16, order = 0)]
        [Tooltip("Sector light")]
        [SerializeField]
        private int lightBlinkCount = 4;
        [Tooltip("Help arrow")]
        [SerializeField]
        private int arrowBlinkCount = 2;
        

        [Header("Result event, after spin")]
        [Space(16, order = 0)]
        [SerializeField]
        private UnityEvent resultEvent;

        [Header("Simulation, only for test")]
        [Space(32, order = 0)]
        [SerializeField]
        private bool simulate = false;
        [SerializeField]
        private int simPos = 0;
        [SerializeField]
        private bool debug = false;

        private Sector[] sectors;
        private int rand = 0;
        private int sectorsCount = 0;
        private float angleSpeed = 0;
        private float sectorAngleRad;
        private float sectorAngleDeg;
        private int currSector = 0;
        private int nextSector = 0;
        private TweenSeq tS;
        private TweenSeq lightTS;
        
        

        #region regular
        void OnValidate()
        {
            Validate();
        }

        void Start()
        {
            sectors = GetComponentsInChildren<Sector>();
            sectorsCount = (sectors != null) ? sectors.Length : 0;
            if(debug) Debug.Log("sectorsCount: " + sectorsCount);
            if (sectorsCount > 0)
            {
                sectorAngleDeg = 360f / sectorsCount;
                sectorAngleRad = 360f / sectorsCount * Mathf.Deg2Rad;
            }
            if (pointerAnimator)
            {
                pointerAnimator.enabled = false;
                pointerAnimator.speed = 0;
                pointerAnimator.transform.localEulerAngles = Vector3.zero;
            }
            if (lampsController) lampsController.lampFlash = lampsFlashAtStart;
            UpdateRand();
            if (arrowBeviour) arrowBeviour.Show(arrowBlinkCount,0.1f);
            
        }

        void Update()
        {
            UpdateRand();
        }

        void OnDestroy()
        {
            CancelSpin();
        }
        #endregion regular

        /// <summary>
        /// Start spin, call completeCallBack(int result, bool isBigWin) 
        /// </summary>
        /// <param name="completeCallBack"></param>
        public void StartSpin(Action<int, bool> completeCallBack)
        {
            if (arrowBeviour) arrowBeviour.CancelTween();
            if (tS != null) return;
            if (debug) Debug.Log("rand: " + rand);
            nextSector = rand;
            if (spinButton) spinButton.interactable = false;
            CancelLight();

           
            


            RotateWheel(() =>
            {
                if (spinButton) spinButton.interactable = true;
                if (arrowBeviour) arrowBeviour.Show(arrowBlinkCount, 3f);
                if (sectorLight) SectorLightShow(null);

                
                bool isBigWin = false;
                int res = GetWin(ref isBigWin);
                completeCallBack?.Invoke(res, isBigWin);
            });
        }

        /// <summary>
        /// Start spin
        /// </summary>
        public void StartSpin()
        {
            StartSpin(null);
        }

        /// <summary>
        /// Async rotate wheel to next sector
        /// </summary>
        private void RotateWheel(Action rotCallBack)
        {
            // validate input
            Validate();

            //change lamps state
            if (lampsController) lampsController.lampFlash = lampsFlashDuringSpin;

            // get next reel position
            nextSector = (!simulate) ? nextSector : simPos;
            if (debug) Debug.Log("next: " + nextSector + " ;angle: " + GetAngleToNextSector(nextSector));

            // create reel rotation sequence - 4 parts  in - (continuous) - main - out
            float oldVal = 0f;
            tS = new TweenSeq();
            float angleZ = 0;

            tS.Add((callBack) => // in rotation part
            {
                SimpleTween.Value(gameObject, 0f, inRotAngle, inRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      if (Reel) Reel.Rotate(0, 0, -val + oldVal);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (callBack != null) callBack();
                                  }).SetDelay(spinStartDelay);
            });

            tS.Add((callBack) =>  // main rotation part
            {
                oldVal = 0f;
                pointerAnimator.enabled = true;
                spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
                angleZ = GetAngleToNextSector(nextSector) + 360.0f * spinSpeedMultiplier;
                SimpleTween.Value(gameObject, 0, -(angleZ + outRotAngle + inRotAngle), mainRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      angleSpeed = -val + oldVal;
                                      if (Reel) Reel.Rotate(0, 0, angleSpeed);
                                      oldVal = val;
                                      if (pointerAnimator)
                                      {
                                          pointerAnimator.speed = Mathf.Abs(angleSpeed);
                                      }
                                  })
                                  .SetEase(mainRotEase)
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (pointerAnimator)
                                      {
                                          pointerAnimator.enabled = false;
                                          pointerAnimator.speed = 0;
                                          pointerAnimator.transform.localEulerAngles = Vector3.zero;
                                      }
                                      if (lampsController) lampsController.lampFlash = lampsFlashEnd;
                                      if (callBack != null) callBack();
                                  });
            });

            tS.Add((callBack) =>  // out rotation part
            {
                oldVal = 0f;
                SimpleTween.Value(gameObject, 0, outRotAngle, outRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      if (Reel) Reel.Rotate(0, 0, -val + oldVal);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (pointerAnimator)
                                      {
                                          pointerAnimator.transform.localEulerAngles = Vector3.zero;
                                      }
                                      currSector = nextSector;
                                      CheckResult();
                                      if (callBack != null) callBack();
                                  });
            });


            tS.Add((callBack) =>
            {
                if (resultEvent != null) resultEvent.Invoke();
                if (rotCallBack != null) rotCallBack();
                tS = null;
                if (callBack != null) callBack();
            });

            tS.Start();
        }

        private void Validate()
        {
            mainRotTime = Mathf.Max(0.1f, mainRotTime);

            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);

            outRotTime = Mathf.Clamp(outRotTime, 0, 1f);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);
            spinSpeedMultiplier = Mathf.Max(1, spinSpeedMultiplier);
            spinStartDelay = Mathf.Max(0, spinStartDelay);

            lightBlinkCount = Mathf.Max(0, lightBlinkCount);

            if (simulate)
            {
                sectors = GetComponentsInChildren<Sector>();
                sectorsCount = (sectors != null) ? sectors.Length : 0;
                simPos = Mathf.Clamp(simPos, 0, sectorsCount - 1);
            }
        }

        /// <summary>
        /// Return angle in degree to next symbol position in symbOrder array
        /// </summary>
        /// <param name="nextOrderPosition"></param>
        /// <returns></returns>
        private float GetAngleToNextSector(int nextOrderPosition)
        {
            if (currSector < nextOrderPosition)
            {
                return (nextOrderPosition - currSector) * sectorAngleDeg;
            }
            return (sectors.Length - currSector + nextOrderPosition) * sectorAngleDeg;
        }

        /// <summary>
        /// Upadate random value rand
        /// </summary>
        private void UpdateRand()
        {
            rand = UnityEngine.Random.Range(0, sectorsCount);
        }

        public void CancelSpin()
        {
            if (this)
            {
                CancelLight();

                if (tS != null) tS.Break();
                tS = null;

                SimpleTween.Cancel(gameObject, false);
                if (pointerAnimator)
                {
                    pointerAnimator.enabled = false;
                    pointerAnimator.speed = 0;
                    pointerAnimator.transform.localEulerAngles = Vector3.zero;
                }
            }
        }

        public void CancelLight()
        {
            if (this)
            {
                if (sectorLight)
                {
                    SimpleTween.Cancel(sectorLight.gameObject, false);
                    sectorLight.color = new Color(1, 1, 1, 0);
                }
                if (lightTS != null) lightTS.Break();
                lightTS = null;
            }
        }

        /// <summary>
        /// Check result and invoke sector hit event
        /// </summary>
        private void CheckResult()
        {
            int coins = 0;
            bool isBigWin = false;

            if (sectors != null && currSector >= 0 && currSector < sectors.Length)
            {
                Sector s = sectors[currSector];
                if (s != null)
                {
                    isBigWin = s.BigWin;
                    coins = s.Coins;
                    s.PlayHit(Reel.position);
                }
            }
            if (debug) Debug.Log("Coins: " + coins + " ;IsBigWin: " + isBigWin);
        }

        /// <summary>
        /// Return spin result, coins
        /// </summary>
        /// <param name="isBigWin"></param>
        /// <returns></returns>
        public int GetWin(ref bool isBigWin)
        {
            int res = 0;
            isBigWin = false;
            if (sectors != null && currSector >= 0 && currSector < sectors.Length)
            {
                isBigWin = sectors[currSector].BigWin;
                return sectors[currSector].Coins;
            }
            return res;
        }

        private void SectorLightShow(Action completeCallBack)
        {
            if (!sectorLight || lightTS != null)
            {
                if (completeCallBack != null) completeCallBack();
                return;
            }
            lightTS = new TweenSeq();
            float fadeTime = 0.2f;
            float delay = 0.2f;
            float stayTime = 0.2f;
            EaseAnim ease = EaseAnim.EaseInOutSine;
            GameObject gO = sectorLight.gameObject;

            for (int i = 0; i < lightBlinkCount; i++)
            {
                lightTS.Add((callBack) =>  //fadein
                {

                    SimpleTween.Value(gO, 0, 1, fadeTime)
                        .SetOnUpdate((float val) => { if (this && sectorLight) sectorLight.color = new Color(1, 1, 1, val); })
                        .SetDelay(delay)
                        .SetEase(ease)
                        .AddCompleteCallBack(callBack);
                });

                lightTS.Add((callBack) => //fadeout
                {
                    SimpleTween.Value(gO, 1, 0, fadeTime)
                        .SetOnUpdate((float val) => { if (this && sectorLight) sectorLight.color = new Color(1, 1, 1, val); })
                        .SetDelay(stayTime)
                        .SetEase(ease)
                        .AddCompleteCallBack(callBack);
                });
            }

            lightTS.Add((callBack) =>
            {
                lightTS = null;
                if (completeCallBack != null) completeCallBack();
                if (callBack != null) callBack();
            });
            lightTS.Start();
        }
    }
}