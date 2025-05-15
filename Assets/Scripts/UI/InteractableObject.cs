using OnGame.Prefabs.Entities;
using OnGame.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace OnGame.UI
{
    public class InteractableObject : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private DialogueData data;
        [SerializeField] private LayerMask player;
        [SerializeField] private UnityEvent onInteract;
    
        private Character playerCharacter;

        private void Start()
        {
            if (interactPrompt == null) return;
            interactPrompt.SetActive(false);
        }

        
        private void Update()
        {
            if (playerCharacter == null || !playerCharacter.IsInteracting) return;
            
            playerCharacter.IsInteracting = false;
            if (gameObject.CompareTag("NPC")) { UIManager.instance.StartDialogue(data); }
            else 
            { 
                // 씬 이동 등에 사용
                onInteract?.Invoke();
            }
        }

        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((player.value & (1 << other.gameObject.layer)) == 0) return;
            
            interactPrompt.SetActive(true);
            playerCharacter = Helper.GetComponent_Helper<Character>(other.gameObject);
            Debug.Log("Interact");
        }

        
        private void OnTriggerExit2D(Collider2D other)
        {
            if ((player.value & (1 << other.gameObject.layer)) == 0) return;
            
            interactPrompt.SetActive(false);
            playerCharacter = null;
        }
    }
}
