using UnityEngine;

/// <summary>
/// This struct is used to transport info from the tower itself, to the projectile fired
/// Main purpose is to differentiate between tiers
/// </summary>

public struct CurrentTower
{
    public int currentTier;
    public TowerInfo info;

    public CurrentTower(int currentTier, TowerInfo info)
    {
        this.currentTier = currentTier;
        this.info = info;
    }
}
