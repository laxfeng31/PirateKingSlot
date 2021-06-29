//using UnityEngine;
//using System.Collections;
//using System;
//using System.Collections.Generic;

///*
//   021218 
//   add stopallclip

//    160119
//    set clips as private
//    add StopAllClip(bool stopBackgroundMusik)

//    240119
//    add   public void SetNewBackGroundClipAndPlay(AudioClip newBackGroundClip)
//    fix  void PlayBkgMusik(bool play)
//            // set clip if failed
//            if (aSbkg && !aSbkg.clip && bkgMusic) aSbkg.clip = bkgMusic;
//*/

//namespace Mkey
//{
//    public class SoundMasterController : MonoBehaviour
//    {
//        [Space(8, order = 0)]
//        [Header("Default Sound Settings", order = 1)]
//        private bool soundOn = true;
//        private bool musicOn = true;
//        private float volume = 1.0f;
//        public static SoundMasterController Instance;
//        [Space(8, order = 0)]
//        [Header("Audio Sources", order = 1)]
//        [SerializeField]
//        private AudioSource aSclick;
//        [SerializeField]
//        private AudioSource aSbkg;
//        [SerializeField]
//        private AudioSource aSloop;

//        [Space(8, order = 0)]
//        [Header("AudioClips", order = 1)]
//        [SerializeField]
//        private AudioClip menuClick;
//        [SerializeField]
//        private AudioClip menuPopup;
//        [SerializeField]
//        private AudioClip menuCheck;
//        [SerializeField]
//        private AudioClip screenChange;
//        [SerializeField]
//        private AudioClip bkgMusic;
//        [SerializeField]
//        private AudioClip winCoins;
//        [SerializeField]
//        private AudioClip slotRotation;
//        [SerializeField]
//        private AudioClip slotLoose;
//        [SerializeField]
//        private AudioClip bigWin;
//        [HideInInspector]
//        [SerializeField]
//        private AudioClip reelStop;

//        [SerializeField]
//        private AudioClip miniJackPot;
//        [SerializeField]
//        private AudioClip maxiJackPot;
//        [SerializeField]
//        private AudioClip megaJackPot;

//        [SerializeField]
//        private AudioClip winFreeSpin;

//        WaitForEndOfFrame wff;
//        WaitForSeconds wfs0_1;

//        public SlotPlayer player
//        {
//            get { return SlotPlayer.Instance; }
//        }

//        private string saveNameSound = "mk_soundon";
//        private string saveNameMusic = "mk_musicon";
//        private string saveNameVolume = "mk_volume";

//        public bool SoundOn
//        {
//            get
//            {
//                return soundOn;
//            }
//            set
//            {
//                soundOn = value;
//                if (player && player.SaveData) PlayerPrefs.SetInt(saveNameSound, (soundOn) ? 1 : 0);
//            }
//        }

//        public bool MusicOn
//        {
//            get
//            {
//                return musicOn;
//            }
//            set
//            {
//                bool upd = (musicOn != value);
//                musicOn = value;
//                if (player && player.SaveData) PlayerPrefs.SetInt(saveNameMusic, (musicOn) ? 1 : 0);
//                if (upd) UpdateLevelBkgMusik();
//            }
//        }

//        public float Volume
//        {
//            get
//            {
//                return volume;
//            }
//            set
//            {
//                volume = Mathf.Clamp(value, 0, 1);
//                if (player && player.SaveData) PlayerPrefs.SetFloat(saveNameVolume, volume);
//                ApplyVolume();
//            }
//        }

//        public List<AudioSource> tempAudioSources;
//        private int audioSoucesMaxCount = 5;

//        void Awake()
//        {

//            if (Instance == null)
//            {
//                Instance = this;
//            }
//            else
//            {
//                Destroy(gameObject);
//                return;
//            }
//            wff = new WaitForEndOfFrame();
//            wfs0_1 = new WaitForSeconds(0.1f);
//        }

//        void Start()
//        {
//            if (player && player.SaveData)
//            {
//                if (!PlayerPrefs.HasKey(saveNameSound))
//                {
//                    PlayerPrefs.SetInt(saveNameSound, (soundOn) ? 1 : 0);
//                }
//                soundOn = (PlayerPrefs.GetInt(saveNameSound) > 0) ? true : false;

//                if (!PlayerPrefs.HasKey(saveNameMusic))
//                {
//                    PlayerPrefs.SetInt(saveNameMusic, (musicOn) ? 1 : 0);
//                }
//                musicOn = (PlayerPrefs.GetInt(saveNameMusic) > 0) ? true : false;

//                if (!PlayerPrefs.HasKey(saveNameVolume))
//                {
//                    PlayerPrefs.SetFloat(saveNameVolume, 1.0f);
//                }
//                volume = PlayerPrefs.GetFloat(saveNameVolume);
//            }

