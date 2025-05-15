using OnGame.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnGame.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private GameObject topUI;
        [SerializeField] private GameObject bottomUI;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject settingPanel;
        
        [Header("Status")]
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private TextMeshProUGUI levelText;
        
        // hp setting
        [SerializeField] private GaugeBar hpBar;
        [SerializeField] private Image hpBarImage;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private float lowHpThreshold = 30f;
        [SerializeField] private Color hpColor = new Color(0.6f, 0.8f, 0.3f);
        [SerializeField] private Color lowHpColor = new Color(0.8f, 0.3f, 0.3f);
        
        // mp
        [SerializeField] private GaugeBar mpBar;
        [SerializeField] private TextMeshProUGUI mpText;
        
        // exp
        [SerializeField] private GaugeBar expBar;
        [SerializeField] private TextMeshProUGUI expText;
        
        [Header("Coin")]
        [SerializeField] private TextMeshProUGUI coinText;

        [Header("Weapon")]
        [SerializeField] private TextMeshProUGUI weaponText;
        [SerializeField] private Image[] upgradeLights;

        [Header("Inventory")]
        [SerializeField] private Image[] inventorySlots;

        [Header("Settings")]
        [SerializeField] private Button settingsButton;
        
        
        public void SetStageText(int stage) => stageText.text = $"STAGE {stage}";
        public void SetLevelText(int level) => levelText.text = level.ToString();
        public void SetCoinText(int coin) => coinText.text = $"{coin} G";

        public void SetHp(float hp)
        {
            hpBar.Value = hp;
            hpText.text = hp.ToString();
            hpBarImage.color = hp < lowHpThreshold ? lowHpColor : hpColor;
        }

        
        public void SetMp(float mp)
        {
            mpBar.Value = mp;
            mpText.text = mp.ToString();
        }
        
        
        public void SetWeaponInfo(string weaponName, int level)
        {
            weaponText.text = weaponName;
            Color activeColor = Color.green;
            Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            
            for (int i = 0; i < upgradeLights.Length; i++)
            {
                upgradeLights[i].color = i < level ? activeColor : inactiveColor;
            }
        }
        
        
        public void SetInventoryIcon(int index, Sprite icon)
        {
            if (index < 0 || index >= inventorySlots.Length) return;
            inventorySlots[index].sprite = icon;
            inventorySlots[index].enabled = icon != null;
        }


        public void ClearInventory()
        {
            foreach (var slot in inventorySlots)
            {
                slot.sprite = null;
                slot.enabled = false;
            }
        }


        public void HideGameUI()
        {
            gameObject.SetActive(false);
        }

        
        public void HideTopUI()
        {
            topUI.SetActive(false);
        }
        
        
        public void ShowTopUI()
        {
            topUI.SetActive(true);
        }
        
        
        public void HideBottomUI()
        {
            bottomUI.SetActive(false);
        }


        public void ShowBottomUI()
        {
            bottomUI.SetActive(true);
        }
        
        
        public void HideGameOverPanel()
        {
            gameOverPanel.SetActive(false);
        }


        public void ShowGameOverPanel()
        {
            gameOverPanel.SetActive(true);
        }


        public void ShowSettingPanel()
        {
            Time.timeScale = 0;
            settingPanel.SetActive(true);
        }


        public void HideSettingPanel()
        {
            Time.timeScale = 1f;
            settingPanel.SetActive(false);
        }
    }
}
