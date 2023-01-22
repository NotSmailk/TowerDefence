using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    [field: SerializeField, Range(0.5f, 2.0f)] private float _shootsPerSecond = 1.0f;
    [field: SerializeField, Range(0.5f, 3.0f)] private float _shellBlastRadius = 1.0f;
    [field: SerializeField, Range(1.0f, 100.0f)] private float _damage = 10f;
    [field: SerializeField] private Transform _mortar;

    private float _launchSpeed;
    private float _launchProgress;

    private static readonly float _diffCoef = 10f;
    private static readonly float _gravityCoef = 9.81f;

    public override GameTileContentType Type => GameTileContentType.MortarTower;

    private void Awake()
    {
        CalculateLaunchSpeed();
    }

    private void OnValidate()
    {
        CalculateLaunchSpeed();
    }

    private void CalculateLaunchSpeed()
    {
        if (_mortar == null)
            return;

        float x = _targetingRange;
        float y = _mortar.position.y * -1f;
        _launchSpeed = Mathf.Sqrt(_gravityCoef * (y + Mathf.Sqrt(x * x + y * y)));
    }

    public override void GameUpdate()
    {
        _launchProgress += Time.deltaTime * _shootsPerSecond;
        while(_launchProgress > 1f)
        {
            if (IsAcquireTarget(out TargetPoint target))
            {
                Launch(target);
                _launchProgress -= 1f;
            }
            else 
            {
                _launchProgress = 0.9f;
            }
        }
    }

    private void Launch(TargetPoint target)
    {
        Vector3 launchPoint = _mortar.position;
        Vector3 targetPoint = target.Position;
        targetPoint.y = 0.0f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;

        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float g = _gravityCoef;
        float s = _launchSpeed ;
        float s2 = s * s;

        float r = Mathf.Abs(s2 * s2 - g * (g * x * x + 2f * y * s2));
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        _mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

        Vector3 launchVelocity = new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y);
        QuickGame.SpawnShell().Initialize(launchPoint, targetPoint, launchVelocity, _shellBlastRadius, _damage);
    }
}
