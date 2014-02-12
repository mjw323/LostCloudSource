using UnityEngine;

[AddComponentMenu("Camera/Framing")]
public class Framing : MonoBehaviour {
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
      dynamicCamera.anchor.position);
    Vector3 targetTranslation = anchorPosLocal - targetViewportPosLocal;
    float translationDiff = (targetTranslation - translation).magnitude;
    float panStep = panCurve.Evaluate(translationDiff) * panSpeed *
      Time.deltaTime;
    translation = Vector3.Lerp(translation, targetTranslation, panStep);
    translation.z = 0;
    transform.Translate(translation);
  }

  public DynamicCamera dynamicCamera;
	public Vector3 targetViewportPosition;
  public float panSpeed;
  public Curve panCurve;

  new private Transform transform;
  new private Camera camera;
  private Vector3 translation;
}