using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// Easy way to add new attack types.
/// Serves not only for descriptive purposes, but also for checking what kind of attack to do as tower.
/// </summary>

[CreateAssetMenu(fileName = "New AttackType", menuName = "ScriptableObjects/AttackType")]
public class AttackType : ScriptableObject
{
    public string attackType;
    public string attackTypeDescription;
}