//            tempAudioSources = new List<AudioSource>();
//            PlayBkgMusik(musicOn);
//            ApplyVolume();
//        }

//        #region play sounds

//        public void SoundPlayClipAtPos(float playDelay, AudioClip aC, Action callBack, Vector3 pos, float volumeK)
//        {
//            StartCoroutine(PlayClipAtPoint(playDelay, aC, callBack, pos, volumeK));
//        }

//        public void SoundPlayClick(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, menuClick, callBack));
//        }

//        public void SoundPlayPopUp(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, menuPopup, callBack));
//        }

//        public void SoundPlayCheck(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, menuCheck, callBack));
//        }

//        public void SoundPlayScreenChange(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, screenChange, callBack));
//        }

//        public void SoundPlayWinCoins(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, winCoins, callBack));
//        }

//        public void SoundPlayWinFreeSpin(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, winFreeSpin, callBack));
//        }

//        public void SoundPlayMiniJackPot(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, miniJackPot, callBack));
//        }

//        public void SoundPlayMaxiJackPot(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, maxiJackPot, callBack));
//        }

//        public void SoundPlayMegaJackPot(float playDelay, Action callBack)
//        {
//            StartCoroutine(PlayClip(playDelay, megaJackPot, callBack));
//        }


//        public void SoundPlayRotation(float playDelay, bool loop, Action callBack)
//        {
//            StartCoroutine(PlayLoopClip(playDelay, loop, slotRotation, callBack));
            
//        }

//        public void SoundPlaySlotLoose(float playDelay, bool loop, Action callBack)
//        {
//            StartCoroutine(PlayLoopClip(playDelay, loop, slotLoose, callBack));
//        }

//        public void SoundPlayBigWin(float playDelay, bool loop, Action callBack)
//        {
//            StartCoroutine(PlayLoopClip(playDelay, loop, bigWin, callBack));
//        }

//        public void SoundPlayReelStop(float playDelay, bool loop, Action callBack)
//        {
//            StartCoroutine(PlayLoopClip(playDelay, loop, reelStop, callBack));
//        }

//        /// <summary>
//        /// play clip using inner audiosource - asClick
//        /// </summary>
//        /// <param name="playDelay"></param>
//        /// <param name="aC"></param>
//        /// <param name="callBack"></param>
//        /// <returns></returns>
//        IEnumerator PlayClip(float playDelay, AudioClip aC, Action callBack)
//        {
//            if (soundOn)
//            {
//                if (!aSclick) aSclick = GetComponent<AudioSource>();
//                float delay = 0f;
//                while (delay < playDelay)
//                {
//                    delay += Time.deltaTime;
//                    yield return wff;
//                }

//                if (aSclick && aC)
//                {
//                    aSclick.clip = aC;
//                    aSclick.Play();
//                }

//                while (aSclick.isPlaying)
//                    yield return wff;
//                if (callBack != null)
//                {
//                    callBack();
//                }
//            }
//        }

//        /// <summary>
//        /// play loop clip using inner audiosource - asLoop
//        /// </summary>
//        /// <param name="playDelay"></param>
//        /// <param name="loop"></param>
//        /// <param name="aC"></param>
//        /// <param name="callBack"></param>
//        /// <returns></returns>
//        IEnumerator PlayLoopClip(float playDelay, bool loop, AudioClip aC, Action callBack)
//        {
//            if (soundOn)
//            {
//                if (!aSloop) aSloop = GetComponent<AudioSource>();
//                float delay = 0f;
//                while (delay < playDelay)
//                {
//                    delay += Time.deltaTime;
//                    yield return wff;
//                }

//                if (aSloop && aC)
//                {
//                    aSloop.clip = aC;
//                    aSloop.loop = loop;
//                    aSloop.Play();
//                }
//                while (aSloop.isPlaying)
//                    yield return wff;
//                if (callBack != null)
//                {
//                    callBack();
//                }
//            }
//        }

//        /// <summary>
//        /// play loop clip using inner audiosource - asLoop
//        /// </summary>
//        /// <param name="playDelay"></param>
//        /// <param name="loop"></param>
//        /// <param name="aC"></param>
//        /// <param name="callBack"></param>
//        /// <returns></returns>
//        public IEnumerator PlayLoopClip(float playDelay, bool loop, AudioSource aSource, AudioClip aC, Action callBack)
//        {
//            if (soundOn && aSource && aC)
//            {
//                float delay = 0f;
//                while (delay < playDelay)
//                {
//                    delay += Time.deltaTime;
//                    yield return wff;
//                }

//                aSource.clip = aC;
//                aSource.loop = loop;
//                aSource.Play();

//                while (aSource.isPlaying)
//                    yield return wff;
//                if (callBack != null)
//                {
//                    callBack();
//                }
//            }
//        }

//        /// <summary>
//        /// Stop loop clip inner audiosource 
//        /// </summary>
//        public void StopLoopClip()
//        {
//            if (aSloop)
//            {
//                aSloop.Stop();
//            }

