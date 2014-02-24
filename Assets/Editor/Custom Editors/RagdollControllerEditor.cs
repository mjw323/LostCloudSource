using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RagdollController))]
public class RagdollControllerEditor : Editor {
	public void OnEnable() {
		instance = (RagdollController)target;
	}

	public override void OnInspectorGUI() {
		if (instance == null) {
			return;
		}

		DrawDefaultInspector();
		// Disable the use of this button outside of Play Mode.
		if (!EditorApplication.isPlaying) {
			GUI.enabled = false;
		}
		if (GUILayout.Button("Ragdoll")) {
			instance.DoRagdoll();
		}
		GUI.enabled = true;
	}

	private RagdollController instance;
}