using UnityEngine;

public class RespawnNode : MonoBehaviour {
    // To be called only by the Respawn component.
    public Respawn Master
    {
        set { master = value; }
    }

    [HideInInspector] private Respawn master;
    [HideInInspector] new private Transform transform;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        master.SpawnLocation = transform.position;
    }
}