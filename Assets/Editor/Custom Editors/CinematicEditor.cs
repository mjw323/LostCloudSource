using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cinematic), true)]
public class CinematicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (instance == null) { return; }
        DrawDefaultInspector();
        if (!EditorApplication.isPlaying) { GUI.enabled = false; }
        if (instance.IsPlaying) { GUI.enabled = false; }
        if (GUILayout.Button("Play")) {
            instance.Play();
        }
        GUI.enabled = true;
    }

    private void OnEnable()
    {
        instance = (Cinematic)target;
    }

    private Cinematic instance;
}