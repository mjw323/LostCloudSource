using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour
{
    [SerializeField] private Texture2D texture;
    private float alpha = 0f;
    private bool inProgress;
	private const int depth = -1000;

    public bool InProgress
    {
        get { return inProgress; }
    }

    public void Fire(float duration)
    {
        inProgress = true;
        StartCoroutine(DoFlash(duration));
    }

    private IEnumerator DoFlash(float duration)
    {
        float timeModifier = 1.0f / duration;
        while (alpha > 0) {
            float progress = 1.0f - alpha;
            alpha -= Time.deltaTime * timeModifier * progress;
            yield return null;
        }
        alpha = 0;
        inProgress = false;
    }

    private void Awake()
    {
        inProgress = false;
    }
	
	private void OnGUI()
    {
        if (!inProgress) return;
		Color newGuiColor = GUI.color;
        newGuiColor.a = alpha;
		GUI.color = newGuiColor;
		GUI.depth = depth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);	
	}
}