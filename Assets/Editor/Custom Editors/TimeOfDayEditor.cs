using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeOfDay))]
public class TimeOfDayEditor : Editor
{
    public void OnEnable()
    {
        instance = (TimeOfDay)target;
    }

    override public void OnInspectorGUI()
    {
        if (instance == null) { return; }

        DrawDefaultInspector();

        bool disableDay = !EditorApplication.isPlaying || instance.IsDay || instance.InProgress;
        if (disableDay) {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Day")) {
            instance.GotoDay();
        }
        GUI.enabled = true;

        bool disableNight = !EditorApplication.isPlaying || instance.IsNight || instance.InProgress;
        if (disableNight) {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Night")) {
            instance.GotoNight();
        }
        GUI.enabled = true;
    }

    private TimeOfDay instance;
}