using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OnGame.Contents.Items
{
  public class ItemManager : MonoBehaviour
  {
    public static ItemManager Instance { get; private set; }
    [SerializeField] private bool load = false;
    [SerializeField] private AssetLabelReference itemLabel;
    [SerializeField] private SerializableDictionary<string, ItemData> itemDict = new();

    public void Load()
    {
      if(load) return;
      
      Instance = this;
      load = true;
        
      var handle = Addressables.LoadAssetsAsync<ItemData>(itemLabel, data =>
      {
        itemDict[data.name] = data;
      });
      
      handle.WaitForCompletion();
    }

    public bool TryGetItem(string key, out ItemData item)
    {
      return itemDict.TryGetValue(key, out item);
    }

    public ItemData this[string key] => itemDict[key];
  }
}
