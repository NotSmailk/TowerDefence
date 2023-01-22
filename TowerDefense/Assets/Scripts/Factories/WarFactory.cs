using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Factories/War Factory")]
public class WarFactory : GameObjectFactory
{
    [field: SerializeField] private Shell _shellPrefab;
    [field: SerializeField] protected Explosion _explosionPrefab;

    public Shell Shell => Get(_shellPrefab);
    public Explosion Explosion => Get(_explosionPrefab);

    public void Reclaim(WarEntity entity)
    {
        Destroy(entity.gameObject);
    }

    private T Get<T>(T prefab) where T : WarEntity
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}
