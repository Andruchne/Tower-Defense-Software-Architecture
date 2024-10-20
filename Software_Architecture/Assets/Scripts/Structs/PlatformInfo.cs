using UnityEngine;

/// <summary>
/// This struct is used in PlatformEaser
/// Used to store the platform with it's initialPosition, before setback
/// </summary>

public struct PlatformInfo
{
    public GameObject gameObject;
    public Vector3 initialPosition;

    public PlatformInfo(GameObject gameObject, Vector3 initialPosition)
    {
        this.gameObject = gameObject;
        this.initialPosition = initialPosition;
    }
}
