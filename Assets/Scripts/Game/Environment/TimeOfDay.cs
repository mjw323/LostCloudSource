using UnityEngine;
using System.Collections;

public class TimeOfDay : MonoBehaviour
{
    public void GotoDay()
    {
        if (isDay || inProgress) { return; }
        inProgress = true;
        StartCoroutine(DoGotoDay());
    }

    public void GotoNight()
    {
        if (!isDay || inProgress) { return; }
        inProgress = true;
        StartCoroutine(DoGotoNight());
    }

    public bool IsDay
    {
        get { return isDay; }
    }

    public bool IsNight
    {
        get { return !isDay; }
    }

    public bool InProgress
    {
        get { return inProgress; }
    }

    [SerializeField] private float fadeTime;

    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject moon;
    private Light sunLight;
    private float sunLightIntensity;
    private Light moonLight;
    private float moonLightIntensity;

    [SerializeField] private Color dayLightColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private Color dayFogColor = new Color(0.46f, 0.709f, 0.949f);
    [SerializeField] private float dayFogDensity = 0.01f;

    [SerializeField] private Color nightLightColor = new Color(0.075f, 0.075f, 0.09f);
    [SerializeField] private Color nightFogColor = new Color(0.051f, 0.051f, 0.098f);
    [SerializeField] private float nightFogDensity = 0.0035f;

    // Serialized (but hidden) so that the custom editor will refresh when it changes
    [HideInInspector][SerializeField] private bool isDay = true;
    private bool inProgress = false;

    private IEnumerator DoGotoDay()
    {
        sun.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            float progress = elapsedTime / fadeTime;
            float lerp = Mathf.SmoothStep(0, 1.0f, progress);
            
            RenderSettings.skybox.SetFloat("_Blend", 1.0f - lerp);
            RenderSettings.ambientLight = Color.Lerp(nightLightColor, dayLightColor, lerp);
            RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, lerp);
            RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lerp);
            
            moonLight.intensity = Mathf.Lerp(moonLightIntensity, 0, lerp);
            sunLight.intensity = Mathf.Lerp(0, sunLightIntensity, lerp);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        moon.SetActive(false);
        isDay = true;
        inProgress = false;
    }

    private IEnumerator DoGotoNight()
    {
        moon.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < fadeTime) {
            float progress = elapsedTime / fadeTime;
            float lerp = Mathf.SmoothStep(0, 1.0f, progress);

            RenderSettings.skybox.SetFloat("_Blend", lerp);
            RenderSettings.ambientLight = Color.Lerp(dayLightColor, nightLightColor, lerp);
            RenderSettings.fogColor = Color.Lerp(dayFogColor, nightFogColor, lerp);
            RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, nightFogDensity, lerp);

            moonLight.intensity = Mathf.Lerp(0, moonLightIntensity, lerp);
            sunLight.intensity = Mathf.Lerp(sunLightIntensity, 0, lerp);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sun.SetActive(false);
        isDay = false;
        inProgress = false;
    }

    private void Awake()
    {
        sunLight = sun.GetComponent<Light>();
        moonLight = moon.GetComponent<Light>();
    }

    private void Start()
    {
        RenderSettings.skybox.SetFloat("_Blend", 0);
        RenderSettings.ambientLight = dayLightColor;
        RenderSettings.fogColor = dayFogColor;
        RenderSettings.fogDensity = dayFogDensity;
        RenderSettings.fogMode = FogMode.Linear;

        sunLightIntensity = sunLight.intensity;
        moonLightIntensity = moonLight.intensity;

        // Start off in day-time.
        moonLight.intensity = 0;
    }
}