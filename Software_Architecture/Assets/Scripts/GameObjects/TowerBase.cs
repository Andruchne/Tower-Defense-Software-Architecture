using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    // Life
    private float Life
    {
        get
        {
            return life;
        }
        set
        {
            life = value;
            Mathf.Clamp(life, 0, maxLife);
        }
    }

    private float life;
    private float maxLife;

    // Gold
    private int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            if (gold < 0) { gold = 0; }
        }
    }

    private int gold;

    // Getter methods
    // Should only be used by the GameManager
    public float GetLife()
    {
        return Life;
    }

    public int GetGold()
    {
        return Gold;
    }
}
