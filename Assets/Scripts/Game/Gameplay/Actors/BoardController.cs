using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for Noke in the board-riding state.
/// </summary>
[AddComponentMenu ("Player/BoardController")]
public class BoardController : MonoBehaviour
{
	/// <summary>
	/// Force Noke to dismount her hoverboard.
	/// </summary>
	public void DismissBoard()
	{
		boardMovement.DismissBoard();
	}

	private void OnDismissBoard()
	{
		enabled = false;
		footController.enabled = true;
	}

	private void Awake()
	{
		transform = GetComponent<Transform>();

		footController = GetComponent<FootController>();
		animator = GetComponent<Animator>();


		leanId = Animator.StringToHash("Lean");
		jumpId = Animator.StringToHash("BoardJump");

		GameObject board = GameObject.FindWithTag("Board");
		boardTransform = board.GetComponent<Transform>();
		boardMovement = board.GetComponent<Hover>();
		boardMovement.OnDismissBoard += OnDismissBoard;

		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		cameraTransform = mainCamera.GetComponent<Transform>();

		oldParent = transform.parent;
	}

	private void OnEnable()
	{
		boardTransform.position = transform.position;
		boardTransform.forward = transform.forward;
		transform.parent = boardTransform;
		transform.parent.Translate(0f, 0.25f, 0f); // Prevent fall-through
		boardMovement.enabled = true;
	}

	private void OnDisable()
	{
		transform.parent = oldParent;
		boardMovement.enabled = false;
	}

	private void OnDestroy()
	{
		if (boardMovement != null) // Possible during shutdown
			boardMovement.OnDismissBoard -= OnDismissBoard;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire3")) // TODO: Rename this input!
			boardMovement.DismissBoard();
		animator.SetFloat(leanId, boardMovement.Lean());
		animator.SetBool(jumpId, boardMovement.Jump());
		/*boardMovement.Move(
			Input.GetAxis("Vertical"),
			Input.GetAxis("Horizontal"),
			Input.GetButton("Jump"),
			Input.GetAxis("Glide"),
			Input.GetButton("Glide"));*/
	}

	// Internal references
	[HideInInspector] new private Transform transform;
	[HideInInspector] private FootController footController;
	[HideInInspector] private Animator animator;

	// External references
	[HideInInspector] private Transform boardTransform;
	[HideInInspector] private Hover boardMovement;
	[HideInInspector] private Transform cameraTransform;
	[HideInInspector] private Transform oldParent;

	// Animator parameter references
	//[HideInInspector] private int speedId;
	//[HideInInspector] private int directionId;
	//[HideInInspector] private int jumpId;
	[HideInInspector] private int leanId;
	[HideInInspector] private int jumpId;

	// Animator state references
	/*[HideInInspector] private int pivotLeftId;
	// [HideInInspector] private int pivotRightId; // Not yet implemented
	[HideInInspector] private int plantLeftId;
	[HideInInspector] private int plantRightId;*/
	/*
	public Animator Animator
	{
		get{return this.animator;}
	}*/
}