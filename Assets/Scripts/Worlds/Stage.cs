using System;
using System.Collections.Generic;
using System.Linq;
using OnGame.Prefabs.Entities;
using OnGame.Scenes.World;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace OnGame.Worlds
{
  [Serializable]
  public class Stage
  {
    private static int RoomSize => WorldManager.RoomSize;

    [Header("Generator Option")]
    [SerializeField] private StageData theme;
    public StageData Theme => theme;
    public int RoomCount => rooms.Length;
    public int StageSize => theme.StageSize;

    // 스테이지 생성 옵션
    // 스테이지에 등장하는 방 목록
    [SerializeField] private SerializableDictionary<RoomType, List<RoomData>> roomPools = new();

    // 생성된 방 목록
    [SerializeField] private Room[] rooms;

    // 스테이지 레이어 관리용 변수
    [Header("Components")] public Tilemap[] floors, structures;
    [SerializeField] private Transform floorContainer, structureContainer;
    [SerializeField] private TilemapRenderer[] floorRenderers, structureRenderers;
    [SerializeField] private TilemapCollider2D[] structureColliders;

    // 남은 방 생성 개수
    [Header("Remain Room Counts")] [SerializeField]
    private bool isStarted;

    [SerializeField] private SerializableDictionary<RoomType, int> roomCounts;
    public int TotalRoomCount => roomCounts.Sum(x => x.Value);

    public Stage(StageData theme, Transform floorContainer, Transform structureContainer)
    {
      this.theme = theme;
      this.floorContainer = floorContainer;
      this.structureContainer = structureContainer;

      // room data 목록 초기화
      foreach (RoomType type in Enum.GetValues(typeof(RoomType))) roomPools[type] = new List<RoomData>();

      // Stage Data에서 Room Data목록 불러오기
      foreach (var room in theme.roomTable) roomPools[room.type].Add(room);

      // Stage Data 레이어 정보를 기반으로 레이어 미리 생성하기.
      floors = new Tilemap[theme.floorCount];
      structures = new Tilemap[theme.structureCount];
      floorRenderers = new TilemapRenderer[theme.floorCount];
      structureRenderers = new TilemapRenderer[theme.structureCount];
      structureColliders = new TilemapCollider2D[theme.structureCount];

      // Stage Data를 기반으로 floor 레이어 타일맵을 미리 생성합니다.
      for (var i = 0; i < floors.Length; i++)
      {
        var layer = new GameObject("floor_" + i);
        floors[i] = layer.AddComponent<Tilemap>();
        var floorRenderer = layer.AddComponent<TilemapRenderer>();

        layer.transform.SetParent(floorContainer);
        layer.transform.localPosition = Vector3.zero;
        floorRenderers[i] = floorRenderer;
        floorRenderer.sortingLayerName = "Background";
        floorRenderer.sortingOrder = i;
        floorRenderer.material = theme.tilemapMaterial;
      }

      // Stage Data를 기반으로 structure 레이어 타일맵을 미리 생성합니다.
      for (var i = 0; i < structures.Length; i++)
      {
        var layer = new GameObject("structure_" + i);
        structures[i] = layer.AddComponent<Tilemap>();
        var structureRenderer = layer.AddComponent<TilemapRenderer>();
        structureColliders[i] = layer.AddComponent<TilemapCollider2D>();

        layer.transform.SetParent(structureContainer);
        layer.transform.localPosition = Vector3.zero;
        structureRenderers[i] = structureRenderer;
        structureRenderer.sortingLayerName = "Background";
        structureRenderer.sortingOrder = i + floors.Length;
        structureRenderers[i].material = theme.tilemapMaterial;
      }

      // 스테이지 정보 불러오기
      roomCounts = new SerializableDictionary<RoomType, int>
      {
        [RoomType.Battle] = theme.battleRoomCount,
        [RoomType.Event] = theme.eventRoomCount,
        [RoomType.Shop] = theme.shopRoomCount
      };

      rooms = new Room[StageSize * StageSize];
    }

    /// <summary>
    ///   외부에서 접근할 수 있는 스테이지안에 방 생성 트리거
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Room GenerateRoom(Vector2Int position)
    {
      if (!isStarted)
      {
        // 첫 시작시 방 고정
        isStarted = true;
        return CreateRoom(position, theme.startRoom);
      }

      if (TotalRoomCount > 0)
      {
        // 보스전 전에는 일반 방 생성
        var possibleTypes =
          (from type in roomCounts.Keys
            where roomCounts[type] > 0
            select type)
          .ToArray();

        var roomType = possibleTypes[Random.Range(0, possibleTypes.Length)];
        roomCounts[roomType]--;

        var roomData = roomPools[roomType][Random.Range(0, roomPools[roomType].Count)];
        return CreateRoom(position, roomData);
      }

      // 보스전 진입방 생성
      return CreateRoom(position, theme.endRoom);
    }


    /// <summary>
    ///   RoomData를 받아와서 position에 해당하는 섹터(월드 포지션 x 스테이지 고유 좌표입니다.)에 해당 방을 그립니다.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="roomData"></param>
    /// <returns></returns>
    private Room CreateRoom(Vector2Int position, RoomData roomData)
    {
      if (rooms == null) return null;

      // 방 인스턴스 생성
      var result = rooms[StageSize * position.y + position.x] = new Room
      {
        data = roomData
      };

      // RoomSize 만큼씩 데이터를 잘라서 추출 후 월드에 방을 그리기
      var floorLayers = 
        (from _ in Enumerable.Range(0, roomData.floorLayerCount) 
          select new TileChangeData[RoomSize * RoomSize]
            ).ToArray();
      
      var structureLayers = 
        (from _ in Enumerable.Range(0, roomData.structureLayerCount) 
          select new TileChangeData[RoomSize * RoomSize]
          ).ToArray();
      
      for (var y = 0; y < RoomSize; y++)
      {
        for (var x = 0; x < RoomSize; x++)
        {
          var worldPosition =
            new Vector3Int(position.x * RoomSize + x + position.x, position.y * RoomSize + y + position.y);

          for (var i = 0; i < roomData.floorLayerCount; i++)
          {
            floorLayers[i][x + y * RoomSize] = new TileChangeData
            {
              position = worldPosition,
              tile = roomData[x, y, i],
              color = roomData.mapColors[i],
              transform = Matrix4x4.identity
            };
          }

          for (var i = 0; i < roomData.structureLayerCount; i++)
          {
            structureLayers[i][x + y * RoomSize] = new TileChangeData
            {
              position = worldPosition,
              tile = roomData[x, y, i + roomData.floorLayerCount],
              color = roomData.mapColors[i + roomData.floorLayerCount],
              transform = Matrix4x4.identity
            };
          }
        }
      }

      // 추출한 맵 데이터를 기반으로 월드에 그리기
      // ignoreLockFlags - 타일을 강제로 설정하는? 느낌 - 런타임 상에서는 중요한 동작은 아닌 거 같네요
      // 추가 - ignoreLockFlags true로 설정헤야 타일 색상이 바꿔져요!
      for (var i = 0; i < floorLayers.Length; i++)
        floors[i].SetTiles(floorLayers[i], true);

      for (var i = 0; i < structureLayers.Length; i++)
        structures[i].SetTiles(structureLayers[i], true);

      // 방에 고정적으로 설정된 엔티티 소환
      for (var i = 0; i < roomData.entityPositions.Length; i++)
      {
        var spawnPosition = roomData.entityPositions[i];
        var prefab = roomData.entityPrefabs[i];

        if (prefab == null) continue;

        Object.Instantiate(prefab, WorldManager.Instance.transform).transform.position = spawnPosition;
      }

      // 전투 방일시 RoomData의 엔티티 테이블에 따라 엔티티 소환
      if (roomData.type == RoomType.Battle)
      {
        // 좌표 연산용 기본 레이어 가져오기
        var map = floors[0];
        var enemies = new List<Enemy>();

        foreach (var data in roomData.spawnTable)
        {
          // 소환 확률 연산 후 소환하지 않을 시 스킵
          if (data.probability < Random.Range(0, 100)) continue;

          var count = Random.Range(data.min, data.max + 1);
          // 소환할 구역 선택
          var sector = (from rect in roomData.spawnArea
            orderby Random.Range(0, roomData.spawnArea.Length * 10)
            select rect).First();

          // count 만큼 소환 반복
          for (var i = 0; i < count; i++)
          {
            // 소환 구역 내에서 랜덤 지정 선택
            // 벽 끼임 방지를 위해 타일맵 포지션 기반으로 소환
            var spawnPoint = map.CellToWorld(new Vector3Int(
              position.x * (RoomSize + 1) + Random.Range(sector.x, sector.x + sector.width),
              position.y * (RoomSize + 1) + Random.Range(sector.y, sector.y + sector.height)
            ));

            var obj = Object.Instantiate(data.entityPrefab, WorldManager.Instance.transform);
            var enemy = obj.GetComponent<Enemy>();
            enemies.Add(enemy);

            enemy.Character.onDeath.AddListener(() =>
            {
              result.aliveCount--;
              if (result.aliveCount <= 0) result.onClear.Invoke();
            });

            obj.transform.position = spawnPoint;
          }
        }

        result.enemies = enemies.ToArray();
        result.aliveCount = enemies.Count;
      }

      // 근처에 방이 있는지 체크하고, 존재할 시 문 생성
      // 방의 (0, 0) (타일맵 기준)좌표
      var roomPosition = new Vector2Int(position.x * (RoomSize + 1), position.y * (RoomSize + 1));
      // top
      {
        if (position.y + 1 < StageSize && this[position.x, position.y + 1])
        {
          int x = roomPosition.x + RoomSize / 2 - 2, y = roomPosition.y + RoomSize;

          for (var i = 0; i < theme.verticalDoor.Length; i++)
            floors[theme.baseLayer].SetTile(new Vector3Int(x + i, y), theme.verticalDoor[i]);
        }
      }

      // down
      {
        if (position.y - 1 >= 0 && this[position.x, position.y - 1])
        {
          int x = roomPosition.x + RoomSize / 2 - 2, y = roomPosition.y - 1;

          for (var i = 0; i < theme.verticalDoor.Length; i++)
            floors[theme.baseLayer].SetTile(new Vector3Int(x + i, y), theme.verticalDoor[i]);
        }
      }
      
      // left
      {
        if (position.x + 1 < StageSize && this[position.x + 1, position.y])
        {
          int x = roomPosition.x + RoomSize, y = roomPosition.y + RoomSize / 2 - 2;

          for (var i = 0; i < theme.verticalDoor.Length; i++)
            floors[theme.baseLayer].SetTile(new Vector3Int(x, y + i), theme.sideDoor[^(i+1)]);
        }
      }
      
      // right
      {
        if (position.x - 1 >= 0 && this[position.x - 1, position.y])
        {
          int x = roomPosition.x - 1, y = roomPosition.y + RoomSize / 2 - 2;
          
          for (var i = 0; i < theme.verticalDoor.Length; i++)
            floors[theme.baseLayer].SetTile(new Vector3Int(x, y + i), theme.sideDoor[^(i + 1)]);
        }
      }
      
      // 물리 콜라이더 다시 연산하기
      for (var i = 0; i < structures.Length; i++)
      {
        structureColliders[i].ProcessTilemapChanges();
      }

      result.position = position;

      return result;
    }

    /// <summary>
    ///   방의 정가운데 위치를 반환합니다.
    /// </summary>
    /// <param name="roomPosition"></param>
    /// <returns></returns>
    public Vector3 GetRoomPosition(Vector2Int roomPosition)
    {
      var pos1 = new Vector3Int(roomPosition.x * (RoomSize + 1) + RoomSize / 2,
        roomPosition.y * (RoomSize + 1) + RoomSize / 2);
      return floors[0].CellToWorld(pos1);
    }

    public Room this[int x, int y]
      => rooms?[StageSize * y + x];
    
    public Room this[Vector2Int position]
      => rooms?[StageSize * position.y + position.x];
  }
}