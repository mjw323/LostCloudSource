using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour {
	public bool IsRagdolled {
		get { return isRagdolled; }
	}

	public void DoRagdoll() {
		isRagdolled = true;
		SetKinematic(false);
		animator.enabled = false;
	}

    public void DoGetUp() {
        if (!isRagdolled) return;
        SetKinematic(true);
        // Synchronize the positions of Noke's capsule and her skeleton.
        root.localPosition = Vector3.zero;
        animator.enabled = true;
	}

    [SerializeField] private Transform root;
    [HideInInspector][SerializeField] private Animator animator;
    [HideInInspector][SerializeField] private bool isRagdolled;

	private void SetKinematic(bool value) {
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigidbodies.Length; i++) {
			rigidbodies[i].isKinematic = value;
		}
	}

	private void Awake() {
		animator = GetComponent<Animator>();
        isRagdolled = false;
	}
}