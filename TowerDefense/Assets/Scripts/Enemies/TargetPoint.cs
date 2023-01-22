using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TargetPoint : MonoBehaviour
{
    public bool IsEnabled { get; set; } = false;
    public float ColliderSize { get; private set; }
    public Enemy Enemy { get; private set; }
    public Vector3 Position => transform.position;

    private const int EnemyLayerMask = 1 << 9;

    private static Collider[] _buffer = new Collider[100];
    public static int BufferedCount { get; private set; }

    private void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        ColliderSize = GetComponent<SphereCollider>().radius * transform.localScale.x;
    }

    public static bool FillBuffer(Vector3 posistion, float range)
    {
        Vector3 top = posistion;
        top.y += 3f;
        BufferedCount = Physics.OverlapCapsuleNonAlloc(posistion, top, range, _buffer, EnemyLayerMask);
        return BufferedCount > 0;
    }

    public static TargetPoint GetBuffered(int index)
    {
        var target = _buffer[index].GetComponent<TargetPoint>();
        return target;
    }
}
