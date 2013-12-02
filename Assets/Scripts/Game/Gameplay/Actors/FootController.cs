using UnityEngine;

/// <summary>
/// Controller for Noke in the on-foot state.
/// </summary>
[AddComponentMenu ("Player/FootController")]
[RequireComponent (typeof(FootMovement))]
public class FootController : MonoBehaviour
{
	void Awake()
	{
		player = GetComponent<FootMovement>();
		
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

	[HideInInspector] private FootMovement player;
	[HideInInspector] private Transform cameraTransform;
}