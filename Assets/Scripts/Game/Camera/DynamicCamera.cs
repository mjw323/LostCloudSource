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

    public void EnableFollow()
    {
        shouldFollowPlayer = true;
    }

    public void DisableFollow()
    {
        shouldFollowPlayer = false;
    }

    public void TeleportBehind()
    {
        transform.position = playerAnchor.position - playerAnchor.forward * targetDistance;
    }

    public void PushAnchor(Transform anchor)
    {
        oldAnchor = playerAnchor;
        playerAnchor = anchor;
    }

    public void PopAnchor()
    {
        playerAnchor = oldAnchor;
    }

	[SerializeField] private float targetDistance = 4.0f;
	[SerializeField] private float zoomSpeed = 1.0f;
	[SerializeField] private Curve zoomCurve;
	[SerializeField] private float rotationSpeed = 60.0f;
	[SerializeField] private bool lookAtEnabled = true;
	
	[HideInInspector] new private Transform transform;
	[HideInInspector][SerializeField] private Transform playerAnchor;
	[HideInInspector] private float distance;
	[HideInInspector] private float elevationAngle;
    [HideInInspector] private bool shouldFollowPlayer = true;
    [HideInInspector] private Transform oldAnchor;
	
	[HideInInspector] private Quaternion savedAngle;
	
	[HideInInspector] private float elevationAngleSave;
	[HideInInspector] private bool wasFollowing = false;
	
	[HideInInspector] private Transform enemyAnchor;
	[HideInInspector] private NavMeshAI navAgent;
	public bool followEnemy = false;
	[HideInInspector] public bool stuckOnYorex = false;

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
			navAgent = enemyAnchor.GetComponent<NavMeshAI>();
		}
	}

	private void Awake() {
		transform = GetComponent<Transform>();
		enemyAnchor = GameObject.FindWithTag ("Yorex").transform;
		GetNavAgent();
	}

	private void Start() {
		ComputeDistanceAndElevationAngle();
	}

	private void LateUpdate() {
		Quaternion rotation = Quaternion.identity;
		Transform goalAnchor;
		stuckOnYorex = (followEnemy || (navAgent.state==6 && navAgent.rise));
		
		if (stuckOnYorex != wasFollowing){ //follow state changed
			wasFollowing = stuckOnYorex;
			if (stuckOnYorex){ //started folowing
				savedAngle = this.transform.rotation;
				elevationAngleSave = elevationAngle;
			}
			else{ //stopped following; go back to old elevation
				elevationAngle = elevationAngleSave;
				this.transform.rotation = savedAngle;
				if (this.transform.position.y<playerAnchor.position.y){
					this.transform.position += Vector3.up*(playerAnchor.position.y-this.transform.position.y);
				}
			}
		}
		
		if (stuckOnYorex){
					goalAnchor = enemyAnchor;
		}
		else{
					goalAnchor = playerAnchor;
			}
		
		if (goalAnchor != null) {
			if (!stuckOnYorex){
			elevationAngle = Mathf.Clamp(elevationAngle+(Input.GetAxis("RightStickY") - Input.GetAxis("Mouse Y")), -50f, 70f);
			float rotationAngle = (Input.GetAxis("RightStickX") + Input.GetAxis(
				"Mouse X")) * rotationSpeed * Time.deltaTime;	
			rotation = Quaternion.AngleAxis(rotationAngle,Vector3.up);
			}
			else{
				elevationAngle = Mathf.Lerp (elevationAngle,(enemyAnchor.position.y - transform.position.y)/1f,.5f); //why won't you look at yorex!
			}

			Vector3 relativePos = transform.position - goalAnchor.position;
			Vector3 relativePosXz = new Vector3(relativePos.x, 0, relativePos.z);
			Vector3 xzDirection = rotation * relativePosXz.normalized;
			Quaternion elevation = Quaternion.AngleAxis(elevationAngle,
			Vector3.Cross(xzDirection, Vector3.up));
			Vector3 direction = elevation * xzDirection;

			float distanceDiff = Mathf.Abs(distance - targetDistance);
			float distanceStep = zoomCurve.Evaluate(distanceDiff) * zoomSpeed
				* Time.deltaTime;

			if (!stuckOnYorex && (navAgent.enabled)&&(Input.GetAxis("Target") > 0.8f || Input.GetButton("Target")  || navAgent.state==6 /*|| navAgent.state==3*/)){
				if (navAgent.state==0){
					direction = Vector3.Slerp(direction, -goalAnchor.forward, .1f);
				} /*else { // Forces player to look at Yorex even through geometry, making it hard to control
					direction = Vector3.Slerp(direction, Vector3.Normalize(
						goalAnchor.position - enemyAnchor.position), .1f);
				}*/
			}
			float targD = targetDistance;
			if (stuckOnYorex){targD = 20f;}
            distance = Mathf.Lerp(distance, targD, distanceStep);
			//if (!stuckOnYorex){
            if (shouldFollowPlayer) {
			    transform.position = Vector3.Lerp (transform.position,playerAnchor.position + direction * distance,0.9f);
            }
        //}
			//if (!stuckOnYorex){transform.position = Vector3.Lerp(transform.position, goalAnchor.position + direction * distance, 0.9f);}

			transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(goalAnchor.position - transform.position), 0.9f);
			//LookAt(goalAnchor.position);

#if DYNAMIC_CAMERA_DEBUG_DRAW
			Debug.DrawLine(goalAnchor.position, goalAnchor.position + relativePos,
                     Color.red);
			Debug.DrawLine(goalAnchor.position,
			               goalAnchor.position + relativePosXz, Color.blue);
			Debug.DrawRay(goalAnchor.position, direction, Color.green);
#endif
		}
	}
}