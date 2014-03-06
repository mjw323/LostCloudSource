//#define DYNAMIC_CAMERA_DEBUG_DRAW
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("Camera/Dynamic Camera")]
public class DynamicCamera : MonoBehaviour {
	[ExposeProperty]
	public Transform PlayerAnchor {
		get { return playerAnchor; }
		set {
			playerAnchor = value;
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
#endif
				ComputeDistanceAndElevationAngle();
		}
	}

	[ExposeProperty]
	public Transform EnemyAnchor {
		get { return enemyAnchor; }
		set {
			enemyAnchor = value;
			GetNavAgent();
		}
	}

	public float Distance {
		get { return distance; }
	}

	[SerializeField] private float targetDistance = 4.0f;
	[SerializeField] private float zoomSpeed = 1.0f;
	[SerializeField] private Curve zoomCurve;
	[SerializeField] private float rotationSpeed = 60.0f;
	[SerializeField] private bool lookAtEnabled = false;
	
	[HideInInspector] new private Transform transform;
	[HideInInspector][SerializeField] private Transform playerAnchor;
	[HideInInspector] private float distance;
	[HideInInspector] private float elevationAngle;
	[HideInInspector] private Transform enemyAnchor;
	[HideInInspector] private NavMeshAgent navAgent;

	private void ComputeDistanceAndElevationAngle() {
		if (playerAnchor != null) {
			Vector3 relativePos = transform.position - playerAnchor.position;
			Vector3 relativePosXz = new Vector3(relativePos.x, 0, relativePos.z);
			elevationAngle = Vector3.Angle(relativePosXz, relativePos);
			distance = relativePos.magnitude;
		}
	}

	private void GetNavAgent() {
		if (enemyAnchor != null) {
			navAgent = enemyAnchor.GetComponent<NavMeshAgent>();
		}
	}

	private void Awake() {
		transform = GetComponent<Transform>();
		GetNavAgent();
	}

	private void Start() {
		ComputeDistanceAndElevationAngle();
	}

	private void LateUpdate() {
		if (playerAnchor != null) {
			elevationAngle += (Input.GetAxis("RightStickY") -
			                   Input.GetAxis("Mouse Y"));
			float rotationAngle = (Input.GetAxis("RightStickX") + Input.GetAxis(
				"Mouse X")) * rotationSpeed * Time.deltaTime;	
			Quaternion rotation = Quaternion.AngleAxis(rotationAngle,Vector3.up);

			Vector3 relativePos = transform.position - playerAnchor.position;
			Vector3 relativePosXz = new Vector3(relativePos.x, 0, relativePos.z);
			Vector3 xzDirection = rotation * relativePosXz.normalized;
			Quaternion elevation = Quaternion.AngleAxis(elevationAngle,
			Vector3.Cross(xzDirection, Vector3.up));
			Vector3 direction = elevation * xzDirection;

			float distanceDiff = Mathf.Abs(distance - targetDistance);
			float distanceStep = zoomCurve.Evaluate(distanceDiff) * zoomSpeed
				* Time.deltaTime;

			if (lookAtEnabled && Input.GetAxis("Target") > 0.8f){
				if (!navAgent.enabled){
					direction = Vector3.Slerp(direction, -playerAnchor.forward, .1f);
				} else {
					direction = Vector3.Slerp(direction, Vector3.Normalize(
						playerAnchor.position - enemyAnchor.position), .1f);
				}
			}
				
			distance = Mathf.Lerp(distance, targetDistance, distanceStep);

			transform.position = playerAnchor.position + direction * distance;

			transform.LookAt(playerAnchor.position);

#if DYNAMIC_CAMERA_DEBUG_DRAW
			Debug.DrawLine(playerAnchor.position, playerAnchor.position + relativePos,
                     Color.red);
			Debug.DrawLine(playerAnchor.position,
			               playerAnchor.position + relativePosXz, Color.blue);
			Debug.DrawRay(playerAnchor.position, direction, Color.green);
#endif
		}
	}
}