using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OnGame
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("오디오 믹서")] 
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        [Header("오디오 소스")] 
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("사운드 데이터")] 
        [SerializeField] private List<SoundData> soundDataList;
        
        [SerializeField] private int sfxPoolSize = 10;

        [SerializeField] private SerializableDictionary<SoundType, AudioClip> soundDict;
        [SerializeField] private List<AudioSource> sfxSourcePool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                soundDict = new ();
                soundDataList = SoundData.LoadAllSoundData();
                foreach (var data in soundDataList)
                {
                    soundDict[data.soundType] = data.clip;
                }

                // SFX 풀 생성
                sfxSourcePool = new List<AudioSource>();
                for (var i = 0; i < sfxPoolSize; i++)
                {
                    var sourceObj = new GameObject($"SFXSource_{i}");
                    sourceObj.transform.SetParent(transform);
                    var source = sourceObj.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = sfxMixerGroup;
                    sfxSourcePool.Add(source);
                }

                LoadVolumeSet();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySFX(SoundType type)
        {
            if (!soundDict.ContainsKey(type))
            {
                return;
            }

            var clip = soundDict[type];
            var availableSource = sfxSourcePool.Find(source => !source.isPlaying);

            if (availableSource != null)
            {
                availableSource.PlayOneShot(clip);
            }
        }

        public void PlayBGM(SoundType type)
        {
            if (soundDict.ContainsKey(type))
            {
                bgmSource.Stop();
                bgmSource.clip = soundDict[type];
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }

        public float MasterVolume
        {
            get => audioMixer.GetFloat("MasterVolume", out var value) ? value : 0f;
            set
            {
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
                PlayerPrefs.SetFloat("MasterVolume", value);
            }
        }

        public float BGMVolume
        {
            get => audioMixer.GetFloat("BGMVolume", out var value) ? value : 0f;
            set
            {
                audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
                PlayerPrefs.SetFloat("BGMVolume", value);
            }
        }

        public float SFXVolume
        {
            get => audioMixer.GetFloat("SFXVolume", out var value) ? value : 0f;
            set
            {
                audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
                PlayerPrefs.SetFloat("SFXVolume", value);
            }
        }

        private void LoadVolumeSet()
        {
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
    }
}
