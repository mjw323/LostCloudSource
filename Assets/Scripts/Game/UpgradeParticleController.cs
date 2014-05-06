using UnityEngine;
using System.Collections;

public class UpgradeParticleController : MonoBehaviour
{
    private ParticleSystem[] particles;
    private Interactive interactive;

    private void OnInteracted()
    {
        for (int i = 0; i < particles.Length; ++i) {
            particles[i].enableEmission = false;
        }
    }
	
    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
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