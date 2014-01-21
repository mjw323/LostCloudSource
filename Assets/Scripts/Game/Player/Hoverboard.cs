using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Hoverboard : MonoBehaviour
{
	public delegate void DismissedBoardHandler();
	public event DismissedBoardHandler OnDismissBoard;

	public void DismissBoard()
	{
		shouldDismount = true;
	}

	private void Awake()
	{
		transform = GetComponent<Transform>();
		renderer = GetComponentInChildren<Renderer>();

		Corners corners = gameObject.AddComponent<Corners>();
		sensors = new HoverboardSensors(transform, corners);
		thrusters = new HoverboardThrusters(transform, corners);
		particles = new HoverboardParticles(transform);
		Destroy(corners);

		GameObject noke = GameObject.FindWithTag("Player");
		nokeAnimator = noke.GetComponent<Animator>();

		GameObject camera = GameObject.FindWithTag("MainCamera");
		cameraWhoosh = camera.GetComponent<CameraWhoosh>();

		shouldDismount = false;
	}

	private void Start()
	{
		ridingId = Animator.StringToHash("Riding");
	}

	private void OnEnable()
	{
		renderer.enabled = true;
		rigidbody.isKinematic = false;
		nokeAnimator.SetBool(ridingId, true);
	}

	private void OnDisable()
	{
		renderer.enabled = false;
		rigidbody.isKinematic = true;
		nokeAnimator.SetBool(ridingId, false);
	}

	private void Update()
	{
		if (shouldDismount)
			OnDismissBoard();
		shouldDismount = false;
	}

	private void FixedUpdate()
	{

	}

	[HideInInspector] private Animator nokeAnimator;
	[HideInInspector] private CameraWhoosh cameraWhoosh;

	[HideInInspector] new private Transform transform;
	[HideInInspector] new private Renderer renderer;
	[HideInInspector] private HoverboardSensors sensors;
	[HideInInspector] private HoverboardThrusters thrusters;
	[HideInInspector] private HoverboardParticles particles;

	[HideInInspector] private int ridingId;
	[HideInInspector] private bool shouldDismount;
}