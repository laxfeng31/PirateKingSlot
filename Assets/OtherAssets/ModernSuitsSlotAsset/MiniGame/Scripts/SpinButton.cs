using UnityEngine;
using UnityEngine.UI;
using Mkey;

namespace MkeyFW
{
    public class SpinButton : MonoBehaviour
    {
        [SerializeField]
        private Button.ButtonClickedEvent clickEvent;
        public bool interactable = true;
        private Collider2D bCollider;
        private Collider2D[] hitList;
        private SpriteRenderer sR;

        #region regular
        void Start()
        {
            bCollider = GetComponent<Collider2D>();
            sR = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (!bCollider) return;
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Vector3 wPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    hitList = Physics2D.OverlapPointAll(new Vector2(wPos.x, wPos.y));
                    if (hitList.Length > 0)
                    {
                        for (int i = 0; i < hitList.Length; i++)
                        {
                            if (hitList[i] == bCollider)
                            {
                                OnClickEvent();
                            }
                        }

                    }
                }
            }
#if !UNITY_ANDROID && !UNITY_IOS
            else if (Input.GetMouseButtonDown(0))
            {
                Vector3 wPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hitList = Physics2D.OverlapPointAll(new Vector2(wPos.x, wPos.y));
                if (hitList.Length > 0)
                {
                    for (int i = 0; i < hitList.Length; i++)
                    {
                        if (hitList[i] == bCollider)
                        {
                            OnClickEvent();
                        }
                    }
                }
            }
#endif
        }
        #endregion regular

        /// <summary>
        /// Raise click event
        /// </summary>
        private void OnClickEvent()
        {
            if (!interactable) return;
            if (clickEvent != null) clickEvent.Invoke();
            SimpleTween.Value(gameObject, -0.2f, 0.2f, 0.3f)
                .SetOnUpdate((float val)=>
                {
                    if(this && sR)
                    {
                        if (val < 0)
                            val = -(val + 0.2f);
                        else
                            val = val - 0.2f;
                        sR.color = new Color(1 + val, 1 + val, 1 + val, 1);
                    }
                })
                .AddCompleteCallBack(()=> { if (this && sR) sR.color = new Color(1, 1, 1, 1); });
        }
    }
}