/// <summary>
/// Slows the enemies in range, instead of damaging them
/// The power decides, how long this effect lasts
/// </summary>

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
