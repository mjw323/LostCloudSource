//#define FOOT_CONTROLLER_DEBUG_LOG_DIRECTIONS
//#define FOOT_CONTROLLER_DEBUG_DRAW
using UnityEngine;
using Conditional = System.Diagnostics.ConditionalAttribute;

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

	private void Awake()
	{
		footMovement = GetComponent<FootMovement>();
		footMovement.OnSummonBoard += OnSummonBoard;

		boardController = GetComponent<BoardController>();
		
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		camera = mainCamera.GetComponent<Transform>();
	}

	private void OnEnable()
	{
		footMovement.enabled = true;
	}

	private void OnDisable()
	{
		footMovement.enabled = false;
	}

	private void OnDestroy()
	{
		if (footMovement != null) // Possible during shutdown
			footMovement.OnSummonBoard -= OnSummonBoard;
	}

  [Conditional("FOOT_CONTROLLER_DEBUG_LOG_DIRECTIONS")]
  private void LogDirections(Vector3 cameraForward, Vector3 moveDirection) {
    Debug.Log("[Foot Controller] Camera Forward: " + cameraForward, gameObject);
    Debug.Log("[Foot Controller] Move Direction: " + moveDirection, gameObject);
  }

	[Conditional("FOOT_CONTROLLER_DEBUG_DRAW")]
	private void DrawRay(Vector3 origin, Vector3 direction, Color color) {
		Debug.DrawRay(origin, direction, color);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire3")) // TODO: Rename this input!
			footMovement.SummonBoard();

		float stickX = Input.GetAxis("Horizontal");
		float stickY = Input.GetAxis("Vertical");
		Vector3 stickDirection = new Vector3(stickX, 0f, stickY);
		
    // Correct camera's look direction to always be parallel to XZ-plane
		Vector3 cameraLook = Vector3.Cross(camera.right, Vector3.up);
    DrawRay(camera.position, cameraLook, Color.green);
		
		Quaternion stickToWorld = Quaternion.FromToRotation(Vector3.forward,
			cameraLook);
		Vector3 moveDirection = stickToWorld * stickDirection;
		
		if (Vector3.Magnitude(moveDirection)>.3f){
				this.GetComponent<Animator>().enabled=true;
		}
		
		footMovement.MoveTowards(moveDirection);

		if (Input.GetButtonDown("Jump"))
			footMovement.Jump();

    LogDirections(cameraLook, moveDirection);
		DrawRay(camera.position, camera.forward, Color.blue);
		DrawRay(transform.position, moveDirection, Color.red);
	}

	// Internal references
	[HideInInspector] private FootMovement footMovement;
	[HideInInspector] private BoardController boardController;

	// External references
	[HideInInspector] private Transform camera;
}