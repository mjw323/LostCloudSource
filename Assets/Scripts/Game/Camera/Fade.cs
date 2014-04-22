using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    [SerializeField] private float initialFadeInSeconds;
    [SerializeField] private Texture2D texture;
    private float alpha;
    private bool inProgress;
    private const int depth = -1000;

    public bool InProgress
    {
        get { return inProgress; }
    }

    public Coroutine FadeIn(float duration)
    {
        inProgress = true;
        return StartCoroutine(DoFadeIn(duration));
    }

    public Coroutine FadeOut(float duration)
    {
        inProgress = false;
        return StartCoroutine(DoFadeOut(duration));
    }

    private IEnumerator DoFadeIn(float duration)
    {
        float timeModifier = 1.0f / duration;
        while (alpha > 0) {
            alpha -= Time.deltaTime * timeModifier;
            yield return null;
        }
        alpha = 0;
        inProgress = false;
    }

    private IEnumerator DoFadeOut(float duration)
    {
        float timeModifier = 1.0f / duration;
        while (alpha < 1.0f) {
            alpha += Time.deltaTime * timeModifier;
            yield return null;
        }
        alpha = 1.0f;
        inProgress = false;
    }

    private void Awake()
    {
        alpha = 1.0f;
        inProgress = false;
        FadeIn(initialFadeInSeconds);
    }

    private void OnGUI()
    {
        if (alpha == 0) return;
        Color newGuiColor = GUI.color;
        newGuiColor.a = alpha;
        GUI.color = newGuiColor;
        GUI.depth = depth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
    }
}