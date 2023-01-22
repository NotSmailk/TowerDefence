using UnityEngine;

public abstract class Tower : GameTileContent
{
    [field: SerializeField, Range(1.5f, 10.5f)] protected float _targetingRange = 1.5f;

    public abstract GameTileContentType Type { get; }

    protected bool IsAcquireTarget(out TargetPoint target)
    {
        if (TargetPoint.FillBuffer(transform.localPosition, _targetingRange))
        {
            target = TargetPoint.GetBuffered(0).GetComponent<TargetPoint>();
            return true;
        }

        target = null;
        return false;
    }

    protected bool IsTargetTracked(ref TargetPoint target)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 myPos = transform.localPosition;
        Vector3 targetPos = target.Position;

        if (Vector3.Distance(myPos, targetPos) > _targetingRange + target.ColliderSize * target.Enemy.Scale || !target.IsEnabled)
        {
           target = null;
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.x += 0.01f;
        Gizmos.DrawWireSphere(position, _targetingRange);
    }
}
