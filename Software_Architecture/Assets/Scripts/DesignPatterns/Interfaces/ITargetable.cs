using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    void Hit();
    void Destroyed();

    // This is only supposed to work for short time predictions
    Vector3 GetNextPosition(float timeInSeconds);
}
