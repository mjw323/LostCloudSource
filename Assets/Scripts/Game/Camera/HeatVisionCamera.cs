using UnityEngine;
using System;
using System.Collections;

public class HeatVisionCamera : MonoBehaviour
{
    public int targetThreashold = 5;

    /// Blur iterations - larger number means more blur.
    public int blurIterations = 4;

    /// Blur spread for each iteration. Lower values
    /// give better looking blur, but require more iterations to
    /// get large blurs. Value is usually between 0.5 and 1.0.
    public float blurSpread = 5.0f;

    // --------------------------------------------------------
    // The blur iteration shader.
    // Basically it just takes 4 texture samples and averages them.
    // By applying it repeatedly and spreading out sample locations
    // we get a Gaussian blur approximation.

    public Shader blurShader = null;
    public Shader replacementShader = null;

    [Serializable]
    public class Timer {
        [HideInInspector] public float ticks = 0.0f;
        public float frequency = 0.1f;
    }

    public Timer updateTimer;
    private bool visible = false;
    private bool ready = false;

    //private static string blurMatString =

    public int textureSize = 8;
    private RenderTexture m_RenderTexture = null;
    protected RenderTexture visionTexture
    {
        get
        {
            if (m_RenderTexture == null)
            {
                m_RenderTexture = new RenderTexture(textureSize, textureSize, 0);
                m_RenderTexture.hideFlags = HideFlags.DontSave;
            }

            return m_RenderTexture;
        }
    }

    static Material m_Material = null;
    protected Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(blurShader);
                m_Material.hideFlags = HideFlags.DontSave;
            }
            return m_Material;
        }
    }

    protected void OnDisable()
    {
        if (m_Material)
        {
            DestroyImmediate(m_Material);
        }
    }

    // -------------------------------------------------------

    protected void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        // Disable if the shader can't run on the users graphics card
        if (!blurShader || !material.shader.isSupported)
        {
            enabled = false;
            return;
        }

        updateTimer.ticks = 0.0f;

        camera.SetReplacementShader(replacementShader, "RenderType");
        camera.targetTexture = null;
    }

    void OnDestroy() {
        camera.ResetReplacementShader();
    }


    void Update() {
        updateTimer.ticks -= Time.deltaTime;
        if(updateTimer.ticks < updateTimer.frequency) {
             ready = true;
             updateTimer.ticks = updateTimer.frequency;
        }
    }

    // Performs one blur iteration.
    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    // Downsamples the texture to a quarter resolution.
    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    // Called by the camera to apply the image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int rtW = source.width / 4;
        int rtH = source.height / 4;
        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

        // Copy source to the 4x4 smaller texture.
        DownSample4x(source, buffer);

        // Blur the small texture
        for (int i = 0; i < blurIterations; i++)
        {
            RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            FourTapCone(buffer, buffer2, i);
            RenderTexture.ReleaseTemporary(buffer);
            buffer = buffer2;
        }

        Graphics.Blit(buffer, visionTexture);

        // Downsample temp rt into camera render texture
        //DownSample4x(rt, destination);

        RenderTexture.ReleaseTemporary(buffer);
    }

    // Called after rendering is done
    void OnPostRender() {
        if (ready) {
            int hits = 0;
            Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
            RenderTexture.active = visionTexture;

            // Read vision color buffer into temp texture
            tex.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0, false);
            tex.Apply();

            for (int z = 0; z < textureSize; z++) {
                for (int x = 0; x < textureSize; x++) {
                    Color color = tex.GetPixel(x,z);
                    
                    if (color.r > 0.0f)
                        hits++;
                }
            }

            RenderTexture.active = null;

            // Check hits
            visible = false;
            if (hits > targetThreashold)
                visible = true;

            ready = false;
        }
    }

    public bool CanSee()
    {
        return visible;
    }
}
