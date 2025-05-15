using System;
using OnGame.Prefabs.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace OnGame.Worlds
{
  [Serializable]
  public class Room
  {
    // 방의 적이 모두 처치됬을 때 이벤트
    public UnityEvent onClear;

    public bool IsClear => aliveCount == 0;

    // 스테이지의 방 위치
    public Vector2Int position;
    public RoomData data;
    public Enemy[] enemies;
    public int aliveCount;

    public RoomType Type => data.type;

    public static implicit operator bool(Room room)
    {
      return room != null;
    }
  }
}