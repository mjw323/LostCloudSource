using UnityEngine;
using System.Collections;

[AddComponentMenu("Cinematics/Sky Cinematic")]
public class SkyCinematic : Cinematic
{
    protected override IEnumerator PlayCinematic()
    {
        // Disable dynamic camera control so that we can direct it
        cameraFraming.enabled = false;
        cameraMovement.enabled = false;

        Quaternion startRot = cameraHolder.rotation;
        Quaternion endRot = Quaternion.LookRotation(Vector3.up, -cameraHolder.forward);
        
        yield return StartCoroutine(Look(startRot, endRot));
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(Look(endRot, startRot));
    }

    [SerializeField] private float lookTime;
    [SerializeField] private float waitTime;

    private Transform cameraHolder;
    private Framing cameraFraming;
    private DynamicCamera cameraMovement;

    private IEnumerator Look(Quaternion start, Quaternion end)
    {
        float elapsed = 0;
        while (elapsed < lookTime) {
            float progress = elapsed / lookTime;
            cameraHolder.rotation = Quaternion.Slerp(start, end, Mathf.SmoothStep(0, 1.0f, progress));
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void Awake()
    {
        Camera mainCamera = Camera.main;
        cameraHolder = mainCamera.transform.parent;
        cameraFraming = mainCamera.GetComponent<Framing>();
        cameraMovement = cameraHolder.GetComponent<DynamicCamera>();
    }
}