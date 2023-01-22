using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Explosion : WarEntity
{
    [field: SerializeField, Range(0f, 1f)] private float _duration = 0.5f;

    private float _age;
    private static MaterialPropertyBlock _propertyBlock;

    public void Initialize(Vector3 position, float blastRadius, float damage = 0f)
    {
        if (damage > 0f)
        {
            TargetPoint.FillBuffer(position, blastRadius);
            for (int i = 0; i < TargetPoint.BufferedCount; i++)
            {
                TargetPoint.GetBuffered(i).Enemy.TakeDamage(damage);
            }
        }        

        transform.localPosition = position;
    }

    public override bool GameUpdate()
    {
        _age += Time.deltaTime;
        if (_age >= _duration)
        {
            Recycle();
            return false;
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        return true;
    }
}
