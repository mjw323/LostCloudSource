using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public class ReflectionCamera : MonoBehaviour {
    [Serializable]
    public class CameraProperties
    {
        public float clipPlaneOffset = 0.07f;
        public LayerMask reflectionMask;
        public bool reflectSkybox = false;
        public Color clearColor = Color.grey;
    }

    public CameraProperties properties;
    private RenderTexture rt;
    private Camera mainCamera;

    public void Awake()
    {
        GameObject go = GameObject.FindWithTag("MainCamera");
        mainCamera = go.GetComponent<Camera>();
        if (mainCamera != null)
        {
            // Enable depth texture
            mainCamera.depthTextureMode |= DepthTextureMode.Depth;
            Initialize(mainCamera);
        }
    }

    private void Initialize( Camera targetCamera )
    {
        // No camera component
        if (!gameObject.GetComponent(typeof(Camera)))
        {
            gameObject.AddComponent(typeof(Camera));
        }

        camera.backgroundColor = properties.clearColor;
        camera.clearFlags = properties.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;

        camera.cullingMask = properties.reflectionMask & ~(1 << LayerMask.NameToLayer("Water"));
        camera.backgroundColor = Color.black;
        camera.enabled = false;

        if (!camera.targetTexture)
        {
            rt = new RenderTexture(Mathf.FloorToInt(camera.pixelWidth), Mathf.FloorToInt(camera.pixelHeight), 24);
            rt.hideFlags = HideFlags.DontSave;
        }
    }

    void Update()
    {
        camera.depthTextureMode |= DepthTextureMode.Depth;
        //camera.cullingMask = properties.reflectionMask & ~(1 << LayerMask.NameToLayer("Water"));
        camera.depthTextureMode = DepthTextureMode.None;
        camera.renderingPath = RenderingPath.Forward;
        camera.backgroundColor = properties.clearColor;
        camera.clearFlags = properties.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        camera.targetTexture = rt;

        if (properties.reflectSkybox)
        {
            if (mainCamera.gameObject.GetComponent(typeof(Skybox)))
            {
                Skybox sb = (Skybox)camera.gameObject.GetComponent(typeof(Skybox));
                if (!sb)
                    sb = (Skybox)camera.gameObject.AddComponent(typeof(Skybox));
                sb.material = ((Skybox)mainCamera.GetComponent(typeof(Skybox))).material;
            }
        }

        GL.SetRevertBackfacing(true);

        // Default reflective surface
        Transform reflectiveSurface = transform;
        
        // Find nearest reflective surface
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, -Vector3.up, out hit))
        {
            reflectiveSurface = hit.transform;
        }

        Vector3 eulerA = mainCamera.transform.eulerAngles;

        camera.transform.eulerAngles = new Vector3(-eulerA.x, eulerA.y, eulerA.z);
        //camera.transform.position = mainCamera.transform.position;

        Vector3 pos = reflectiveSurface.transform.position;
        pos.y = reflectiveSurface.position.y;
        Vector3 normal = reflectiveSurface.transform.up;
        float d = -Vector3.Dot(normal, pos) - properties.clipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.zero;
        reflection = CalculateReflectionMatrix(reflection, reflectionPlane);
        Vector3 oldpos = mainCamera.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldpos);

        camera.worldToCameraMatrix = mainCamera.worldToCameraMatrix * reflection;

        Vector4 clipPlane = CameraSpacePlane(camera, pos, normal, 1.0f);

        Matrix4x4 projection = mainCamera.projectionMatrix;
        projection = CalculateObliqueMatrix(projection, clipPlane);
        camera.projectionMatrix = projection;

        //camera.transform.position = newpos;
        Vector3 euler = mainCamera.transform.eulerAngles;
        camera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

        camera.Render();

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
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];

        return projection;
    }

    static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1.0F - 2.0F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2.0F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2.0F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2.0F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2.0F * plane[1] * plane[0]);
        reflectionMat.m11 = (1.0F - 2.0F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2.0F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2.0F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2.0F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2.0F * plane[2] * plane[1]);
        reflectionMat.m22 = (1.0F - 2.0F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2.0F * plane[3] * plane[2]);

        reflectionMat.m30 = 0.0F;
        reflectionMat.m31 = 0.0F;
        reflectionMat.m32 = 0.0F;
        reflectionMat.m33 = 1.0F;

        return reflectionMat;
    }

    static float sgn(float a)
    {
        if (a > 0.0F) return 1.0F;
        if (a < 0.0F) return -1.0F;
        return 0.0F;
    }

    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * properties.clipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;

        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
}
