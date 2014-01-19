using UnityEngine;
using System;
using System.Collections;

[AddComponentMenu ("Camera-Control/ThirdPersonCameraOriginal")]
public class ThirdPersonCameraOriginal : MonoBehaviour
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
			Vector3 awayOffset = -transform.forward * follow.distanceAway;
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