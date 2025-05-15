using System.Collections;
using OnGame.Prefabs.Entities;
using OnGame.Prefabs.Projectiles;
using UnityEngine;

namespace OnGame.Prefabs.Items.Weapon.WeaponHandlers
{
    public class BossWeaponHandler : RangeWeaponHandler
    {
        // Prefab Fields
        [Header("Melee Attack Indicator Prefab")]
        [SerializeField] private GameObject areaIndicatorPrefab; //광역패턴 표시기
        
        // Pattern Setting Fields
        [Header("Boss Pattern Settings")]
        [SerializeField] private float meleeRange = 3f;
        [SerializeField] private float patternCooldown = 3f;
        [SerializeField] private float aoePatternDuration = 5f;
        [SerializeField] private float aoePatternInterval = 1f;

        // Fields
        private bool aoeTriggered50;
        private bool aoeTriggered25;
        private bool aoeLooping = false;
        private float lastPatternTime;
        
        // Properties
        public Transform PlayerTransform {
            get {
                if (Character is EnemyCharacter enemy) return enemy.Target.transform;
                return null;
            }
        }

        public override void Attack()
        {
            OnAttackAfterDelay();
        }

        protected override void OnAttackAfterDelay()
        {
            if (Time.time - lastPatternTime < patternCooldown) return;
            
            lastPatternTime = Time.time;
            var distance = DistanceToTarget(transform.position, PlayerTransform.position);
            
            if (Character is EnemyCharacter enemy)
            {
                var healthPercent = (float)enemy.Health.Value / enemy.Health.Max;
                switch (healthPercent)
                {
                    case <= 0.1f:
                        StartCoroutine(AoePattern());
                        break;
                    case <= 0.25f when !aoeTriggered25:
                        aoeTriggered25 = true;
                        enemy.OnHealthRecover(Mathf.CeilToInt(enemy.Health.Max * 0.3f));
                        StartCoroutine(AoePattern());
                        break;
                    case <= 0.5f when !aoeTriggered50:
                        aoeTriggered50 = true;
                        enemy.OnHealthRecover(Mathf.CeilToInt(enemy.Health.Max * 0.2f));
                        StartCoroutine(AoePattern());
                        break;
                    default: break;
                }
            }
            
            StartCoroutine(distance <= meleeRange ? MeleePattern() : RangedPattern());
        }
        
        private IEnumerator MeleePattern()
        {
            Debug.Log("근거리 범위공격 시작");
            
            var indicator = Instantiate(areaIndicatorPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            Destroy(indicator);
            
            var center = Character.transform.position;
            var hit = Physics2D.OverlapCircle(center, meleeRange, target.value);
            if (hit == null) yield break;
            
            Debug.Log($" 근거리 감지됨: {hit.name}");
            if (Character is not EnemyCharacter enemy) yield break;
            enemy.Target.OnDamage(-Power * 5);
            if (IsOnKnockback) { enemy.Target.ApplyKnockBack(transform, KnockBackPower); }
        }

        private IEnumerator RangedPattern()
        {
            Debug.Log("원거리 투사체 발사 시작");

            var count = Random.Range(3, 7);

            for (var i = 0; i < count; i++)
            {
                base.OnAttackAfterDelay();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator AoePattern()
        {
            Debug.Log("광역 패턴 시작");
            
            float elapsedTime = 0f;
            while (elapsedTime < aoePatternDuration)
            {
                for (int i = 0; i < 8; i++)
                {
                    var angle = i * 45f;
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
                    projectileManager.ShootBullet(this, transform.position, dir);
                }
                elapsedTime += aoePatternInterval;
                yield return new WaitForSeconds(aoePatternInterval);
            }
        }

        private float DistanceToTarget(Vector3 start, Vector3 end)
        {
            return Vector2.Distance(start, end);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, meleeRange);
        }
    }
}
