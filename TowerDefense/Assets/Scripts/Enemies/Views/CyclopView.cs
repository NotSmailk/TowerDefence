using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopView : EnemyView
{
    public override void OnDieAnimationFinished()
    {
        _enemy.Recycle();
    }
}
