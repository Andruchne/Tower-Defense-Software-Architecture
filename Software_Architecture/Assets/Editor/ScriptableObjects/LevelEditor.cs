using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private SerializedProperty _wavesProp;
    private SerializedProperty _intervalPerSpawnProp;

    private void OnEnable()
    {
        // Get references to the serialized properties
        _wavesProp = serializedObject.FindProperty("waves");
        _intervalPerSpawnProp = serializedObject.FindProperty("intervalPerSpawn");

        // To synchronize existing scriptableObjects, whenever new values get added
        serializedObject.Update();
        MatchArraySizes();
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Add a title or description for the arrays
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Wave Configuration", EditorStyles.boldLabel);

        // Ensure both arrays are the same size
        MatchArraySizes();
        DrawArrays();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultInspectorFields()
    {
        // Draw the default fields for "waves" and "intervalPerSpawn"
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waves"));
    }

    private void MatchArraySizes()
    {
        // Synchronize the sizes of the arrays
        int newSize = _wavesProp.arraySize;
        _intervalPerSpawnProp.arraySize = newSize;
    }

    private void DrawArrays()
    {
        if (_wavesProp.arraySize > 0)
        {
            // Draw headers for each array
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wave", GUILayout.Width(250));
            EditorGUILayout.LabelField("Interval Per Spawn", GUILayout.Width(250));
            EditorGUILayout.EndHorizontal();
        }

        // Draw the array fields for "waves" and "intervalPerSpawn"
        for (int i = 0; i < _wavesProp.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw each element for "waves" and "intervalPerSpawn"
            EditorGUILayout.PropertyField(_wavesProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(250));
            EditorGUILayout.PropertyField(_intervalPerSpawnProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(250));

            // Button to remove elements from both arrays
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                _wavesProp.DeleteArrayElementAtIndex(i);
                _intervalPerSpawnProp.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // Button to add a new wave and interval
        if (GUILayout.Button("Add New Wave"))
        {
            _wavesProp.arraySize++;
            _intervalPerSpawnProp.arraySize++;
        }

        CheckForNullElements();
    }

    private void CheckForNullElements()
    {
        // Check for null elements in the waves array
        for (int i = 0; i < _wavesProp.arraySize; i++)
        {
            if (_wavesProp.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox($"Wave at index {i} is null.", MessageType.Error);
            }
        }
    }
}
