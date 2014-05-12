using UnityEngine;
using System.Collections;

[AddComponentMenu("Cinematics/Sound Machine Cinematic")]
public class SoundMachineCinematic : Cinematic
{
    protected override IEnumerator PlayCinematic()
    {
        // Schedule monster to flee
        StartCoroutine(FleeAfterSeconds(followTime * fleePercentage));
        
        // Disable dynamic camera control so that we can direct it
        cameraFraming.enabled = false;
        cameraMovement.enabled = false;

		Vector3 oldPos = cameraHolder.position;
		Quaternion oldRot = cameraHolder.rotation;
        
        // Move camera into place
        cameraHolder.position = soundMachineCameraSpot.position;
        cameraHolder.LookAt(soundMachineTransform);

        // Zoom in on / track the monster
        Quaternion startRot = cameraHolder.rotation;
        float elapsedTime = 0;
        while (elapsedTime < followTime) {
            float newZ = cameraHolder.position.z + (20.0f * Time.deltaTime);
            cameraHolder.position = new Vector3(cameraHolder.position.x, cameraHolder.position.y, newZ);
            
            float closeness = Mathf.SmoothStep(0, 1.0f, elapsedTime / followTime);
            Vector3 toMonster = monsterTransform.position - cameraHolder.position;
            Quaternion newRot = Quaternion.LookRotation(toMonster);
			cameraHolder.rotation = Quaternion.Slerp(startRot, newRot, closeness);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

		// Look up to the sky and transition to day
		Quaternion endRot = Quaternion.LookRotation(Vector3.up, -cameraHolder.forward);
		yield return StartCoroutine(Look(cameraHolder.rotation, endRot));
		yield return timeOfDay.GotoDay();

		// Return the camera to its original position
		cameraHolder.position = oldPos;
		yield return StartCoroutine(Look(endRot, oldRot));
		cameraFraming.enabled = true;
		cameraMovement.enabled = true;
    }

    [SerializeField] private float followTime;
	[SerializeField] private float lookTime;
    [SerializeField] private float fleePercentage; // [0, 1]

	[SerializeField] private TimeOfDay timeOfDay;

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
		monsterAI.gameObject.GetComponent<NavMeshAgent> ().enabled = false;
        monsterAI.DayFlee();

    }

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

	private IEnumerator Move(Vector3 start, Vector3 end)
	{
		float elapsed = 0;
		while (elapsed < lookTime) {
			float progress = elapsed / lookTime;
			cameraHolder.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1.0f, progress));
			elapsed += Time.deltaTime;
			yield return null;
		}
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