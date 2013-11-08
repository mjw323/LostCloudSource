using UnityEngine;
using System;

public class Hover : MonoBehaviour
{
	[Serializable]
	public class HoverProperties
	{
		public float hoverHeight;
		public float hoverDamping;
	}
	public HoverProperties hoverProperties;

	public float thrustPower;
	public float steerPower;
	public float angleClamp = 50.0f;
	public float airAngleClamp = 10.0f;
	private float currentClamp;

	[HideInInspector] Transform[] m_sensors;
	RaycastHit[] m_hits;
	Transform m_thruster;
	float m_thrust;
	float m_lean;
	float m_glide;
	private bool m_jump;
	private bool detach;
	
	private float jumpPower; // measures how long jump was held, 0 - 1
	public float jumpLength = 3.0f; //how many seconds to build maximum jump
	public float jumpForce = 20.0f; //The max force applied when jumping
	public float glideForce = 400.0f;
	public float glideLength = 4.0f;
	public float glideApex = 1.0f; //how many seconds after you start gliding at which it reaches its apex
	public float glideApexForce = 600.0f; //glide power ranges from power*1 to power*(1+ApexAdd), with most power at apex time
	
	private float hoverMod;
	private float steerMod;
	private float glideLeft = 0.0f;
	private float airTime = 0.0f;
	private float glidePower;
	
	private bool onGround;
	private Vector3 clampVector;
	

	private bool grinding = false;
	private Transform grindRail = null;
	private Transform[] grindPoints;
	private Transform lastRail = null;
	private Transform grindPoint = null;
	private Vector3 grindDir;
	public float grindSpeed = 10000;
	public float maxGrindDist = 5.0f; // Maximum distance for impules toward rail
	public float grindPointDist = 0.75f; // distance to a grind point before you seek the next one

	// Uses a temporary BoxCollider (unless there already is one attached) to compute the dimensions of the board.
	Vector3 CalculateBoardDimensions()
	{
		bool createdBoxCollider = false;
		BoxCollider boxCollider = GetComponent<BoxCollider>();
		if (!boxCollider)
		{
			boxCollider = gameObject.AddComponent<BoxCollider>();
			createdBoxCollider = true;
		}
		Vector3 boxDimensions = new Vector3(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y, boxCollider.size.z * transform.localScale.z);
		if (createdBoxCollider)
		{
			Destroy(boxCollider);
		}
		return boxDimensions;
	}

	Vector3[] CalculateSensorPositions(Vector3 boardDimensions)
	{
		Vector3[] sensorPositions = new Vector3[4];
		sensorPositions[0] = new Vector3(transform.position.x - boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z + boardDimensions.z / 2);
		sensorPositions[1] = new Vector3(transform.position.x + boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z + boardDimensions.z / 2);
		sensorPositions[2] = new Vector3(transform.position.x - boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z - boardDimensions.z / 2);
		sensorPositions[3] = new Vector3(transform.position.x + boardDimensions.x / 2, transform.position.y - boardDimensions.y / 2, transform.position.z - boardDimensions.z / 2);
		return sensorPositions;
	}

	void CreateSensors(Vector3[] sensorPositions)
	{
		m_sensors = new Transform[sensorPositions.Length];
		for (int i = 0; i < m_sensors.Length; i++) {
			GameObject sensor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sensor.name = "Sensor";
			sensor.transform.parent = transform;
			sensor.transform.localPosition = transform.InverseTransformPoint(sensorPositions[i]);
			Destroy(sensor.GetComponent<MeshRenderer>());
			Destroy(sensor.GetComponent<Collider>());
			m_sensors[i] = sensor.transform;
		}
	}

