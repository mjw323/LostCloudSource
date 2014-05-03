using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Assumptions:
// * Component is attached to Noke, and that she has a Ragdoll.
// * The root bone of Noke's skeleton has been assigned to 'root'.
public class Death : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private Respawn respawn;
    [SerializeField] private DynamicCamera dynamicCamera;
    [SerializeField] private Transform root;
    [HideInInspector] private Ragdoll ragdoll;

    // To be called when Noke takes a swim or tries to escape the island by jumping off.
    public void OnFall()
    {
        ragdoll.DoRagdoll();
        dynamicCamera.DisableFollow();
        dynamicCamera.PushAnchor(root);
        StartCoroutine(DoDeath());
    }

    // To be called when Noke is hit by Yorex or when she bumps her noggin.
    public void OnHit()
    {
        ragdoll.DoRagdoll();
        StartCoroutine(DoDeath());
    }

    private IEnumerator DoDeath()
    {
        yield return new WaitForSeconds(delay);
        respawn.BeginRespawn();
    }

    private void Awake()
    {
        ragdoll = GetComponent<Ragdoll>();
    }
}