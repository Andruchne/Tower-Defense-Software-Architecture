using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TowerInfo))]
public class TowerInfoEditor : Editor
{
    private SerializedProperty _towerModelProp;
    private SerializedProperty _damageProp;
    private SerializedProperty _rangeProp;
    private SerializedProperty _attackCooldownProp;
    private SerializedProperty _effectRadiusProp;

    private void OnEnable()
    {
        _towerModelProp = serializedObject.FindProperty("towerModel");
        _damageProp = serializedObject.FindProperty("damage");
        _rangeProp = serializedObject.FindProperty("range");
        _attackCooldownProp = serializedObject.FindProperty("attackCooldown");
        _effectRadiusProp = serializedObject.FindProperty("effectRadius");

        // To synchronize existing scriptableObjects, whenever new values get added
        serializedObject.Update();
        MatchArraySizes();
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspectorFields();

        // Add a title or description before the arrays
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tower Model & Stat Configuration", EditorStyles.boldLabel);

        // Ensure arrays have the same size
        MatchArraySizes();
        // Draw custom array editors for towerModel and towerDamage
        DrawArrays();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultInspectorFields()
    {
        // Draw default inspector fields, except for the arrays
        EditorGUILayout.PropertyField(serializedObject.FindProperty("towerTypeName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileMoveType"));
    }

    private void MatchArraySizes()
    {
        if (_towerModelProp.arraySize != _damageProp.arraySize)
        {
            // Synchronize array sizes if they don't match
            int newSize = Mathf.Max(_towerModelProp.arraySize, _damageProp.arraySize);
            _towerModelProp.arraySize = newSize;
            _damageProp.arraySize = newSize;
            _rangeProp.arraySize = newSize;
            _attackCooldownProp.arraySize = newSize;
            _effectRadiusProp.arraySize = newSize;
        }
    }

    private void DrawArrays()
{
    if (_towerModelProp.arraySize > 0)
    {
        // Draw the labels above the array elements
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Tower Model", GUILayout.Width(200));
        EditorGUILayout.LabelField("Damage", GUILayout.Width(100));
        EditorGUILayout.LabelField("Range", GUILayout.Width(100));
        EditorGUILayout.LabelField("Attack Cooldown", GUILayout.Width(100));
        EditorGUILayout.LabelField("Effect Radius", GUILayout.Width(100));
        EditorGUILayout.LabelField("", GUILayout.Width(100)); // Placeholder for the "Remove" button
        EditorGUILayout.EndHorizontal();
    }

    // Draw the array fields and remove buttons
    for (int i = 0; i < _towerModelProp.arraySize; i++)
    {
        EditorGUILayout.BeginHorizontal();

        // Draw the elements of towerModel, damage, range, and attackCooldown
        EditorGUILayout.PropertyField(_towerModelProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(200));
        EditorGUILayout.PropertyField(_damageProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
        EditorGUILayout.PropertyField(_rangeProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
        EditorGUILayout.PropertyField(_attackCooldownProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
        EditorGUILayout.PropertyField(_effectRadiusProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));

        // Button to remove elements from all arrays
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            _towerModelProp.DeleteArrayElementAtIndex(i);
            _damageProp.DeleteArrayElementAtIndex(i);
            _rangeProp.DeleteArrayElementAtIndex(i);
            _attackCooldownProp.DeleteArrayElementAtIndex(i);
            _effectRadiusProp.DeleteArrayElementAtIndex(i);
        }

        EditorGUILayout.EndHorizontal();
    }

    // Button to add a new element to all arrays
    if (GUILayout.Button("Add New Tower Model & Damage"))
    {
        _towerModelProp.arraySize++;
        _damageProp.arraySize++;
        _rangeProp.arraySize++;
        _attackCooldownProp.arraySize++;
        _effectRadiusProp.arraySize++;
    }

    CheckForNullElements();
}

    private void CheckForNullElements()
    {
        for (int i = 0; i < _towerModelProp.arraySize; i++)
        {
            // Check if towerModel is not assigned
            if (_towerModelProp.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox($"towerModel at index {i} is null.", MessageType.Error);
            }
        }
    }
}