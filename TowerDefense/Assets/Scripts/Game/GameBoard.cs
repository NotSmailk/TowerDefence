using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [field: SerializeField] private Transform _ground;
    [field: SerializeField] private GameTile _tilePrefab;

    private Vector2Int _size;
    private GameTile[] _tiles;
    private Queue<GameTile> _searchFrontier = new Queue<GameTile>();
    private GameTileContentFactory _contentFactory;
    private List<GameTile> _spawnpoints = new List<GameTile>();
    private List<GameTileContent> _contentToUpdate = new List<GameTileContent>();

    public int SpawnpointsCount => _spawnpoints.Count;

    public void Initialze(Vector2Int size, GameTileContentFactory contentFactory)
    {
        if (_ground.TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.enabled = false;

        _size = size;
        _ground.localScale = new Vector3(size.x, 1f, size.y);

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);

        _tiles = new GameTile[size.x * size.y];
        _contentFactory = contentFactory;

        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = _tiles[i] = Instantiate(_tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, _tiles[i - 1]);
                }

                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, _tiles[i - size.x]);
                }

                tile.IsAlternative = (x & 1) == 0;

                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;   
                }
            }
        }

        Clear();
    }

    public void GameUpdate()
    {
        for (int i = 0; i < _contentToUpdate.Count; i++)
        {
            _contentToUpdate[i].GameUpdate();
        }
    }

    public bool FindPaths()
    {
        foreach (var tile in _tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                _searchFrontier.Enqueue(tile);
            }
            else 
            {
                tile.ClearPath();
            }
        }

        if (_searchFrontier.Count == 0)
            return false;

        while (_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();

            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchFrontier.Enqueue(tile.GrowPathSouth());
                    _searchFrontier.Enqueue(tile.GrowPathNorth());
                    _searchFrontier.Enqueue(tile.GrowPathWest());
                    _searchFrontier.Enqueue(tile.GrowPathEast());
                }
                else
                {
                    _searchFrontier.Enqueue(tile.GrowPathWest());
                    _searchFrontier.Enqueue(tile.GrowPathEast());
                    _searchFrontier.Enqueue(tile.GrowPathSouth());
                    _searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        foreach (var tile in _tiles)
        {
            if (!tile.HasPath)
                return false;
        }

        foreach (var tile in _tiles)
        {
            tile.ShowPath();
        }

        return true;
    }

    public void Build(GameTile tile, GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination:
                BuildDestination(tile);
                break;
            case GameTileContentType.Spawnpoint:
                BuildSpawnPoint(tile);
                break;
            case GameTileContentType.Wall:
                BuildWall(tile);
                break;
            case GameTileContentType.LaserTower:
                BuildTower(tile, type);
                break;
            case GameTileContentType.MortarTower:
                BuildTower(tile, type);
                break;
        }
    }

    private void BuildDestination(GameTile tile)
    {
        if (tile.Content.Type != GameTileContentType.Empty)
            return;

        tile.Content = _contentFactory.Get(GameTileContentType.Destination);
        FindPaths();
    }

    private void BuildSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type != GameTileContentType.Empty)
            return;

        tile.Content = _contentFactory.Get(GameTileContentType.Spawnpoint);
        _spawnpoints.Add(tile);
    }

    private void BuildWall(GameTile tile)
    {
        if (tile.Content.Type != GameTileContentType.Empty)
            return;

        tile.Content = _contentFactory.Get(GameTileContentType.Wall);
        if (FindPaths() == false)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
    }

    private void BuildTower(GameTile tile, GameTileContentType type)
    {
        if (tile.Content.Type != GameTileContentType.Empty || type <= GameTileContentType.BeforeAttackers)
            return;

        tile.Content = _contentFactory.Get(type);
        if (FindPaths())
        {
            _contentToUpdate.Add(tile.Content);
        }
        else
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
    }

    private void DestroyDestination(GameTile tile)
    {
        if (tile.Content.Type != GameTileContentType.Destination)
            return;

        tile.Content = _contentFactory.Get(GameTileContentType.Empty);
        if (FindPaths() == false)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    private void DestroySpawnPoint(GameTile tile)
    {
        if (tile.Content.Type != GameTileContentType.Spawnpoint)
            return;
        if (_spawnpoints.Count <= 1)
            return;

        _spawnpoints.Remove(tile);
        tile.Content = _contentFactory.Get(GameTileContentType.Empty);
    }

    private void DestroyWall(GameTile tile)
    {
        if (tile.Content.Type != GameTileContentType.Wall)
            return;

        tile.Content = _contentFactory.Get(GameTileContentType.Empty);
        FindPaths();
    }

    private void DestroyTower(GameTile tile)
    {
        if (tile.Content.Type <= GameTileContentType.BeforeAttackers)
            return;

        _contentToUpdate.Remove(tile.Content);
        tile.Content = _contentFactory.Get(GameTileContentType.Empty);
        FindPaths();
    }


    public GameTile GetTile(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + _size.x * 0.5f);
            int y = (int)(hit.point.z + _size.y * 0.5f);
            if (x >= 0 && x < _size.x && y >= 0 && y < _size.y)
            {
                return _tiles[x + y * _size.x];
            }
        }
        return null;
    }

    public GameTile GetSpawnpoint(int index)
    {
        return _spawnpoints[index];
    }

    public void Clear()
    {
        foreach (GameTile tile in _tiles)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
        }
        _spawnpoints.Clear();
        _contentToUpdate.Clear();
        BuildDestination(_tiles[_tiles.Length / 2]);
        BuildSpawnPoint(_tiles[0]);
    }
}

