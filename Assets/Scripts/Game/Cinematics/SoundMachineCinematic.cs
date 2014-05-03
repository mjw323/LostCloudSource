using UnityEngine;
using System.Collections;

[AddComponentMenu("Cinematics/Sound Machine Cinematic")]
public class SoundMachineCinematic : Cinematic
{
    protected override IEnumerator PlayCinematic()
    {
        // Schedule monster to flee
        StartCoroutine(FleeAfterSeconds(duration * fleePercentage));
        
        // Disable dynamic camera control so that we can direct it
        cameraFraming.enabled = false;
        cameraMovement.enabled = false;
        
        // Move camera into place
        cameraHolder.position = soundMachineCameraSpot.position;
        cameraHolder.LookAt(soundMachineTransform);
        
        // Zoom in on / track the monster
        Quaternion startRot = cameraHolder.rotation;
        float elapsedTime = 0;
        while (elapsedTime < duration) {
            float newZ = cameraHolder.position.z + (20.0f * Time.deltaTime);
            cameraHolder.position = new Vector3(cameraHolder.position.x, cameraHolder.position.y, newZ);
            
            float closeness = Mathf.SmoothStep(0, 1.0f, elapsedTime / duration);
            Vector3 toMonster = monsterTransform.position - cameraHolder.position;
            Quaternion newRot = Quaternion.LookRotation(toMonster);
            cameraHolder.rotation = Quaternion.Slerp(startRot, newRot, closeness);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    [SerializeField] private float duration;
    [SerializeField] private float fleePercentage; // [0, 1]

    [SerializeField] private GameObject soundMachine;
    private Transform soundMachineTransform;
    private Transform soundMachineCameraSpot;

    private Transform cameraHolder;
    private Framing cameraFraming;
    private DynamicCamera cameraMovement;

    [SerializeField] private GameObject monster;
    private Transform monsterTransform;
    private NavMeshAI monsterAI;

    private IEnumerator FleeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        monsterAI.DayFlee();
    }

    private void Awake()
    {
        soundMachineTransform = soundMachine.transform;
        soundMachineCameraSpot = soundMachineTransform.Find("__speakerCam");
        
        Camera mainCamera = Camera.main;
        cameraHolder = mainCamera.transform.parent;
        cameraFraming = mainCamera.GetComponent<Framing>();
        cameraMovement = cameraHolder.GetComponent<DynamicCamera>();

        monsterTransform = monster.GetComponent<Transform>();
        monsterAI = monster.GetComponent<NavMeshAI>();
    }
}