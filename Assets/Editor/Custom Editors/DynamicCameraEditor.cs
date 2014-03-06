using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicCamera))]
public class DynamicCameraEditor : Editor {
	public void OnEnable() {
		instance = (DynamicCamera)target;
		properties = Property.GetProperties(instance);
	}

	public override void OnInspectorGUI() {
		if (instance == null) {
			return;
		}

		DrawDefaultInspector();
		Property.ExposeProperties(properties);
		if (GUI.changed) {
			EditorUtility.SetDirty(instance);
		}
	}

	private DynamicCamera instance;
	private Property[] properties;
}