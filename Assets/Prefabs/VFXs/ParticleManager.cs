using System;
using System.Collections.Generic;
using OnGame.Prefabs.Items.Weapon.WeaponHandlers;
using UnityEngine;
using UnityEngine.Pool;

namespace OnGame.Prefabs.VFXs
{
    public enum ParticleType
    {
        ImpactParticle,
    }
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }
        public readonly Dictionary<string, ObjectPool<PooledParticle>> ParticlePool = new();
        
        [Header("Prefabs")]
        [SerializeField] private ParticleData[] particleDataList;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                foreach (var data in particleDataList)
                {
                    var releasedContainer = new GameObject("ReleasedParticle_" + data.name);

                    releasedContainer.transform.SetParent(transform);
                    var pool = data.CreatePool(transform, releasedContainer.transform);
                    ParticlePool[data.prefab.name] = pool;
                    Debug.Log("Particle Pool Created : " + data.name);
                }
            } else Destroy(gameObject);
        }

        public void CreateParticleAtPosition(Vector3 position, ParticleType particleType,
            RangeWeaponHandler weaponHandler)
        {
            // Get Particle from Pool
            var origin = particleDataList[(int) particleType];
            var particle = ParticlePool[origin.name].Get();
            particle.transform.SetPositionAndRotation(position, Quaternion.identity);
            particle.transform.SetParent(transform);
            
            particle.Init(weaponHandler, this);
        }
    }
}