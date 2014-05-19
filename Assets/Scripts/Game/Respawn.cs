using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    // Used by RespawnNode to update the player's respawn location.
    // Should also be used by GameController each night.
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

    // For use by GameController.
    public void OnSunrise()
    {
        isNight = false;
        dayNodeContainer.SetActive(true);
    }

    // For use by GameController.
    public void OnNightfall()
    {
        isNight = true;

        // Disable all of the daytime respawn nodes
        dayNodeContainer.SetActive(false);
    }

    [SerializeField] private DynamicCamera dynamicCamera;
    [SerializeField] private float fadeInSeconds;
    [SerializeField] private float fadeOutSeconds;
    [SerializeField] private float delay;
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAI monster;
    [SerializeField] private GameObject dayNodeContainer;
    [HideInInspector] private Fade fade;
    [HideInInspector] private Flash flash;
    [HideInInspector] private Ragdoll ragdoll;
    private Vector3 spawnLocation;
    private bool inProgress;
    private bool isNight;

    private IEnumerator DoRespawn()
    {
        yield return fade.FadeOut(fadeOutSeconds);
        player.position = spawnLocation;
        if (isNight) {
            monster.TeleportNearPoint(player.position);
        }
        ragdoll.GetUp();
        dynamicCamera.PopAnchor();
        dynamicCamera.TeleportBehind();
        dynamicCamera.EnableFollow();
        yield return new WaitForSeconds(delay);
        flash.Fire(1.0f);
        fade.FadeIn(0);
        inProgress = false;
    }

    private void Awake()
    {
        inProgress = false;
        fade = dynamicCamera.GetComponentInChildren<Fade>();
        flash = dynamicCamera.GetComponentInChildren<Flash>();
        ragdoll = player.GetComponent<Ragdoll>();
    }

    private void Start()
    {
        RespawnNode[] dayNodes = dayNodeContainer.GetComponentsInChildren<RespawnNode>();
        for (int i = 0; i < dayNodes.Length; ++i) {
            dayNodes[i].Master = this;
        }
    }
}