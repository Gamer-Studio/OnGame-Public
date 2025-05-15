using UnityEngine;

namespace OnGame.Contents.Items
{
  [CreateAssetMenu(fileName = "NewItemData", menuName = "OnGame/Item Data")]
  public class ItemData : ScriptableObject 
  {
    public string description;
    public Sprite sprite;
    public int price;

    public virtual ItemBase Create()
    {
      return new ItemBase();
    }
  }
}