using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Events;
using DarkTonic.MasterAudio;

namespace Mkey
{
    public class LineBehavior : MonoBehaviour
    {
        public int number;
        public RayCaster[] rayCasters;

        public WinData win;
        private bool winTweenComplete = true;

        private MaterialPropertyBlock[] mpB;
        public List<SpriteRenderer> dotList;
        private Renderer[] rend;

        public TypeOfWinningLine typeOfLine;

        [SerializeField]
        private Sprite dotSprite;
        [SerializeField]
        private Material dotMaterial;
        private int dotSortingLayerID = 0; //next updates
        [SerializeField]
        private int dotSortingOrder;

        [SerializeField]
        private float dotDistance = 2f;
        [SerializeField]
        private float burnTime = 1f; // next updates 
        [SerializeField]
        private int burnSpeed = 4;   // next updates

        public LineRenderer lineRender;

        public SpriteLine spirteLineScript;

        public Material lineMaterial;

        public Color lineColor;

        
        [Range(0.5f, 3f)]
        public float lineFlashingSpeed = 1f;

        
        [Range(0.1f, 100f)]
        public float lineRendererWidth = 0.2f;

        
        [Range(2f, 100f)]
        public float lineSpeed = 100f;

        public WinningBoxSequenceType typeOfBoxWinning = WinningBoxSequenceType.Always_Display;

       
        [Range(0.5f, 200f)]
        public float boxSizeWidth = 4.1f;
        
        [Range(0.5f, 200f)]
        public float boxSizeHeight = 3.13f;

        
        public bool useRaycasterAsBoxPos = true;

        private WinningBoxesDisplay winBoxes;


        private List<Vector3> linePositions = new List<Vector3>();
        private bool isWinFlashing;
        private List<LineRenderer> tempWinBoxesLine = new List<LineRenderer>();

        [Tooltip("Line color in line info panel")]
        public Color lineInfoColor = Color.white;
        [Tooltip("BackGround color in line info panel")]
        public Color lineInfoBGColor = Color.blue;

        public Action<bool> ChangeSelectionEvent;

        private LinesController linesController;
        public LineButtonBehavior LineButton { get; private set; }

        public UILineButtonBehavior uiButton;
        public UILineButtonBehavior uiButtonLeft;
       
        public bool IsSelected
        {
            get; private set;
        }

        public bool IsWinningLine
        {
            get { return win != null; }
        }

        /// <summary>
        /// Get spins won
        /// </summary>
        public int WonSpins
        {
            get
            {
                return (win == null) ? 0 : win.FreeSpins;
            }
        }

        /// <summary>
        /// Get coins won
        /// </summary>
        internal double WonCoins
        {
            get
            {
                return (win == null) ? 0 : win.Pay;
            }
        }

        /// <summary>
        /// Return true if is won tween complete
        /// </summary>
        internal bool IsWinTweenComplete
        {
            get { return winTweenComplete; }
        }

        private void Start()
        {
            SlotController.Instance.SubscribeLineB(this);
        }

        public LineRenderer LineRender
        {
            get
            {
                lineRender.widthMultiplier = lineRendererWidth;
                return lineRender;
            }
            set
            {

                lineRender = value;

            }
        }

