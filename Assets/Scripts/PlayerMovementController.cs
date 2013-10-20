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
		hoverMovement.Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
	}
}