
using UnityEngine;
using UnityEditor;

/// <summary>
/// This script formats the TowerInfo scriptable objects
/// </summary>

[CustomEditor(typeof(TowerInfo))]
public class TowerInfoEditor : Editor
{
    private SerializedProperty _towerModelProp;
    private SerializedProperty _powerProp;
    private SerializedProperty _rangeProp;
    private SerializedProperty _attackCooldownProp;
    private SerializedProperty _effectRadiusProp;
    private SerializedProperty _costProp;

    private void OnEnable()
    {
        _towerModelProp = serializedObject.FindProperty("towerModel");
        _powerProp = serializedObject.FindProperty("power");
        _rangeProp = serializedObject.FindProperty("range");
        _attackCooldownProp = serializedObject.FindProperty("attackCooldown");
        _effectRadiusProp = serializedObject.FindProperty("effectRadius");
        _costProp = serializedObject.FindProperty("cost");

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

        MatchArraySizes();
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
        // Synchronize array sizes
        int newSize = Mathf.Max(_towerModelProp.arraySize, _powerProp.arraySize);
        _towerModelProp.arraySize = newSize;
        _powerProp.arraySize = newSize;
        _rangeProp.arraySize = newSize;
        _attackCooldownProp.arraySize = newSize;
        _effectRadiusProp.arraySize = newSize;
        _costProp.arraySize = newSize;
    }

    private void DrawArrays()
    {
        if (_towerModelProp.arraySize > 0)
        {
            // Draw the labels above the array elements
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tower Model", GUILayout.Width(200));
            EditorGUILayout.LabelField("Power", GUILayout.Width(100));
            EditorGUILayout.LabelField("Range", GUILayout.Width(100));
            EditorGUILayout.LabelField("Attack Cooldown", GUILayout.Width(100));
            EditorGUILayout.LabelField("Effect Radius", GUILayout.Width(100));
            EditorGUILayout.LabelField("Cost", GUILayout.Width(100));

            // Placeholder for the "Remove" button
            EditorGUILayout.LabelField("", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
        }

        // Draw the array fields and buttons
        for (int i = 0; i < _towerModelProp.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw the elements
            EditorGUILayout.PropertyField(_towerModelProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(200));
            EditorGUILayout.PropertyField(_powerProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
            EditorGUILayout.PropertyField(_rangeProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
            EditorGUILayout.PropertyField(_attackCooldownProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
            EditorGUILayout.PropertyField(_effectRadiusProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));
            EditorGUILayout.PropertyField(_costProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(100));

            // Button to remove elements from all arrays
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _towerModelProp.DeleteArrayElementAtIndex(i);
                _powerProp.DeleteArrayElementAtIndex(i);
                _rangeProp.DeleteArrayElementAtIndex(i);
                _attackCooldownProp.DeleteArrayElementAtIndex(i);
                _effectRadiusProp.DeleteArrayElementAtIndex(i);
                _costProp.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // Button to add new element
        if (GUILayout.Button("Add New Tower Tier"))
        {
            _towerModelProp.arraySize++;
            _powerProp.arraySize++;
            _rangeProp.arraySize++;
            _attackCooldownProp.arraySize++;
            _effectRadiusProp.arraySize++;
            _costProp.arraySize++;
        }

        CheckForNullElements();
    }

    private void CheckForNullElements()
    {
        for (int i = 0; i < _towerModelProp.arraySize; i++)
        {
            if (_towerModelProp.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox($"TowerModel at index {i} is null.", MessageType.Error);
            }
        }
    }
}