using OnGame.Prefabs.Entities;
using OnGame.Prefabs.Items.Weapon.WeaponHandlers;
using OnGame.Prefabs.VFXs;
using OnGame.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace OnGame.Prefabs.Projectiles
{

    public class Projectile : MonoBehaviour
    {
        // Pooling
        protected static ObjectPool<Projectile> pool;
        
        [Header("Collision Layer")]
        [SerializeField] protected LayerMask levelCollisionLayer;

        [Header("Component Fields")] 
        [SerializeField] protected RangeWeaponHandler weaponHandler;

        [SerializeField] protected Rigidbody2D rigidBody;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Transform pivot;
        [SerializeField] protected ProjectileManager projectileManager;
        [SerializeField] protected ParticleManager particleManager;

        [Header("Fields")] 
        [SerializeField] protected float currentDuration;
        [SerializeField] protected Vector2 launchDirection;
        [SerializeField] protected bool isReady;
        public bool fxOnDestroy = true;

        /// <summary>
        /// Initialize Projectile GameObject.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="handler"></param>
        /// <param name="manager"></param>
        public void Init(Vector2 direction, RangeWeaponHandler handler, ProjectileManager manager, ParticleManager particleManager)
        {
            projectileManager = manager;
            this.particleManager = particleManager;
            weaponHandler = handler;
            launchDirection = direction;
            currentDuration = 0;
            transform.localScale = Vector3.one * handler.BulletSize;
            spriteRenderer.color = handler.ProjectileColor;

            // Movement direction will be changed with X axis of Direction Vector.
            transform.right = launchDirection;

            pivot.localRotation = Quaternion.Euler(direction.x < 0 ? 180 : 0, 0, 0);

            isReady = true;
        }

        /// <summary>
        /// Update is called every time
        /// </summary>
        protected virtual void Update()
        {
            if (!isReady) { return; }

            currentDuration += Time.deltaTime;

            if(currentDuration > weaponHandler.Duration)
            {
                Release(transform.position, false);
            }

            rigidBody.velocity = launchDirection * weaponHandler.Speed;
        }

        /// <summary>
        /// Collision Resolution of Projectile GameObject
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
            {
                Release(collision.ClosestPoint(transform.position) - launchDirection * 0.2f, fxOnDestroy);
            }
            else if(weaponHandler.Target.value == (weaponHandler.Target.value | (1 << collision.gameObject.layer)))
            {
                switch (weaponHandler.Character)
                {
                    case Character character:
                    {
                        var enemy = Helper.GetComponent_Helper<EnemyCharacter>(collision.gameObject);
                        if (enemy.IsInvincible || !enemy.IsAlive) return;
                        enemy.OnDamage(-character.Return_CalculatedDamage());
                        
                        if(weaponHandler.IsOnKnockback) enemy.ApplyKnockBack(transform, weaponHandler.KnockBackPower);
                        break;
                    }
                    case EnemyCharacter enemyCharacter:
                    {
                        var player = Helper.GetComponent_Helper<Character>(collision.gameObject);
                        if (player.IsInvincible || !player.IsAlive) return;
                        player.OnDamage(-enemyCharacter.Return_CalculatedDamage());
                        if(weaponHandler.IsOnKnockback) player.ApplyKnockBack(transform, weaponHandler.KnockBackPower);
                        break;
                    }
                }

                Release(collision.ClosestPoint(transform.position), fxOnDestroy);
            }
        }

        /// <summary>
        ///   탄환이 Destroy 될 때 대신 ObjectPool이 있다면 해당 객체를 Release하는 메소드
        ///   하위 클래스에서 상속시 꼭 재구현해야하는 메소드입니다.
        /// </summary>
        /// <param name="position">소멸 이펙트 생성위치</param>
        /// <param name="createFx">소멸 이펙트 생성여부</param>
        public virtual void Release(Vector3 position, bool createFx)
        {
            if (createFx) particleManager.CreateParticleAtPosition(position, ParticleType.ImpactParticle, weaponHandler);

            if (pool != null || projectileManager.ProjectilePool.TryGetValue(name, out pool)) pool.Release(this);
            else Destroy(gameObject);
        }
    }
}