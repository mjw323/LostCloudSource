using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RagdollController))]
public class RagdollControllerEditor : Editor {
	public void OnEnable() {
		instance = (RagdollController)target;
		controlled = instance.GetComponent<Ragdoll>();
	}

	public override void OnInspectorGUI() {
		if (instance == null) {
			return;
		}

		DrawDefaultInspector();

		if (!EditorApplication.isPlaying || controlled.IsRagdolled) {
			GUI.enabled = false;
		}
		if (GUILayout.Button("Ragdoll")) {
			instance.Ragdoll();
		}
		GUI.enabled = true;

		if (!EditorApplication.isPlaying || controlled.IsGettingUp ||
		    !controlled.IsRagdolled) {
			GUI.enabled = false;
		}
		if (GUILayout.Button("Get Up")) {
			instance.GetUp();
		}
		GUI.enabled = true;
	}

	private RagdollController instance;
	private Ragdoll controlled;
}