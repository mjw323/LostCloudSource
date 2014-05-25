using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skip))]
public class SkipEditor : Editor
{
	public void OnEnable()
	{
		instance = (Skip)target;
	}

	public override void OnInspectorGUI()
	{
		if (instance == null) { return; }

		DrawDefaultInspector();

		if (!EditorApplication.isPlaying) {
			GUI.enabled = false;
		}

		if (GUILayout.Button("Day One")) {
			instance.SkipDayOne ();
		}
		if (GUILayout.Button("Day Two")) {
			instance.SkipDayTwo ();
		}
		if (GUILayout.Button("Day Three")) {
			instance.SkipDayThree();
		}
		if (GUILayout.Button("Night")) {
			instance.SkipNight();
		}
		if (GUILayout.Button("Ending")) {
			instance.SkipEnding();
		}

		GUI.enabled = true;
	}

	private Skip instance;
}