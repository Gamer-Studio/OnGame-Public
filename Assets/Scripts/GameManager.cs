using OnGame.Worlds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OnGame
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance { get; private set; }
    public UnityEvent onLoad;
    public StageData nextStageData = null;

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
      }
      else
      {
        Destroy(gameObject);
      }
    }

    public void Load()
    {
      onLoad.Invoke();
    }
  }
}