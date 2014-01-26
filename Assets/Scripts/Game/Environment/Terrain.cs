using UnityEngine;
using System.Collections;

[ExecuteInEditMode,System.Serializable]
public class Terrain : MonoBehaviour {
	// Terrain shader
	public Material terrainMaterial;

	// Various Settings
	public float BlendHeightOffset = 0.0f;

	// Uniforms
	private System.String X = "_X";
	private System.String Y = "_Y";
	private System.String Z = "_Z";
	private System.String Scale1 = "_Scale1";
	private System.String Scale2 = "_Scale2";
	private System.String Ymax = "_Ymax";
	private System.String invRange = "_invRange"; // 1.0 / Range
	private System.String Offset = "_HeightOffset";
	private System.String Ground = "_Ground";	
	private System.String Wall = "_Wall";
	private System.String Ramp = "_Ramp";
	private System.String RimPower = "_RimPower";
	private System.String RimColor = "_RimColor";

	public float xWeight = 1.0f;
	public float yWeight = 1.0f;
	public float zWeight = 1.0f;

	public float groundScale = 1.0f;
	public Texture2D groundTexture;

	public float wallScale = 1.0f;
	public Texture2D wallTexture;

	public float rimPower = 1.0f;
	public Color rimColor;
	public Texture2D ramp;

	void Awake() {
		// Search for material and create if needed
		if(renderer.sharedMaterial.name != gameObject.name) {
			renderer.material = new Material(Shader.Find("LostCloud/Terrain"));
			renderer.material.name = gameObject.name;
		}
	}

	void Update() {
		terrainMaterial = renderer.sharedMaterial;

		float r = renderer.bounds.max.y - renderer.bounds.min.y;
		terrainMaterial.SetFloat(X,xWeight);
		terrainMaterial.SetFloat(Y,yWeight);
		terrainMaterial.SetFloat(Z,zWeight);
		terrainMaterial.SetFloat(Ymax,renderer.bounds.max.y);
		terrainMaterial.SetFloat(invRange,1.0f/r);
		terrainMaterial.SetFloat(Offset,BlendHeightOffset);
		terrainMaterial.SetFloat(Scale1,groundScale);
		terrainMaterial.SetFloat(Scale2,wallScale);
		terrainMaterial.SetFloat(RimPower,rimPower);
		terrainMaterial.SetColor(RimColor,rimColor);
		terrainMaterial.SetTexture(Ground,groundTexture);
		terrainMaterial.SetTexture(Wall,wallTexture);
	}
}
