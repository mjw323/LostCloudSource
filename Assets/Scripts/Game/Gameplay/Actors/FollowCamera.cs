using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu ("Camera-Control/FollowCamera")]
public class FollowCamera : MonoBehaviour
{
	public void Enable()
	{
		followTask.Resume();
		lookTask.Resume();
	}

	public void Disable()
	{
		followTask.Pause();
		lookTask.Pause();
	}

	[Serializable]
	public class FollowParameters
	{
		public float distanceAbove;
		public float distanceAway;
		public float time;
	}

	[Serializable]
	public class LookParameters
	{
		public float distanceAbove;
		public float speed;
	}

	private void Awake()
	{
		GameObject player = GameObject.FindWithTag("Player");
		focus = player.GetComponent<Transform>();
		transform = GetComponent<Transform>();
		followTask = new Task(Follow());
		lookTask = new Task(Look());
	}

	private IEnumerator Follow()
	{
		while (true)
		{
			Vector3 aboveOffset = focus.up * follow.distanceAbove;
			Vector3 lookDirection = focus.position - transform.position;
			lookDirection.y = 0;
			lookDirection.Normalize();
			Vector3 awayOffset = -lookDirection * follow.distanceAway;
			Vector3 targetPosition = focus.position + aboveOffset + awayOffset;
			transform.position = Vector3.SmoothDamp(transform.position,
				targetPosition, ref velocity, follow.time);
			yield return null;
		}
	}

	private IEnumerator Look()
	{
		while (true)
		{
			Vector3 aboveOffset = focus.up * look.distanceAbove;
			Vector3 lookPosition = focus.position + aboveOffset;
			Quaternion rotation = Quaternion.LookRotation(lookPosition -
				transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation,
				rotation, Time.deltaTime * look.speed);
			yield return null;
		}
	}

	[HideInInspector] private Transform focus;

	[HideInInspector] new private Transform transform;

	[SerializeField] private FollowParameters follow;
	[SerializeField] private LookParameters look;

	[HideInInspector] private Task followTask;
	[HideInInspector] private Task lookTask;

	private Vector3 velocity;
}