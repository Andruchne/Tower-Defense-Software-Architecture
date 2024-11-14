using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    public event Action<ITargetable> OnTargetDestroyed;

    void Hit(float damage);
    void Defeated();

    // This is only supposed to work for short time predictions
    Vector3 GetNextPosition(float timeInSeconds);
}
