using UnityEngine;

public class RagdollController : MonoBehaviour {
	public void DoRagdoll() {
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigidbodies.Length; i++) {
			rigidbodies[i].isKinematic = false;
		}
		GetComponent<Animator>().enabled = false;
	}
}