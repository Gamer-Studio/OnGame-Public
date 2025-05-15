using OnGame.Prefabs.Entities;
using OnGame.Prefabs.Projectiles;
using UnityEngine;

namespace OnGame.Prefabs.Items.Weapon.WeaponHandlers
{
    public class RangeWeaponHandler : WeaponHandler
    {
        [Header("Weapon Type Settings")] 
        [SerializeField] private bool isMagicWeapon;
        [SerializeField] private int manaCost;
        
        [Header("Ranged Attack Data")]
        [SerializeField] private Transform projectileSpawnPosition;
        [SerializeField] private int bulletIndex;
        [SerializeField] private float bulletSize = 1f;
        [SerializeField] private float duration;
        [SerializeField] private float spread;
        [SerializeField] private int numberOfProjectilesPerShot;
        [SerializeField] private float multipleProjectileAngle;
        [SerializeField] private Color projectileColor;
        protected ProjectileManager projectileManager;

        public int BulletIndex => bulletIndex;
        public float BulletSize => bulletSize;
        public float Duration => duration;
        public float Spread { get => spread; set => spread = value; }
        public int NumberOfProjectilesPerShot { get => numberOfProjectilesPerShot; set => numberOfProjectilesPerShot = value; }
        public float MultipleProjectileAngle { get => multipleProjectileAngle; set => multipleProjectileAngle = value; }

        public Color ProjectileColor => projectileColor;

        protected override void Start()
        {
            base.Start();
            projectileManager = ProjectileManager.Instance;
        }

        protected override void OnAttackAfterDelay()
        {
            var projectileAngleSpace = multipleProjectileAngle;
            var numberOfProjectilePerShot = numberOfProjectilesPerShot;
            var minAngle = -(numberOfProjectilePerShot / 2f) * projectileAngleSpace;

            for(var i = 0; i < numberOfProjectilePerShot; i++)
            {
                var angle = minAngle + projectileAngleSpace * i;
                var randomSpread = Random.Range(-spread, spread);
                angle += randomSpread;
                
                switch (Character)
                {
                    case Character player:
                        if (isMagicWeapon)
                        {
                            if (player.Mana.Value < manaCost) { Debug.Log("Warning! 마나 부족"); return; } 
                            player.OnManaConsume(manaCost);
                        }
                        CreateProjectile(player.LookAtDirection, angle);
                        break;
                    case EnemyCharacter enemy:
                        CreateProjectile(enemy.LookAtDirection, angle);
                        break;
                }
            }
        }

        protected void CreateProjectile(Vector2 lookDirection, float angle)
        {
            projectileManager.ShootBullet(this, projectileSpawnPosition.position, RotateVector2(lookDirection, angle));
        }

        private static Vector2 RotateVector2(Vector2 v, float degree)
        {
            return Quaternion.Euler(0, 0, degree) * v;
        }
    }
}