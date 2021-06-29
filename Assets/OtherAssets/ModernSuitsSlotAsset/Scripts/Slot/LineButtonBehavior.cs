using UnityEngine;
using System;
using UnityEngine.UI;

namespace Mkey
{
    public class LineButtonBehavior : MonoBehaviour, ICustomMessageTarget
    {
        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite pressedSprite;
        private SpriteRenderer spriteRenderer;
        public Action PointerDownEvent;
        [SerializeField]
        private Image ImageRenderer;
        [SerializeField]
        private Image ImageRenderer2;
        [SerializeField]
        private Sprite normalSprite2;
        [SerializeField]
        private Sprite pressedSprite2;
        [SerializeField]
        private bool is2D;
        #region regular
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        #endregion regular

        internal void Refresh(bool lineSelected)
        {
            if (!is2D)
            {
                if (spriteRenderer) spriteRenderer.sprite = (lineSelected) ? pressedSprite : normalSprite;
            }
            else
            {
                if (ImageRenderer) ImageRenderer.sprite = (lineSelected) ? pressedSprite : normalSprite;
                if (ImageRenderer2) ImageRenderer2.sprite = (lineSelected) ? pressedSprite2 : normalSprite2;
            }
        }

        #region touch callbacks
        public void PointerUp(TouchPadEventArgs tpea)
        {

        }
        public void PointerDown(TouchPadEventArgs tpea)
        {
            if (PointerDownEvent != null) PointerDownEvent();
        }
        public void DragBegin(TouchPadEventArgs tpea) { }
        public void DragEnter(TouchPadEventArgs tpea) { }
        public void DragExit(TouchPadEventArgs tpea) { }
        public void DragDrop(TouchPadEventArgs tpea) { }
        public void Drag(TouchPadEventArgs tpea) { }
        public GameObject GetDataIcon()
        {
            return null;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        #endregion touch callbacks
    }
}

