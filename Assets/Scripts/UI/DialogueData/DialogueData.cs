using System.Collections.Generic;
using UnityEngine;

namespace OnGame.UI
{
    [CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public string speakerName;
        public Sprite portrait;

        [TextArea]
        public List<string> dialogueLines;
    }
}
