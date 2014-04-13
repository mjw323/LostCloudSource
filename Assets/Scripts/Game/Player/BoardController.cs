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
		jumpingId = Animator.StringToHash("Jumping");
		landingId = Animator.StringToHash("Landing");
		landStrId = Animator.StringToHash("LandStrength");
		groundedId = Animator.StringToHash("Grounded");
		needLandId = Animator.StringToHash("NeedToLand");

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
        boardMovement.Move(Input.GetAxis("Vertical"),
                           Input.GetAxis("Horizontal"),
                           Input.GetButton("Jump"),
                           Input.GetAxis("Glide"),
                           Input.GetButton("Glide"));
        if (Input.GetButtonDown("Fire3")) {
            boardMovement.DismissBoard();
        }
    }

	private void FixedUpdate()
	{
		animator.SetFloat(leanId, boardMovement.Lean());
		animator.SetFloat(landingId, boardMovement.Landing());
		
		if (boardMovement.Landing() > 0.25f && !needToLand) {
            needToLand = true;
            animator.SetBool(needLandId, true);
        } else {
            animator.SetBool(needLandId, false);
        }

		if (boardMovement.Landing() <= 0f) {
            needToLand = false;
        }
		
		if (landing == 0f && boardMovement.Landing()>0) {
            animator.SetFloat(landStrId, boardMovement.Landing());
        }

		animator.SetBool(jumpingId, boardMovement.Jumping());
		animator.SetBool(jumpId, boardMovement.Jump());
		animator.SetBool(groundedId, boardMovement.Grounded());
		
		landing = boardMovement.Landing();
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
	[HideInInspector] private int leanId;
	[HideInInspector] private int jumpId;
	[HideInInspector] private int jumpingId;
	[HideInInspector] private int landingId;
	[HideInInspector] private float landing;
	[HideInInspector] private int landStrId;
	[HideInInspector] private int groundedId;
	[HideInInspector] private int needLandId;
	private bool needToLand = false;
}