        #region regular
        /// <summary>
        /// Start from linesController
        /// </summary>
        /// <param name="linesController"></param>
        internal void InitStart(LinesController linesController, bool useLinesControllerMaterial)
        {
            this.linesController = linesController;
            LineCreator lC = GetComponent<LineCreator>();
            List<Vector3> positions = new List<Vector3>();
            if (lC && lC.enabled && lC.handlesPositions != null && lC.handlesPositions.Count > 1)
            {
                foreach (var item in lC.handlesPositions)
                {
                    positions.Add(transform.TransformPoint(item));
                }
            }
            else
            {
                // create line using raycasters
                foreach (var item in rayCasters)
                {
                    if (item)
                    {
                        positions.Add(item.transform.position);
                    }
                }
            }
            if (useLinesControllerMaterial)
            {
                dotSprite = linesController.dotSprite;
                dotMaterial = linesController.dotMaterial;
                dotSortingLayerID = linesController.dotSortingLayerID;
                dotSortingOrder = linesController.dotSortingOrder;
                dotDistance = linesController.dotDistance;
            }
            switch (typeOfLine)
            {
                case TypeOfWinningLine.dots:
                    {
                        dotList = CreateDotLine(positions, dotSprite, dotMaterial, dotSortingLayerID, dotSortingOrder, dotDistance, false);

                        //2) cache data 
                        if (dotList != null && dotList.Count > 0)
                        {
                            rend = new Renderer[dotList.Count];
                            mpB = new MaterialPropertyBlock[dotList.Count];
                            for (int i = 0; i < dotList.Count; i++)
                            {
                                rend[i] = dotList[i];
                                MaterialPropertyBlock mP = new MaterialPropertyBlock();
                                mP.Clear();
                                rend[i].GetPropertyBlock(mP);
                                mpB[i] = mP;
                            }
                        }
                        break;
                    }
                case TypeOfWinningLine.line:
                    {
                        if (LineRender != null)
                        {
                            linePositions = positions;


                            LineRender.colorGradient = GetGradient(lineColor, 1f);
                            LineRender.sortingLayerID = dotSortingLayerID;
                            LineRender.sortingOrder = dotSortingOrder;
                            LineRender.material = lineMaterial;
                            if (useRaycasterAsBoxPos)
                            {
                                winBoxes = new WinningBoxesDisplay(rayCasters, transform, new Vector2(boxSizeWidth, boxSizeHeight), LineRender);
                            }
                            else
                            {
                                winBoxes = new WinningBoxesDisplay(rayCasters, transform, new Vector2(boxSizeWidth, boxSizeHeight), LineRender, positions.ToArray());
                            }



                        }
                        break;
                    }
            }
            //2) cache data 
            if (dotList != null && dotList.Count > 0)
            {
                rend = new Renderer[dotList.Count];
                mpB = new MaterialPropertyBlock[dotList.Count];
                for (int i = 0; i < dotList.Count; i++)
                {
                    rend[i] = dotList[i];
                    MaterialPropertyBlock mP = new MaterialPropertyBlock();
                    mP.Clear();
                    rend[i].GetPropertyBlock(mP);
                    mpB[i] = mP;
                }
            }



            win = null;

            LineButton = GetComponentInChildren<LineButtonBehavior>();
            if (LineButton) // set event handlers
            {
                LineButton.PointerDownEvent += ButtonClickHandler;
                ChangeSelectionEvent += LineButton.Refresh;
            }

            if (uiButton)
            {
                uiButton.button.onClick.AddListener(ButtonClickHandler);
                ChangeSelectionEvent += uiButton.Refresh;
            }

            if (uiButtonLeft)
            {
                uiButtonLeft.button.onClick.AddListener(ButtonClickHandler);
                ChangeSelectionEvent += uiButtonLeft.Refresh;
            }
          
        }

        void OnDrawGizmosSelected()
        {

        }

        void OnDrawGizmos()
        {

        }

        private void OnDestroy()
        {
            if (LineButton)
            {
                LineButton.PointerDownEvent -= ButtonClickHandler;
                ChangeSelectionEvent -= LineButton.Refresh;
            }
            if (uiButton)
            {
                
                ChangeSelectionEvent -= uiButton.Refresh;
            }
            if (uiButtonLeft)
            {
                ChangeSelectionEvent -= uiButtonLeft.Refresh;
            }
        }

        #endregion regular

        public void ButtonClickHandler()
        {
            
            linesController.LineButton_Click(this);
        }

        /// <summary>
        /// Select line
        /// </summary>
        public void Select(bool setVisible, float burnDelay)
        {
            IsSelected = true;
            LineBurn(true, burnDelay, null);
            OnChangeSelectionEvent();
        }

        /// <summary>
        /// Select line
        /// </summary>
        public void Select(bool setVisible, bool burn, float burnDelay)
        {
            IsSelected = true;
            LineBurn(burn, burnDelay, null);
            OnChangeSelectionEvent();
        }

        /// <summary>
        /// Deselect line
        /// </summary>
        public void DeSelect()
        {
            IsSelected = false;
            LineBurn(false, 0, null);
            OnChangeSelectionEvent();
        }

        #region dotline

        /// <summary>
        /// Create dotline use raycasters
        /// </summary>
        private List<SpriteRenderer> CreateDotLine(List<Vector3> positions, Sprite sprite, Material material, int sortingLayerID, int sortingOrder, float distance, bool setActive)
        {
            if (positions == null || positions.Count < 2) return null;
            List<SpriteRenderer> dList = new List<SpriteRenderer>();
            int length = positions.Count;

            for (int i = 0; i < length - 2; i++)
            {
                CreateDotLine(ref dList, sprite, material, positions[i], positions[i + 1], 0, sortingOrder, distance, true, false);
            }
            CreateDotLine(ref dList, sprite, material, positions[length - 2], positions[length - 1], 0, sortingOrder, distance, true, true);
            if (dList != null)
                dList.ForEach((r) => { if (r != null) r.gameObject.SetActive(setActive); });
            return dList;
        }

