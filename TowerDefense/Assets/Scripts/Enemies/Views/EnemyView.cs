using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyView : MonoBehaviour
{
    protected Animator _animator;
    protected private Enemy _enemy;

    public bool IsInited { get; set; }

    protected const string DIED_KEY = "Die";
    protected const string WALK_KEY = "Walk Forward";

    public virtual void Init(Enemy enemy)
    {
        _animator = GetComponent<Animator>();
        _enemy = enemy;
    }

    public virtual void Die()
    {
        _animator.SetBool(WALK_KEY, false);
        _animator.SetBool(DIED_KEY, true);
    }

    public virtual void OnDieAnimationFinished() { }

    public void OnSpawnAnimationFinished()
    {
        IsInited = true;
        _animator.SetBool(WALK_KEY, true);
        GetComponent<TargetPoint>().IsEnabled = true;
    }
}
