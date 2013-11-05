using UnityEngine;
using System;

[AddComponentMenu ("Scripts/Character/Movement/FootMovement")]
[RequireComponent (typeof(CharacterController))]
public class FootMovement : MonoBehaviour
{
	[HideInInspector] private Transform m_transform;
	[HideInInspector] private CharacterController m_characterController;

	[SerializeField] private float m_speed;
	[SerializeField] private float m_gravity;
	[SerializeField] private float m_rotationDegreesPerSecond;

	private Vector3 m_direction;
	private float m_yVelocity;
	private float m_localAngle;

	public void MoveTowards(Vector3 direction)
	{
		m_direction = direction;
		m_direction.y = 0f;
		m_direction = Vector3.ClampMagnitude(m_direction, 1f);
	}

	void Awake()
	{
		m_transform = GetComponent<Transform>();
		m_characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		m_localAngle = Vector3.Angle(m_transform.forward, m_direction);
		m_localAngle *= (Vector3.Cross(m_transform.forward, m_direction).y >= 0f ? 1f : -1f);
		Debug.DrawRay(m_transform.position, m_transform.forward, Color.blue);
		Debug.DrawRay(m_transform.position, m_direction, Color.red);
		print(m_localAngle);

		if (m_characterController.isGrounded)
		{
			m_yVelocity = 0f;
		}
		else
		{
			m_yVelocity -= m_gravity * Time.deltaTime;
		}

		Vector3 xzVelocity = m_transform.forward * Mathf.Lerp(0f, m_speed, m_direction.sqrMagnitude);
		Vector3 finalVelocity = new Vector3(0f, m_yVelocity, 0f) + xzVelocity;
		m_characterController.Move(finalVelocity * Time.deltaTime);
	}

	void FixedUpdate()
	{
		if ( (m_direction.sqrMagnitude > 0) )//&& (m_localAngle >= 0 && m_direction.x >= 0) || (m_localAngle < 0 && m_direction.x < 0))
		{
			//Vector3 yRotation = new Vector3(0f, Mathf.Lerp(0f, m_rotationDegreesPerSecond * Mathf.Sign(m_localAngle), m_direction.sqrMagnitude));
			m_transform.rotation = Quaternion.RotateTowards(m_transform.rotation, Quaternion.LookRotation(m_direction, Vector3.up), m_rotationDegreesPerSecond * Time.deltaTime);
			//m_transform.rotation = m_transform.rotation * deltaRotation;	
		}
	}
}