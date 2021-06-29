
using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterAudioController : MonoBehaviour
{
   
    public bool muteSFX;
    [Range(0,1)]
    public float volumeSFX = 1;
    public bool muteBackground;
    [Range(0, 1)]
    public float volumeBackground = 1;
    void Start()
    {
        //UISetting.musicToggledEvent += SetAllMusicVolume;
        //UISetting.soundToggledEvent += SetAllSoundVolume;
    }

    

    

    private void OnDestroy()
    {
        //UISetting.musicToggledEvent -= SetAllMusicVolume;
        //UISetting.soundToggledEvent -= SetAllSoundVolume;
    }

    /// <summary>
    /// 設定所有聲音音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SetAllAudioVolume(float volume)
    {
        Debug.Log("Stop All Music!");
        MasterAudio.SetBusVolumeByName("SFX", volumeSFX);
        MasterAudio.SetBusVolumeByName("Background", volumeBackground);
    }

    /// <summary>
    /// 設定所有音樂音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SetAllMusicVolume(bool isOpen)
    {
        int isOpenInt = isOpen ? 1 : 0;
        muteBackground = isOpen;
        if (muteBackground)
        {
            MasterAudio.MuteBus("Background");
        }
        else
        {
            MasterAudio.UnmuteBus("Background");
        }

    }

    /// <summary>
    /// 設定所有音效音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SetAllSoundVolume(bool isOpen)
    {
        int isOpenInt = isOpen ? 1 : 0;
        muteSFX = isOpen;
        if (muteSFX)
        {
            MasterAudio.MuteBus("SFX");
        }
        else
        {
            MasterAudio.UnmuteBus("SFX");
        }
    }
}
