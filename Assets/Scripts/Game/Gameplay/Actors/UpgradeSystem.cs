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
	public float NightWaitTime;
	//private NavMeshAgent navAgent;
	private float gettingUpgrade = 0f;
	
	private float respawnTimer = 5.0f; //how many second after death to respawn
	private float respawnTimeLeft = -1f;
	
	public GameObject boardGeo;
	public GameObject upgrade1;
	public GameObject upgrade2;
	public GameObject upgrade3;

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
	
	public int UpgradesFound(){
		//return how many upgrades player has
		int howMany = 0;
		if (Hoverboard.GetComponent<Hover>().canGlide){howMany += 1;}
		if (Hoverboard.GetComponent<Hover>().canGrind){howMany += 1;}
		if (Hoverboard.GetComponent<Hover>().canWater){howMany += 1;}
		
		return howMany;
	}
	
	void GotBigUpgrade(float index){
		gettingUpgrade = index;
	
		if(index == 3){
		//Start Final Cutscene
	}
		HasPlayerGottenNextUpgrade = true;
		MainCamera.SendMessage("fadeDayOut");
		StartCoroutine(NightTime());
		//Destroy(collision.gameObject);
		
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log ("collision with "+other.tag);
        if (other.tag=="Yorex" && other.transform.GetComponent<NavMeshAI>().state == 3){
			Debug.Log ("I'M DEEEEAAAAAAAAAADDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
			YorexStrike();
		}
    }
	
	public void YorexStrike(){
					respawnTimeLeft = 0f;
//			this.GetComponent<RagdollController>().Ragdoll();
			this.GetComponent<Hover>().DismissBoard();
	}
	
	void Update(){
		if (respawnTimeLeft >= 0f){
			respawnTimeLeft += Time.deltaTime;
			if (respawnTimeLeft >= respawnTimer){
				MainCamera.GetComponent<FadeInOut>().fadeOutIn(this.gameObject,"NightRespawn");
				respawnTimeLeft = -1f;
			}
		}
	}
	
	void NightRespawn(){
		this.transform.position = GameObject.FindWithTag("NightSpawn").transform.GetChild((int) gettingUpgrade).position;
		Enemy.GetComponent<NavMeshAI>().TeleportNearPoint(this.transform.position);
//		this.GetComponent<RagdollController>().GetUp();
		
		this.GetComponent<FootMovement>().enabled = true;
		this.GetComponent<FootController>().enabled = true;
		this.GetComponent<CharacterController>().enabled = true;
	}

	void ActivateSoundMachine(){
		if(HasPlayerGottenNextUpgrade){
			Debug.Log ("activated sound machine");
			
			Hoverboard.rigidbody.isKinematic = true;
			HasPlayerGottenNextUpgrade = false;
			
			if(gettingUpgrade == 0){
				Hoverboard.gameObject.GetComponent<Hover>().canGlide = true;
				GameObject clone1 = (GameObject)Instantiate(upgrade1, Vector3.zero, Quaternion.identity);
				clone1.transform.parent = boardGeo.transform;
				clone1.transform.localPosition = new Vector3(0,0,0);
				clone1.transform.localRotation = Quaternion.identity;
			}
			
			if(gettingUpgrade == 1){
				Hoverboard.gameObject.GetComponent<Hover>().canGrind = true;
				GameObject clone2 = (GameObject)Instantiate(upgrade2, Vector3.zero, Quaternion.identity);
				clone2.transform.parent = boardGeo.transform;
				clone2.transform.localPosition = new Vector3(0,0,0);
				clone2.transform.localRotation = Quaternion.identity;
			}
			
			if(gettingUpgrade == 2){
				Hoverboard.gameObject.GetComponent<Hover>().canWater = true;
				GameObject clone3 = (GameObject)Instantiate(upgrade3, Vector3.zero, Quaternion.identity);
				clone3.transform.parent = boardGeo.transform;
				clone3.transform.localPosition = new Vector3(0,0,0);
				clone3.transform.localRotation = Quaternion.identity;
			}
			//state = 0;
			MainCamera.SendMessage("fadeDayOut");
			StartCoroutine(DayTime());
		}
	}

	IEnumerator NightTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeDayIn");

	}

	IEnumerator DayTime(){
		yield return new WaitForSeconds(NightWaitTime);
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
		//navAgent.speed = 0;
		//navAgent.enabled = false;
	}
}
