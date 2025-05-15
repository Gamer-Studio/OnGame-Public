using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace OnGame.Utils
{
    [ AddComponentMenu("Util/Scene Loader")]
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            LoadScene(sceneName, null);
        }

        public void LoadScene(string sceneName, object message)
        {
            // 다음 씬에 데이터를 전달하는 코드
            if (message != null)
            {
                void MessageHandler (Scene scene, LoadSceneMode mode)
                {
                    if (EventSystem.current is { } system)
                    {
                        system.SendMessage("OnMessage", message);
                    }
                    
                    SceneManager.sceneLoaded -= MessageHandler;
                };
            
                SceneManager.sceneLoaded += MessageHandler;
            }
            
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        [Obsolete("대신 void LoadScene(string sceneName) 을 사용하세요.")]
        public void LoadMainMenu() => LoadScene("MainMenuScene");

        [Obsolete("대신 void LoadScene(string sceneName) 을 사용하세요.")]
        public void LoadTestScene() => LoadScene("TestScene");

        [Obsolete("대신 void LoadScene(string sceneName) 을 사용하세요.")]
        public void LoadGameScene() => LoadScene("WorldScene");

        [Obsolete("대신 void LoadScene(string sceneName) 을 사용하세요.")]
        public void LoadIntroScene() => LoadScene("IntroScene");
    }
}
