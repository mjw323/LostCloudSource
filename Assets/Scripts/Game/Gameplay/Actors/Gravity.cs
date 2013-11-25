using UnityEngine;

/// <summary>
/// Applies gravity to the Player.
/// </summary>
[AddComponentMenu ("Player/Gravity")]
[RequireComponent (typeof(CharacterController))]
public class Gravity : MonoBehaviour
{
	void Awake()
	{
		characterController = GetComponent<CharacterController>();
	}
	
	void Update()
	{
		if (characterController.isGrounded)
			velocity.y = -gravity * Time.deltaTime;
		else
		{
			velocity.y -= gravity * Time.deltaTime;
			if (velocity.y < -maxFallSpeed)
				velocity.y = -maxFallSpeed;
		}
		
		characterController.Move(velocity * Time.deltaTime);
	}
	
	[SerializeField] private float gravity;
	[SerializeField] private float maxFallSpeed;
	
	// Internal references
	[HideInInspector] private CharacterController characterController;
	
	// Temporary state
	private Vector3 velocity;
}