using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Factories/Enemy Factory")]
public class EnemyFactory : GameObjectFactory
{
    [Serializable]
    private class EnemyConfig
    {
        public Enemy Prefab;
        [field: FloatRangeSlider(0.5f, 2.0f)] public FloatRange Scale = new FloatRange(1f);
        [field: FloatRangeSlider(-0.4f, 0.4f)] public FloatRange PathOffset = new FloatRange(0f);
        [field: FloatRangeSlider(0.1f, 5f)] public FloatRange Speed = new FloatRange(1f);
        [field: FloatRangeSlider(10f, 1000f)] public FloatRange Health = new FloatRange(100f);
    }

    [field: SerializeField] EnemyConfig _burrow;
    [field: SerializeField] EnemyConfig _fiery;
    [field: SerializeField] EnemyConfig _cyclop;

    public Enemy Get(EnemyType type)
    {
        var config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.Prefab);
        instance.OriginFactory = this;
        instance.Initialize(config.Scale.RandomValueInRange, 
            config.PathOffset.RandomValueInRange, 
            config.Speed.RandomValueInRange,
            config.Health.RandomValueInRange);
        return instance;
    }

    private EnemyConfig GetConfig(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Cyclop:
                return _cyclop;
            case EnemyType.Fiery:
                return _fiery;
            case EnemyType.Burrow:
                return _burrow;
        }

        Debug.LogError($"No config for type: {type}");
        return _fiery;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }
}

