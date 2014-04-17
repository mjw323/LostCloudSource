using UnityEngine;

[AddComponentMenu("Camera/Framing")]
public class Framing : MonoBehaviour {
	[SerializeField] private DynamicCamera dynamicCamera;
	[SerializeField] private Vector3 targetViewportPosition;
	[SerializeField] private float panSpeed;
	[SerializeField] private Curve panCurve;
	
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

		
		/////////////////set///////////////
		transform.Translate(translation);
	}
	

}