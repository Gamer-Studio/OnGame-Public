using OnGame.Prefabs.Entities;
using OnGame.Utils;
using UnityEngine;

namespace OnGame.Prefabs.Items.Weapon.WeaponHandlers
{
    public class MeleeWeaponHandler : WeaponHandler
    {
        [Header("Melee Attack Data")]
        [SerializeField] private Vector2 collideBoxSize = Vector2.one;

        protected override void Start()
        {
            base.Start();
            collideBoxSize *= WeaponSize;
        }

        protected override void OnAttackAfterDelay()
        {
            switch (Character)
            {
                case Character character:
                    var hitOfPlayer = Physics2D.BoxCast(transform.position + (Vector3)character.LookAtDirection * collideBoxSize.x, collideBoxSize, 0, Vector2.zero, 0, target);
                    if (hitOfPlayer.collider == null) return;
                    var enemy = Helper.GetComponent_Helper<EnemyCharacter>(hitOfPlayer.collider.gameObject);
                    if (enemy.IsInvincible || !enemy.IsAlive) break;
                    enemy.OnDamage(-character.Return_CalculatedDamage());
                    if (IsOnKnockback) enemy.ApplyKnockBack(transform, KnockBackPower);
                    break;
                
                case EnemyCharacter enemyCharacter:
                    var hitOfEnemy = Physics2D.BoxCast(transform.position + (Vector3)enemyCharacter.LookAtDirection * collideBoxSize.x, collideBoxSize, 0, Vector2.zero, 0, target);
                    if (hitOfEnemy.collider == null) return;
                    var player = Helper.GetComponent_Helper<Character>(hitOfEnemy.collider.gameObject);
                    if (player.IsInvincible || !player.IsAlive) break;
                    player.OnDamage(-enemyCharacter.Return_CalculatedDamage());
                    if (IsOnKnockback) player.ApplyKnockBack(transform, KnockBackPower);
                    break;
            }            
        }
    }
}