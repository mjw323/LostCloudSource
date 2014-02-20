using UnityEngine;
using System.Collections;

public class DayNightTransition : MonoBehaviour {
	public float waitTime = 1f;
	public float moveTime = 2f;
	public float stayTime = 1f;
	public float fadeTime = 1f;

	public Color dayLight = new Color (.2f, .2f, .2f);
	public Color dayFog = new Color(.46f,.709f,.949f);
	public float dayFogDensity = .01f;
	public Color nightLight = new Color (.075f, .075f, .09f);
	public Color nightFog = new Color (.051f, .051f, .098f);
	public float nightFogDensity = .0035f;

	public GameObject Sun;
	public GameObject Moon;
	private float sunLight;
	private float moonLight;

	private Framing camControl;
	private DynamicCamera parentControl;
	private Quaternion fromDir;
	private Quaternion toDir;
	private bool lookUp = false;
	private bool animating = false;
	private float timePassed = 0f;
	private UpgradeSystem noke;

	// Use this for initialization
	void Start () {
		camControl = this.GetComponent<Framing> ();
		parentControl = this.transform.parent.GetComponent<DynamicCamera> ();
		noke = GameObject.FindWithTag ("Player").GetComponent<UpgradeSystem> ();

		sunLight = Sun.GetComponent<Light> ().intensity;
		moonLight = Moon.GetComponent<Light> ().intensity;

		RenderSettings.skybox.SetFloat("_Blend",0f);
		RenderSettings.ambientLight = dayLight;
		RenderSettings.fogColor = dayFog;
		RenderSettings.fogDensity = dayFogDensity;
		RenderSettings.fogMode = FogMode.Linear;

		Moon.GetComponent<Light> ().intensity = 0f;
	}

	void fadeOut(){
		camControl.enabled = false;
		parentControl.enabled = false;

		timePassed = 0f;
		lookUp = true;
		animating = true;
		fromDir = this.transform.parent.rotation;
		toDir = Quaternion.LookRotation (Vector3.up,-this.transform.parent.forward);

		Moon.active = true;
		Sun.active = true;
	}

	void fadeIn(){
		lookUp = false;
		timePassed = 0f;
		animating = true;
	}
	
	// Update is called once per frame
	void Update () {
		timePassed += Time.deltaTime;

		if (animating) { // in motion
			if (lookUp){ //looking up to the sky
				this.transform.parent.rotation = Quaternion.Slerp(fromDir,toDir,Mathf.SmoothStep(0f, 1f, Mathf.Min(1f,Mathf.Max(timePassed-waitTime,0f)/moveTime)));
				float skyFade = timePassed - (moveTime /*+ stayTime + waitTime*/);
				float lerp = Mathf.SmoothStep(0f, 1f, Mathf.Min(1f,skyFade/fadeTime));
				if (skyFade > 0f){ 
					if (noke.HasPlayerGottenNextUpgrade){ //fade to night if we're carrying the next upgrade
						RenderSettings.skybox.SetFloat("_Blend",lerp);
						RenderSettings.ambientLight = Color.Lerp(dayLight,nightLight,lerp);
						RenderSettings.fogColor = Color.Lerp(dayFog,nightFog,lerp);
						RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity,nightFogDensity,lerp);

						Moon.GetComponent<Light> ().intensity = Mathf.Lerp(0f,moonLight,lerp);
						Sun.GetComponent<Light> ().intensity = Mathf.Lerp(sunLight,0f,lerp);
					}
					else{ //fade to day if we aren't carrying the next upgrade
						RenderSettings.skybox.SetFloat("_Blend",1f - lerp);
						RenderSettings.ambientLight = Color.Lerp(nightLight,dayLight,lerp);
						RenderSettings.fogColor = Color.Lerp(nightFog,dayFog,lerp);
						RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity,dayFogDensity,lerp);

						Moon.GetComponent<Light> ().intensity = Mathf.Lerp(moonLight,0f,lerp);
						Sun.GetComponent<Light> ().intensity = Mathf.Lerp(0f,sunLight,lerp);
					}
				}else{
					//Debug.Log("skyfade at "+skyFade);
				}
			}
			else{ // looking back down at the layer
				this.transform.parent.rotation = Quaternion.Slerp(toDir,fromDir,Mathf.SmoothStep(0f, 1f, Mathf.Min(1f,timePassed/moveTime)));
				if (timePassed >= moveTime){ //reached old position; time to close up shop, boys
					camControl.enabled = true;
					parentControl.enabled = true;
					animating = false;

					Sun.active = !noke.HasPlayerGottenNextUpgrade;
					Moon.active = noke.HasPlayerGottenNextUpgrade;
				}
			}
		}
	}
}
