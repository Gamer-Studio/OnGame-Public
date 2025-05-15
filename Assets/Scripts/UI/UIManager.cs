using UnityEngine;

namespace OnGame.UI
{
    public enum UIState
    {
        Default, // 게임 진행
        Dialogue, // 대화창
        InfoWindow, // tab 상태창
        Setting,
        GameOver// esc 설정창
    }
    
    public partial class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        
        public GameUI gameUI;
        public DialogueUI dialogueUI;
        private UIState currentState = UIState.Default;
        
        
        public UIState CurrentState
        {
            get => currentState;
            set
            {
                if (currentState == value) return;

                currentState = value;
                OnUIStateChanged(currentState);
            }
        }
        
        
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
        
        
        private void OnUIStateChanged(UIState newState)
        {
            switch (newState)
            {
                case UIState.Default:
                    HideSettingPanel();
                    ShowTopUI();
                    ShowBottomUI();
                    // 나머지 추가
                    break;

                case UIState.Dialogue:
                    HideBottomUI();
                    // 나머지 추가
                    break;
                
                case UIState.GameOver:
                    HideTopUI();
                    HideBottomUI();
                    HideDialogueUI();
                    HideSettingPanel();
                    ShowGameOverPanel();
                    break;
                
                case UIState.Setting:
                    ShowSettingPanel();
                    break;

                // TODO : InfoWindow, Setting
            }
        }
    }
}
