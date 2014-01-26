using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Terrain)),CanEditMultipleObjects]
public class TerrainEditor : Editor {
	SerializedProperty offset;
	SerializedProperty xaxis;
	SerializedProperty yaxis;
	SerializedProperty zaxis;

	SerializedProperty groundscale;
	SerializedProperty wall1scale;
	SerializedProperty wall2scale;
	SerializedProperty rimpower;

	SerializedProperty ground;
	SerializedProperty wall1;
	SerializedProperty wall2;
	SerializedProperty ramp;

	SerializedProperty rimColor;

	// Use this for initialization
	void OnEnable() {
		// Floats
		offset = serializedObject.FindProperty("BlendHeightOffset");

		xaxis = serializedObject.FindProperty("xWeight");
		yaxis = serializedObject.FindProperty("yWeight");
		zaxis = serializedObject.FindProperty("zWeight");

		groundscale = serializedObject.FindProperty("groundScale");
		wall1scale = serializedObject.FindProperty("wallScale");
		rimpower = serializedObject.FindProperty("rimPower");

		rimColor = serializedObject.FindProperty("rimColor");

		// Textures
		ground = serializedObject.FindProperty("groundTexture");
		wall1 = serializedObject.FindProperty("wallTexture");
		ramp = serializedObject.FindProperty("ramp");
	}
	
	// Update is called once per frame
	public override void OnInspectorGUI() {
		serializedObject.Update();

		// Offset Slider
		EditorGUILayout.Slider(offset,0.0f,40.0f,new GUIContent("Blend Height Offset"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// Axis Sliders
		EditorGUILayout.Slider(xaxis,0.0f,1.0f,new GUIContent("X-Axis Blend Weight"));
		EditorGUILayout.Slider(yaxis,0.0f,1.0f,new GUIContent("Y-Axis Blend Weight"));
		EditorGUILayout.Slider(zaxis,0.0f,1.0f,new GUIContent("Z-Axis Blend Weight"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// Texture Scale Sliders
		EditorGUILayout.Slider(groundscale,0.0f,100.0f,new GUIContent("Ground Scale"));
		EditorGUILayout.PropertyField(ground,new GUIContent("Ground Texture"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Slider(wall1scale,0.0f,100.0f,new GUIContent("Wall Scale"));
		EditorGUILayout.PropertyField(wall1,new GUIContent("Wall Texture"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Slider(rimpower,0.0f,100.0f,new GUIContent("Rim Power"));
		EditorGUILayout.PropertyField(rimColor,new GUIContent("Rim Color"));
		EditorGUILayout.PropertyField(ramp,new GUIContent("Ramp Texture"));

		serializedObject.ApplyModifiedProperties();
	}
}
