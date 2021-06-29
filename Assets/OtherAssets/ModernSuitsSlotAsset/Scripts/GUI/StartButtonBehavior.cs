using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using DarkTonic.MasterAudio;

namespace Mkey
{
    public enum SpinTypes { Single, Auto }
    
    
    public class StartButtonBehavior : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {

        //public Text autoText;
        private SpinTypes spinType = SpinTypes.Single;
        private SpinTypes SpinType
        {
            get { return spinType; }
            set { spinType = value; if (ChangeStateDelegate != null) ChangeStateDelegate(value == SpinTypes.Auto); /*SetButtonText();*/ }
        }
        //public StateControllerManager Scm;
        public Action<bool> ChangeStateDelegate;
        public Action ClickDelegate;
        bool up = true;
        float downTime = 0;

        public void OnPointerDown(PointerEventData eventData)
        {
            //MasterAudio.PlaySound("OnPointerDown");
            if (gameObject.name == "Auto")
            {
                StateControllerManager.ChangeState("AutoStop-Press");
            } else
            {
                StateControllerManager.ChangeState("Stop-Press");
            }
            up = false;
            //Debug.Log(gameObject.name + " Was down.");
            
            if (SpinType == SpinTypes.Auto)
            {
                SpinType = SpinTypes.Single;
                return;
            }
            downTime = Time.time;
            StartCoroutine(CheckAuto());
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020)
            MasterAudio.PlaySound("Spin_Clicking");
            //(Updated)>>>>>>>>>>>>>>>> (8/12/2020) 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //StateControllerManager.ChangeState("Unpressed");

            up = true;
          // Debug.Log(this.gameObject.name + " Was exit.");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
            if(gameObject.name == "Auto")
            {
                StateControllerManager.ChangeState("AutoStop-Disable");
            }

            up = true;
            //Debug.Log(gameObject.name + " Was up. SpinType: " + SpinType);
            ClickDelegate?.Invoke();
        }

        IEnumerator CheckAuto()
        {
            bool cancel = false;
            WaitForEndOfFrame wef = new WaitForEndOfFrame();
            float dTime;
            while (!up && !cancel)
            {
                dTime = Time.time - downTime;
                if (dTime > 2.0f)
                {
                    SpinType = SpinTypes.Auto;
                    cancel = true;
                }
                yield return wef;
            }
        }

        internal void ResetAuto()
        {
            SpinType = SpinTypes.Single;
        }

        /*private void SetButtonText()
        {
          if(autoText)  autoText.text = (SpinType == SpinTypes.Single) ? "Hold to AutoSpin" : "AUTO";
        }*/
    }
}