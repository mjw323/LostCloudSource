using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Ragdoll))]
[RequireComponent(typeof(FootController))]
public class RagdollController : MonoBehaviour {
	public void Ragdoll() {
		footController.enabled = false;
		ragdoll.DoRagdoll();
	}

	public void GetUp() {
		ragdoll.DoGetUp();
	}

  [HideInInspector][SerializeField] private Ragdoll ragdoll;
  [HideInInspector][SerializeField] private FootController footController;

	private void OnFinishedGettingUp() {
		footController.enabled = true;
	}

	private void Awake() {
		ragdoll = GetComponent<Ragdoll>();
		footController = GetComponent<FootController>();
	}
}