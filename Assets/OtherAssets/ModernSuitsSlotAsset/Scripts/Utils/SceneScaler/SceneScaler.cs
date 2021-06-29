using UnityEngine;

/* Scale scene objects according base resolution
    changes
    180119
    - add h-w adjust
    - add validate

 */
namespace Mkey
{
    [ExecuteInEditMode]
    public class SceneScaler : MonoBehaviour {
        [SerializeField]
        private int baseWidth = 2732;
        [SerializeField]
        private int baseHeight = 2048;

        private float baseRatio = 0.75f;
        [HideInInspector]
        [SerializeField]
        private float baseRatioOld = 0.75f;

        [Range(0,1)]
        [SerializeField]
        private float Height_Width = 1;
        [HideInInspector]
        [SerializeField]
        private float Height_WidthOld = 1;

        [SerializeField]
        private float additionalScale = 1;
        [HideInInspector]
        [SerializeField]
        private float additionalScaleOld = 1;

        private float currRatio;
        private int width = 0;
        private int height = 0;
        private float sc = 1f;
        private bool debug = false;

        #region regular
        void Start()
        {
            SetScale();
        }

        void Update()
        {
            SetScale();
        }
        #endregion regular

        void SetScale()
        {
            #region validate
            Height_Width = Mathf.Clamp01(Height_Width);
            baseHeight = Mathf.Abs(baseHeight);
            baseWidth = Mathf.Abs(baseWidth);
            baseHeight = Mathf.Max(1, baseHeight);
            baseWidth = Mathf.Max(1, baseWidth);
            additionalScale = Mathf.Clamp01(additionalScale);
            #endregion validate

            baseRatio = baseWidth / (float)baseHeight;
            if (width != Screen.width || height != Screen.height || baseRatio != baseRatioOld || Height_Width != Height_WidthOld || additionalScaleOld != additionalScale)
            {
                width = Screen.width;
                height = Screen.height;
                currRatio = width / (float)height;
                Height_WidthOld = Height_Width;
                additionalScaleOld = additionalScale;
                BaseScale();
            }
            if (debug) Debug.Log("width: " + width + " ; height: " + height + " ;baseW/baseH: " + baseRatio + "currW/currH: " + currRatio + " ;scale: " + sc);
        }

        void BaseScale()
        {
            sc = currRatio / baseRatio;
            sc = Mathf.Lerp(1, sc, Height_Width) * additionalScale;
            gameObject.transform.localScale = new Vector3(sc, sc, sc);
        }
    }
}