#if UNITY_EDITOR

using OnGame.Prefabs.RoomBlueprints;
using OnGame.Scenes.World;
using OnGame.Worlds;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(RoomBlueprint))]
public class RoomDataCreator : Editor
{
  private static int RoomSize => WorldManager.RoomSize;
  private static string _beforePath = "Assets";

  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    var blueprint = (RoomBlueprint)target;

    if (GUILayout.Button("Create Room Data"))
    {
      var path = EditorUtility.SaveFilePanel("Save Room Data", _beforePath, blueprint.name, "asset");

      if (!string.IsNullOrEmpty(path))
      {
        // 데이터 변환
        var result = CreateInstance<RoomData>();
        result.type = blueprint.roomType;
        result.floorLayerCount = blueprint.floors.Length;
        result.structureLayerCount = blueprint.structures.Length;
        var layerData = new TileBase[RoomSize * RoomSize * (blueprint.floors.Length + blueprint.structures.Length)];
        var mapColors = new Color[blueprint.floors.Length + blueprint.structures.Length];
        result.entityPositions = new Vector3[blueprint.entityContainer.childCount];
        result.entityPrefabs = new GameObject[blueprint.entityContainer.childCount];

        for (var y = 0; y < RoomSize; y++)
        for (var x = 0; x < RoomSize; x++)
        {
          var position = new Vector3Int(blueprint.startPosition.x + x, blueprint.startPosition.y + y);
          var index = x + y * RoomSize;

          for (var i = 0; i < blueprint.floors.Length; i++)
          {
            var floor = blueprint.floors[i];
            var tile = floor.GetTile(position);
            layerData[i * RoomSize * RoomSize + index] = tile;
          }

          for (var i = 0; i < blueprint.structures.Length; i++)
          {
            var structure = blueprint.structures[i];
            var tile = structure.GetTile(position);

            layerData[(blueprint.floors.Length + i) * RoomSize * RoomSize + index] = tile;
          }
        }

        // 맵 색상 * 머티리얼 바인딩
        for (var i = 0; i < blueprint.floors.Length; i++)
        {
          mapColors[i] = blueprint.floors[i].color;
        }
        for (var i = 0; i < blueprint.structures.Length; i++)
        {
          mapColors[blueprint.floors.Length + i] = blueprint.structures[i].color;
        }

        for(var i = 0; i < blueprint.entityContainer.childCount; i++)
        {
          result.entityPositions[i] = blueprint.entityContainer.GetChild(i).transform.position;
          var obj = blueprint.entityContainer.GetChild(i).gameObject;

          if(PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected)
          {
            result.entityPrefabs[i] = PrefabUtility.GetCorrespondingObjectFromSource(obj);
          }
        }

        // 변환된 데이터 넣기
        result.layers = layerData;
        result.spawnArea = blueprint.spawnArea;
        result.mapColors = mapColors;

        // 변환된 데이터 저장
        AssetDatabase.CreateAsset(result, "Assets" + path[Application.dataPath.Length..]);
        AssetDatabase.SaveAssets();
        _beforePath = path;
      }
    }
  }
}

#endif