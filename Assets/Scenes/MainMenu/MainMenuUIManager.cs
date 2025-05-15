using OnGame.Worlds;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OnGame
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [Header("메인 화면")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject modeSelectPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [Header("메인 메뉴 구성")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button modeSelectButton;
        [SerializeField] private Button settingsButton;

        public void StartGame(StageData data)
        {
            GameManager.Instance.nextStageData = data;
            SceneManager.LoadScene("WorldScene");
        }
    }
}
