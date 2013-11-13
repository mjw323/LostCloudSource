using UnityEngine;
using System;

[AddComponentMenu ("Player/FootMovement")]
[RequireComponent (typeof(CharacterController))]
public class FootMovement : MonoBehaviour
{
	[Serializable]
	public class RunningParameters
	{
		public float speed;
		public float rotationDegreesPerSecond;
	}

	[Serializable]
	public class FallingParameters
	{
		public float gravity;
		public float maxSpeed;
	}

	[SerializeField] private RunningParameters m_running;
	[SerializeField] private FallingParameters m_falling;

	[HideInInspector] Transform m_transform;
	[HideInInspector] CharacterController m_characterController;
	[HideInInspector] Animator m_animator;

	[HideInInspector] private int m_speedId;
	[HideInInspector] private int m_directionId;

	private Vector3 m_direction;
	private Vector3 m_xzVelocity;
	private float m_yVelocity;

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
		m_animator = GetComponentInChildren<Animator>();
		if (m_animator == null)
		{
			Debug.LogError("Animator component not found on any child object.");
			Debug.Break();
		}

		m_speedId = Animator.StringToHash("Speed");
		m_directionId = Animator.StringToHash("Direction");
	}

	void Update()
	{
		float runSpeed = m_direction.sqrMagnitude * m_running.speed;
		float turnAngle;
		if (m_direction == Vector3.zero)
			turnAngle = 0f;
		else
		{
			float angleSign = (Vector3.Cross(m_direction, m_transform.forward).y >= 0) ? -1f : 1f;
			turnAngle = Vector3.Angle(m_direction, m_transform.forward) * angleSign;
		}

		if (m_xzVelocity != Vector3.zero)
		{
			m_transform.rotation = Quaternion.RotateTowards(m_transform.rotation, Quaternion.LookRotation(m_direction, m_transform.up), m_running.rotationDegreesPerSecond * Time.deltaTime);
		}

		m_animator.SetFloat(m_speedId, runSpeed);
		m_animator.SetFloat(m_directionId, turnAngle);

		if (m_characterController.isGrounded)
			m_yVelocity = 0f;
		else
			m_yVelocity -= m_falling.gravity * Time.deltaTime;

		m_xzVelocity = m_transform.forward * runSpeed;

		Vector3 finalVelocity = m_xzVelocity;
		finalVelocity.y = m_yVelocity;
		m_characterController.Move(finalVelocity * Time.deltaTime);
	}
}