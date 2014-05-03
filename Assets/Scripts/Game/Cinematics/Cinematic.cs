using UnityEngine;
using System.Collections;

public abstract class Cinematic : MonoBehaviour
{
    public Coroutine Play()
    {
        if (inProgress) { return StartCoroutine(Null()); }
        inProgress = true;
        return StartCoroutine(WrapCinematic());
    }

    public bool IsPlaying
    {
        get { return inProgress; }
    }

    // To be overridden by concrete cinematics.
    protected abstract IEnumerator PlayCinematic();

    [HideInInspector][SerializeField] private bool inProgress = false;

    // Wraps concrete cinematics and flips inProgress once the cinematic completes.
    private IEnumerator WrapCinematic()
    {
        yield return StartCoroutine(PlayCinematic());
        inProgress = false;
    }

    // Empty coroutine, returned if you attempt to play an in-progress cinematic.
    private IEnumerator Null()
    {
        yield return null;
    }
}