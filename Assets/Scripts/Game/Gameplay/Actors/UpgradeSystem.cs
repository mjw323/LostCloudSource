using UnityEngine;
using System.Collections;



public class UpgradeSystem : MonoBehaviour {

	public bool DebugUpgrades;
	public bool HasPlayerGottenNextUpgrade;
	public Transform Player;
	public Camera MainCamera;
	public GameObject Enemy;
	public GameObject Sun;
	public GameObject Moon;
	public Material DaySky;
	public Material NightSky;
	public int FadeWaitTime;
	private NavMeshAgent navAgent;


	// Use this for initialization
	void Start () {

	HasPlayerGottenNextUpgrade = false;
	
	Player.gameObject.GetComponent<Hover>().canGlide = false;
	Player.gameObject.GetComponent<Hover>().canGrind = false;
	Player.gameObject.GetComponent<Hover>().canWater = false;
		
	if(DebugUpgrades == true){
		Player.gameObject.GetComponent<Hover>().canGlide = true;
		Player.gameObject.GetComponent<Hover>().canGrind = true;
		Player.gameObject.GetComponent<Hover>().canWater = true;
	}
	
	}
	
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.name == "Upgrade01"){
		Player.gameObject.GetComponent<Hover>().canGlide = true;
	}
	 
	if(collision.gameObject.name == "Upgrade02"){
		Player.gameObject.GetComponent<Hover>().canGrind = true;
	}
	
	if(collision.gameObject.name == "Upgrade03"){
		Player.gameObject.GetComponent<Hover>().canWater = true;
	}
	
	if(collision.gameObject.name == "Upgrade04"){
		//Start Final Cutscene
	}
	
	
	if(collision.gameObject.tag == "Upgrade"){
		HasPlayerGottenNextUpgrade = true;
		Player.rigidbody.isKinematic = true;
		navAgent = Enemy.GetComponent<NavMeshAgent>();
		navAgent.speed = 0;
		navAgent.enabled = false;
		MainCamera.SendMessage("fadeOut");
		StartCoroutine(NightTime());
		Destroy(collision.gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.name == "StartSphere" && HasPlayerGottenNextUpgrade == true){
			Player.rigidbody.isKinematic = true;
			navAgent = Enemy.GetComponent<NavMeshAgent>();
			navAgent.speed = 0;
			navAgent.enabled = false;
			MainCamera.SendMessage("fadeOut");
			StartCoroutine(DayTime());
		}
	}

	IEnumerator NightTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Sun.active = false;
		Moon.active = true;
		
		//Set Render Settings and Fog
		RenderSettings.fog = enabled;
		RenderSettings.fogColor = new Color(.051f,.051f,.098f);
		RenderSettings.fogMode = FogMode.ExponentialSquared;
		RenderSettings.fogDensity = .0035f;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		RenderSettings.ambientLight = new Color(.075f,.075f,.09f);
		RenderSettings.skybox = NightSky;
		
		Player.rigidbody.isKinematic = false;
		navAgent.enabled = true;
		navAgent.speed = 12;
	}

	IEnumerator DayTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Sun.active = true;
		Moon.active = false;
		HasPlayerGottenNextUpgrade = false;
		
		
		//Set Render Settings and Fog
		RenderSettings.fog = enabled;
		RenderSettings.fogColor = new Color(.46f,.709f,.949f);
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogDensity = .01f;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		RenderSettings.ambientLight = new Color(.2f,.2f,.2f);
		RenderSettings.skybox = DaySky;
		
		Player.rigidbody.isKinematic = false;
		navAgent.speed = 0;
		navAgent.enabled = false;
	}
}
