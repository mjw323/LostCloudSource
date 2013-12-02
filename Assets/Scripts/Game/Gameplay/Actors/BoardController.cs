using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for Noke in the board-riding state.
/// </summary>
[AddComponentMenu ("Player/BoardController")]
public class BoardController : MonoBehaviour
{
	private void OnDismissBoard()
	{
		enabled = false;
		footController.enabled = true;
	}

	void Awake()
	{
		transform = GetComponent<Transform>();

		footController = GetComponent<FootController>();

		GameObject board = GameObject.FindWithTag("Board");
		boardTransform = board.GetComponent<Transform>();
		boardMovement = board.GetComponent<Hover>();
		boardMovement.OnDismissBoard += OnDismissBoard;

		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		cameraTransform = mainCamera.GetComponent<Transform>();

		oldParent = transform.parent.GetComponent<Transform>();
	}

	void OnEnable()
	{
		boardTransform.position = transform.position;
		boardTransform.forward = transform.forward;
		transform.parent = boardTransform;
		transform.parent.Translate(0f, 0.25f, 0f); // Prevent fall-through
		boardMovement.enabled = true;
	}

	void OnDisable()
	{
		transform.parent = oldParent;
		boardMovement.enabled = false;
	}

	void OnDestroy()
	{
		if (boardMovement != null) // Possible during shutdown
			boardMovement.OnDismissBoard -= OnDismissBoard;
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire3")) // TODO: Rename this input!
			boardMovement.DismissBoard();

		boardMovement.Move(
			Input.GetAxis("Vertical"),
			Input.GetAxis("Horizontal"),
			Input.GetButton("Jump"),
			Input.GetAxis("Glide"),
			Input.GetButton("Glide"));
	}

	// Internal references
	[HideInInspector] new private Transform transform;
	[HideInInspector] private FootController footController;

	// External references
	[HideInInspector] private Transform boardTransform;
	[HideInInspector] private Hover boardMovement;
	[HideInInspector] private Transform cameraTransform;
	[HideInInspector] private Transform oldParent;
}