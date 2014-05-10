using UnityEngine;
using System.Collections;

[AddComponentMenu("Cinematics/Upgrade Cinematic")]
public class UpgradeCinematic : Cinematic
{
	protected override IEnumerator PlayCinematic()
	{
		// Disable dynamic camera control so that we can direct it
		cameraFraming.enabled = false;
		cameraMovement.enabled = false;
		
		Quaternion startRot = cameraHolder.rotation;
		Quaternion endRot = Quaternion.LookRotation(Vector3.up, -cameraHolder.forward);
		
		yield return StartCoroutine(Look(startRot, endRot));
		yield return timeOfDay.GotoNight();

		// Trigger Yorex spawn and camera follow
		monster.StartAI (); 
		cameraMovement.enabled = true;
		cameraMovement.followEnemy = true;
		cameraFraming.enabled = true;

		// Return camera to default state
		cameraFraming.enabled = true;
		cameraMovement.enabled = true;

		yield return StartCoroutine(WaitForMonsterToLand());
		cameraMovement.followEnemy = false;
	}
	
	[SerializeField] private float lookTime;
	[SerializeField] private NavMeshAI monster;
	[SerializeField] private TimeOfDay timeOfDay;
	
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

	private IEnumerator WaitForMonsterToLand()
	{
		while (true) {
			if (monster.state != 6) { break; }
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