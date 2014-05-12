using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    [SerializeField] private float showHideTime = 0.5f;
    new private Transform transform;
    new private Renderer renderer;
    private Transform camera;
    private bool isHidden = true;

    // The graphic we currently use for billboards is not properly rotated.
    private static readonly Quaternion rotationOffset = Quaternion.Euler(0, -90.0f, 0);

    public void Show()
    {
        if (isHidden) {
            StopAllCoroutines();
            isHidden = false;
            StartCoroutine(DoShow());
        }
    }

    public void Hide()
    {
        if (!isHidden) {
            StopAllCoroutines();
            isHidden = true;
            StartCoroutine(DoHide());
        }
    }

    private IEnumerator DoShow()
    {
        float timeModifier = 1.0f / showHideTime;
        Color newColor = renderer.material.color;
        while (newColor.a < 1.0f) {
            newColor.a += Time.deltaTime * timeModifier;
            renderer.material.color = newColor;
            yield return null;
        }
        newColor.a = 1.0f;
        renderer.material.color = newColor;
    }

    private IEnumerator DoHide()
    {
        float timeModifier = 1.0f / showHideTime;
        Color newColor = renderer.material.color;
        while (newColor.a > 0) {
            newColor.a -= Time.deltaTime * timeModifier;
            renderer.material.color = newColor;
            yield return null;
        }
        newColor.a = 0;
        renderer.material.color = newColor;
    }

    private void Awake()
    {
        transform = GetComponent<Transform>();
        renderer = GetComponent<Renderer>();
        camera = Camera.main.GetComponent<Transform>();
    }

    private void Start()
    {
        // Default to hidden
        Color defaultColor = renderer.material.color;
        defaultColor.a = 0;
        renderer.material.color = defaultColor;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(camera.position - transform.position) * rotationOffset;
    }

    private void OnDestroy()
    {
        // Clean up the copied material created by modifying the alpha value
        Destroy(renderer.material);
    }
}