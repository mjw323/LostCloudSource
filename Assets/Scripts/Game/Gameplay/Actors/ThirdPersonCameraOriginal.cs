using UnityEngine;

/// <summary>
/// Camera controller that orbits around the player.
/// </summary>
[AddComponentMenu ("Camera-Control/ThirdPersonCameraOriginal")]
public class ThirdPersonCameraOriginal : MonoBehaviour
{
	void Awake()
	{	
		transform = GetComponent<Transform>();

		GameObject player = GameObject.FindWithTag("Player");
		target = player.GetComponent<Transform>();
	}

	void LateUpdate()
	{
		Vector3 targetOffset = target.position + offset;
		Vector3 lookDirection = targetOffset - transform.position;
		lookDirection.y = 0f;
		lookDirection = lookDirection.normalized;

		Vector3 targetPosition = target.position + (Vector3.up * distanceAbove) - (lookDirection * distanceAwayFrom);
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followTime);
		transform.LookAt(targetOffset);
	}

	[SerializeField] private Vector3 offset;
	[SerializeField] private float distanceAbove;
	[SerializeField] private float distanceAwayFrom;
	[SerializeField] private float followTime;

	[HideInInspector] new private Transform transform;

	[HideInInspector] private Transform target;

	private Vector3 velocity;
}