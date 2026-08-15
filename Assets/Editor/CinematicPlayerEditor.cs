using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinematicPlayer))]
public class CinematicPlayerEditor : Editor
{
    private SerializedProperty _stepsProp;

    private void OnEnable()
    {
        _stepsProp = serializedObject.FindProperty("_steps");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Cinematic Steps", EditorStyles.boldLabel);

        for (int i = 0; i < _stepsProp.arraySize; i++)
        {
            DrawStep(_stepsProp.GetArrayElementAtIndex(i), i);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Step"))
        {
            _stepsProp.InsertArrayElementAtIndex(_stepsProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawStep(SerializedProperty step, int index)
    {
        var type = step.FindPropertyRelative("Type");
        var stepType = (CinematicStepType)type.enumValueIndex;

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Step {index}: {stepType}", EditorStyles.boldLabel);
        if (GUILayout.Button("Remove", GUILayout.Width(70)))
        {
            _stepsProp.DeleteArrayElementAtIndex(index);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(type);

        switch (stepType)
        {
            case CinematicStepType.SwitchCamera:
            case CinematicStepType.RestoreCamera:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("Camera"));
                break;
            case CinematicStepType.ZoomCamera:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("Camera"));
                EditorGUILayout.PropertyField(step.FindPropertyRelative("FloatA"), new GUIContent("Target Size"));
                EditorGUILayout.PropertyField(step.FindPropertyRelative("FloatB"), new GUIContent("Duration"));
                break;
            case CinematicStepType.Wait:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("FloatA"), new GUIContent("Seconds"));
                break;
            case CinematicStepType.WalkPlayer:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("FloatA"), new GUIContent("Direction (1 / -1)"));
                break;
            case CinematicStepType.TurnPlayer:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("FloatA"), new GUIContent("Face Right? (>0=yes)"));
                break;
            case CinematicStepType.PlayDialogue:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("Dialogue"), true);
                break;
            case CinematicStepType.Bark:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("BarkSpeaker"));
                EditorGUILayout.PropertyField(step.FindPropertyRelative("BarkText"));
                EditorGUILayout.PropertyField(step.FindPropertyRelative("FloatA"), new GUIContent("Duration"));
                break;
            case CinematicStepType.Shake:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("ImpulseSource"));
                break;
            case CinematicStepType.OpenFakeWall:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("Wall"));
                break;
            case CinematicStepType.StartCaveIn:
                EditorGUILayout.PropertyField(step.FindPropertyRelative("CaveIn"));
                break;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
}