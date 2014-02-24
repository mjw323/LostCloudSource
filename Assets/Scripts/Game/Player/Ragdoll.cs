using UnityEngine;

public class Ragdoll : MonoBehaviour {
	public bool IsRagdolled {
		get { return isRagdolled; }
	}

	public void DoRagdoll() {
		isRagdolled = true;
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigidbodies.Length; i++) {
			rigidbodies[i].isKinematic = false;
		}
		GetComponent<Animator>().enabled = false;
	}

	private bool isRagdolled = false;
}