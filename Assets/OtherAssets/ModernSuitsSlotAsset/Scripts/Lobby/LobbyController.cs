using Jacovone.AssetBundleMagic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

namespace Mkey {
    public class LobbyController : MonoBehaviour {

        private bool isMenuShow;
        public GameObject MenuGO;
        [SerializeField]
        GameObject downloadPanelGb;
        [SerializeField]
        GameObject downloadGO;
        [SerializeField]
        GameObject loadingGO;
        [SerializeField]
        Text downloadPercentText;
        [SerializeField]
        Image progressBarFill;
        [SerializeField]
        Text loadingPercentText;
        [SerializeField]
        Image loadingBarFill;
        bool isLoading;
        private float progress;
        private string definiteSceneName;
        public bool isMute;

        private void Start()
        {
            isMenuShow = false;
            isLoading = false;
            
            
        }

        void Update()
        {
            
            if (isLoading == true)
            {
                loadingScene();
            }
            

        }

        public void SceneLoad(string url)
        {
            
            MagicBundleLoad(url);
            
            //SceneManager.LoadScene(1);
        }

        public void setScene(string scenename)
        {
            PlayerPrefs.SetString("current_Scene", scenename);
        }

        public void Slider_Click()
        {
            GuiController.Instance.ShowShop();
        }

        public void toggleMenu()
        {
            Debug.Log("Clicked");
            if (!isMenuShow)
            {
                MenuGO.SetActive(true);
                isMenuShow = true;
            }
            else
            {
                MenuGO.SetActive(false);
                isMenuShow = false;
            }
        }
        
        public void MagicBundleLoad(string url)
        {
            if (AssetBundleMagic.Bundles.ContainsKey(url))
            {
                
                AssetBundle a = null;
                if (AssetBundleMagic.Bundles.TryGetValue(url, out a))
                {
                    if (a.isStreamedSceneAssetBundle)
                    {
                        string[] scenePaths = a.GetAllScenePaths();
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                        isLoading = true;
                        definiteSceneName = sceneName;
                    }
                }
                else
                {
                    Debug.Log("No Asset bundle");
                }
            }
            else
            {
                Debug.Log("Old");
                AssetBundleMagic.DownloadUpdatedBundle(url, delegate (AssetBundleMagic.Progress p)
                {
                    stopDownload = false;
                    StartCoroutine(DownloadProgrss(p));
                }, delegate (AssetBundle ab)
                {
                    if (ab.isStreamedSceneAssetBundle)
                    {
                        string[] scenePaths = ab.GetAllScenePaths();
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                        isLoading = true;
                        definiteSceneName = sceneName;
                    }
                }, delegate (string s) { Debug.Log(s); stopDownload = true; downloadPanelGb.SetActive(false); });
            }
            
            
        }

        bool stopDownload = false;

        IEnumerator DownloadProgrss(AssetBundleMagic.Progress p)
        {
            if (p != null)
            {
                
                downloadPanelGb.SetActive(true);
                downloadGO.SetActive(true);
                while (p.GetProgress()< 1f)
                {
                    progressBarFill.fillAmount = (p.GetProgress()/ 0.85f);
                    if ( p.GetProgress()/0.85f * 100 >= 100){
                        downloadPercentText.text = "100.00%";
                    }
                    else
                    {
                        downloadPercentText.text = ((p.GetProgress() / 0.85f) * 100).ToString("0.00") + "%";
                    }
                    if (stopDownload)
                    {
                        progressBarFill.fillAmount = 1;
                        downloadPercentText.text = 100 + "%";
                        break;
                    }
                    yield return null;
                }
                //downloadPanelGb.SetActive(false);

            }
            
        }

        private void loadingScene()
        {
            downloadGO.SetActive(false);
            loadingGO.SetActive(true);
            float progressValueMultiplier_1 = 0.4f;
            float progressValueMultiplier_2 = 0.06f;
            if (progress < 1f)
            {
                progress += progressValueMultiplier_1 * progressValueMultiplier_2;
                string processedText = Mathf.Round(((progress * 100) - 0.8f)).ToString();
                loadingPercentText.text = processedText + "%";
                loadingBarFill.fillAmount = progress;
                if (progress >= 1f)
                {
                    progress = 1.1f;
                    isLoading = false;
                    SceneManager.LoadScene(definiteSceneName);
                }
            }
        }

        private void loadingScene(string sceneName)
        {
            downloadGO.SetActive(false);
            loadingGO.SetActive(true);
            float progressValueMultiplier_1 = 0.4f;
            float progressValueMultiplier_2 = 0.06f;
            if (progress < 1f)
            {
                progress += progressValueMultiplier_1 * progressValueMultiplier_2;
                string processedText = Mathf.Round(((progress * 100) - 0.8f)).ToString();
                loadingPercentText.text = processedText + "%";
                loadingBarFill.fillAmount = progress;
                if (progress >= 1f)
                {
                    progress = 1.1f;
                    isLoading = false;
                    SceneManager.LoadScene(sceneName);
                }
            }
        }

        public void toggleSound()
        {
            if (!isMute)
            {
                isMute = true;
                MasterAudio.MuteEverything();
                StateControllerManager.ChangeState("Mute");
            }
            else if(isMute)
            {
                isMute = false;
                MasterAudio.UnmuteEverything();
                StateControllerManager.ChangeState("Unmute");
            }
        }

    }

}