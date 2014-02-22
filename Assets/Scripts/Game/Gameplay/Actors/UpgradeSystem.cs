using UnityEngine;
using System.Collections;

public class UpgradeSystem : MonoBehaviour {

	public bool DebugUpgrades;
	public bool HasPlayerGottenNextUpgrade;
	public GameObject Hoverboard;
	public Camera MainCamera;
	public GameObject Enemy;
	//public GameObject Sun;
	//public GameObject Moon;
	//public Material DaySky;
	//public Material NightSky;
	public float FadeWaitTime;
	private NavMeshAgent navAgent;
	private float gettingUpgrade = 0f;


	// Use this for initialization
	void Start () {

	HasPlayerGottenNextUpgrade = false;
	
	Hoverboard.GetComponent<Hover>().canGlide = false;
	Hoverboard.GetComponent<Hover>().canGrind = false;
	Hoverboard.GetComponent<Hover>().canWater = false;
		
		if(DebugUpgrades == true){
			Hoverboard.GetComponent<Hover>().canGlide = true;
			Hoverboard.GetComponent<Hover>().canGrind = true;
			Hoverboard.GetComponent<Hover>().canWater = true;
		}
	
	}
	
	void GotBigUpgrade(float index){
		gettingUpgrade = index;
	
		if(index == 3){
		//Start Final Cutscene
	}
	
	
		HasPlayerGottenNextUpgrade = true;
		Hoverboard.rigidbody.isKinematic = true;
		navAgent = Enemy.GetComponent<NavMeshAgent>();
		navAgent.speed = 0;
		navAgent.enabled = false;
		Enemy.GetComponent<NavMeshAI>().state = 1;
		MainCamera.SendMessage("fadeDayOut");
		StartCoroutine(NightTime());
		//Destroy(collision.gameObject);
		
		if(gettingUpgrade == 0){
				Hoverboard.gameObject.GetComponent<Hover>().canGlide = true;
			}
			
			if(gettingUpgrade == 1){
				Hoverboard.gameObject.GetComponent<Hover>().canGrind = true;
			}
			
			if(gettingUpgrade == 2){
				Hoverboard.gameObject.GetComponent<Hover>().canWater = true;
			}
		
	}

	void ActivateSoundMachine(){
		if(HasPlayerGottenNextUpgrade){
			Hoverboard.rigidbody.isKinematic = true;
			HasPlayerGottenNextUpgrade = false;
			Enemy.GetComponent<NavMeshAI>().state = 0;
			MainCamera.SendMessage("fadeDayOut");
			StartCoroutine(DayTime());
		}
	}

	IEnumerator NightTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeDayIn");
		//Sun.active = false;
		//Moon.active = true;
		
		//Set Render Settings and Fog
		/*RenderSettings.fog = enabled;
		//RenderSettings.fogColor = new Color(.051f,.051f,.098f);
		//RenderSettings.fogMode = FogMode.ExponentialSquared;
		//RenderSettings.fogDensity = .0035f;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		//RenderSettings.ambientLight = new Color(.075f,.075f,.09f);
		//RenderSettings.skybox = NightSky;*/
		
		Hoverboard.rigidbody.isKinematic = false;
		navAgent.enabled = true;
		navAgent.speed = 12;
	}

	IEnumerator DayTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeDayIn");
		//Sun.active = true;
		//Moon.active = false;
		HasPlayerGottenNextUpgrade = false;
		
		
		//Set Render Settings and Fog
		/*RenderSettings.fog = enabled;
		//RenderSettings.fogColor = new Color(.46f,.709f,.949f);
		//RenderSettings.fogMode = FogMode.Linear;
		//RenderSettings.fogDensity = .01f;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		//RenderSettings.ambientLight = new Color(.2f,.2f,.2f);
		//RenderSettings.skybox = DaySky;*/
		
		Hoverboard.rigidbody.isKinematic = false;
		navAgent.speed = 0;
		navAgent.enabled = false;
	}
}
