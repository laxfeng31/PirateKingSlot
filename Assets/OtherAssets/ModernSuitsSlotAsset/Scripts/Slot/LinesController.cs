using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
	public class LinesController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Destroy existing and create all possible lines at start using raycasters")]
        private bool createAllPossibleLines = false;
        [SerializeField]
        [Tooltip("Select all lines at start, or only first line")]
        private bool selectAllLines = true;
        private bool setLineVisible = false;
        private float burnDelay = 0;
        internal List<LineBehavior> lines;
        [SerializeField]
        public Sprite dotSprite;
        [SerializeField]
        public Material dotMaterial;
        [HideInInspector]
        public int dotSortingLayerID = 0; //next updates
        [SerializeField]
        public int dotSortingOrder;
        [SerializeField]
        public float dotDistance = 0.4f;

        [SerializeField]
        [Tooltip("Burn all selected lines at scene start, if createAllPossibleLines == true - not work")]
        private bool burnLinesAtStart = true;

        public int SelectedLinesCount
        {
            get; private set;
        }
		
        private SlotPlayer SP
        {
            get { return SlotPlayer.Instance; }
        }
		
        #region regular
        void Start()
        {
            SlotController sC = GetComponentInParent<SlotController>();
            if (createAllPossibleLines && sC)
            {
                // remove all existing lines
                lines = new List<LineBehavior>(GetComponentsInChildren<LineBehavior>());
                foreach (var item in lines)
                {
                    if (item)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
                lines = null;

                // create all possible lines
                List<int[]> rcCombos = new List<int[]>();
                SlotGroupBehavior[] sGB = sC.slotGroupsBeh;
             //   Debug.Log("sgb: " + sGB.Length);

                int[] rcCounts = new int[sGB.Length];// raycasters counts by reel
                for (int i = 0; i < sGB.Length; i++)
                {
                    rcCounts[i] = sGB[i].RayCasters.Length;
                 //   Debug.Log(" rcCounts[i]: " + rcCounts[i]);
                }

                rcCombos = CreateRCCombos(rcCounts);
               // Debug.Log("rcCombos: " + rcCombos.Count);
                RayCaster[] rcComb = new RayCaster[sGB.Length];
                for (int i = 0; i < rcCombos.Count; i++)
                {
                    int[] combo = rcCombos[i];
                    for (int j = 0; j < combo.Length; j++)
                    {
                        int rcNum = combo[j]-1;
                        rcComb[j] =(rcNum>=0) ? sGB[j].RayCasters[rcNum] : null;
                    }
                    CreateLine(rcComb, i+1);
                }
            }

            lines = new List<LineBehavior>(GetComponentsInChildren<LineBehavior>());

            // sort lines by number
            lines.Sort((LineBehavior a, LineBehavior b) =>
            {
                if (a == null & b == null) return 0;
                else if (a == null) return -1;
                else if (b == null) return 1;
                else
                {
                    return a.number.CompareTo(b.number);
                }
            });

            lines.ForEach((l) => { l.InitStart(this, createAllPossibleLines); });
            if (selectAllLines)
            {
                SelectAllLines(burnLinesAtStart);
            }
            else
            {
                SelectFirstLine(burnLinesAtStart);
            }
        }
        #endregion regular

        private void SelectFirstLine(bool burn)
        {
            if (lines == null || lines.Count == 0) return;
            if (lines[0] && !lines[0].IsSelected) lines[0].Select(setLineVisible, burn, burnDelay);
            if (lines.Count > 1) // deselect top lines
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    if (lines[i] && lines[i].IsSelected) lines[i].DeSelect();
                }
            }
            SelectedLinesCount = 1;
            if (SP) SP.SetSelectedLinesCount(SelectedLinesCount);
        }

        public void SelectAllLines(bool burn)
        {
            if (createAllPossibleLines && SelectedLinesCount > 0) return; //avoid max bet
            if (createAllPossibleLines ) burn = false; //not burn
            int selected = 0;
            if (lines == null || lines.Count == 0) return;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] && !lines[i].IsSelected) lines[i].Select(setLineVisible, burn, burnDelay);
                selected++;
            }
            SelectedLinesCount = selected;
			if(SP) SP.SetSelectedLinesCount(SelectedLinesCount);
        }

        private void SelectBottomLines(int number)
        {
            int selected = 0;
            for (int i = 0; i < number-1; i++)
            {
                if (lines[i] && !lines[i].IsSelected) lines[i].Select(setLineVisible, burnDelay);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] && lines[i].IsSelected) selected++;
            }
            SelectedLinesCount = selected;
            if (SP) SP.SetSelectedLinesCount(SelectedLinesCount);
        }

        private void DeSelectTopLines(int number)
        {
            int selected = 0;
            for (int i = number; i < lines.Count; i++)
            {
                if (lines[i] && lines[i].IsSelected) lines[i].DeSelect();
            }
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] && lines[i].IsSelected) selected++;
            }

            SelectedLinesCount = selected;
			if(SP) SP.SetSelectedLinesCount(SelectedLinesCount);
        }

        /// <summary>
        /// Add pay line
        /// </summary>
        internal void IncSelectedLines()
        {
            foreach (var item in lines)
            {
                if (!item.IsSelected)
                {
                    item.Select(setLineVisible, burnDelay);
                    SelectedLinesCount++;
                    break;
                }
            }
			if(SP) SP.SetSelectedLinesCount(SelectedLinesCount);
        }

        /// <summary>
        /// Remove pay line
        /// </summary>
        internal void DecSelectedLines()
        {
            if (SelectedLinesCount <= 1) return;
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].IsSelected)
                {
                    lines[i].DeSelect();
                    SelectedLinesCount--;
                    break;
                }
            }
			if(SP) SP.SetSelectedLinesCount(SelectedLinesCount);
        }

        internal void LineButton_Click(LineBehavior line)
        {
            if (line.IsSelected)
            {
               if(line.number!=1) line.DeSelect();
                DeSelectTopLines(line.number);
            }
            else
            {
                line.Select(false, 0);
                SelectBottomLines(line.number);
            }
        }

        private void CreateLine(RayCaster [] raycasters, int number)
        {
            GameObject l = new GameObject();
            LineBehavior lB = l.AddComponent<LineBehavior>();
            l.transform.parent = transform.transform;
            lB.rayCasters = new RayCaster[raycasters.Length];
            for (int i = 0; i < raycasters.Length; i++)
            {
                lB.rayCasters[i] = raycasters[i];
            }
            lB.name = "Line " + number;
            lB.number = number;
          //  lB.SetmaterialProperty(dotSprite, dotMaterial, dotSortingOrder, dotDistance);
        }

        /// <summary>
        /// Return all possible rc combos by rc number (from 1 to rc.length)
        /// </summary>
        /// <param name="counts"></param>
        /// <returns></returns>
        private List<int[]> CreateRCCombos(int[] counts)
        {
            List<int[]> res = new List<int[]>();
            int length = counts.Length;
            int decLength = length-1;
            int[] counter = new int[length];
            for (int i = decLength; i >= 0; i--)
            {
                counter[i] = (counts[i] > 0) ? 1 : 0; // 0 - empty 
            }
            int[] copy = new int[length];//copy arr
            counter.CopyTo(copy, 0);
            res.Add(copy);

            bool result = true;
            while (result)
            {
                result = false;
                for (int i = decLength; i >= 0; i--)    // find new combo
                {
                    if (counter[i] < counts[i] && counter[i]>0) 
                    {
                        counter[i]++;
                        if (i != decLength) // reset low "bytes"
                        {
                            for (int j = i + 1; j < length; j++)
                            {
                               if(counter[j] > 0) counter[j] = 1;
                            }
                        }
                        result = true;
                        copy = new int[length];//copy arr
                        counter.CopyTo(copy, 0);
                        res.Add(copy);
                        break;
                    }
                }
            }
            return res;
        }
    }
}
