using UnityEngine;

namespace OnGame.UI
{
    public partial class UIManager : MonoBehaviour
    {
        // 상호작용 시 사용 (자동으로 show/hide)
        public void StartDialogue(DialogueData data) => dialogueUI.StartDialogue(data);
        
        public void ShowDialogueUI() => dialogueUI.ShowDialogueUI();
        public void HideDialogueUI() => dialogueUI.HideDialogueUI();
    }
}
