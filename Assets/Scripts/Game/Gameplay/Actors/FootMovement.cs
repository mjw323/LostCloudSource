using UnityEngine;

/// <summary>
/// Controller for the player character's on-foot movement behavior.
/// <summary>
[AddComponentMenu ("Player/FootMovement")]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(CharacterController))]
public class FootMovement : MonoBehaviour
{
	/// <summary>
	/// Move towards the provided direction.
	/// </summary>
	/// <param name="direction">
	/// Direction to move towards. The player will rotate towards this
	/// direction if she is not already facing it, and run along it if
	/// she is.
	/// </param>
	public void MoveTowards(Vector3 direction)
	{
		this.direction = direction;
	}

	/// <summary>
	/// Causes the player to jump.
	/// </summary>
	/// <remarks>
	/// Calling this method while the player is mid-air will not cause
	/// them to jump upon landing.
	/// </remarks>
	public void Jump()
	{
		if (characterController.isGrounded)
			shouldJump = true;
	}

	/// <summary>
	/// Place Noke on her board here.
	/// </summary>
	public delegate void SummonedBoardHandler();

	/// <summary>
	/// Fired when Noke should get on her board.
	/// </summary>
	public event SummonedBoardHandler OnSummonBoard;

	/// <summary>
	/// Summons Noke's board and places her on it.
	/// </summary>
	/// <remarks>
	/// Disables the set of scripts pertaining to on-foot movement and
	/// enables the one for board-riding.
	/// </remarks>
	public void SummonBoard()
	{
		if (characterController.isGrounded)
			shouldBoard = true;
	}

	private void Awake()
	{
		transform = GetComponent<Transform>();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();

		speedId = Animator.StringToHash("Speed");
		directionId = Animator.StringToHash("Direction");
		jumpId = Animator.StringToHash("Jump");

		pivotLeftId = Animator.StringToHash("Base Layer.Locomotion.TurnOnSpot");
		plantLeftId = Animator.StringToHash(
			"Base Layer.Locomotion.PlantTurnLeft");
		plantRightId = Animator.StringToHash(
			"Base Layer.Locomotion.PlantTurnRight");
	}

	private void OnEnable()
	{
		characterController.enabled = true;
		animator.applyRootMotion = true;

		// Enforce "uprightness"
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
	}

	private void OnDisable()
	{
		characterController.enabled = false;
		animator.applyRootMotion = false;
	}

	private void Update()
	{
		if (shouldBoard)
		{
			OnSummonBoard();
			shouldBoard = false; // Reset to avoid boarding at unexpected time
			return;
		}
		shouldBoard = false; // See above

		float turnAngle;
		if (direction == Vector3.zero)
			turnAngle = 0f;
		else
		{
			float angleSign;
			if (Vector3.Cross(direction, transform.forward).y >= 0)
				angleSign = -1f;
			else
				angleSign = 1f;
			turnAngle =  Vector3.Angle(direction, transform.forward) * angleSign;
		}
		animator.SetFloat(directionId, turnAngle);

		AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		bool isPivoting = (animStateInfo.nameHash == pivotLeftId);
		bool isPlanting = ((animStateInfo.nameHash == plantLeftId) ||
			(animStateInfo.nameHash == plantRightId));

		// Manually rotate if we aren't in an animation-driven turn/plant
		if (turnAngle != 0f && !(isPivoting || isPlanting))
		{
			Quaternion targetRotation = Quaternion.LookRotation(direction,
				transform.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation,
				targetRotation, turnSpeed * Time.deltaTime);
		}

		float runSpeed = direction.sqrMagnitude * maxSpeed;
		if (runSpeed < minSpeed)
			runSpeed = 0f;
		animator.SetFloat(speedId, runSpeed);

		// Reset the Jump animator parameter to prevent looping
		if (jumpedLastFrame)
		{
			animator.SetBool(jumpId, false);
			jumpedLastFrame = false;
		}

		if (shouldJump)
		{
			animator.SetBool(jumpId, true);
			shouldJump = false;
			jumpedLastFrame = true;
		}

		if (shouldLiftOff)
			shouldLiftOff = false;
		else if (characterController.isGrounded)
			velocity.y = -gravity * Time.deltaTime;
		else
		{
			velocity.y -= gravity * Time.deltaTime;
			if (velocity.y < -maxFallSpeed)
				velocity.y = -maxFallSpeed;
		}
		
		characterController.Move(velocity * Time.deltaTime);
	}

	// Called when Noke's feet leave the ground in her jump animation.
	private void LiftOffIdle()
	{
		velocity.y = jumpForce;
		shouldLiftOff = true;
	}

	// Same deal, but when running.
	private void LiftOffRunning()
	{
		velocity.y = jumpForce;
		shouldLiftOff = true;
	}

	[SerializeField] private float minSpeed;
	[SerializeField] private float maxSpeed;
	[SerializeField] private float turnSpeed; // Degrees per second
	[SerializeField] private float gravity;
	[SerializeField] private float maxFallSpeed;
	[SerializeField] private float jumpForce;

	// Internal references
	[HideInInspector] new private Transform transform;
	[HideInInspector] private Animator animator;
	[HideInInspector] private CharacterController characterController;
	[HideInInspector] private float speed = 0f;

	// Animator parameter references
	[HideInInspector] private int speedId;
	[HideInInspector] private int directionId;
	[HideInInspector] private int jumpId;

	// Animator state references
	[HideInInspector] private int pivotLeftId;
	// [HideInInspector] private int pivotRightId; // Not yet implemented
	[HideInInspector] private int plantLeftId;
	[HideInInspector] private int plantRightId;

	// Temporary state
	private Vector3 direction;
	private Vector3 velocity;
	private bool shouldJump;
	private bool jumpedLastFrame;
	private bool shouldBoard;
	private bool shouldLiftOff;

	public float Speed
	{
		get{return this.speed;}
		set{speed = direction.sqrMagnitude * maxSpeed;}
	}
	
	public Animator Animator
	{
		get{return this.animator;}
	}
}