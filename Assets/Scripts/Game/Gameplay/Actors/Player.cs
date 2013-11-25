using UnityEngine;

/// <summary>
/// State machine that controls Noke's movement.
/// </summary>
[AddComponentMenu ("Scripts/Character/Player")]
[RequireComponent (typeof(FootMovement))]
public class Player : MonoBehaviour
{
	/// <summary>
	/// Signal intent to move towards a direction.
	/// </summary>
	/// <param name="direction">
	/// World-space direction to move the player towards.
	/// </param>
	public void MoveTowards(Vector3 direction)
	{
		footMovement.MoveTowards(direction);
	}

	/// <summary>
	/// Signal intent to jump.
	/// </summary>
	public void Jump()
	{
		footMovement.Jump();
	}

	void Awake()
	{
		footMovement = GetComponent<FootMovement>();
	}

	[HideInInspector] private FootMovement footMovement;
}