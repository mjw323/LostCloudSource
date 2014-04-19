using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Assumptions:
// * Component is attached to Noke, and that she has a RagdollController.
public class Death : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private Respawn respawn;
    [SerializeField] private DynamicCamera dynamicCamera;
    [HideInInspector] private RagdollController ragdollController;

    // To be called when Noke takes a swim or tries to escape the island by jumping off.
    public void OnFall()
    {
        ragdollController.Ragdoll();
        dynamicCamera.DisableFollow();
        StartCoroutine(DoDeath());
    }

    // To be called when Noke is hit by Yorex or when she bumps her noggin.
    public void OnHit()
    {
        ragdollController.Ragdoll();
        StartCoroutine(DoDeath());
    }

    private IEnumerator DoDeath()
    {
        yield return new WaitForSeconds(delay);
        respawn.BeginRespawn();
    }

    private void Awake()
    {
        ragdollController = GetComponent<RagdollController>();
    }
}