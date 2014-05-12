using UnityEngine;
using System.Collections;

public class SoundMachine : MonoBehaviour
{
    public delegate void Used();
    public event Used OnUsed;

    // Call this to re-arm the sound machine each night.
    // NOTE: Starts the game de-activated.
    public void Reactivate()
    {
        interactive.Reactivate();
    }

    // Call this when Yorex breaks the sound machine
    public void GetWrecked()
    {
        for (int i = 0; i < numCones; i++) {
            cones[i].Stop();
        }

        // Turn on our damage particles
        for (int j = 0; j < numParticles; j++) {
            particles[j].enableEmission = true;
        }
    }

    [SerializeField] private float duration;

    private Interactive interactive;
    private AudioCone[] cones;
    private int numCones;
    private ParticleSystem[] particles;
    private int numParticles;

    private void OnInteracted()
    {
        if (OnUsed != null) { OnUsed(); }
        for (int i = 0; i < numCones; i++) {
            cones[i].MakeWaves(duration);
        }
    }

    private void Awake()
    {
        interactive = GetComponent<Interactive>();
        cones = GetComponentsInChildren<AudioCone>();
        particles = GetComponentsInChildren<ParticleSystem>();
    }
	
    private void Start()
    {
        interactive.OnInteracted += OnInteracted;

        numCones = cones.Length;
        numParticles = particles.Length;

        // Hide our particles until we're broken
        for (int i = 0; i < numParticles; i++) {
            particles[i].enableEmission = false;
        }
	}

    private void OnApplicationQuit()
    {
        interactive.OnInteracted -= OnInteracted;
    }
}