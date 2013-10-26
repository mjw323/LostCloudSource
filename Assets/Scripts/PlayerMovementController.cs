// PlayerMovementController.cs
// Author: Zach Greer
using UnityEngine;
using System.Collections;

// Signals intent to an attached Movement component.
public class PlayerMovementController : MonoBehaviour {
	HoverMovement hoverMovement;

	void Start() {
		hoverMovement = GetComponent<HoverMovement>();
	}
	
	void Update() {
		hoverMovement.Move(
				Mathf.Sign(Input.GetAxis("Vertical"))*Mathf.Pow(Mathf.Abs(Input.GetAxis("Vertical")),1/2), 
				Input.GetAxis("Horizontal"),
				Input.GetButton("Jump"));
	}
}