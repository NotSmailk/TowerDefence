using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [field: SerializeField] private GameTileContentType _type;

    public bool IsBlockingPath => (int)Type > 50;
    public GameTileContentType Type => _type;
    public GameTileContentFactory OriginFactory { get; set; }

    public void Recycle()
    {
        OriginFactory.Reclaim(this);
    }

    public virtual void GameUpdate()
    {
        
    }
}
