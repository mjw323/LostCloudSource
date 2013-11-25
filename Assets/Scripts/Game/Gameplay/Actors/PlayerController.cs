using UnityEngine;

/// <summary>
/// Maps controller input to player intent and forwards it along to the Player
/// state machine.
/// </summary>
[AddComponentMenu ("Player/PlayerController")]
[RequireComponent (typeof(Player))]
public class PlayerController : MonoBehaviour
{
	[HideInInspector] private Player m_player;
	[HideInInspector] private ThirdPersonCamera m_camera;

	void Awake()
	{
		player = GetComponent<Player>();
		
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		cameraTransform = mainCamera.GetComponent<Transform>();
	}

	void Update()
	{
		float stickX = Input.GetAxis("Horizontal");
		float stickY = Input.GetAxis("Vertical");
		Vector3 stickDirection = new Vector3(stickX, 0f, stickY);
		
		Vector3 cameraLook = cameraTransform.forward;
		cameraLook.y = 0f; // Flatten look direction into 2D
		
		Quaternion stickToWorld = Quaternion.FromToRotation(Vector3.forward,
			cameraLook.normalized);
		Vector3 moveDirection = stickToWorld * stickDirection;

		player.MoveTowards(moveDirection);

		if (Input.GetButtonDown("Jump"))
			player.Jump();
	}

	[HideInInspector] private Player player;
	[HideInInspector] private Transform cameraTransform;
}