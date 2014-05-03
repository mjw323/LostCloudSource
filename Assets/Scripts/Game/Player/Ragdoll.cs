using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FootController))]
public class Ragdoll : MonoBehaviour {
	public bool IsRagdolled {
		get { return isRagdolled; }
	}

	public void DoRagdoll() {
        if (isRagdolled) { return; }
        footController.enabled = false;
		isRagdolled = true;
		SetKinematic(false);
		animator.enabled = false;
	}

    public void GetUp() {
        if (!isRagdolled) { return; }
        footController.enabled = true;
        isRagdolled = false;
        SetKinematic(true);
        // Synchronize the positions of Noke's capsule and her skeleton.
        root.localPosition = Vector3.zero;
        animator.enabled = true;
	}

    [SerializeField] private Transform root;
    private Animator animator;
    private FootController footController;
    private bool isRagdolled;

	private void SetKinematic(bool value) {
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigidbodies.Length; i++) {
			rigidbodies[i].isKinematic = value;
		}
	}

	private void Awake() {
		animator = GetComponent<Animator>();
        footController = GetComponent<FootController>();
        isRagdolled = false;
	}
}