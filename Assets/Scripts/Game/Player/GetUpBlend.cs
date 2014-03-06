using UnityEngine;
using System;

public class GetUpBlend : MonoBehaviour {
  [Serializable]
  class BodyPart {
    public Transform transform;
    public Vector3 initPos;
    public Quaternion initRot;
  }

  [SerializeField] private float ragdollToMecanim = 0.5f;
  [SerializeField] private float mecanimToGetUp = 0.05f;
  [HideInInspector][SerializeField] private Transform root;
  [HideInInspector][SerializeField] private Transform head;
  [HideInInspector][SerializeField] private int notPlayerMask = ~(1 << 8);
  [HideInInspector][SerializeField] private Animator animator;
  [HideInInspector][SerializeField] private float beginTime;
  [HideInInspector][SerializeField] private BodyPart[] bodyParts;
  [HideInInspector][SerializeField] private Vector3 initFeetPos;
  [HideInInspector][SerializeField] private Vector3 initHeadPos;
  [HideInInspector][SerializeField] private Vector3 initHipPos;

  private void Awake() {
    animator = GetComponent<Animator>();
    root = animator.GetBoneTransform(HumanBodyBones.Hips);
    head = animator.GetBoneTransform(HumanBodyBones.Head);
    Transform[] transforms = GetComponentsInChildren<Transform>();
    bodyParts = new BodyPart[transforms.Length];
    for (int i = 0; i < transforms.Length; i++) {
      bodyParts[i] = new BodyPart();
      bodyParts[i].transform = transforms[i];
    }
  }

  private Vector3 Midpoint(Vector3 a, Vector3 b) {
    return (b - a) * 0.5f;
  }

  private Vector3 HeadPosition() {
    return animator.GetBoneTransform(HumanBodyBones.Head).position;
  }

  private Vector3 HipPosition() {
    return animator.GetBoneTransform(HumanBodyBones.Hips).position;
  }

  private Vector3 FeetMidpoint() {
    Vector3 leftFootPos = animator.GetBoneTransform(HumanBodyBones.LeftFoot)
      .position;
    Vector3 rightFootPos = animator.GetBoneTransform(HumanBodyBones.RightFoot)
      .position;
    return Midpoint(leftFootPos, rightFootPos);
  }

  // Script should begin disabled and be enabled by Ragdoll when getting up.
  private void OnEnable() {
    beginTime = Time.time;

    for (int i = 0; i < bodyParts.Length; i++) {
      bodyParts[i].initPos = bodyParts[i].transform.position;
      bodyParts[i].initRot = bodyParts[i].transform.rotation;
    }
    initFeetPos = FeetMidpoint();
    initHeadPos = HeadPosition();
    initHipPos = HipPosition();
  }

  private Vector3 NewRootPosition() {
    Vector3 hipPos = animator.GetBoneTransform(HumanBodyBones.Hips).position;
    Vector3 hipCurrToInit = initHipPos - hipPos;
    Vector3 newRootPos = root.position + hipCurrToInit;
    RaycastHit hit;
    Ray hipDown = new Ray(newRootPos, Vector3.down);
    if (Physics.Raycast(hipDown, out hit, Mathf.Infinity, notPlayerMask)) {
      newRootPos.y = Mathf.Max(newRootPos.y, hit.point.y);
    } else {
      newRootPos.y = 0; // What does this mean?
    }
    return newRootPos;
  }

  private Quaternion NewRootRotation() {
    Vector3 dollDir = initHeadPos - initFeetPos;
    Vector3 animDir = HeadPosition() - FeetMidpoint();

    // Rotate only around the Y axis
    dollDir.y = 0;
    animDir.y = 0;

    Quaternion adjustRot = Quaternion.FromToRotation(animDir.normalized,
                                                     dollDir.normalized);
    return root.rotation * adjustRot;
  }

  private void LateUpdate() {
    if (Time.time < beginTime + mecanimToGetUp) {
      root.position = NewRootPosition();
      root.rotation = NewRootRotation();
    }
    float progress = (Time.time - beginTime - mecanimToGetUp)/ragdollToMecanim;
    float blendAmount = Mathf.Lerp(1.0f, 0.0f, progress);
    for (int i = 0; i < bodyParts.Length; i++) {
      if (bodyParts[i].transform == root) {
        root.position = Vector3.Lerp(root.position, bodyParts[i].initPos,
                                     blendAmount);
      }
      // Avoid face-mangling transformations
      if (!bodyParts[i].transform.IsChildOf(head)) {
        bodyParts[i].transform.rotation = Quaternion.Slerp(
          bodyParts[i].transform.rotation, bodyParts[i].initRot, blendAmount);
      }
    }
    if (blendAmount == 0.0f) {
      enabled = false;
    }
  }
}