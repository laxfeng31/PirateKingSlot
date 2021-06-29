using UnityEngine;
using MkeyFW;
using System.Collections;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

namespace Mkey
{
    public class SlotEvents : MonoBehaviour
    {

        public MiniGameInstantiator Instantiator;
        public bool autoStartMiniGame = true;
        public bool isBonus;
        public Animator bonusAnimation;
        public GameObject bonusAnimationObject;

        public static SlotEvents Instance;

        public bool MiniGameStarted { get { return (Instantiator && Instantiator.MiniGame); } }

        private void Awake()
        {
            Instance = this;
            if (isBonus)
            {
                PlayerPrefs.SetString("bonus", "Yes");
            } else
            {
                PlayerPrefs.SetString("bonus", "No");
            }
        }

        public void Bonus5()
        {
            Debug.Log("-------------- Bonus 5 win --------------------");
        }

        public void StartFortuneWheel()
        {
            Debug.Log("-------------- mini game start --------------------");
            Instantiator?.Create(autoStartMiniGame);
        }

        public void Scatter_5()
        {
            Debug.Log("-------------- Scatter 5 win --------------------");
            SlotPlayer.Instance.AddLevelProgress(100f);
            
            
            
        }

        public void Scatter_3()
        {
            Debug.Log("-------------- Scatter 3 win --------------------");
            SlotPlayer.Instance.AddLevelProgress(100f);
        }

        public void LineEventAAA()
        {
            Debug.Log("-------------- AAA win --------------------");
        }

        public void goMiniGame()
        {
            StartCoroutine("runMiniGameAnimation");
        }

        IEnumerator runMiniGameAnimation()
        {
            bonusAnimationObject.SetActive(true);
            bonusAnimation.Play("bonus");

            yield return new WaitForSeconds(2.2f);

            SceneManager.LoadScene(4);
            //SceneLoader.Instance.LoadScene(4);

        }
    }
}