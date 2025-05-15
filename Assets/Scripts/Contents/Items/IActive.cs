using UnityEngine;

namespace OnGame.Contents.Items
{
  public interface IActive
  {
    void Shoot();
    void Use();
    bool ConsumeOnUse { get; set; }
    
    // Part Management
    void AddPart<T>(T part) where T : ItemBase, IPassive;
    // 혹시 모르니까 만들어둔 메소드
    void RemovePart<T>(T part) where T : ItemBase, IPassive;
    ItemBase[] GetParts();
  }
}