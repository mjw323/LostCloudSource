using UnityEngine;
using System.Collections;

public class AudioCone : MonoBehaviour
{
    public void MakeWaves(float duration)
    {
        if (busy) { return; }
        busy = true;
        StartCoroutine(DoMakeWaves(duration));
    }

    // Called when Yorex destroys the sound machine
    public void Stop()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.zero;
    }

	[SerializeField] private float scrollSpeed = -3f;

    new private Transform transform;
    new private Renderer renderer;
	private float intensity = 0;
    private bool busy = false;

    private IEnumerator DoMakeWaves(float duration)
    {
        StartCoroutine(Grow(duration));
        while (busy) {
            float texOffset = Time.time * scrollSpeed;
            renderer.material.SetTextureOffset("_MainTex", new Vector2(0, texOffset));
            renderer.material.SetColor("_Color", new Vector4(1.0f, 1.0f, 1.0f, intensity));
            transform.localScale = new Vector3(Mathf.Max(Mathf.Sqrt(intensity)*10f,transform.localScale.x),
                                               Mathf.Max(Mathf.Sqrt(intensity)*10f,transform.localScale.y),
                                               Mathf.Max(Mathf.Pow(intensity,2f)*40f,transform.localScale.z));
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }

    private IEnumerator Grow(float duration)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime) {
            intensity = Mathf.Sqrt (Mathf.Max (0f,(Time.time-(endTime-duration))/duration)); // ?
            yield return null;
        }
        yield return StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        while (intensity > 0.01f) {
            intensity -= intensity * 0.2f;
            yield return null;
        }
        intensity = 0;
        busy = false;
    }

	private void Awake()
    {
        transform = GetComponent<Transform>();
        renderer = GetComponent<Renderer>();
    }
}
