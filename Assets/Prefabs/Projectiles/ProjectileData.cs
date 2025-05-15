using OnGame.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace OnGame.Prefabs.Projectiles
{
  [CreateAssetMenu(fileName = "new ProjectTile", menuName = "OnGame/ProjectTile Data", order = 0)]
  public class ProjectileData : ScriptableObject
  {
    public GameObject prefab;
    
    /// <summary>
    /// 프리팹기반으로 ObjectPool 생성시 사용하는 메소드, 인스턴스된 게임오브젝트에는 사용하지 마세요!
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="releasedContainer">Release될 시 담기는 transform</param>
    /// <returns></returns>
    public virtual ObjectPool<Projectile> CreatePool(Transform manager, Transform releasedContainer)
    {
      return new ObjectPool<Projectile>(() =>
        {
          // create
          var obj = Instantiate(prefab, releasedContainer);
          obj.SetActive(false);
          obj.name = prefab.name;
          return Helper.GetComponent_Helper<Projectile>(obj);
        }, projectile =>
        {
          // get
          projectile.transform.SetParent(manager);
          projectile.gameObject.SetActive(true);
        }, projectile =>
        {
          // release
          projectile.transform.SetParent(releasedContainer);
          projectile.gameObject.SetActive(false);
        },
        projectile =>
        {
          // destroy
          Destroy(projectile.gameObject);
        });
    }
  }
}