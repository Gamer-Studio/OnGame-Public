using System.Collections;
using OnGame.Contents.Items;
using OnGame.Contents.Items.Base;
using UnityEngine;
using OnGame.UI;
using UnityEngine.Serialization;

namespace OnGame.Scenes.World
{
    public class Inventory : MonoBehaviour
    {
        // 배열 크기 절 대 변경하면 안됨
        [SerializeField] private ItemBase[] items = new ItemBase[5];
        [SerializeField] private ItemBase[] equipment = new ItemBase[4];
        [SerializeField] private GameUI gameUI;
        [FormerlySerializedAs("itemTable")] [SerializeField] private ItemManager itemManager;
        private int selectItemIndex = -1;
        
        private void Awake()
        {
            itemManager = ItemManager.Instance;
            gameUI.ClearInventory();
            ClearInventory();
            ClearEquipment();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z)) SetItem(itemManager["testPotion"]);
            if (Input.GetKeyDown(KeyCode.X)) SetItem(itemManager["testPotion"]);
            if (Input.GetKeyDown(KeyCode.C)) SetItem(itemManager["testBlueCoin"]);
            if (Input.GetKeyDown(KeyCode.F)) SetEquipItem();
            if (Input.GetKeyDown(KeyCode.R))
            {
                gameUI.ClearInventory(); //임시 인벤토리슬롯UI 초기화
                ClearInventory();
            }
            
            for (int i = 0; i < 5; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    selectItemIndex = i;
                }
            }
        }
        
        public void SetItem(ItemData data)
        {
           var newItem = data.Create();
           if(newItem is IPassive passiveItem){ passiveItem.Apply(WorldManager.Instance.player); }
           for (var i = 0; i < items.Length; i++)
           {
               if (items[i] == null) { items[i] = newItem; break; }
           }
           SetInventoryUI();
        }

        public void RemoveItem(int index)
        {
            var item = items[index];
            if (item is IPassive passiveItem)
            {
                passiveItem.Remove(WorldManager.Instance.player);
            }

            if (item is IActive activeItem)
            {
                activeItem.Use();
                foreach (var part in activeItem.GetParts())
                {
                    if(part is IPassive passivePart) passivePart.Remove(WorldManager.Instance.player);
                }
            }
        }
        
        private void SetEquipItem() //장비 강화
        {
            if (selectItemIndex < 0 || selectItemIndex >= items.Length) return;

            var selectedItem = items[selectItemIndex];
            
            if (selectedItem is not IActive) return;
            if(selectedItem is IPassive passiveItem){ passiveItem.Remove(WorldManager.Instance.player); }
            
            for (int i = 0; i < equipment.Length; i++)
            {
                if (equipment[i] == null)
                {
                    (equipment[i], items[selectItemIndex]) = (items[selectItemIndex], equipment[i]);
                    if(items[selectItemIndex] is IActive activeItem) activeItem.Use();
                    break;
                }
            }
            SetInventoryUI();
            SetEquipmentUI();
        }
    
        private void SetInventoryUI()
        {
            for (int i = 0; i < items.Length; i++)
            {
                Sprite icon = items[i]?.data?.sprite;
                gameUI.SetInventoryIcon(i, icon);
            }
        }

        private void SetEquipmentUI()
        {
            int Count = 0;
            foreach (var equip in equipment)
            {
                if (equip != null) Count++;
            }

            gameUI.SetWeaponInfo("weapon", Count); 
        }
        
        
        private void ClearInventory()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = null;
            }
        }
        private void ClearEquipment()
        {
            for (int i = 0; i < equipment.Length; i++)
            {
                equipment[i] = null;
            }
        }
    }
}