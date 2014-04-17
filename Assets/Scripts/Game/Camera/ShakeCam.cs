using UnityEngine;

public class ShakeCam: MonoBehaviour {
	
	private float screenShake = 0f;
	private float shakeAmount = 0f;
	[HideInInspector][SerializeField] private MotionBlur mblur;

	[HideInInspector][SerializeField] new private Transform transform;
	[HideInInspector][SerializeField] new private Camera camera;
	[HideInInspector][SerializeField] private Vector3 translation;
	

	private void Awake() {
		transform = GetComponent<Transform>();
		camera = GetComponent<Camera>();
		mblur = GetComponent<MotionBlur>();
	}

	private void LateUpdate() {
		transform.Translate(-translation);
		translation = new Vector3(0,0,0);
		
		/////////////////screen shake///////////////
		if (screenShake > 0f){
			translation.x += Mathf.Sin(8f * Mathf.PI * screenShake) * shakeAmount * 0.5f * Mathf.Min (1f,screenShake / 0.5f);
			translation.y += Mathf.Sin(12f * Mathf.PI * screenShake) * shakeAmount * Mathf.Min (1f,screenShake / 0.5f);
			screenShake -= Time.deltaTime;
			
			mblur.enabled = true;
			mblur.blurAmount = shakeAmount * Mathf.Min (1f,screenShake / 0.5f);
		}
		else{mblur.enabled = false;}
		
		/////////////////set///////////////
		transform.Translate(translation);
	}
	
	public void ShakeScreen(float secs, float amt){
		Debug.Log ("shaking screen by "+amt+" for "+secs+" seconds!");
		screenShake = secs;
		shakeAmount = amt;
	}
}