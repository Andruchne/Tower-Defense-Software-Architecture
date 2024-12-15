using UnityEngine;

/// <summary>
/// Interface for anything that is supposed to move (e.g. enemies)
/// </summary>

public interface IMoveBehaviour
{
    void Move(float speed = 1);
    void Stop();
    bool GetDestinationReached();
    Vector3 GetNextPosition(float timeInSeconds);
}
