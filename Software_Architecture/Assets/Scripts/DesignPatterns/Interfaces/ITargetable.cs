using System;
using UnityEngine;

/// <summary>
/// Interface, used for attacking/targeting
/// Any class which is supposed to be able to be targeted and hit, should implement this
/// </summary>

public interface ITargetable
{
    event Action<ITargetable> OnTargetDestroyed;

    void Hit(float damage);
    void Defeated();

    // This is only supposed to work for short time predictions
    Vector3 GetNextPosition(float timeInSeconds);
}