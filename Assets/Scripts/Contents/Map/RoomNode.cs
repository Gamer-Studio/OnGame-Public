using System.Collections.Generic;
using UnityEngine;

namespace OnGame.Contents.Map
{
    public enum Direction{North, South, East, West}
    public class RoomNode
    {
        public Vector2Int Position;
        public Dictionary<Direction, RoomNode> Neighbors = new();
    }
}