//        }

//        /// <summary>
//        /// Play or stop background musik
//        /// </summary>
//        /// <param name="play"></param>
//        public void PlayBkgMusik(bool play)
//        {
//            // set clip if failed
//            if (aSbkg && !aSbkg.clip && bkgMusic) aSbkg.clip = bkgMusic;

//            if (play && aSbkg && !aSbkg.isPlaying)
//            {
//                aSbkg.volume = 0;
//                aSbkg.Play();
//                SimpleTween.Value(gameObject, 0.0f, 0.45f, 3.5f).SetOnUpdate((float val) => { aSbkg.volume = val; }).
//                    AddCompleteCallBack(() => { });
//            }

//            else if (!play && aSbkg)
//            {
//                SimpleTween.Value(gameObject, 0.45f, 0.0f, 2f).SetOnUpdate((float val) => { aSbkg.volume = val; }).
//                      AddCompleteCallBack(() => { aSbkg.Stop(); });

//            }
//        }

//        IEnumerator PlayClipAtPoint(float playDelay, AudioClip aC, Action callBack, Vector3 pos, float volumeK)
//        {
//            if (soundOn && tempAudioSources.Count < audioSoucesMaxCount)
//            {
//                AudioSource aSt = CreateASAtPos(pos);
//                tempAudioSources.Add(aSt);
//                aSt.volume = Volume * volumeK;

//                float delay = 0f;
//                while (delay < playDelay)
//                {
//                    delay += Time.deltaTime;
//                    yield return wff;
//                }

//                if (aC)
//                {
//                    aSt.clip = aC;
//                    aSt.Play();
//                }

//                while (aSt && aSt.isPlaying)
//                    yield return wff;

//                tempAudioSources.Remove(aSt);
//                if (aSt) Destroy(aSt.gameObject);
//                if (callBack != null)
//                {
//                    callBack();
//                }
//            }
//        }

//        /// <summary>
//        /// Stop all clips with backround musik
//        /// </summary>
//        internal void StopAllClip()
//        {
//            if (aSloop) aSloop.Stop();
//            if (aSclick) aSclick.Stop();
//            if (aSbkg) aSbkg.Stop();
//            if (tempAudioSources != null)
//            {
//                foreach (var item in tempAudioSources)
//                {
//                    item.Stop();
//                }
//            }
//        }

//        /// <summary>
//        /// Stop all clips with or without backround musik
//        /// </summary>
//        /// <param name="stopBackgroundMusik"></param>
//        internal void StopAllClip(bool stopBackgroundMusik)
//        {
//            if (aSloop) aSloop.Stop();
//            if (aSclick) aSclick.Stop();
//            if (aSbkg && stopBackgroundMusik) aSbkg.Stop();
//            if (tempAudioSources != null)
//            {
//                foreach (var item in tempAudioSources)
//                {
//                    item.Stop();
//                }
//            }
//        }

//        private AudioSource CreateASAtPos(Vector3 pos)
//        {
//            GameObject aS = new GameObject();
//            aS.transform.position = pos;
//            return aS.AddComponent<AudioSource>();
//        }

//        private void UpdateLevelBkgMusik()
//        {
//            bool play = musicOn;

//            AudioClip nClip = bkgMusic;
//            if (nClip != aSbkg.clip)
//            {
//                aSbkg.Stop();
//                aSbkg.Play();
//                aSbkg.clip = nClip;
//            }

//            SimpleTween.Cancel(gameObject, true);

//            if (play && aSbkg && !aSbkg.isPlaying)
//            {
//                aSbkg.volume = 0;
//                aSbkg.Play();
//                SimpleTween.Value(gameObject, 0.0f, volume, 3.5f).SetOnUpdate((float val) => { aSbkg.volume = val; }).
//                       AddCompleteCallBack(() => { });
//            }

//            else if (!play && aSbkg && aSbkg.isPlaying)
//            {
//                SimpleTween.Value(gameObject, volume, 0.0f, 2f).SetOnUpdate((float val) => { aSbkg.volume = val; }).
//                      AddCompleteCallBack(() => { aSbkg.Stop(); });
//            }
//        }

//        #endregion play sounds

//        private void ApplyVolume()
//        {
//            if (aSclick)
//            {
//                aSclick.volume = Volume;
//            }

//            if (aSbkg)
//            {
//                aSbkg.volume = Volume;
//            }

//            if (aSloop)
//            {
//                aSloop.volume = Volume;
//            }

//            if (tempAudioSources.Count > 0)
//            {
//                tempAudioSources.ForEach((ast) => { ast.volume = Volume; });
//            }
//        }

//        public void SetNewBackGroundClipAndPlay(AudioClip newBackGroundClip)
//        {
//            bkgMusic = newBackGroundClip;
//            UpdateLevelBkgMusik();
//        }

//    }
//}