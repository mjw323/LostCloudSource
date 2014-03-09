using UnityEngine;

[AddComponentMenu("Camera/Framing")]
public class Framing : MonoBehaviour {
	[SerializeField] private DynamicCamera dynamicCamera;
	[SerializeField] private Vector3 targetViewportPosition;
	[SerializeField] private float panSpeed;
	[SerializeField] private Curve panCurve;
	
	private float screenShake = 0f;
	private float shakeAmount = 0f;

	[HideInInspector][SerializeField] new private Transform transform;
	[HideInInspector][SerializeField] new private Camera camera;
	[HideInInspector][SerializeField] private Vector3 translation;

	private void Awake() {
		transform = GetComponent<Transform>();
		camera = GetComponent<Camera>();
	}

	private void LateUpdate() {
		/////////////////frame///////////////
		transform.Translate(-translation);
		targetViewportPosition.z = dynamicCamera.Distance;
		Vector3 targetViewportPosLocal = transform.InverseTransformPoint(
			camera.ViewportToWorldPoint(targetViewportPosition));
		Vector3 anchorPosLocal = transform.InverseTransformPoint(
			dynamicCamera.PlayerAnchor.position);
		Vector3 targetTranslation = anchorPosLocal - targetViewportPosLocal;
		float translationDiff = (targetTranslation - translation).magnitude;
		float panStep = panCurve.Evaluate(translationDiff) * panSpeed *
			Time.deltaTime;
		translation = Vector3.Lerp(translation, targetTranslation, panStep);
		translation.z = 0;
		
		/////////////////screen shake///////////////
		if (screenShake > 0f){
			translation.x += Mathf.Sin(4f * Mathf.PI * screenShake) * shakeAmount * 0.5f * Mathf.Min (1f,screenShake / 0.5f);
			translation.y += Mathf.Sin(6f * Mathf.PI * screenShake) * shakeAmount * Mathf.Min (1f,screenShake / 0.5f);
			screenShake -= Time.deltaTime;
		}
		
		/////////////////set///////////////
		transform.Translate(translation);
	}
	
	public void ShakeScreen(float secs, float amt){
		screenShake = secs;
		shakeAmount = amt;
	}
}