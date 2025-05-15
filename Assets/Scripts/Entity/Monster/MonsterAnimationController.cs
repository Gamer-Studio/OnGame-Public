using System.Collections;
using UnityEngine;
using OnGame.Utils;

namespace OnGame
{
    public class MonsterAnimatorController : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private SpriteResolverHook hook;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private string basePrefix = "orc_shaman"; // 예: orc_warrior
        [SerializeField] private float frameDelay = 0.2f;

        private int frameIndex = 0;
        private float timer = 0f;
        private const int frameCount = 4;

        private string currentState = "Idle";
        private Color originalColor;

        private void Awake()
        {
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;

            if (hook != null)
                hook.SetCategory(currentState);
        }

        private void Update()
        {
            if (hook == null) return;

            timer += Time.deltaTime;
            if (timer < frameDelay) return;
            
            string label = $"{basePrefix}_{currentState.ToLower()}_{frameIndex}";
            hook.SetCategoryAndLabel(currentState, label);
            frameIndex = (frameIndex + 1) % frameCount;
            timer = 0f;
        }

        public void SetState(string newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            frameIndex = 0;
            timer = 0f;

            if (hook != null) hook.SetCategory(currentState);

            if (newState != "Damage") return;
            spriteRenderer.color = Color.red;
            CancelInvoke(nameof(RestoreColor));
            Invoke(nameof(RestoreColor), 0.3f);
        }

        private void RestoreColor()
        {
            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }
}
