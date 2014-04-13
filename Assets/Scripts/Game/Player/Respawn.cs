using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    [SerializeField] private float fadeInSeconds;
    [SerializeField] private float fadeOutSeconds;
    [SerializeField] private float locationUpdateIntervalSeconds;
    [SerializeField] private Transform player;
    private Vector3 location;
    private bool inProgress;

    private const float checkDistance = 10f;

    public void BeginRespawn()
    {
        if (!inProgress) {
            StartCoroutine(DoRespawn());
        }
    }

    private IEnumerator DoRespawn()
    {
        yield return new WaitForSeconds(fadeOutSeconds);
        player.position = location;
        yield return new WaitForSeconds(fadeInSeconds);
    }

    private void UpdateLocation()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, -Vector3.up, out hit, checkDistance)) {
            if (hit.transform.tag == "Terrain") {
                //respawnLocation = transform.position;
            }
        }
    }

    private IEnumerator DoUpdateLocation()
    {
        while (true) {
            yield return new WaitForSeconds(locationUpdateIntervalSeconds);
            UpdateLocation();
            yield return null;
        }
    }

    private void Awake()
    {
        inProgress = false;
        StartCoroutine(DoUpdateLocation());
    }
}