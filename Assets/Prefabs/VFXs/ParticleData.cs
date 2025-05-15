using OnGame.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace OnGame.Prefabs.VFXs
{
    [CreateAssetMenu(fileName = "ParticleData", menuName = "OnGame/Particle Data", order = 0)]
    public class ParticleData : ScriptableObject
    {
        public GameObject prefab;
        
        public virtual ObjectPool<PooledParticle> CreatePool(Transform manager, Transform releasedContainer)
        {
            return new ObjectPool<PooledParticle>(() =>
                {
                    // create
                    var obj = Instantiate(prefab, releasedContainer);
                    obj.SetActive(false);
                    obj.name = prefab.name;
                    return Helper.GetComponent_Helper<PooledParticle>(obj);
                }, particle =>
                {
                    // get
                    particle.transform.SetParent(manager);
                    particle.gameObject.SetActive(true);
                }, particle =>
                {
                    // release
                    particle.transform.SetParent(releasedContainer);
                    particle.gameObject.SetActive(false);
                },
                particle =>
                {
                    // destroy
                    Destroy(particle.gameObject);
                });
        }
    }
}