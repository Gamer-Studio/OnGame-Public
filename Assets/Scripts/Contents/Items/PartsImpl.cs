using System.Collections;
using System.Collections.Generic;
using OnGame.Contents.Items;
using OnGame.Scenes.World;
using UnityEngine;

namespace OnGame
{
    public class testActive : IActive
    {
        private List<ItemBase> parts = new();

        public void Shoot() => Debug.Log("공격");
        public void Use() => Debug.Log("사용");

        public void AddPart<T>(T part) where T : ItemBase, IPassive => parts.Add(part);
        public void RemovePart<T>(T part) where T : ItemBase, IPassive => parts.Remove(part);
        public ItemBase[] GetParts() => parts.ToArray();

        public bool ConsumeOnUse { get; set; } = false;
    }
    
    public class testPassive : IPassive
    {
        
        public void Apply(Player player) => Debug.Log("불속성 적용됨");

        public void Remove(Player player)
        {
            
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (key == "fire" && typeof(T) == typeof(float))
            {
                value = (T)(object)1.5f;
                return true;
            }

            value = default;
            return false;
        }
    }
}