	void CreateThruster(Vector3 boardDimensions)
	{
		Vector3 thrusterPosition = transform.position;
		thrusterPosition.z = transform.position.z - boardDimensions.z / 2;
		GameObject thruster = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		thruster.name = "Thruster";
		thruster.transform.parent = transform;
		thruster.transform.localPosition = transform.InverseTransformPoint(thrusterPosition);
		Destroy(thruster.GetComponent<MeshRenderer>());
		Destroy(thruster.GetComponent<Collider>());
		m_thruster = thruster.transform;
	}

	public void Move(float thrust, float lean, bool jump, float glide, bool glide2)
	{
		m_thrust = thrust;
		m_lean = lean;
		m_jump = jump;
		if (glide2){m_glide = 1.0f;}
		else{m_glide = Mathf.Abs (glide);}
		
	}

	void Awake()
	{
		Vector3 boardDimensions = CalculateBoardDimensions();
		CreateSensors(CalculateSensorPositions(boardDimensions));
		CreateThruster(boardDimensions);
		m_hits = new RaycastHit[m_sensors.Length];
	}
	
	void OnCollisionEnter(Collision collision) {
		detach = false;
		grinding = false;
		Debug.Log("Hit ground!");
	}
	/*
	void OnCollisionStay(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			if(contact.otherCollider.tag=="Grind") {
				grinding = true;
				grindRail = contact.otherCollider.transform;
				Debug.Log("Yarrr! Ye be colldin' with a rail.");
			}
		}
	}

	void OnCollisionExit(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			if(contact.otherCollider.tag=="Grind") {
				grinding = false;
				Debug.Log("Yarrr! Ye be leavin' the rail.");
			}
		}
	}*/
	
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Grind" && col.gameObject.transform!=lastRail){ 
				grindRail = col.gameObject.transform;
				grindPoints = grindRail.GetComponentsInChildren<Transform>();
				//Debug.Log("Found rail");
			}
		
	}
	void OnTriggerExit(Collider col) {
		if (col.gameObject.transform==lastRail){
			lastRail = null;
		}
		if(col.gameObject.transform == grindRail){ 
				// OH NOES!!111 Find rail again if we're close enough can
				grindPoint = nearestGrindPoint(null);
				if( grindPoint == null ) {
					grinding = false;
					grindRail = null;
					Debug.Log("Lost rail :(");
				} else {
					rigidbody.velocity = Vector3.zero;
				}
		}
		
	}

	void FixedUpdate()
	{
		onGround = false;
		
		hoverMod = 1;
		if (detach){hoverMod = 0.5f;}
		
		for (int i = 0; i < m_sensors.Length; i++)
		{
			if (Physics.Raycast(m_sensors[i].position, -m_sensors[i].up, out m_hits[i], hoverProperties.hoverHeight*(hoverMod)))
			{
				onGround = true;
				if (!detach){
				float hoverForce = (hoverProperties.hoverHeight - m_hits[i].distance) * hoverProperties.hoverDamping * Time.deltaTime;
				rigidbody.AddForceAtPosition(m_sensors[i].up * hoverForce, m_sensors[i].position);
				}
				/*if (m_hits[i].collider.gameObject.tag=="Grind"){
					grinding = true;
					grindRail = m_hits[i].transform;
					Debug.Log("Yaarrr! Ye be grindin, ye scallywag.");
				}*/
			}
		}

		// GRINDS ----------------------------------------------------------------------------------------------------
		// TODO: BUGGY :(
		//       Following rail path seems to work, but smooth out grind mechanics a bit.
		Vector3 dir;
		// Impulse toward rail on button press, if board is close enough
		if(Input.GetButton("Grind") && grindRail != null) {
			if (!grinding) {
				Debug.Log("hey girl i see u tryna grind");
				grindPoint = nearestGrindPoint(grindPoint);
				Debug.Log ("Now grinding to "+grindPoint.position + " dir: " + grindDir.normalized);
				rigidbody.velocity = Vector3.zero; //cancel all forces
				grinding = true;
			}
			/*RaycastHit[] hits;
			hits = rigidbody.SweepTestAll(rigidbody.velocity.normalized, maxGrindDist);

			foreach( RaycastHit hit in hits ) {
				if( hit.transform.tag=="Grind"  ) {
					Debug.Log("Avast! Be ye lookin' to grind? " + hit.point);
					Debug.DrawLine(hit.point, transform.position,Color.green,5.0f);
					dir = hit.point - transform.position;
					rigidbody.AddForce(dir.normalized * grindSpeed);
				}
			}*/
		}

		if(grinding){
			rigidbody.useGravity = false;
			m_lean = 0;
			m_thrust = 0;
			// Find nearest point on the rail in the
			// direction of the player velocity		
			
			if (grindPoint == null || !Input.GetButton("Grind")){
				grinding = false;
				lastRail = grindRail;
				grindRail = null;
			}
			else{
				if (grindPoint == null){Debug.Log("Couldn't find next point!");}
				//Debug.DrawLine(grindPoint.position, transform.position,Color.red,25.0f);//,false);
				dir = grindPoint.position - transform.position;
				//transform.rotation += (dir.normalized - transform.rotation)*.2;
				rigidbody.AddForce(grindDir.normalized * grindSpeed /** Vector3.Distance(grindPoint.position,transform.position)*/);
				Debug.DrawLine(grindPoint.position,10000 * grindDir.normalized, Color.red,300.0f,false);
				
				// Did we pass the point? Hmm....
				dir = grindPoint.position - transform.position;
				if( Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) < 0 ) {
					grindPoint = nearestGrindPoint(null);
					rigidbody.velocity = Vector3.zero;
				} /*else if (Vector3.Distance(transform.position,grindPoint.position)<grindPointDist) {
					grindPoint = nearestGrindPoint(grindPoint);
					Debug.Log ("Now grinding to "+grindPoint);
				}*/
			}
		}
		else{rigidbody.useGravity = true;}
		
		clampVector = transform.rotation.eulerAngles;
		
		if (clampVector.z > 180) { clampVector.z = - 180 + (clampVector.z-180);}
		if (clampVector.x > 180) { clampVector.x = - 180 + (clampVector.x-180);}
		
		currentClamp = angleClamp + ((airAngleClamp-angleClamp) * Mathf.Clamp (airTime/0.5f,0.0f,1.0f));
		
		clampVector.z = Mathf.Clamp (clampVector.z,-currentClamp,currentClamp);
		clampVector.x = Mathf.Clamp (clampVector.x,-currentClamp,currentClamp);
		
		transform.localEulerAngles = clampVector;
		
		if (!onGround && m_glide>0.5f && glideLeft > 0){
			glidePower = glideForce + (((glideApexForce-glideForce) * Mathf.Pow (Mathf.Clamp(1 - (Mathf.Abs ((glideLength - glideApex)-glideLeft))/(glideLength-glideApex),0,1.0f),4.0f)));
			rigidbody.AddForce(transform.up * glidePower);
			glideLeft = Mathf.Clamp(glideLeft-(Time.deltaTime),0.0f,glideLength);
		}
		
		//transform.rotation.eulerAngles.z = Mathf.Clamp (transform.rotation.eulerAngles.z, -90.0f, 90.0f);
		//transform.rotation.eulerAngles.x = Mathf.Clamp (transform.rotation.eulerAngles.x, -90.0f, 90.0f);
		
		if (!onGround && detach){detach = false;}
		
		if (onGround){
			airTime = 0;
			glideLeft = Mathf.Clamp(glideLeft+(Time.deltaTime*3),0.0f,glideLength);
		if (m_jump){
			jumpPower = Mathf.Clamp(jumpPower+(Time.deltaTime)/jumpLength,0.0f,1.0f);}
		else{
			if (jumpPower > 0.0f){
				detach = true; 
				rigidbody.AddForce(transform.up * jumpForce * Mathf.Sqrt(jumpPower), ForceMode.Impulse);
				jumpPower = 0.0f;
			}
			}
		}
		else{
				jumpPower = 0.0f;
				airTime += (Time.deltaTime);
		}
		
		steerMod = 1;
		if (!onGround){steerMod = 0.35f;}
		
		rigidbody.AddForceAtPosition(m_thruster.forward * m_thrust * thrustPower * (1 + (0.25f * jumpPower)), m_thruster.position);
		rigidbody.AddTorque(transform.up * m_lean * steerMod * steerPower * (0.5f + ((1 - jumpPower)/1)));
		
		m_thrust = 0;
		m_lean = 0;
		m_jump = false;
	}
	
	Transform nearestGrindPoint(Transform exclude)
	{
		Vector3 dir, velocity;
		Transform nearestTrans = null;

		// Project velocity to XZ
		velocity = rigidbody.velocity;
		velocity.y = 0.0f;
		velocity.Normalize();

		Vector3 nearestPoint = new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
		foreach( Transform point in grindPoints ) {
			Debug.DrawRay(point.position,Vector3.up,Color.blue,500.0f,false);
			if(point != exclude) {
				dir = point.position - transform.position;
				// Rail in front of where we're going?
				if( Vector3.Dot(velocity.normalized,dir.normalized) > 0 ) {
					// Current point closer than nearest?
					if(Vector3.Distance(point.position,transform.position) < Vector3.Distance(nearestPoint,transform.position)) {
						nearestPoint = point.position;
						nearestTrans = point;
						
						Debug.DrawRay(transform.position,Vector3.forward,Color.blue,50.0f,false);
						Debug.DrawRay(transform.position,Vector3.right,Color.red,50.0f,false);
						Debug.DrawRay(transform.position,Vector3.up,Color.green,50.0f,false);

						dir = nearestTrans.position - transform.position;
						Debug.DrawRay(transform.position,dir.normalized,Color.white,50.0f,false);
						Debug.DrawRay(transform.position,rigidbody.velocity.normalized,Color.red,50.0f,false);
						// Construct grind direction vector
						/*if(point.parent != null) {
							Debug.Log("parent 1");
							dir = point.parent.position - transform.position;
							// behind us
							if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) > 0) {
								grindDir = point.position - point.parent.position;
							} else if( point.childCount != 0 ) {
								Debug.Log("child 1");
								dir = point.GetChild(0).position - transform.position;

								// behind us
								if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) > 0) {
									grindDir = point.position - point.GetChild(0).position;
								}
							} else if( point.childCount != 0 ) {
								Debug.Log("child 2");
								dir = point.GetChild(0).position - transform.position;

								// behind us
								if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) > 0) {
									grindDir = point.position - point.GetChild(0).position;
								} else if(point.parent != null) {
									Debug.Log("parent 2");
									dir = point.parent.position - transform.position;
									// behind us
									if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) > 0) {
										grindDir = point.position - point.parent.position;
									}
								}
							}
						}*/
					}
				}
			}
		}

		// Construct grind direction vector
		if( nearestTrans != null ) {
			if( nearestTrans.childCount != 0 ) {
				dir = nearestTrans.GetChild(0).position - transform.position;
				if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) < 0) {
					grindDir = nearestTrans.position - nearestTrans.GetChild(0).position;
					grindDir.Normalize();
				} else if (Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) > 0 && nearestTrans.parent == null) {
					grindDir = nearestTrans.position - transform.position;
					grindDir.Normalize();
				}
			} else if ( nearestTrans.parent != null ) {
				dir = nearestTrans.parent.position - transform.position;
				if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) < 0) {
					grindDir = nearestTrans.position - nearestTrans.parent.position;
					grindDir.Normalize();
				} else if(Vector3.Dot(rigidbody.velocity.normalized,dir.normalized) > 0 && nearestTrans.childCount == 0) {
					grindDir = nearestTrans.position - transform.position;
					grindDir.Normalize();
				}
			}
		}

		return nearestTrans;
	}
}