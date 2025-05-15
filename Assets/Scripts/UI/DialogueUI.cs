using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnGame.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private Image speakerImage;
        [SerializeField] private TextMeshProUGUI speakerName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button skipButton;
        
        private DialogueData currentData;
        private int currentIndex;
        
        private void Start()
        {
            HideDialogueUI();
        }


        public void SetDialogueData(DialogueData data)
        {
            currentData = data;
            currentIndex = 0;
        }
        
        
        public void StartDialogue(DialogueData data)
        {
            UIManager.instance.CurrentState = UIState.Dialogue;
            ShowDialogueUI();
            currentData = data;
            currentIndex = 0;
            ShowCurrentLine();
        }
        
        
        private void ShowCurrentLine()
        {
            string line = currentData.dialogueLines[currentIndex];
            ShowLines(currentData.speakerName, line, currentData.portrait);
        }
        
        
        private void ShowLines(string speakerID, string dialogueLine, Sprite portrait)
        {
            speakerName.text = speakerID;
            dialogueText.text = dialogueLine;
            speakerImage.sprite = portrait;
        }
        
        
        public void ShowDialogueUI()
        {
            gameObject.SetActive(true);
        }
        
        
        public void HideDialogueUI()
        {
            gameObject.SetActive(false);
        }


        public void OnSkipButtonClick()
        {
            NextLine();
        }
        
        
        private void NextLine()
        {
            currentIndex++;
            if (currentIndex >= currentData.dialogueLines.Count)
            {
                EndDialogue();
            }
            else
            {
                ShowCurrentLine();
            }
        }

        
        private void EndDialogue()
        {
            UIManager.instance.CurrentState = UIState.Default;
            HideDialogueUI();
            currentData = null;
        }
        
        
        public bool IsDialogueActive => gameObject.activeSelf;
    }
}
