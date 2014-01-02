using UnityEngine;

/// <summary>
/// Controller for Noke in the on-foot state.
/// </summary>
[AddComponentMenu ("Player/FootController")]
[RequireComponent (typeof(FootMovement))]
public class FootController : MonoBehaviour
{
	private void OnSummonBoard()
	{
		enabled = false;
		boardController.enabled = true;
	}

	void Awake()
	{
		footMovement = GetComponent<FootMovement>();
		footMovement.OnSummonBoard += OnSummonBoard;

		boardController = GetComponent<BoardController>();
		
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		cameraTransform = mainCamera.GetComponent<Transform>();
	}

	void OnEnable()
	{
		footMovement.enabled = true;
	}

	void OnDisable()
	{
		footMovement.enabled = false;
	}

	void OnDestroy()
	{
		if (footMovement != null) // Possible during shutdown
			footMovement.OnSummonBoard -= OnSummonBoard;
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire3")) // TODO: Rename this input!
			footMovement.SummonBoard();

		float stickX = Input.GetAxis("Horizontal");
		float stickY = Input.GetAxis("Vertical");
		Vector3 stickDirection = new Vector3(stickX, 0f, stickY);
		
		Vector3 cameraLook = cameraTransform.forward;
		cameraLook.y = 0f; // Flatten look direction into 2D
		
		Quaternion stickToWorld = Quaternion.FromToRotation(Vector3.forward,
			cameraLook.normalized);
		Vector3 moveDirection = stickToWorld * stickDirection;

		footMovement.MoveTowards(moveDirection);

		if (Input.GetButtonDown("Jump"))
			footMovement.Jump();
	}

	// Internal references
	[HideInInspector] private FootMovement footMovement;
	[HideInInspector] private BoardController boardController;

	// External references
	[HideInInspector] private Transform cameraTransform;
}