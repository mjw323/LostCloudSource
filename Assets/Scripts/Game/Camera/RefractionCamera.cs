using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public class RefractionCamera : MonoBehaviour
{
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

    private void Initialize(Camera targetCamera)
    {
        // No camera component
        if (!gameObject.GetComponent(typeof(Camera)))
        {
            gameObject.AddComponent(typeof(Camera));
        }

        camera.enabled = false;

        if (!camera.targetTexture)
        {
            rt = new RenderTexture(Mathf.FloorToInt(camera.pixelWidth), Mathf.FloorToInt(camera.pixelHeight), 24);
            rt.hideFlags = HideFlags.DontSave;
        }
    }

    void Update()
    {
        camera.transform.position = mainCamera.transform.position;
        camera.transform.rotation = mainCamera.transform.rotation;
        camera.projectionMatrix = mainCamera.projectionMatrix;
        camera.depthTextureMode |= DepthTextureMode.Depth;
        camera.renderingPath = RenderingPath.Forward;
        camera.targetTexture = rt;
        camera.Render();
    }
}
