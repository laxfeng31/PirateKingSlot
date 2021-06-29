using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script_Loading : MonoBehaviour
{

    [SerializeField]
    private Image loadingProgressBar;
    [SerializeField]
    private Text loadingText;
    private string sceneName;
    private float progressValue = 1.1f;
    public float progressValueMultiplier_1 = 0.4f;
    public float progressValueMultiplier_2 = 0.06f;
    // Start is called before the first frame update
    void Start()
    {
        sceneName = PlayerPrefs.GetString("current_Scene");
        Debug.Log(sceneName);
        LoadLevel();
    }

    // Update is called once per frame
    void Update()
    {
        Loading();
    }

    public void LoadLevel()
    {
        progressValue = 0f;

        //Time.timeScale = 0f;
    }

    private void Loading()
    {
        if(progressValue < 1f)
        {
            progressValue += progressValueMultiplier_1 * progressValueMultiplier_2;
            string processedText = Mathf.Round(((progressValue * 100) - 0.8f)).ToString();
            if (loadingText) loadingText.text = processedText+"%";
            if (loadingProgressBar) loadingProgressBar.fillAmount = progressValue;
            if (progressValue >= 1f)
            {
                progressValue = 1.1f;
                StartCoroutine("loadLobby");
            }
        }
    }

    IEnumerator loadLobby()
    {
        yield return new WaitForSeconds(1f);
        if (sceneName == "WongChoy"){
            SceneManager.LoadScene("Slot_3X5_WongChoy");
        }
        else if (sceneName == "TheDiscovery")
        {
            SceneManager.LoadScene("Slot_3X5_TheDiscovery");
        }
        else if (sceneName == "EgyptDream")
        {
            SceneManager.LoadScene("Slot_3X5_EgyptDream");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
        
    }
}
