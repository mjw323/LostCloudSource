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
	private System.String reflectionSampler = "_ReflectionTex";

	// Clip height
	public float clipPlaneOffset = 0.07f;
		
	private Vector3 oldpos = Vector3.zero;
	private GameObject board;
	private Camera reflectionCamera;
	private Camera mainCamera;

	// Player Uniforms
	private System.String boardDirection = "_BoardDirection";
	private System.String boardPosition = "_BoardPosition";
	private System.String boardVelocity = "_BoardVelocity";


	public void Awake() {
		GameObject go = GameObject.FindWithTag("MainCamera");
		mainCamera = go.GetComponent<Camera>();
		if( mainCamera != null ) {
			mainCamera.depthTextureMode |= DepthTextureMode.Depth;
			reflectionCamera = SetupReflectionCamera(mainCamera);
		}
		board = GameObject.FindWithTag("Board");
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
			RenderTexture rt = new RenderTexture(Mathf.FloorToInt(reflectCamera.pixelWidth), Mathf.FloorToInt(reflectCamera.pixelHeight), 24);//new RenderTexture(Mathf.FloorToInt(reflectCamera.pixelWidth*0.5F), Mathf.FloorToInt(reflectCamera.pixelHeight*0.5F), 24);	
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
	}	
	
	private void RenderReflection(Camera cam, Camera reflectCamera) 
	{
		//if (!reflectCamera)
		//	return;
			
		//if(waterMaterial && !waterMaterial.HasProperty(reflectionSampler)) {
		//	return;
		//}
		
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