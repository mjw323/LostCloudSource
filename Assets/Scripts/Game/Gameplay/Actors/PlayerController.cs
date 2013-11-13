using UnityEngine;

[AddComponentMenu ("Player/PlayerController")]
[RequireComponent (typeof(Player))]
public class PlayerController : MonoBehaviour
{
	[HideInInspector] private Player m_player;
	[HideInInspector] private ThirdPersonCamera m_camera;

	private Vector3 StickToWorldSpace(Vector3 stickDirection)
	{
		Vector3 cameraDirection = m_camera.transform.forward;
		cameraDirection.y = 0f;

		Quaternion stickToWorld = Quaternion.FromToRotation(Vector3.forward, cameraDirection.normalized);
		return stickToWorld * stickDirection;
	}

	void Awake()
	{
		m_player = GetComponent<Player>();
		
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		if (mainCamera == null)
		{
			Debug.LogError("Could not find object with tag \"MainCamera\".");
			Debug.Break();
		}
		m_camera = mainCamera.GetComponent<ThirdPersonCamera>();
		if (m_camera == null)
		{
			Debug.LogError("Main camera does not contain a \"ThirdPersonCamera\" component.");
			Debug.Break();
		}
	}

	void Update()
	{
		m_player.MoveTowards(StickToWorldSpace(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))));
	}
}