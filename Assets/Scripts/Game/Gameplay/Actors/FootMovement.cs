using UnityEngine;
using System;

[AddComponentMenu ("Player/FootMovement")]
[RequireComponent (typeof(CharacterController))]
public class FootMovement : MonoBehaviour
{
	public void MoveTowards(Vector3 direction)
	{
		m_direction = direction;
		m_direction.y = 0f;
		m_direction = Vector3.ClampMagnitude(m_direction, 1f);
	}

	private float CalculateRunSpeed()
	{
		float runSpeed = m_direction.sqrMagnitude * m_running.speed;
		if (runSpeed < m_running.threshold)
			return 0f;
		else
			return runSpeed;
	}

	private float CalculateTurnAngle()
	{
		if (m_direction == Vector3.zero)
			return 0f;
		else
		{
			float angleSign = (Vector3.Cross(m_direction, m_transform.forward).y >= 0) ? -1f : 1f;
			return Vector3.Angle(m_direction, m_transform.forward) * angleSign;
		}
	}

	private void RotateTowardsMovementDirection(float angleFacingToMovement)
	{
		if (angleFacingToMovement != 0f)
		{
			m_transform.rotation = Quaternion.RotateTowards(m_transform.rotation, Quaternion.LookRotation(m_direction, m_transform.up), m_running.rotationDegreesPerSecond * Time.deltaTime);
		}
	}

	private void UpdateAnimator(float speed, float direction)
	{
		m_animator.SetFloat(m_speedId, speed);
		m_animator.SetFloat(m_directionId, direction);
	}

	private void UpdateYVelocity()
	{
		if (m_characterController.isGrounded)
			m_yVelocity = 0f;
		else
		{
			m_yVelocity -= m_falling.gravity * Time.deltaTime;
			if (m_yVelocity < -m_falling.maxSpeed)
				m_yVelocity = -m_falling.maxSpeed;
		}
	}

	private void UpdateXZVelocity(float speed)
	{
		m_xzVelocity = m_transform.forward * speed;
	}

	private Vector3 CalculateFinalVelocity()
	{
		Vector3 velocity = m_xzVelocity;
		velocity.y = m_yVelocity;
		return velocity;
	}

	void Awake()
	{
		m_transform = GetComponent<Transform>();
		m_characterController = GetComponent<CharacterController>();
		m_animator = GetComponentInChildren<Animator>();

		m_speedId = Animator.StringToHash("Speed");
		m_directionId = Animator.StringToHash("Direction");
	}

	void Update()
	{
		float runSpeed = CalculateRunSpeed();
		float turnAngle = CalculateTurnAngle();

		UpdateAnimator(runSpeed, turnAngle);

		RotateTowardsMovementDirection(turnAngle);

		UpdateYVelocity();
		UpdateXZVelocity(runSpeed);

		m_characterController.Move(CalculateFinalVelocity() * Time.deltaTime);
	}

	[Serializable]
	public class RunningParameters
	{
		public float speed;
		public float threshold;
		public float rotationDegreesPerSecond;
	}

	[Serializable]
	public class JumpingParameters
	{
		public float height;
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
}