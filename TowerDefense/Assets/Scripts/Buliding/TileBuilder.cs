using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuilder : MonoBehaviour
{
    [field: SerializeField] private List<BuildButton> _buttons;

    private bool _isEnabled;
    private GameTileContentFactory _contentFactory;
    private Camera _camera;
    private GameBoard _gameBoard;
    private GameTileContent _pendingTile;

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);

    private void Awake()
    {
        _buttons.ForEach(b => b.AddListener(OnBuildingSelected));
    }

    public void Initialize(GameTileContentFactory contentFactory, Camera camera, GameBoard gameBoard)
    {
        _contentFactory = contentFactory;
        _camera = camera;
        _gameBoard = gameBoard;
    }

    private void Update()
    {
        if (!_isEnabled || _pendingTile == null)
            return;

        var plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(TouchRay, out var position))
        {
            _pendingTile.transform.position = TouchRay.GetPoint(position);
        }

        if (IsPointerUp())
        {
            var tile = _gameBoard.GetTile(TouchRay);
            if (tile != null && tile.Content.Type == GameTileContentType.Empty)
                _gameBoard.Build(tile, _pendingTile.Type);

            Destroy(_pendingTile.gameObject);
            _pendingTile = null;
        }
    }

    private bool IsPointerUp()
    {
        #if UNITY_EDITOR
        return Input.GetMouseButtonUp(0);
        #else
        return Input.touches.Length == 0;
        #endif
    }

    public void Enable()
    {
        _isEnabled = true;
    }

    public void Disable()
    {
        _isEnabled = false;
    }

    private void OnBuildingSelected(GameTileContentType type)
    {
        _pendingTile = _contentFactory.Get(type);
    }
}
