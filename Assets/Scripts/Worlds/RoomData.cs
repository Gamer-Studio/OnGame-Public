using System;
using OnGame.Contents.Items;
using OnGame.Scenes.World;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OnGame.Worlds
{
  [CreateAssetMenu(fileName = "new RoomData", menuName = "OnGame/Room Data", order = 0)]
  public class RoomData : ScriptableObject
  {
    private static int RoomSize => WorldManager.RoomSize;
    public RoomType type;

    // 20 * 20 * layerCount 크기의 배열
    // 직접 설정하면 댕노가다니 프리팹에 자동 생성 스크립트 붙여놔서 버튼 딸-깍으로 맵을 생성할 수 있습니다.
    public TileBase[] layers;
    public Color[] mapColors;
    public int floorLayerCount, structureLayerCount;

    // 방에 고정적으로 생성되는 엔티티
    // 0~3번은 Top, Down, Left, Right door Censor로 고정적입니다.
    // door censor - 플레이어가 방을 클리어하고, 문에 다가갔는지 판별하는 오브젝트
    public Vector3[] entityPositions;
    public GameObject[] entityPrefabs;

    // RoomBluePrint 컴포넌트가 붙어있어야만 한다. 
    // public GameObject originMap;
    
    public SpawnData[] spawnTable;
    public ItemSpawnData[] itemTable;
    public RectInt[] spawnArea;
    public RectInt dropArea;
    
    public TileBase this[int x, int y, int layer] 
      => layers[RoomSize * RoomSize * layer + x + y * RoomSize];
  }

  [Serializable]
  public class SpawnData
  {
    // 소환하는 엔티티
    public GameObject entityPrefab;
    // 소환하는 엔티티의 최소, 최대 수
    public int min, max;
    // 해당 데이터의 엔티티를 소환할 확률
    // 100일 경우 무조건 소환 0 일 경우 소환하지 않음
    [Range(0, 100)]
    public int probability;
  }
  
  [Serializable]
  public class ItemSpawnData
  {
    // 드랍할 아이템
    public ItemData itemPrefab;
    // 드랍 엔티티의 최소, 최대 수
    public int min, max;
    // 해당 데이터의 아이템을 드랍할 확률
    // 100일 경우 무조건 드랍 0 일 경우 드랍하지 않음
    [Range(0, 100)]
    public int probability;
  }

  public enum RoomType
  {
    Battle = 0,
    Event = 1,
    Shop = 2
  }
}