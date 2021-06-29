using UnityEngine;
using System.Collections.Generic;
using System;
using DarkTonic.MasterAudio;

namespace Mkey
{
    public class PayTableController : PopUpsController
    {
        [SerializeField]
        private GameObject[] tabs;
        private int currTabIndex = 0;

        [Space(10)]
        [SerializeField]
        private PayLineHelper payLineHelperPrefab;
        [SerializeField]
        private RectTransform payLinesGridParent;

        [SerializeField]
        private bool createPayLinesAuto = true;

        void Start()
        {
           if(createPayLinesAuto) CreatePayLinesInfo();
            SetActiveTab(currTabIndex);
        }

        void CreatePayLinesInfo()
        {
            LineBehavior[] lines =  FindObjectsOfType<LineBehavior>();

            Array.Sort(lines, delegate (LineBehavior x, LineBehavior y) // sort by name  x==y ->0; x>y ->1; x<y -1
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;// If x is null and y is null, they'reequal. 
                    }
                    else
                    {
                        return -1;// If x is null and y is not null, y is greater.
                    }
                }
                else
                {
                    return x.name.CompareTo(y.name);
                }
            });

            foreach (var item in lines)
            {
               PayLineHelper pLH = Instantiate(payLineHelperPrefab); //Debug.Log(item.ToString());
               pLH.Create(item, SlotController.Instance.slotGroupsBeh);
               RectTransform pLHRT = pLH.GetComponent<RectTransform>();
               pLHRT.localScale = payLinesGridParent.lossyScale;
               pLHRT.SetParent(payLinesGridParent);
            }
        }

        public void Cancel_Click()
        {
            //if (SoundMasterController.Instance) SoundMasterController.Instance.SoundPlayClick(0.0f, null);

            CloseButton_click();
            MasterAudio.PlaySoundAndForget("ClickSound");
            Debug.Log("ClickSound");
        }

        public override void RefreshWindow()
        {
            base.RefreshWindow();
        }

        private void GetChilds(GameObject g, ref List<GameObject> gList)
        {
            int childs = g.transform.childCount;
            if (childs > 0)//The condition that limites the method for calling itself
                for (int i = 0; i < childs; i++)
                {
                    Transform gT = g.transform.GetChild(i);
                    GameObject gC = gT.gameObject;
                    if (gC) gList.Add(gC);
                    GetChilds(gT.gameObject, ref gList);
                }
        }

        public void NextTab_Click()
        {
            currTabIndex =(int) Mathf.Repeat(++currTabIndex, tabs.Length);
            SetActiveTab(currTabIndex);
        }

        public void PrevTab_Click()
        {
            currTabIndex = (int)Mathf.Repeat(--currTabIndex, tabs.Length);
            SetActiveTab(currTabIndex);
        }

        private void SetActiveTab(int index)
        {
            if (tabs == null || tabs.Length == 0) return;
            if (index < 0) index = 0;
            if (index >= tabs.Length) index = tabs.Length - 1;
            for (int i = 0; i <tabs.Length; i++)
            {
              if(tabs[i]) tabs[i].SetActive(i==index);
            }
        }
    }
}