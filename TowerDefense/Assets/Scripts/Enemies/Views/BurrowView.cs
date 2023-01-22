using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowView : EnemyView
{
    public override void OnDieAnimationFinished()
    {
        _enemy.Recycle();
    }
}
