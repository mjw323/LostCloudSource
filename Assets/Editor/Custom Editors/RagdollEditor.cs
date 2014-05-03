using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ragdoll))]
public class RagdollEditor : Editor {
	public void OnEnable() {
		instance = (Ragdoll)target;
	}

	public override void OnInspectorGUI() {
		if (instance == null) {
			return;
		}

		DrawDefaultInspector();

		if (!EditorApplication.isPlaying || instance.IsRagdolled) {
			GUI.enabled = false;
		}
		if (GUILayout.Button("Ragdoll")) {
			instance.DoRagdoll();
		}
		GUI.enabled = true;

		if (!EditorApplication.isPlaying || !instance.IsRagdolled) {
			GUI.enabled = false;
		}
		if (GUILayout.Button("Get Up")) {
			instance.GetUp();
		}
		GUI.enabled = true;
	}

	private Ragdoll instance;
}