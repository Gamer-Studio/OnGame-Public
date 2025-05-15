#if UNITY_EDITOR
using OnGame.Scenes.World;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldManager))]
public class StageViewer : Editor
{
  private int selectedRoom = 0;
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();
    var world = (WorldManager)target;
    var stage = world.Stage;

    if (stage.Theme)
    {
      GUILayout.Label("Room viewer");
      selectedRoom= GUILayout.SelectionGrid(selectedRoom, new string[stage.RoomCount], stage.StageSize);
      
      // TODO 여기에 선택한 방의 정보를 그려주는 스크립트 작성
    }
  }
}
#endif