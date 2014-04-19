using UnityEngine;

// Assumptions:
// * Parent object has a Respawn component.
public class RespawnNode : MonoBehaviour {
    [HideInInspector] private Respawn master;
    [HideInInspector] new private Transform transform;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        master = transform.parent.GetComponent<Respawn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        master.SpawnLocation = transform.position;
    }
}