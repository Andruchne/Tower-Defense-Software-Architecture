using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveBehaviour
{
    void Move(float speed = 1);
    Vector3 GetCurrentVelocity();
}
