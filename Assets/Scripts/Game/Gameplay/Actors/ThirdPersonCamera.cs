using UnityEngine;

[AddComponentMenu ("Scripts/Camera/ThirdPersonCamera")]
public class ThirdPersonCamera : MonoBehaviour
{
	[HideInInspector] private Transform m_transform;
	[HideInInspector] private Transform m_target;

	[SerializeField] private Vector3 m_offset;
	[SerializeField] private float m_distanceAbove;
	[SerializeField] private float m_distanceAwayFrom;
	[SerializeField] private float m_followTime;

	private Vector3 m_velocity;

	void Awake()
	{
		m_transform = GetComponent<Transform>();

		GameObject player = GameObject.FindWithTag("Player");
		if (player == null)
		{
			Debug.LogError("Could not find object with tag \"Player\".");
			Debug.Break();
		}
		m_target = player.GetComponent<Transform>();
	}

	void LateUpdate()
	{
		Vector3 targetOffset = m_target.position + m_offset;
		Vector3 lookDirection = targetOffset - m_transform.position;
		lookDirection.y = 0f;
		lookDirection = lookDirection.normalized;

		Vector3 targetPosition = m_target.position + (Vector3.up * m_distanceAbove) - (lookDirection * m_distanceAwayFrom);
		m_transform.position = Vector3.SmoothDamp(m_transform.position, targetPosition, ref m_velocity, m_followTime);
		m_transform.LookAt(targetOffset);
	}
}