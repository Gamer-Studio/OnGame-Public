using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace OnGame
{
    public class SoundTest : MonoBehaviour
    {
        public void PlayMainBGM()
        {
            AudioManager.Instance.PlayBGM(SoundType.MainBGM);
            Debug.Log("메인");
        }

        public void PlayBossBGM()
        {
            AudioManager.Instance.PlayBGM(SoundType.BossBGM);
            Debug.Log("보스");
        }

        public void PlayAttackSFX()
        {
            AudioManager.Instance.PlaySFX(SoundType.Attack);
            Debug.Log("공격");
        }

        public void PlayHitSFX()
        {
            AudioManager.Instance.PlaySFX(SoundType.Hit);
            Debug.Log("피격");
        }
        public void PlayMainScenes()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
