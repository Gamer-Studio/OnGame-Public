using OnGame.Prefabs.Entities;
using UnityEngine;

namespace OnGame.Scenes.World
{
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        [SerializeField] private Entity entity;
        [SerializeField] private Animator animator;

        public void OnDamage_AnimationEnd()
        {
            switch (entity)
            {
                case Character player:
                    player.IsInvincible = false;
                    animator.SetBool(Character.IsDamage, false);
                    break;
                case EnemyCharacter enemy:
                    enemy.IsInvincible = false;
                    animator.SetBool(enemy.IsDamage, false);
                    break;
            }
        }
    }
}
