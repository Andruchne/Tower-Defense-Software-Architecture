using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ExtendedButton))]
public class ExtendedButtonEditor : ButtonEditor
{
    SerializedProperty hoverSound;
    SerializedProperty clickSound;

    protected override void OnEnable()
    {
        base.OnEnable();
        hoverSound = serializedObject.FindProperty("hoverSound");
        clickSound = serializedObject.FindProperty("clickSound");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Button Sounds", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(hoverSound);
        EditorGUILayout.PropertyField(clickSound);

        serializedObject.ApplyModifiedProperties();
    }
}