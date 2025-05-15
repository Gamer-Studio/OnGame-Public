using OnGame.Worlds;
using UnityEngine;

namespace OnGame.Scenes.World
{
  public partial class WorldManager
  {
    public const int RoomSize = 20;

    [Header("Stage Generator")] [SerializeField]
    private Stage stage;
    [SerializeField] private Transform floorContainer, structureContainer;

    public Stage Stage => stage;

    // 테스트용 데이터
    [Header("Test data")]
    [SerializeField] private StageData testStageData;

    public void StartStage(StageData stageData)
    {
      if (stage != null) ResetWorld();

      stage = new Stage(stageData, floorContainer, structureContainer);

      // 시작 방 랜덤 생성

      // var startRoomPosition = new Vector2Int(Random.Range(0, stage.StageSize), Random.Range(0, stage.StageSize));
      var startRoomPosition = new Vector2Int(1, 1);
      stage.GenerateRoom(startRoomPosition);
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x, startRoomPosition.y - 1));
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x, startRoomPosition.y + 1));
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x + 1, startRoomPosition.y));
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x - 1, startRoomPosition.y));
      
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x - 1, startRoomPosition.y - 1));
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x + 1, startRoomPosition.y + 1));
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x - 1, startRoomPosition.y + 1));
      stage.GenerateRoom(new Vector2Int(startRoomPosition.x + 1, startRoomPosition.y - 1));
      player.transform.position = stage.GetRoomPosition(startRoomPosition);
    }

    /// <summary>
    /// 필요없는 레이어만 삭제하는 게 좋긴 하지만, 안전을 위해 그냥 이전 레이어는 모두 삭제합니다.
    /// </summary>
    private void ResetWorld()
    {
      for (var i = 0; i < floorContainer.childCount; i++) Destroy(floorContainer.GetChild(i).gameObject);

      for (var i = 0; i < structureContainer.childCount; i++) Destroy(structureContainer.GetChild(i).gameObject);
    }
  }
}