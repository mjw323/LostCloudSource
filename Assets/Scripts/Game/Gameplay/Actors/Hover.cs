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

	public void Move(float thrust, float lean)
	{
		m_thrust = thrust;
		m_lean = lean;
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
		for (int i = 0; i < m_sensors.Length; i++)
		{
			if (Physics.Raycast(new Ray(m_sensors[i].position, -m_sensors[i].up), out m_hits[i]))
			{
				float hoverForce = (hoverProperties.hoverHeight - m_hits[i].distance) * hoverProperties.hoverDamping * Time.deltaTime;
				rigidbody.AddForceAtPosition(m_sensors[i].up * hoverForce, m_sensors[i].position);
			}
		}
		rigidbody.AddForceAtPosition(transform.forward * thrustPower * m_thrust, m_thruster.position);
		rigidbody.AddTorque(transform.up * steerPower * m_lean);
		m_thrust = 0;
		m_lean = 0;
	}
}