using UnityEngine;

[AddComponentMenu("Camera/Framing")]
public class Framing : MonoBehaviour {
	[SerializeField] private DynamicCamera dynamicCamera;
	[SerializeField] private Vector3 targetViewportPosition;
	[SerializeField] private float panSpeed;
	[SerializeField] private Curve panCurve;

	[HideInInspector] new private Transform transform;
	[HideInInspector] new private Camera camera;
	[HideInInspector] private Vector3 translation;

	private void Awake() {
		transform = GetComponent<Transform>();
		camera = GetComponent<Camera>();
	}

	private void LateUpdate() {
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
		transform.Translate(translation);
	}
}