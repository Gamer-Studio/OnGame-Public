using System;
using OnGame.Contents.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OnGame.Worlds
{
  [CreateAssetMenu(fileName = "new StageData", menuName = "OnGame/StageData", order = 0)]
  public class StageData : ScriptableObject
  {
    public RoomData[] roomTable;
    public TileBase[] verticalDoor, sideDoor; 

    public RoomData startRoom;
    public RoomData endRoom;
    public RoomData bossRoom;

    public Material tilemapMaterial;

    // 현재는 스테이지의 최대 레이어 개수를 미리 지정해줘야함
    public int floorCount, structureCount;

    // 특수 레이어 인덱스 설정
    public int baseLayer, doorLayer = 2;

    // Serializable로 변경 예정
    public ItemData[] itemTable;

    // 스테이지에서 등장하는 방의 개수
    public int battleRoomCount = 3,
      eventRoomCount,
      shopRoomCount = 1;

    public int StageSize
      => (int)Math.Ceiling(Math.Sqrt(battleRoomCount + eventRoomCount + shopRoomCount));
  }
}