using UnityEngine;

namespace OnGame.Scenes.World
{
  public partial class WorldManager : MonoBehaviour
  {
    public static WorldManager Instance { get; private set; }
    [Header("WorldManager")] public Player player;

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
      }
      else
        Destroy(gameObject);
    }

    private void Start()
    {
      var data = GameManager.Instance.nextStageData;
      if (data != null)
      {
        StartStage(data);
      }
    }
  }
}