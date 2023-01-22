using UnityEngine;

[CreateAssetMenu(menuName = "Factories/Game Tile Content Factory")]
public class GameTileContentFactory : GameObjectFactory
{
    [field: SerializeField] private GameTileContent _destinationPrefab;
    [field: SerializeField] private GameTileContent _wallPrefab;
    [field: SerializeField] private GameTileContent _spawnpointPrefab;
    [field: SerializeField] private GameTileContent _emptyPrefab;
    [field: SerializeField] private Tower _laserTowerPrefab;
    [field: SerializeField] private Tower _mortarTowerPrefab;

    public void Reclaim(GameTileContent content)
    { 
        Destroy(content.gameObject);
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination:
                return Get(_destinationPrefab);
            case GameTileContentType.Empty:
                return Get(_emptyPrefab);
            case GameTileContentType.Wall:
                return Get(_wallPrefab);
            case GameTileContentType.Spawnpoint:
                return Get(_spawnpointPrefab);
            case GameTileContentType.LaserTower:
                return Get(_laserTowerPrefab);
            case GameTileContentType.MortarTower:
                return Get(_mortarTowerPrefab);
        }

        return null;
    }

    private T Get<T>(T prefab) where T : GameTileContent
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }
}
