using System.Collections.Generic;
using OnGame.Prefabs.Items.Weapon.WeaponHandlers;
using OnGame.Prefabs.VFXs;
using UnityEngine;
using UnityEngine.Pool;

namespace OnGame.Prefabs.Projectiles
{
    public delegate ObjectPool<Projectile> ProjectilePoolGenerator(Transform rootContainer);
    public class ProjectileManager : MonoBehaviour
    {
        // Singleton
        public static ProjectileManager Instance { get; private set; }

        // Projectile Pool Manager
        public readonly Dictionary<string, ObjectPool<Projectile>> ProjectilePool = new();
        
        // Prefabs Fields
        [Header("Prefabs")]
        [SerializeField] private ProjectileData[] projectileDataList;
        
        [Header("Particle Manager")]
        [SerializeField] private ParticleManager particleManager;
        
        private void Awake()
        {
            if (Instance == null)
            {
                // 여기서 인스턴스 초기화 스크립트 작성
                Instance = this;

                // 프리팹 기반으로 오브젝트 풀을 만드는 코드에요
                // 꼭 프리팹 루트에 Projectile을 상속하는 컴포넌트가 붙어있어야되요!
                foreach (var data in projectileDataList)
                {
                    var releasedContainer = new GameObject("ReleasedProjectile_" + data.name);

                    releasedContainer.transform.SetParent(transform);
                    var pool = data.CreatePool(transform, releasedContainer.transform);
                    ProjectilePool[data.prefab.name] = pool;
                    Debug.Log("Projectile Pool Created : " + data.name);
                }
            }
            else Destroy(gameObject);
        }

        public void ShootBullet(RangeWeaponHandler weaponHandler, Vector2 startPos, Vector2 direction)
        {
            var origin = projectileDataList[weaponHandler.BulletIndex];

            var projectile = ProjectilePool[origin.name].Get();
            projectile.transform.position = startPos;
            projectile.transform.rotation = Quaternion.identity;
            projectile.transform.SetParent(transform);
            
            projectile.Init(direction, weaponHandler, this, particleManager);
        }
    }
}