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

	[HideInInspector] Transform[] m_sensors;
	RaycastHit[] m_hits;
	Transform m_thruster;
	float m_thrust;
	float m_lean;
	private bool m_jump;
	private bool detach;
	
	private float jumpPower; // measures how long jump was held, 0 - 1
	public float jumpLength = 3.0f; //how many seconds to build maximum jump
	public float jumpForce = 20.0f; //The max force applied when jumping
	
	private bool onGround;
	private Vector3 clampVector;

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

	public void Move(float thrust, float lean, bool jump)
	{
		m_thrust = thrust;
		m_lean = lean;
		m_jump = jump;
		
	}

	void Awake()
	{
		Vector3 boardDimensions = CalculateBoardDimensions();
		CreateSensors(CalculateSensorPositions(boardDimensions));
		CreateThruster(boardDimensions);
		m_hits = new RaycastHit[m_sensors.Length];
	}

	void FixedUpdate()
	{
		onGround = false;
		for (int i = 0; i < m_sensors.Length; i++)
		{
			if (Physics.Raycast(m_sensors[i].position, -m_sensors[i].up, out m_hits[i], hoverProperties.hoverHeight*2))
			{
				onGround = true;
				if (!detach){
				float hoverForce = (hoverProperties.hoverHeight - m_hits[i].distance) * hoverProperties.hoverDamping * Time.deltaTime;
				rigidbody.AddForceAtPosition(m_sensors[i].up * hoverForce, m_sensors[i].position);
				}
			}
		}
		
		clampVector = transform.rotation.eulerAngles;
		clampVector.z = Mathf.Clamp (clampVector.z,Mathf.Deg2Rad*-90.0f,Mathf.Deg2Rad*90.0f);
		clampVector.x = Mathf.Clamp (clampVector.x,Mathf.Deg2Rad*-90.0f,Mathf.Deg2Rad*90.0f);
		
		transform.localEulerAngles = clampVector;
		
		//transform.rotation.eulerAngles.z = Mathf.Clamp (transform.rotation.eulerAngles.z, -90.0f, 90.0f);
		//transform.rotation.eulerAngles.x = Mathf.Clamp (transform.rotation.eulerAngles.x, -90.0f, 90.0f);
		
		if (!onGround && detach){detach = false;}
		
		if (onGround){
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
		else{jumpPower = 0.0f;}
		
		rigidbody.AddForceAtPosition(m_thruster.forward * m_thrust * thrustPower * (1 + (0.25f * jumpPower)), m_thruster.position);
		rigidbody.AddTorque(transform.up * m_lean * steerPower * (0.5f + ((1 - jumpPower)/1)));
		

		m_thrust = 0;
		m_lean = 0;
		m_jump = false;
	}
}