        /// <summary>
        /// Create dotLine tile between two points, use world coordinats
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dist"></param>
        /// <param name="createStartPoint"></param>
        /// <param name="createEndPoint"></param>
        private void CreateDotLine(ref List<SpriteRenderer> dList, Sprite sprite, Material material, Vector3 start, Vector3 end, int sortingLayerID, int sortingOrder, float dist, bool createStartPoint, bool createEndPoint)
        {
            Vector3 dir = end - start;
            float seLength = dir.magnitude;

            if (createStartPoint) dList.Add(Creator.CreateSpriteAtPosition(transform, sprite, material, start, sortingLayerID, sortingOrder));

            if (seLength == 0) return;

            Vector3 dirOne = dir / seLength;
            float countf = (dist < seLength) ? seLength / dist + 1f : 2f;
            float count = Mathf.RoundToInt(countf);

            for (int i = 1; i < count - 1; i++)
            {
                dList.Add(Creator.CreateSpriteAtPosition(transform, sprite, material, start + dirOne * ((float)i * seLength / (count - 1f)), sortingLayerID, sortingOrder));
            }

            if (createEndPoint)
            {
                dList.Add(Creator.CreateSpriteAtPosition(transform, sprite, material, end, sortingLayerID, sortingOrder));
            }
        }

        #endregion dotline

        #region lineRenderer
        /// <summary>
        /// Return gradient color for line
        /// </summary>
        Gradient GetGradient(Color setLineColor, float alpha)
        {
            Gradient g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] { new GradientColorKey(setLineColor, 0.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f) }
                );

