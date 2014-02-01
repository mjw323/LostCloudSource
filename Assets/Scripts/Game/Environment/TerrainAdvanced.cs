using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode,System.Serializable]
public class TerrainAdvanced : MonoBehaviour {
	// Terrain shader
	public Material terrainMaterial;

	// Various Settings
	public float BlendHeightOffset = 0.0f;
	public float BlendScale = 1.0f;

	// Uniforms
	private System.String X = "_X";
	private System.String Y = "_Y";
	private System.String Z = "_Z";
	private System.String Scale0 = "_Scale0";
	private System.String Scale1 = "_Scale1";
	private System.String Scale2 = "_Scale2";
	private System.String Scale3 = "_Scale3";
	private System.String Scale4 = "_Scale4";
	private System.String Ymax = "_Ymax";
	private System.String invRange = "_invRange"; // 1.0 / Range
	private System.String Offset = "_HeightOffset";
	private System.String Ground0 = "_Ground0";
	private System.String Ground1 = "_Ground1";
	private System.String Ground2 = "_Ground2";
	private System.String Ground3 = "_Ground3";
	private System.String Ground0Bump = "_Ground0Bump";
	private System.String Ground1Bump = "_Ground1Bump";
	private System.String Ground2Bump = "_Ground2Bump";
	private System.String Ground3Bump = "_Ground3Bump";
	private System.String Wall = "_Wall";
	private System.String Ramp = "_Ramp";

	public bool EnableTriplanar = true;
	public bool EnableHGradient = true;
	public bool EnableBumpMap = true;
	public float xWeight = 1.0f;
	public float yWeight = 1.0f;
	public float zWeight = 1.0f;

	public float groundScale0 = 1.0f;
	public float groundScale1 = 1.0f;
	public float groundScale2 = 1.0f;
	public float groundScale3 = 1.0f;
	public Texture2D groundTexture0;
	public Texture2D groundTexture1;
	public Texture2D groundTexture2;
	public Texture2D groundTexture3;
	public Texture2D groundBump0;
	public Texture2D groundBump1;
	public Texture2D groundBump2;
	public Texture2D groundBump3;

	public float wallScale = 1.0f;
	public Texture2D wallTexture;

	public Texture2D ramp;

	private List<System.String> features = new List<System.String>();

	void Awake() {
		// Search for material and create if needed
		if(renderer.sharedMaterial.name != gameObject.name) {
            renderer.material = new Material(Shader.Find("LostCloud/TerrainAdvanced"));
			renderer.material.name = gameObject.name;
		}
	}

	void Update() {
		terrainMaterial = renderer.sharedMaterial;

		float r = renderer.bounds.max.y - renderer.bounds.min.y;
		
		if(EnableTriplanar) {
			terrainMaterial.SetFloat(X,xWeight);
			terrainMaterial.SetFloat(Y,yWeight);
			terrainMaterial.SetFloat(Z,zWeight);
		}

		terrainMaterial.SetFloat(Ymax,renderer.bounds.max.y);
		terrainMaterial.SetFloat(invRange,1.0f/r);
		terrainMaterial.SetFloat(Offset,BlendHeightOffset);
		terrainMaterial.SetFloat(Scale0,1.0f/groundScale0);
		terrainMaterial.SetFloat(Scale1,1.0f/groundScale1);
		terrainMaterial.SetFloat(Scale2,1.0f/groundScale2);
		terrainMaterial.SetFloat(Scale3,1.0f/groundScale3);
		terrainMaterial.SetFloat(Scale4,1.0f/wallScale);
		terrainMaterial.SetTexture(Ground0,groundTexture0);
		terrainMaterial.SetTexture(Ground1,groundTexture1);
		terrainMaterial.SetTexture(Ground2,groundTexture2);
		terrainMaterial.SetTexture(Ground3,groundTexture3);
		terrainMaterial.SetTexture(Ground0,groundBump0);
		terrainMaterial.SetTexture(Ground1,groundBump1);
		terrainMaterial.SetTexture(Ground2,groundBump2);
		terrainMaterial.SetTexture(Ground3,groundBump3);
		terrainMaterial.SetTexture(Wall,wallTexture);
		
		if(EnableHGradient) {
			terrainMaterial.SetTexture(Ramp,ramp);
		}
		
		features.Clear();

		features.Add(EnableBumpMap ? "BUMPMAP_ON" : "BUMPMAP_OFF");
		features.Add(EnableHGradient ? "HGRAD_ON" : "HGRAD_OFF");
		features.Add(EnableTriplanar ? "TRIPLANAR_ON" : "TRIPLANAR_OFF");

		terrainMaterial.shaderKeywords = features.ToArray();
	}
}
