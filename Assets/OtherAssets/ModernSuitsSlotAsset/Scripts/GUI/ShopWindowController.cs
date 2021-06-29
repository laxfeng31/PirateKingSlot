using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class ShopWindowController : PopUpsController
    {
        public GameObject shopThingPrefab;
        public RectTransform ThingsParent;
        private List<ShopThingHelper> shopThings;

        void Start()
        {
            CreateThingTab();
        }

        public override void RefreshWindow()
        {
            base.RefreshWindow();
        }

        public void Cancel_Click()
        {
            //if (SoundMasterController.Instance) SoundMasterController.Instance.SoundPlayClick(0.0f, null);
            CloseButton_click();
            MasterAudio.PlaySoundAndForget("ClickSound");
            Debug.Log("ClickSound");
        }

        private void CreateThingTab()
        {
            ShopThingHelper[] sT = ThingsParent.GetComponentsInChildren<ShopThingHelper>();
            foreach (var item in sT)
            {
                DestroyImmediate(item.gameObject);
            }

            
        }

        private ShopThingHelper CreateThing(GameObject prefab, RectTransform parent, ShopThingData shopThingData)
        {
            GameObject shopThing = Instantiate(shopThingPrefab);
            shopThing.transform.localScale = ThingsParent.transform.lossyScale;
            shopThing.transform.SetParent(ThingsParent.transform);
            ShopThingHelper sC = shopThing.GetComponent<ShopThingHelper>();
            sC.SetData(shopThingData);
            return sC;
        }

    }
}