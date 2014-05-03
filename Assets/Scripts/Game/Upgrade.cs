using UnityEngine;
using System.Collections;

public class Upgrade : MonoBehaviour
{
    public delegate void Collected(Upgrade which);
    public event Collected OnCollected;
    
    private Interactive interactive;

    private void OnInteracted()
    {
        if (OnCollected != null) { OnCollected(this); }
    }

    private void Awake()
    {
        interactive = GetComponent<Interactive>();
    }

    private void Start()
    {
        interactive.OnInteracted += OnInteracted;
    }

    private void OnApplicationQuit()
    {
        interactive.OnInteracted -= OnInteracted;
    }
}