﻿using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Fade fade;
    [SerializeField] private DynamicCamera dynamicCamera;
    [SerializeField] private float fadeInSeconds;
    [SerializeField] private float fadeOutSeconds;
    [SerializeField] private float delay;
    [SerializeField] private Transform player;
    private Vector3 spawnLocation;
    private bool inProgress;

    public Vector3 SpawnLocation
    {
        set { spawnLocation = value; }
    }

    public void BeginRespawn()
    {
        if (!inProgress) {
            inProgress = true;
            StartCoroutine(DoRespawn());
        }
    }

    private IEnumerator DoRespawn()
    {
        yield return fade.FadeOut(fadeOutSeconds);
        player.position = spawnLocation;
        dynamicCamera.TeleportBehind();
        dynamicCamera.EnableFollow();
        yield return new WaitForSeconds(delay);
        yield return fade.FadeIn(fadeInSeconds);
        inProgress = false;
    }

    private void Awake()
    {
        inProgress = false;
    }
}