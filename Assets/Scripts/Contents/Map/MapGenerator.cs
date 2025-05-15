using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OnGame.Contents.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject roomPrefab;

        private RoomNode startNode;
        private void Start()
        {
            GenerateMap(new Vector2Int(0, 0), 6);
            CreateMap();
        }

        private void CreateMap()
        {
            HashSet<RoomNode> visited = new();
            TraverseAndCreateRoom(startNode, visited);
        }
        
        private void TraverseAndCreateRoom(RoomNode roomNode, HashSet<RoomNode> visited)
        {
            if (!visited.Add(roomNode)) return;
            
            StringBuilder sb = new();
            sb.Append($"{roomNode.Position.x}, {roomNode.Position.y} :");
            foreach (var neighbor in roomNode.Neighbors.Values)
                sb.Append($" {neighbor.Position.x}, {neighbor.Position.y}");
            Debug.Log(sb.ToString());
            
            Instantiate(roomPrefab, new Vector3(roomNode.Position.x, roomNode.Position.y), Quaternion.identity);
            foreach (var neighbor in roomNode.Neighbors.Values)
            {
                TraverseAndCreateRoom(neighbor, visited);
            }
        }

        private void GenerateMap(Vector2Int start, int maxRoomCount)
        {
            Stack<RoomNode> stack = new();
            Dictionary<Vector2Int, RoomNode> visited = new();
            startNode = new RoomNode {Position = start};
            stack.Push(startNode);
            visited.Add(start, startNode);

            while (stack.Count > 0 && visited.Count < maxRoomCount)
            {
                var currentNode = stack.Peek();
                var directions = ShuffleDirections();

                var created = false;
                foreach (var direction in directions)
                {
                    var newPos = Move(currentNode.Position, direction);
                    if (visited.ContainsKey(newPos))
                    {
                        currentNode.Neighbors[direction] = visited[newPos];
                        visited[newPos].Neighbors[OppositeDirection(direction)] = currentNode; 
                        continue;
                    }
                    
                    RoomNode newRoom = new(){Position = newPos};
                    currentNode.Neighbors[direction] = newRoom;
                    newRoom.Neighbors[OppositeDirection(direction)] = currentNode;
                        
                    stack.Push(newRoom);
                    visited.Add(newPos, newRoom);
                    created = true;
                    break;
                }
                
                if(!created) stack.Pop();
            }
        }

        private Vector2Int Move(Vector2Int currentPos, Direction direction)
        {
            return direction switch
            {
                Direction.North => currentPos + Vector2Int.up * 5,
                Direction.South => currentPos + Vector2Int.down * 5,
                Direction.East => currentPos + Vector2Int.right * 5,
                Direction.West => currentPos + Vector2Int.left * 5,
                _ => currentPos
            };
        }

        private Direction OppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                _ => Direction.East
            };
        }

        private List<Direction> ShuffleDirections()
        {
            var directions = from Direction direction in Enum.GetValues(typeof(Direction)) 
                                               select direction;
            directions = directions.OrderBy(_ => Random.value);
            return directions.ToList();
        }
    }
}