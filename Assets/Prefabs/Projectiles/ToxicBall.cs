using System.Collections;
using System.Collections.Generic;
using OnGame.Prefabs.VFXs;
using UnityEngine;

namespace OnGame.Prefabs.Projectiles
{
    public class ToxicBall : Projectile
    {
        public override void Release(Vector3 position, bool createFx)
        {
            if (createFx) particleManager.CreateParticleAtPosition(position, ParticleType.ImpactParticle, weaponHandler);

            Debug.Log(name);
            if(pool != null) pool.Release(this);
            else if (projectileManager.ProjectilePool.TryGetValue(name, out var objectPool)) objectPool.Release(this);
            else Destroy(gameObject);
        }
    }
}