            return g;
        }




        
        /// <summary>
        /// LineRenderer Winning Flashing
        /// </summary>
        private IEnumerator LineFadingInOutC(float speed)
        {

            float alpha = 0;
            float changeAlphaDirection = -1;

            bool changeToBox = true;
            Gradient gradient = new Gradient();


            while (isWinFlashing)
            {
                //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
                if (!animationWinLine.enableAnimationLine)
                {
                    if (alpha <= 0 || alpha >= 1)
                    {
                        changeAlphaDirection = changeAlphaDirection * -1;

                        if (alpha <= 0)
                        {
                            changeToBox = !changeToBox;
                            alpha = 0;
                        }
                    }

                    alpha = alpha + (speed * changeAlphaDirection * Time.deltaTime);
                }
               

                switch (typeOfBoxWinning)
                {
                    case WinningBoxSequenceType.None:

                        break;
                    case WinningBoxSequenceType.Alter_With_Line:
                        if (changeToBox)
                        {
                            if (tempWinBoxesLine.Count > 0)
                            {
                                foreach (LineRenderer r in tempWinBoxesLine)
                                {
                                    r.colorGradient = GetGradient(lineColor, alpha);
                                }
                            }
                        }


                        break;

                    case WinningBoxSequenceType.Always_Display:

                        if (tempWinBoxesLine.Count > 0)
                        {
                            foreach (LineRenderer r in tempWinBoxesLine)
                            {
                                r.colorGradient = GetGradient(lineColor, 1f);
                            }
                        }

                        break;

                    case WinningBoxSequenceType.Sequence_With_Line:
                        if (tempWinBoxesLine.Count > 0)
                        {
                            foreach (LineRenderer r in tempWinBoxesLine)
                            {
                                r.colorGradient = GetGradient(lineColor, alpha);
                            }
                        }
                        break;
                }
                
                if (!animationWinLine.enableAnimationLine)
                {
                    if (!changeToBox || typeOfBoxWinning == WinningBoxSequenceType.Sequence_With_Line)
                    {
                        LineRender.colorGradient = GetGradient(lineColor, alpha);
                    }
                }
                //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)



                yield return new WaitForEndOfFrame();

            }


            if (typeOfBoxWinning != WinningBoxSequenceType.None)
            {
                if (tempWinBoxesLine.Count > 0)
                {
                    foreach (LineRenderer r in tempWinBoxesLine)
                    {
                        r.colorGradient = GetGradient(lineColor, 1f);
                    }
                }
            }
            LineRender.colorGradient = GetGradient(lineColor, 1f);


        }

        #endregion lineRenderer

        /// <summary>
        /// Enable or disable the flashing material
        /// </summary>
        internal void LineFlashing(bool flashing)
        {

            switch (typeOfLine)
            {
                case TypeOfWinningLine.dots:
                    {
                        if (mpB == null || mpB.Length == 0) return;


                        if (flashing)
                        {
                            for (int i = 0; i < mpB.Length; i++)
                            {
                                mpB[i].SetFloat("_FadeEnable", 1);
                            }

                        }
                        else
                        {
                            for (int i = 0; i < mpB.Length; i++)
                            {
                                mpB[i].SetFloat("_FadeEnable", 0);
                            }
                        }

                        for (int i = 0; i < mpB.Length; i++)
                        {
                            rend[i].SetPropertyBlock(mpB[i]);
                        }
                        break;
                    }
                case TypeOfWinningLine.line:
                    {
                        isWinFlashing = flashing;

                        StartCoroutine(LineFadingInOutC(lineFlashingSpeed));
                        break;
                    }
            }


        }




        private IEnumerator LineBurnC(int dotCount, float burnDelay, Action completeCallBack)
        {
            if (IsSelected && dotList != null)
            {
                int p = 0;
                bool a;
                WaitForEndOfFrame wfef = new WaitForEndOfFrame();
                switch (typeOfLine)
                {
                    case TypeOfWinningLine.dots:
                        {
                            for (int c = 0; c < 2; c++)
                            {
                                if (!IsSelected) break;
                                if (burnCancel) break;

                                for (int i = 0; i < dotList.Count + dotCount; i += dotCount)
                                {
                                    if (burnCancel) break;

                                    if (!IsSelected) break;
                                    for (int j = 0; j < dotCount; j++)
                                    {
                                        if ((p = i + j) >= dotList.Count) break;
                                        a = dotList[p].gameObject.activeSelf;
                                        dotList[p].gameObject.SetActive(!a);
                                    }
                                    if (p >= dotList.Count) break;
                                    yield return wfef;
                                }
                                yield return new WaitForSeconds(1.2f);
                            }

                            break;
                        }
                    case TypeOfWinningLine.line:
                        {
                            //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
                            if (animationWinLine.enableAnimationLine)
                            {
                                if (animationWinLine.animationLineGameObj)
                                    animationWinLine.animationLineGameObj.SetActive(true);
                            }
                            else
                            {
                                List<Vector3> tempPos = new List<Vector3>();
                                if (linePositions.Count > 0)
                                {
                                    LineRender.positionCount = 1;
                                    tempPos.Add(linePositions[0]);
                                    LineRender.SetPositions(tempPos.ToArray());
                                }

                                for (int i = 0; i < linePositions.Count; i++)
                                {
                                    if (burnCancel) break;

                                    if (!IsSelected) break;

                                    Vector3 curretPos = linePositions[i];



                                    if (i < linePositions.Count - 1)
                                    {
                                        LineRender.positionCount = i + 2;

                                        tempPos.Add(linePositions[i + 1]);
                                        LineRender.SetPositions(tempPos.ToArray());

                                        while (Vector3.Distance(curretPos, linePositions[i + 1]) >= 0.005f)
                                        {
                                            curretPos = Vector3.MoveTowards(curretPos, linePositions[i + 1], lineSpeed);
                                            LineRender.SetPosition(i + 1, curretPos);
                                            yield return wfef;
                                        }
                                    }



                                }
                            }
                            
                            yield return new WaitForSeconds(1.2f);
                            LineRender.positionCount = 0;
                            if (animationWinLine.enableAnimationLine)
                            {
                                if (animationWinLine.animationLineGameObj)
                                    animationWinLine.animationLineGameObj.SetActive(false);
                            }
                            //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
                            break;
                        }
                }

            }
            completeCallBack?.Invoke();
        }


        bool burnCancel = false;
        internal void LineBurn(bool burn, float burnDelay, Action completeCallBack)
        {
            burnCancel = (!burn) ? true : false;
            StopCoroutine("LineBurnC");
            SetLineVisible(false);
            if (burn)
                StartCoroutine(LineBurnC(3, burnDelay, completeCallBack));
        }

        /// <summary>
        /// Enable or disable line elemnts.
        /// </summary>
        internal void SetLineVisible(bool visible)
        {
            

            switch (typeOfLine)
            {
                case TypeOfWinningLine.dots:
                    {
                        if (dotList == null) return;
                        foreach (var item in dotList)
                            item.gameObject.SetActive(visible);
                        break;
                    }
                case TypeOfWinningLine.line:
                    {
                        if (LineRender != null)
                        {
                            if (visible)
                            {
                                //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
                                MasterAudio.PlaySound("LineJumping");
                                if (animationWinLine.enableAnimationLine)
                                {
                                    if (animationWinLine.animationLineGameObj)
                                    {
                                        animationWinLine.animationLineGameObj.SetActive(true);
                                        if(winType == WinType.Right)
                                        {
                                            animationWinLine.animationLineGameObj.GetComponent<SpriteRenderer>().flipX = true;
                                        }
                                        else
                                        {
                                            animationWinLine.animationLineGameObj.GetComponent<SpriteRenderer>().flipX = false;
                                        }
                                        

                                    } 

                                }
                                else
                                {
                                    LineRender.positionCount = linePositions.Count;
                                    LineRender.SetPositions(linePositions.ToArray());
                                }
                                //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
                                if (typeOfBoxWinning != WinningBoxSequenceType.None)
                                {
                                    if (tempWinBoxesLine.Count > 0)
                                    {
                                        foreach (LineRenderer r in tempWinBoxesLine)
                                        {
                                            r.enabled = true;
                                            r.colorGradient = GetGradient(lineColor, 0f);
                                        }
                                    }
                                }


                            }
                            else
                            {
                                //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
                                if (animationWinLine.enableAnimationLine)
                                {
                                    if (animationWinLine.animationLineGameObj) animationWinLine.animationLineGameObj.SetActive(false);
                                }
                                else
                                {
                                    LineRender.positionCount = 0;
                                }
                                //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)
                                
                                if (typeOfBoxWinning != WinningBoxSequenceType.None)
                                {
                                    if (tempWinBoxesLine.Count > 0)
                                    {
                                        foreach (LineRenderer r in tempWinBoxesLine)
                                        {
                                            r.enabled = false;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
            }


        }

        /// <summary>
        /// Set Order for line spite rendrer.
        /// </summary>
        private void SetLineRenderOrder(int order)
        {
            switch (typeOfLine)
            {
                case TypeOfWinningLine.dots:
                    {
                        foreach (var item in dotList)
                            item.sortingOrder = order;
                        break;
                    }
                case TypeOfWinningLine.line:
                    {
                        if (LineRender != null)
                        {

                            LineRender.sortingOrder = order;
                        }
                        break;
                    }
            }

        }

        //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
        enum WinType
        {
            Left,
            Right,
            Both
        }

        WinType winType;
        [System.Serializable]
        public class AnimationWinLine
        {
            public bool enableAnimationLine;
            public GameObject animationLineGameObj;
        }

        public AnimationWinLine animationWinLine;
        //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)

        /// <summary>
        /// Find  and fill winning symbols list  from left to right, according pay lines
        /// </summary>
        internal void FindWin(List<PayLine> payTable)
        {
            win = null;
            WinData winTemp = null;
            WinData winBackward = null;
            PayLine p = null;

            foreach (var item in payTable)
            {
                // find max win
                winTemp = GetPayLineWin(item);
                if (winTemp != null)
                {
                    if (winTemp.IsReverseWin)
                    {
                        if (winBackward == null)
                        {
                            winBackward = winTemp;
                            p = item;
                        }
                        else
                        {
                            if (winBackward.Pay < winTemp.Pay || winBackward.FreeSpins < winTemp.FreeSpins || winBackward.TotalBetMult < winTemp.TotalBetMult)
                            {

                                winBackward = winTemp;

                            }

                        }
                    }
                    else
                    {
                        if (win == null)
                        {
                            win = winTemp;
                            p = item;
                        }
                        else
                        {
                            if (win.Pay < winTemp.Pay || win.FreeSpins < winTemp.FreeSpins || win.TotalBetMult < winTemp.TotalBetMult)
                            {

                                win = winTemp;
                                p = item;
                            }

                        }
                    }
                   

                }
                
            }
            //if (number == 3 && win != null)
            //{
            //    string idss = "";
            //    for(int z = 0; z < p.line.Length; z++)
            //    {
            //        idss += p.line[z] + " | ";
            //    }
            //    GuiController.Instance.ShowMessage(""+ win.Pay, idss, 2, () => { });
            //}
            //(Updated)>>>>>>>>>>>>>>>> (29/1/2021)
            winType = WinType.Left;
            if (win == null && winBackward != null)
            {

                win = winBackward;
                winBackward = null;
                winType = WinType.Right;
            }
            else if(win != null && winBackward != null)
            {

                if (win.onlyWinOneWay || winBackward.onlyWinOneWay)
                {

                }
                else
                {
                    win.CombineWin(winBackward);
                    winType = WinType.Both;
                }

            }
            //(Updated)>>>>>>>>>>>>>>>> end (29/1/2021)

            if (win != null)
            {
                if (typeOfBoxWinning != WinningBoxSequenceType.None)
                {
                    tempWinBoxesLine = winBoxes.OnWin(win.WinningRaycaster);
                }

            }
            


        }

        /// <summary>
        /// Check if line is wonn, according payline
        /// </summary>
        /// <param name="payLine"></param>
        /// <returns></returns>

        private WinData GetPayLineWin(PayLine payLine)
        {
            if (payLine == null || payLine.line.Length != rayCasters.Length) return null;
            List<SlotSymbol> winnSymbols = new List<SlotSymbol>();
            List<RayCaster> winningRaycaster = new List<RayCaster>();
            SlotSymbol s;
            winningRaycaster.Clear();
            int iconId = -1;
            bool isWildWIn = false;
            bool isScatterwin = false;
            bool isSpecialIconwin = false;
            for (int i = 0; i < rayCasters.Length; i++)
            {
                s = rayCasters[i].GetSymbol();
                //Debug.Log(s.iconID);
                if (payLine.line[i] >= 0 && s.iconID != payLine.line[i])
                {

                    return null;
                }
                else if (payLine.line[i] >= 0 && s.iconID == payLine.line[i])
                {
                    if (s.iconID == SlotController.Instance.wild_id)
                    {
                        isWildWIn = true;
                    }
                    else if (s.iconID == SlotController.Instance.scatter_id)
                    {
                        isScatterwin = true;
                    }
                    else if (s.iconID == SlotController.Instance.specialIcon_id)
                    {
                        isSpecialIconwin = true;
                    }
                    else
                    {
                        if(s.iconID >= 0)
                        iconId = s.iconID;
                    }
                    winnSymbols.Add(s);

                    winningRaycaster.Add(rayCasters[i]);
                }
            }
            
            double pay = payLine.pay;
            double payTotalBet = 0;
            if (isWildWIn)
            {
                if (payLine.totalBetMult > 0)
                {

                    payTotalBet = SlotPlayer.Instance.TotalBet * payLine.totalBetMult;
                    if (payLine.wildMulti)
                    {
                        if (!SlotController.Instance.getUseServerResult())
                        {
                            payTotalBet = payTotalBet * (float)SlotController.Instance.wild_multiply;
                        }
                        
                       
                    }
                    pay = 0;
                }
                else
                {
                    if (payLine.wildMulti)
                    {
                        if (!SlotController.Instance.getUseServerResult())
                        {
                            pay = payLine.pay * (float)SlotController.Instance.wild_multiply;
                        }
                    }
                        
                    
                }
                
            } else {
                if (payLine.totalBetMult > 0)
                {

                    payTotalBet = SlotPlayer.Instance.TotalBet * payLine.totalBetMult;
                    Debug.Log("Scatter Win Amount: " + pay);
                    pay = 0;
                }
            }
            
            return new WinData(winningRaycaster, iconId, winnSymbols, payLine.freeSpins, pay, payTotalBet, payLine.payMult, payLine.totalBetMult, payLine.LineEvent, isWildWIn, isScatterwin, isSpecialIconwin, payLine.reverseWin, payLine.onlyWinOneWay, number);
        }

        /// <summary>
        /// Reset old winnig data 
        /// </summary>
        internal void ResetLineWinning()
        {
            win = null;
        }

        /// <summary>
        /// Instantiate particles for each winning symbol
        /// </summary>
        internal void ShowWinSymbolsParticles(bool activate)
        {
            if (IsWinningLine)
            {
                win.Symbols.ForEach((wS) => { wS.ShowParticles(activate, SlotController.Instance.particlesStars); });
            }
        }

        /// <summary>
        /// Instantiate jump clone for each symbol
        /// </summary>
        

        internal void WinTrigger()
        {
            foreach (SlotSymbol s in win.Symbols)
            {
                //s.WinningSlotTrigger();
            }
        }

        /// <summary>
        /// Instantiate jump clone for each symbol
        /// </summary>
        internal void LineWin( int count, Action<WinData> comleteCallBack)
        {
            winTweenComplete = false;
            if (win == null || win.Symbols == null)
            {
                comleteCallBack?.Invoke(null);
                return;
            }
            ParallelTween pt = new ParallelTween();

            pt.Add((callBack) =>
            {
                foreach (SlotSymbol s in win.Symbols)
                {
                    
                    s.WinTrigger(callBack, count);
                    
                }

            });
            
            pt.Start(() =>
            {
                winTweenComplete = true;
                LineWinCancel();
                comleteCallBack?.Invoke(win);
            });
        }

        internal void LineWinCancel()
        {
            if (win != null && win.Symbols != null)
                win.Symbols.ForEach((ws) => { if (ws != null) ws.WinCancel(); });
        
        }

        

        

        #region raise events
        private void OnChangeSelectionEvent()
        {
            ChangeSelectionEvent?.Invoke(IsSelected);
        }
        #endregion raise events

    }

    public class WinData
    {
        List<SlotSymbol> symbols;
        private int freeSpins = 0;
        private double pay = 0;
        private int payMult = 1;
        private float totalBetMult = 0;
       
        private bool isWildWin = false;
        private bool isScatterWin = false;
        private bool isSpecialIconWin = false;
        private UnityEvent winEvent;
        private ScatterPay scatterP;
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        private SpecialIconPay specialIconP;
        //>>>>>>>>>>>>>>>> end (15/3/2021)(ARC)
        private WildPay wildP;
        private int lineBehaviourNum;
        private int winIconId;
        private double winTotalBetMulti;
        private bool isReverseWin;
        public bool onlyWinOneWay;
        private List<RayCaster> winningRaycaster;
        public double Pay
        {
            get { return pay * SlotPlayer.Instance.LineBet * PayMult; }
        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
        public double WinPay
        {
            get { return pay * PayMult; }
        }
        //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
        
        public int FreeSpins
        {
            get { return freeSpins; }
        }

        public int PayMult
        {
            get { return payMult; }
        }

        public float TotalBetMult
        {
            get { return totalBetMult; }
        }

        public UnityEvent WinEvent
        {
            get { return winEvent; }
        }

        public List<SlotSymbol> Symbols
        {
            get { return symbols; }
        }

        public int LineBehaviourNum { get => lineBehaviourNum; }

        public bool IsWildWin { get => isWildWin; }

        public bool IsScatterWin { get => isScatterWin; }
        
        public ScatterPay ScatterP { get => scatterP; private set => scatterP = value; }
        public WildPay WildP { get => wildP; private set => wildP = value; }
        public List<RayCaster> WinningRaycaster { get => winningRaycaster; private set => winningRaycaster = value; }
        public int WinIconId { get => winIconId; private set => winIconId = value; }
        public double WinTotalBetMulti { get => winTotalBetMulti; private set => winTotalBetMulti = value; }
        public bool IsReverseWin { get => isReverseWin; set => isReverseWin = value; }
        public SpecialIconPay SpecialIconP { get => specialIconP; set => specialIconP = value; }

        public WinData(List<RayCaster> winningRaycaster,int winIconId, List<SlotSymbol> symbols, int freeSpins, double pay, double winTotalBetMulti, int payMult, float totalBetMult, UnityEvent lineEvent, bool isWildWin,bool isScatterWin, bool isSpecialIconWin, bool isReverseWin, bool onlyWinOneWay, int lineId = -1 )
        {
            this.symbols = symbols;
            this.freeSpins = freeSpins;
            this.pay = pay;
            this.winTotalBetMulti = winTotalBetMulti;
            this.payMult = payMult;
            this.totalBetMult = totalBetMult;
            this.winEvent = lineEvent;
            this.lineBehaviourNum = lineId;
            this.isWildWin = isWildWin;
            this.isScatterWin = isScatterWin;
            this.isSpecialIconWin = isSpecialIconWin;
            this.winningRaycaster = winningRaycaster;
            this.winIconId = winIconId;
            this.IsReverseWin = isReverseWin;
            this.onlyWinOneWay = onlyWinOneWay;
            
        }

        public void CombineWin(WinData additionalWin)
        {
            
            this.freeSpins += additionalWin.FreeSpins;
            this.pay = Math.Round((this.pay + additionalWin.pay) * 100f) / 100f; 
            this.payMult = additionalWin.PayMult;
            this.totalBetMult += additionalWin.totalBetMult;
            if (additionalWin.IsWildWin)
            {
                this.isWildWin = additionalWin.IsWildWin;
            }
            if (additionalWin.IsScatterWin)
            {
                this.isScatterWin = additionalWin.IsScatterWin;
            }
            foreach(RayCaster r in additionalWin.WinningRaycaster)
            {
                this.winningRaycaster.Add(r);
            }
            foreach (SlotSymbol r in additionalWin.Symbols)
            {
                this.symbols.Add(r);
            }
            
        }

        public WinData( List<SlotSymbol> symbols, ScatterPay scatterP, bool hasWild)
        {
            this.symbols = symbols;
            isWildWin = hasWild;
            if (SlotController.Instance.PlayFreeSpins && scatterP.enableFeatureMode)
            {
                this.freeSpins = scatterP.featureFreeSpins;
                this.pay = scatterP.featurePay;
                
                this.payMult = scatterP.featurePayMult;
            }
            else
            {
                this.freeSpins = scatterP.freeSpins;
                this.pay = scatterP.pay;
                this.payMult = scatterP.payMult;
                
            }


            this.lineBehaviourNum = -1;
            
            this.scatterP = scatterP;
            

        }
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public WinData(List<SlotSymbol> symbols, SpecialIconPay specialIconP, bool hasWild)
        {
            this.symbols = symbols;
            isWildWin = hasWild;
            if (SlotController.Instance.PlayFreeSpins && specialIconP.enableFeatureMode)
            {
                this.freeSpins = specialIconP.featureFreeSpins;
                this.pay = specialIconP.featurePay;
                
                this.payMult = specialIconP.featurePayMult;
                
            }
            else
            {
                this.freeSpins = specialIconP.freeSpins;
                this.pay = specialIconP.pay;
                this.payMult = specialIconP.payMult;
                
            }


            this.lineBehaviourNum = -1;

            this.SpecialIconP = specialIconP;


        }
        //>>>>>>>>>>>>>>>> (15/3/2021)(ARC)
        public WinData(List<SlotSymbol> symbols, WildPay wildP)
        {
            this.symbols = symbols;

            if (SlotController.Instance.PlayFreeSpins && wildP.enableFeatureMode)
            {
                this.freeSpins = wildP.featureFreeSpins;
                this.pay = wildP.featurePay;
                
                this.payMult = wildP.featurePayMult;
            }
            else
            {
                this.freeSpins = wildP.freeSpins;
                this.pay = wildP.pay;
                this.payMult = wildP.payMult;
                
            }

            this.lineBehaviourNum = -1;

            this.wildP = wildP;
        }

        public override string ToString()
        {
            return "Pay: " + pay + " ; FreeSpin: " + freeSpins + " ; PayMult: " + payMult + " ; TotalBetMult: " + totalBetMult;
        }

    }

    public class WinningBoxesDisplay
    {
        private RayCaster[] rayCasters;
        private List<GameObject> boxesGameObj = new List<GameObject>();
        private List<LineRenderer> listOfBoxesLine = new List<LineRenderer>();



        private bool useRaycastPos;
        private Vector2 boxSize;
        private Transform parentTrans;
        private Vector3[] overridePos;

        private bool isEnable;

        public WinningBoxesDisplay(RayCaster[] rayCasters, Transform parentTrans, Vector2 boxSize, LineRenderer lineRendererRef, Vector3[] overridePos = null)
        {
            this.rayCasters = rayCasters;
            if (overridePos == null)
            {
                useRaycastPos = true;
            }
            else
            {
                useRaycastPos = false;
                this.overridePos = overridePos;
            }

            this.parentTrans = parentTrans;
            this.boxSize = boxSize;

            ///foreach raycast create a box
            CreateBoxes(lineRendererRef);
            RepositionBoxes();
            ///disable on start
            foreach (LineRenderer r in listOfBoxesLine)
            {
                r.enabled = false;
            }
        }

        public List<LineRenderer> OnWin(List<RayCaster> listOfWinRayCaster)
        {
            List<LineRenderer> winningBoxes = new List<LineRenderer>();
            foreach (RayCaster r in listOfWinRayCaster)
            {
                for (int i = 0; i < rayCasters.Length; i++)
                {
                    if (r == rayCasters[i])
                    {

                        listOfBoxesLine[i].enabled = true;

                        winningBoxes.Add(listOfBoxesLine[i]);
                    }
                }
            }

            return winningBoxes;
        }



        private void CreateBoxes(LineRenderer lineRendererRef)
        {
            foreach (RayCaster r in rayCasters)
            {

                GameObject box = new GameObject();

                box.transform.localScale = parentTrans.lossyScale;
                box.transform.parent = parentTrans;




                LineRenderer lr = (LineRenderer)box.AddComponent<LineRenderer>();
                box.AddComponent<SpriteLine>();

                lr.colorGradient = lineRendererRef.colorGradient;
                lr.widthMultiplier = lineRendererRef.widthMultiplier;
                lr.materials = lineRendererRef.materials;
                lr.sortingLayerID = lineRendererRef.sortingLayerID;
                lr.sortingOrder = lineRendererRef.sortingOrder;

                lr.loop = true;

                listOfBoxesLine.Add(lr);

                boxesGameObj.Add(box);


            }
        }

        private void SetBoxPosition(LineRenderer lr, Vector3 pos)
        {

            lr.transform.position = pos;

            lr.positionCount = 5;
            lr.SetPositions(
                new[]
                {
                     new Vector3(pos.x , pos.y + boxSize.y / 2, pos.z),
                        new Vector3(pos.x + boxSize.x/2, pos.y + boxSize.y / 2, pos.z),
                        new Vector3(pos.x + boxSize.x/2, pos.y - boxSize.y / 2, pos.z),
                        new Vector3(pos.x - boxSize.x/2, pos.y - boxSize.y / 2, pos.z),
                        new Vector3(pos.x - boxSize.x/2, pos.y + boxSize.y / 2, pos.z),
                        

                        //new Vector3(pos.x + boxSize.x/2, pos.y + boxSize.y / 2, pos.z),
                        //new Vector3(pos.x + boxSize.x/2, pos.y - boxSize.y / 2, pos.z),
                        //new Vector3(pos.x - boxSize.x/2, pos.y - boxSize.y / 2, pos.z),
                        //new Vector3(pos.x - boxSize.x/2, pos.y + boxSize.y / 2, pos.z),
                        //new Vector3(pos.x + boxSize.x/2 + 0.5f*lr.widthMultiplier, pos.y + boxSize.y / 2, pos.z),
                }
                );
        }

        private void RepositionBoxes()
        {
            if (useRaycastPos)
            {
                for (int i = 0; i < listOfBoxesLine.Count; i++)
                {
                    Vector3 pos = rayCasters[i].transform.position;
                    SetBoxPosition(listOfBoxesLine[i], pos);

                }
            }
            else
            {
                for (int i = 0; i < listOfBoxesLine.Count; i++)
                {
                    Vector3 pos = rayCasters[i].transform.position;
                    if (i < listOfBoxesLine.Count)
                        SetBoxPosition(listOfBoxesLine[i], overridePos[i + 1]);

                }
            }

        }

        private void CreateLineBetweenBoxes()
        {

        }

    }
    public enum TypeOfWinningLine
    {
        dots,
        line,

    }

    public enum WinningBoxSequenceType
    {
        None,
        Sequence_With_Line,
        Alter_With_Line,
        Always_Display,

    }
}
