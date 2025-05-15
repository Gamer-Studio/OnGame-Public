using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnGame
{
    public class SettingUIManager : MonoBehaviour
    {
        [Header("설정 구성")]
        [SerializeField] private Dropdown resolution;
        
        [Header("컴포넌트 바인딩")]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider bgmVolume;
        [SerializeField] private Slider sfxVolume;
        
        private Resolution[] resolutions;

        public bool Fullscreen
        {
            set => Screen.fullScreen = value;
            get => Screen.fullScreen;
        }
        
        public void Start()
        {
            SetupSettings();
            LoadSliderSet();
        }

        private void SetupSettings()
        {
            Fullscreen = true;
            fullscreenToggle.isOn = Fullscreen;

            InitializeResolution();
            resolution.onValueChanged.AddListener(OnResolution);
        }

        
        private void InitializeResolution() // 가능한 해상도 불러오기
        {
            resolutions = Screen.resolutions;
            resolution.ClearOptions();
            
            if (resolutions.Length == 0) return;

            var options = new List<string>();
            var curResolutionIndex = 0;

            for (var i = 0; i < resolutions.Length; i++)
            {
                var option = $"{resolutions[i].width} x {resolutions[i].height}";
                if (!options.Contains(option)) //중복 제거
                {
                    options.Add(option);
                }

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    curResolutionIndex = i;
                }
            }

            resolution.AddOptions(options);
            resolution.value = curResolutionIndex;
            resolution.RefreshShownValue();
        }
        
        //설정 화면
        public void OnResolution(int index)
        {
            if (resolutions == null || resolutions.Length == 0) return;

            var selectedResolution = resolutions[index];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
            
            Debug.Log($"해상도 변경: {selectedResolution.width} x {selectedResolution.height}");
        }
        
        public void OnMasterVolumeChange(float value)
        {
            AudioManager.Instance.MasterVolume = value;
            Debug.Log($"마스터 볼륨: {value:F2}");
        }

        
        public void OnBGMVolumeChange(float value)
        {
            AudioManager.Instance.BGMVolume = value;
            Debug.Log($"배경음 볼륨: {value:F2}");
        }

        
        public void OnSFXVolumeChange(float value)
        {
            AudioManager.Instance.SFXVolume = value;
            Debug.Log($"효과음 볼륨: {value:F2}");
        }
        
        
        private void LoadSliderSet()
        {
            masterVolume.value = AudioManager.Instance.MasterVolume;
            bgmVolume.value = AudioManager.Instance.BGMVolume;
            sfxVolume.value = AudioManager.Instance.SFXVolume;
        }
    }
}
