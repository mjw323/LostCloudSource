using UnityEngine;
using System.Collections.Generic;

public class Death : MonoBehaviour
{
    private Respawn respawn;

    private void Awake()
    {
        respawn = GetComponent<Respawn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water") {
            respawn.BeginRespawn();
        }
    }
}