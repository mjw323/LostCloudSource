using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Water : MonoBehaviour {
	// Water Material
	public Material waterMaterial = null;

	// reflection
	public LayerMask reflectionMask;
	public bool reflectSkybox = false;
	public Color clearColor = Color.grey;

	// Clip height
	public float clipPlaneOffset = 0.07f;
		
	private Vector3 oldpos = Vector3.zero;
	private GameObject board;
	private Camera reflectionCamera;
	private Camera mainCamera;

	// Uniforms
	private System.String boardDirection = "_BoardDirection";
	private System.String boardPosition = "_BoardPosition";
	private System.String boardVelocity = "_BoardVelocity";
	private System.String reflectionSampler = "_ReflectionTex";
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
		GameObject go = GameObject.FindWithTag("MainCamera");
		mainCamera = go.GetComponent<Camera>();
		if( mainCamera != null ) {
			mainCamera.depthTextureMode |= DepthTextureMode.Depth;
			reflectionCamera = SetupReflectionCamera(mainCamera);
		}
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

		waterMaterial.SetVector("waveMapMax",renderer.bounds.max);
		waterMaterial.SetVector("waveMapSize",renderer.bounds.size);
	}	
	
	private Camera SetupReflectionCamera(Camera cam) 
	{		
		System.String reflName = gameObject.name+"Reflection"+cam.name;
		GameObject go = GameObject.Find(reflName);
		
		// No camera game object
		if(!go) {
			go = new GameObject(reflName, typeof(Camera)); 
		}

		// No camera component
		if(!go.GetComponent(typeof(Camera))) {
			go.AddComponent(typeof(Camera));
		}

		Camera reflectCamera = go.camera;
		reflectCamera.backgroundColor = clearColor;
		reflectCamera.clearFlags = reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;				
		
		reflectCamera.cullingMask = reflectionMask & ~(1<<LayerMask.NameToLayer("Water"));
		reflectCamera.backgroundColor = Color.black;
		reflectCamera.enabled = false;		
		
		if(!reflectCamera.targetTexture) {
			RenderTexture rt = new RenderTexture(Mathf.FloorToInt(reflectCamera.pixelWidth), Mathf.FloorToInt(reflectCamera.pixelHeight), 24);
			rt.hideFlags = HideFlags.DontSave;
			reflectCamera.targetTexture = rt;
		}
		
		return reflectCamera;
	}
	
	public void Update()
	{		
		RenderReflection(mainCamera, reflectionCamera);

		waterMaterial.SetVector(boardDirection,board.transform.forward);
		waterMaterial.SetVector(boardPosition,board.transform.position);
		waterMaterial.SetVector(boardVelocity,board.rigidbody.velocity);
		waterMaterial.SetTexture(reflectionSampler, reflectionCamera.targetTexture);

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

	private void RenderReflection(Camera cam, Camera reflectCamera) 
	{		
		reflectCamera.cullingMask = reflectionMask & ~(1<<LayerMask.NameToLayer("Water"));
		reflectCamera.depthTextureMode = DepthTextureMode.None;			
		reflectCamera.renderingPath = RenderingPath.Forward;		
		reflectCamera.backgroundColor = clearColor;				
		reflectCamera.clearFlags = reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;

		if(reflectSkybox) 
		{ 			
			if(cam.gameObject.GetComponent(typeof(Skybox))) 
			{
				Skybox sb = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
				if (!sb) 
					sb = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
				sb.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;				
			}	
		}
							
		GL.SetRevertBackfacing(true);		
							
		Transform reflectiveSurface = transform;			
		Vector3 eulerA = cam.transform.eulerAngles;
					
		reflectCamera.transform.eulerAngles = new Vector3(-eulerA.x, eulerA.y, eulerA.z);
		reflectCamera.transform.position = cam.transform.position;
				
		Vector3 pos = reflectiveSurface.transform.position;
		pos.y = reflectiveSurface.position.y;
		Vector3 normal = reflectiveSurface.transform.up;
		float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
		Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
				
		Matrix4x4 reflection = Matrix4x4.zero;
		reflection = CalculateReflectionMatrix(reflection, reflectionPlane);		
		oldpos = cam.transform.position;
		Vector3 newpos = reflection.MultiplyPoint (oldpos);
						
		reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;
				
		Vector4 clipPlane = CameraSpacePlane(reflectCamera, pos, normal, 1.0f);
				
		Matrix4x4 projection =  cam.projectionMatrix;
		projection = CalculateObliqueMatrix(projection, clipPlane);
		reflectCamera.projectionMatrix = projection;
		
		reflectCamera.transform.position = newpos;
		Vector3 euler = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);	
														
		reflectCamera.Render();
		
		GL.SetRevertBackfacing(false);
	}
		
	static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane) 
	{
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0F,
			1.0F
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
		
		return projection;
	}	
	 
	static Matrix4x4 CalculateReflectionMatrix (Matrix4x4 reflectionMat, Vector4 plane) 
	{
	    reflectionMat.m00 = (1.0F - 2.0F*plane[0]*plane[0]);
	    reflectionMat.m01 = (   - 2.0F*plane[0]*plane[1]);
	    reflectionMat.m02 = (   - 2.0F*plane[0]*plane[2]);
	    reflectionMat.m03 = (   - 2.0F*plane[3]*plane[0]);
	
	    reflectionMat.m10 = (   - 2.0F*plane[1]*plane[0]);
	    reflectionMat.m11 = (1.0F - 2.0F*plane[1]*plane[1]);
	    reflectionMat.m12 = (   - 2.0F*plane[1]*plane[2]);
	    reflectionMat.m13 = (   - 2.0F*plane[3]*plane[1]);
	
	   	reflectionMat.m20 = (   - 2.0F*plane[2]*plane[0]);
	   	reflectionMat.m21 = (   - 2.0F*plane[2]*plane[1]);
	   	reflectionMat.m22 = (1.0F - 2.0F*plane[2]*plane[2]);
	   	reflectionMat.m23 = (   - 2.0F*plane[3]*plane[2]);
	
	   	reflectionMat.m30 = 0.0F;
	   	reflectionMat.m31 = 0.0F;
	   	reflectionMat.m32 = 0.0F;
	   	reflectionMat.m33 = 1.0F;
	   	
	   	return reflectionMat;
	}
	
	static float sgn (float a) {
	       if (a > 0.0F) return 1.0F;
	       if (a < 0.0F) return -1.0F;
	       return 0.0F;
	}	
	
	private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign) 
	{
		Vector3 offsetPos = pos + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint (offsetPos);
		Vector3 cnormal = m.MultiplyVector (normal).normalized * sideSign;
		
		return new Vector4 (cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot (cpos,cnormal));
	}
}