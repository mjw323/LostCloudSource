using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour {
	public bool IsRagdolled {
		get { return isRagdolled; }
	}

	public bool IsGettingUp {
		get { return isGettingUp; }
	}

	public void DoRagdoll() {
		isRagdolled = true;
		SetKinematic(false);
		animator.enabled = false;
	}

	public void DoGetUp() {
    if (!isRagdolled || isGettingUp) return;

		isGettingUp = true;
		SetKinematic(true);
		animator.enabled = true;
		if (animator.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0) {
			animator.SetBool(getUpFromBackAnimId, true);
		} else {
			animator.SetBool(getUpFromBellyAnimId, true);
		}
		StartCoroutine(DisableAnimationLooping());
    getUpBlend.enabled = true;
	}

  [HideInInspector][SerializeField] private Animator animator;
  [HideInInspector][SerializeField] private GetUpBlend getUpBlend;
  [HideInInspector][SerializeField] private bool isRagdolled = false;
  [HideInInspector][SerializeField] private bool isGettingUp = false;
  [HideInInspector][SerializeField]
  private int getUpFromBellyAnimId = Animator.StringToHash("GetUpFromBelly");
  [HideInInspector][SerializeField]
  private int getUpFromBackAnimId = Animator.StringToHash("GetUpFromBack");
	
	private IEnumerator DisableAnimationLooping() {
		// Give the Animator enough time to transition:
		yield return new WaitForSeconds(0.1f);
		animator.SetBool(getUpFromBackAnimId, false);
		animator.SetBool(getUpFromBellyAnimId, false);
	}

	private void OnFinishedGettingUp() {
		isRagdolled = false;
		isGettingUp = false;
	}

	private void SetKinematic(bool value) {
		Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < rigidbodies.Length; i++) {
			rigidbodies[i].isKinematic = value;
		}
	}

	private void Awake() {
		animator = GetComponent<Animator>();
    getUpBlend = GetComponent<GetUpBlend>();
	}
}