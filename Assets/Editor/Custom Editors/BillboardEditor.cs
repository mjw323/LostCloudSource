using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Billboard))]
public class BillboardEditor : Editor
{
    public override void OnInspectorGUI() {
        if (instance == null) { return; }
        DrawDefaultInspector();
        if (!EditorApplication.isPlaying) {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Show")) {
            instance.Show();
        }
        if (GUILayout.Button("Hide")) {
            instance.Hide();
        }
        GUI.enabled = true;
    }

    private void OnEnable()
    {
        instance = (Billboard)target;
    }

    private Billboard instance;
}