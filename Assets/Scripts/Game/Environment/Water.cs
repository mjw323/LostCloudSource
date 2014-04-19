using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Water : MonoBehaviour {
	// Water Material
	public Material waterMaterial = null;
		
	private Vector3 oldpos = Vector3.zero;
	private GameObject board;
	private Camera reflectionCamera;
    private Camera refractionCamera;

	// Uniforms
	private System.String boardDirection = "_BoardDirection";
	private System.String boardPosition = "_BoardPosition";
	private System.String boardVelocity = "_BoardVelocity";
	private System.String reflectionSampler = "_ReflectionTex";
    private System.String refractionSampler = "_RefractionTex";
	private System.String waveMapSampler = "_waveMapTexture";
 
	// Waves
	public int waveRadius = 1;
	public int waveIntensity = 64;
	public float waveDampening = 1.0f/32.0f;
	private int[] waveStateBuffer;
	private Color[] waveColorMap;
	public Texture2D waveMapTexture;
	private int width  = 128;
	private int height = 128;
	private int halfW;
	private int halfH;
	private int oldIdx;
	private int newIdx;

	//public ComputeShader waveShader;
	//private int kID;
	//private RenderTexture waveTexture;

	public void Awake() {
		GameObject go = GameObject.FindWithTag("WaterReflectionCamera");
		reflectionCamera = go.GetComponent<Camera>();

        go = GameObject.FindWithTag("WaterRefractionCamera");
        refractionCamera = go.GetComponent<Camera>();

        /////////////////// Wake stuff below (BROKEN)
		board = GameObject.FindWithTag("Board");

		waveMapTexture = new Texture2D(width,height);
		waveColorMap = new Color[width*height];
		for(int v = 0; v < height; v++) {
			for(int u = 0; u < width; u++) {
				waveColorMap[(v*width) + u] = Color.red;
			}
		}

		waveStateBuffer = new int[width * (height + 2) * 2];

		waveMapTexture.SetPixels(waveColorMap);
		waveMapTexture.Apply();

		if(waveDampening <= 0.0f || waveDampening > 1.0f )
			waveDampening = 1.0f / 32.0f;

		waveDampening = 1.0f/waveDampening;

		oldIdx = width;
		newIdx = width * (height+3);		

		halfW = width / 2;
		halfH = height / 2;

        waterMaterial = renderer.sharedMaterial;

        if (waterMaterial != null)
        {
            waterMaterial.SetVector("waveMapMax", renderer.bounds.max);
            waterMaterial.SetVector("waveMapSize", renderer.bounds.size);
        }

        gameObject.layer = LayerMask.NameToLayer("Water");
	}	
	
	public void Update()
	{
        if (waterMaterial != null)
        {
            //waterMaterial.SetVector(boardDirection, board.transform.forward);
            //waterMaterial.SetVector(boardPosition, board.transform.position);
            //waterMaterial.SetVector(boardVelocity, board.rigidbody.velocity);
            waterMaterial.SetTexture(reflectionSampler, reflectionCamera.targetTexture);
            waterMaterial.SetTexture(refractionSampler, refractionCamera.targetTexture);
        }

		// I am broken :'(
		//if(board.transform.position.x > renderer.bounds.min.x && board.transform.position.x < renderer.bounds.max.x
		//	&& board.transform.position.z > renderer.bounds.min.z && board.transform.position.z < renderer.bounds.max.z ) {
		//	UpdateWake();
		//	CreateWake();
		//}

		// Copy color buffer to texture
		//waveMapTexture.SetPixels(waveColorMap);
		//waveMapTexture.Apply();		
		//waterMaterial.SetTexture(waveMapSampler,waveMapTexture);
	}	

	private void CreateWake() {
		// Map (x,z) |-> (u,v)
		float x = (renderer.bounds.max.x - board.transform.position.x) / renderer.bounds.size.x,
			  z = (renderer.bounds.max.z - board.transform.position.z) / renderer.bounds.size.z;

		x *= width;
		z *= height;

		// Create new wake trail
		for(int v = (int)z-waveRadius; v < (int)z+waveRadius; v++) {
			for(int u = (int)x-waveRadius; u < (int)x+waveRadius; u++) {
				if(u < width && u >= 0 && v < height && v >= 0)
					waveStateBuffer[(v*width)+u] += waveIntensity;
			}
		}
	}

	// Update Wake based on player position
	private void UpdateWake() {
		// Swap States
		int i = oldIdx;
		oldIdx = newIdx;
		newIdx = i;
		int idx = oldIdx;
		i = 0;

		// Update existing wakes
		for (int v = 0; v < height; v++) {
		  for (int u = 0; u < width; u++) {
		  	// Sum neighboring pixels and halve
		    int data = (waveStateBuffer[idx - width] +
		    	    	waveStateBuffer[idx + width] +
		    	    	waveStateBuffer[idx - 1] +
		    	    	waveStateBuffer[idx + 1]) / 2;

		    // Subtract current state and dampen (data * 1/2^5)
		    data -= waveStateBuffer[newIdx + i];
		    data -= data >> 5;

		    // Update state buffer
		    waveStateBuffer[newIdx+i]=data;

		    data = (1024 - data);
		    float r = ((u - halfW)*data/1024) + halfW;
		    float g = ((v - halfH)*data/1024) + halfH;

		    // Map |-> [0,1]
			r *= (1.0f/width);
			g *= (1.0f/height);

		    waveColorMap[i] = new Color(r,g,0.0f,1.0f);

		    idx++;
		    i++;
		  }
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        Death death = other.GetComponent<Death>();
        if (death != null) {
            death.OnFall();
        }
    }
}