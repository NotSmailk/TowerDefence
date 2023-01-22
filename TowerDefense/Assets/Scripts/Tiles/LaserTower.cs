using UnityEngine;

public class LaserTower : Tower
{
    [field: SerializeField, Range(1.0f, 100.0f)] private float _damagePerSecond = 10.0f;
    [field: SerializeField] private Transform _turret;
    [field: SerializeField] private Transform _laserBeam;

    private Vector3 _laserBeamScale;
    private TargetPoint _target;

    public override GameTileContentType Type => GameTileContentType.LaserTower;

    private void Awake()
    {
        _laserBeamScale = _laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (IsTargetTracked(ref _target) || IsAcquireTarget(out _target))
        {
            Shoot();
        }
        else
        {
            _laserBeam.localScale = Vector3.zero;
        }
    }

    private void Shoot()
    {
        var point = _target.Position;
        _turret.LookAt(point);
        _laserBeam.localRotation = _turret.localRotation;

        var distance = Vector3.Distance(_turret.position, point);
        _laserBeamScale.z = distance;
        _laserBeam.localScale = _laserBeamScale;
        _laserBeam.position = _turret.position + 0.5f * distance * _laserBeam.forward;

        _target.Enemy.TakeDamage(_damagePerSecond * Time.deltaTime);
    }

}
