using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Interactive))]
public class Upgrade : MonoBehaviour
{
    public delegate void Collected(Upgrade which);
    public event Collected OnCollected;
    
    private Interactive interactive;

    private void OnInteracted()
    {
        // Interactive component will delete itself soon, so unsubscribe now
        interactive.OnInteracted -= OnInteracted;
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
}