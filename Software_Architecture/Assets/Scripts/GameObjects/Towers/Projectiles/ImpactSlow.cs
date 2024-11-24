using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSlow : ImpactDamage
{
    protected override void EnemyEntered(ITargetable target)
    {
        Enemy enemy = (Enemy)target;

        if (enemy != null)
        {
            enemy.MultiplySpeed(0.5f, _currentTower.info.power[_currentTower.currentTier]);
        }
    }
}
