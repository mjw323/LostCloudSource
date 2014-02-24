using UnityEngine;

[RequireComponent(typeof(Ragdoll))]
[RequireComponent(typeof(FootController))]
public class RagdollController : MonoBehaviour {
	public void DoRagdoll() {
		footController.enabled = false;
		ragdoll.DoRagdoll();
	}

	private void Awake() {
		ragdoll = GetComponent<Ragdoll>();
		footController = GetComponent<FootController>();
	}

	private Ragdoll ragdoll;
	private FootController footController;
}