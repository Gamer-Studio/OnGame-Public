using OnGame.Worlds;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OnGame.Prefabs.RoomBlueprints
{
  public class RoomBlueprint : MonoBehaviour
  {
    // 맵을 잘못 그렸을 때를 위한 맵 시작 지점 변경
    public Vector2Int startPosition = new(0, 0);
    // 여러 레이어에 그릴 경우 레이어 추가해주신 다음 넣어주시면 됩니다.
    public Tilemap[] floors;
    public Tilemap[] structures;
    // 방에 고정적 위치에 소환되는 엔티티
    // 0~3번은 Top, Down, Left, Right door Censor로 고정적이어야합니다.
    // door censor - 플레이어가 방을 클리어하고, 문에 다가갔는지 판별하는 오브젝트
    public Transform entityContainer;

    // 전투방일 시 반드시 설정되어있어야되요!
    // 설정 안됬을 시 null... 슬퍼
    public RectInt[] spawnArea;
    public RoomType roomType;
  }
}