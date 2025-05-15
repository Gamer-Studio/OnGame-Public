using System;
using System.Collections;
using System.Collections.Generic;
using OnGame.Prefabs.Items.Weapon.WeaponHandlers;
using OnGame.Prefabs.VFXs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace OnGame
{
    public class PooledParticle : MonoBehaviour
    {
        // Pooling
        private static ObjectPool<PooledParticle> pool;
        
        [SerializeField] private ParticleManager particleManager;
        [SerializeField] private ParticleSystem particle;

        public void Init(RangeWeaponHandler weaponHandler, ParticleManager manager)
        {
            particleManager = manager;
            
            // Set Particle
            var em = particle.emission;
            em.SetBurst(0, new ParticleSystem.Burst(0, Mathf.Ceil(weaponHandler.BulletSize * 5)));
            var mainModule = particle.main;
            mainModule.startSpeedMultiplier = weaponHandler.BulletSize * 10f;
            
            // Play Particle
            particle.Play();
        }
        
        private void OnParticleSystemStopped()
        {
            if(pool != null || particleManager.ParticlePool.TryGetValue(name, out pool)) pool.Release(this);
            else Destroy(gameObject);
        }
    }
}
