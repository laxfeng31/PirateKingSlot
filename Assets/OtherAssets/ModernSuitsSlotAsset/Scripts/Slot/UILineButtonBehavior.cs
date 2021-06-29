using UnityEngine;
using System;
using UnityEngine.UI;

namespace Mkey
{
    public class UILineButtonBehavior : MonoBehaviour
    {
        
        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite pressedSprite;
        public Button button;
        private Image thisImage;
        public Action PointerDownEvent;

        public Sprite PressedSprite { get => pressedSprite; set => pressedSprite = value; }
        public Sprite NormalSprite { get => normalSprite; set => normalSprite = value; }

        #region regular
        void Start()
        {
            thisImage = GetComponent<Image>();
        }
        #endregion regular

        internal void Refresh(bool lineSelected)
        {
            if (thisImage) thisImage.sprite = (lineSelected) ? PressedSprite : NormalSprite;
        }

    }
}

