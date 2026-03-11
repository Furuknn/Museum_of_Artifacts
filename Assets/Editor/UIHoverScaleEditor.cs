using UnityEngine;
using UnityEditor;
using DG.Tweening;

[CustomEditor(typeof(UI_Hover_Scale))]
public class UIHoverScaleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UI_Hover_Scale script = (UI_Hover_Scale)target;

        EditorGUILayout.Space(8);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Preview Hover", GUILayout.Height(30)))
        {
            DOTween.Init();
            script.EditorPreviewHover();
        }

        if (GUILayout.Button("Preview Return", GUILayout.Height(30)))
        {
            DOTween.Init();
            script.EditorPreviewReturn();
        }

        EditorGUILayout.EndHorizontal();
    }
}