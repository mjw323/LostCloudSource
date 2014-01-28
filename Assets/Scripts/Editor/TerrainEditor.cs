using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainAdvanced)),CanEditMultipleObjects]
public class TerrainEditor : Editor {
	SerializedProperty offset;
	SerializedProperty xaxis;
	SerializedProperty yaxis;
	SerializedProperty zaxis;

	SerializedProperty blendScale;
	SerializedProperty groundscale0;
	SerializedProperty groundscale1;
	SerializedProperty groundscale2;
	SerializedProperty groundscale3;
	SerializedProperty wall1scale;
	SerializedProperty wall2scale;
	SerializedProperty rimpower;

	SerializedProperty ground0;
	SerializedProperty ground1;
	SerializedProperty ground2;
	SerializedProperty ground3;
	SerializedProperty wall1;
	SerializedProperty wall2;
	SerializedProperty ramp;

	SerializedProperty rimColor;

	// Use this for initialization
	void OnEnable() {
		// Floats
		offset = serializedObject.FindProperty("BlendHeightOffset");
		blendScale = serializedObject.FindProperty("BlendScale");

		xaxis = serializedObject.FindProperty("xWeight");
		yaxis = serializedObject.FindProperty("yWeight");
		zaxis = serializedObject.FindProperty("zWeight");

		groundscale0 = serializedObject.FindProperty("groundScale0");
		groundscale1 = serializedObject.FindProperty("groundScale1");
		groundscale2 = serializedObject.FindProperty("groundScale2");
		groundscale3 = serializedObject.FindProperty("groundScale3");
		wall1scale = serializedObject.FindProperty("wallScale");
		rimpower = serializedObject.FindProperty("rimPower");

		rimColor = serializedObject.FindProperty("rimColor");

		// Textures
		ground0 = serializedObject.FindProperty("groundTexture0");
		ground1 = serializedObject.FindProperty("groundTexture1");
		ground2 = serializedObject.FindProperty("groundTexture2");
		ground3 = serializedObject.FindProperty("groundTexture3");
		wall1 = serializedObject.FindProperty("wallTexture");
		ramp = serializedObject.FindProperty("ramp");
	}
	
	// Update is called once per frame
	public override void OnInspectorGUI() {
		serializedObject.Update();

		// Offset Slider
		EditorGUILayout.Slider(offset,0.0f,100.0f,new GUIContent("Blend Height Offset"));
		EditorGUILayout.Slider(blendScale,0.0001f,20.0f,new GUIContent("Blend Scale"));

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
		EditorGUILayout.Slider(groundscale0,0.001f,5.0f,new GUIContent("Ground Scale"));
		EditorGUILayout.PropertyField(ground0,new GUIContent("Ground Texture (Low)"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Slider(groundscale1,0.001f,5.0f,new GUIContent("Ground Scale"));
		EditorGUILayout.PropertyField(ground1,new GUIContent("Ground Texture (Mid-Low)"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Slider(groundscale2,0.001f,5.0f,new GUIContent("Ground Scale"));
		EditorGUILayout.PropertyField(ground2,new GUIContent("Ground Texture (Mid-High)"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Slider(groundscale3,0.001f,5.0f,new GUIContent("Ground Scale"));
		EditorGUILayout.PropertyField(ground3,new GUIContent("Ground Texture (High)"));

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Slider(wall1scale,0.001f,5.0f,new GUIContent("Wall Scale"));
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
