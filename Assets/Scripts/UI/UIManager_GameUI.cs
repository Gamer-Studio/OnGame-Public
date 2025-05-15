using UnityEngine;

namespace OnGame.UI
{
    public partial class UIManager : MonoBehaviour
    {
        // GameUI 세팅
        
        public void SetCoin(int coin) => gameUI.SetCoinText(coin);
        public void SetStage(int coin) => gameUI.SetStageText(coin);
        public void SetLevel(int coin) => gameUI.SetLevelText(coin);
        public void SetHp(float hp) => gameUI.SetHp(hp);
        public void SetMp(float mp) => gameUI.SetMp(mp);
        public void SetWeapon(string weaponName, int level) => gameUI.SetWeaponInfo(weaponName, level);
        public void SetInventory(int index, Sprite icon) => gameUI.SetInventoryIcon(index, icon);
        public void HideGameUI() => gameUI.HideGameUI();
        public void HideTopUI() => gameUI.HideTopUI();
        public void HideBottomUI() => gameUI.HideBottomUI();
        public void ShowBottomUI() => gameUI.ShowBottomUI();
        public void ShowTopUI() => gameUI.ShowTopUI();
        public void HideGameOverPanel() => gameUI.HideGameOverPanel();
        public void ShowGameOverPanel() => gameUI.ShowGameOverPanel();
        public void HideSettingPanel() => gameUI.HideSettingPanel();
        public void ShowSettingPanel() => gameUI.ShowSettingPanel();
        
    }
}
