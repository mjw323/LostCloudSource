using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Controller for the player character's on-foot movement behavior.
/// <summary>
[AddComponentMenu ("Player/FootMovement")]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(CharacterController))]
public class FootMovement : MonoBehaviour
{
	[System.NonSerialized]
	public float lookWeight;
	[System.NonSerialized]
	public Transform enemy;
	public float lookSmoother = 3f;
	/// <summary>
	/// Move towards the provided direction.
	/// </summary>
	/// <param name="direction">
	/// Direction to move towards. The player will rotate towards this
	/// direction if she is not already facing it, and run along it if
	/// she is.
	/// </param>
	private Vector3 prevDir;
	public List<AudioClip> grassSteps;
	public List<AudioClip> sandSteps;
	public float stepFreq = .33f;
	private float stepTime = 0f;
	private bool jumping=false;
	private float airTime = 0f;
	private float jumpTime = 3f;
	
	//public float angleChange = 0f;
	public void MoveTowards(Vector3 direction)
	{
		animator.SetFloat(stickMagId, Vector3.Magnitude (direction));
		if (Vector3.Magnitude (prevDir) > 0f && Vector3.Magnitude (direction) > 0f) {
						animator.SetFloat (angleChangeId, Mathf.Abs(Vector3.Angle (prevDir, direction)));
						/*if (Mathf.Abs(Vector3.Angle (prevDir, direction))>1){
							Debug.Log ("prev: " + prevDir + ", cur: " + direction + ", diff: " + Vector3.Angle (prevDir, direction));
						}*/
				}
		this.direction = direction;
		if (Vector3.Magnitude (direction) > 0f) {
						prevDir = direction;
				}
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
		enemy = GameObject.Find("Enemy").transform;

		speedId = Animator.StringToHash("Speed");
		directionId = Animator.StringToHash("Direction");
		jumpId = Animator.StringToHash("Jump");
		angleChangeId = Animator.StringToHash("AngleChange");
		stickMagId = Animator.StringToHash("StickMag");
		ridingId = Animator.StringToHash("Riding");

		pivotLeftId = Animator.StringToHash("Base Layer.Locomotion.TurnOnSpot");
		plantLeftId = Animator.StringToHash(
			"Base Layer.Locomotion.PlantTurnLeft");
		plantRightId = Animator.StringToHash(
			"Base Layer.Locomotion.PlantTurnRight");
		Debug.Log ("awake");
	}

	private void OnEnable()
	{
		characterController.enabled = true;
		animator.applyRootMotion = true;
		animator.SetBool(ridingId, false);
		
		Debug.Log ("fixing rotation");

		// Enforce "uprightness"
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
	}

	private void OnDisable()
	{
		characterController.enabled = false;
		animator.applyRootMotion = false;
		animator.SetBool(ridingId, true);
	}

	private void FixedUpdate()
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
		if (runSpeed < minSpeed){
			runSpeed = 0f;
			stepTime = 0f;
		}else{
			stepTime += Time.deltaTime;
			if (stepTime >= stepFreq){
				if (this.transform.position.y > 4f){
						audio.clip = grassSteps[Random.Range (0,grassSteps.Count)];
					}else{
					audio.clip = sandSteps[Random.Range (0,sandSteps.Count)];
				}
				audio.Play ();
				stepTime = 0f;
			}
		}
		animator.SetFloat(speedId, runSpeed);
		///////////////////

		
		if (jumping){ //change jumping state on land or if in air for too long
			airTime += Time.deltaTime;
			if (airTime >= jumpTime || characterController.isGrounded){jumping = false;}
		}else{airTime = 0f;}

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
			jumping = true;
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
		//Look at enemy
		animator.SetLookAtWeight(lookWeight);
		if(Input.GetButton("LookAt"))
		{
			animator.SetLookAtPosition(enemy.position);
			lookWeight = Mathf.Lerp(lookWeight, 0f, Time.deltaTime*lookSmoother);
			Debug.Log("looked at"+ enemy.position);
		}
		else
		{	
			lookWeight = Mathf.Lerp(lookWeight, 0f, Time.deltaTime*lookSmoother);
		}
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
	[HideInInspector] private int angleChangeId;
	[HideInInspector] private int ridingId;
	[HideInInspector] private int stickMagId;

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