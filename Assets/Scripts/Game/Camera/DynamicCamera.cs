//#define DYNAMIC_CAMERA_DEBUG_DRAW
using UnityEngine;
using Conditional = System.Diagnostics.ConditionalAttribute;

[AddComponentMenu("Camera/Dynamic Camera")]
public class DynamicCamera : MonoBehaviour {
  public float Distance {
    get { return distance; }
  }

  private void Awake() {
    transform = GetComponent<Transform>();
  }

  private void Start() {
    if (anchor != null) {
      Vector3 relativePos = transform.position - anchor.position;
      Vector3 relativePosXz = new Vector3(relativePos.x, 0, relativePos.z);
      elevationAngle = Vector3.Angle(relativePosXz, relativePos);
      distance = relativePos.magnitude;
    }
  }

  [Conditional("DYNAMIC_CAMERA_DEBUG_DRAW")]
  private void DrawLine(Vector3 start, Vector3 end, Color color) {
    Debug.DrawLine(start, end, color);
  }

  [Conditional("DYNAMIC_CAMERA_DEBUG_DRAW")]
  private void DrawRay(Vector3 origin, Vector3 direction, Color color) {
    Debug.DrawRay(origin, direction, color);
  }

  private void LateUpdate() {
    if (anchor != null) {
      elevationAngle += Input.GetAxis("RightStickY") - Input.GetAxis("Mouse Y");
      float rotationAngle = (Input.GetAxis("RightStickX") + Input.GetAxis(
        "Mouse X")) * rotationSpeed * Time.deltaTime;
      Quaternion rotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);

      Vector3 relativePos = transform.position - anchor.position;
      Vector3 relativePosXz = new Vector3(relativePos.x, 0, relativePos.z);
      Vector3 xzDirection = rotation * relativePosXz.normalized;
      Quaternion elevation = Quaternion.AngleAxis(elevationAngle,
        Vector3.Cross(xzDirection, Vector3.up));
      Vector3 direction = elevation * xzDirection;

      float distanceDiff = Mathf.Abs(distance - targetDistance);
      float distanceStep = zoomCurve.Evaluate(distanceDiff) * zoomSpeed *
        Time.deltaTime;
      distance = Mathf.Lerp(distance, targetDistance, distanceStep);

      transform.position = anchor.position + direction * distance;

      transform.LookAt(anchor.position);

      DrawLine(anchor.position, anchor.position + relativePos, Color.red);
      DrawLine(anchor.position, anchor.position + relativePosXz, Color.blue);
      DrawRay(anchor.position, direction, Color.green);
    }
  }

  public Transform anchor;
  public float targetDistance;
  public float zoomSpeed;
  public Curve zoomCurve;
  public float rotationSpeed;

  new private Transform transform;
  private float distance;
  private float elevationAngle;
}