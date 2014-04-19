using UnityEngine;
using System.Collections;

public class RespawnSystem : MonoBehaviour {
	public Vector3 spawnPoint;
	public Transform player;
	public Camera mainCamera;
	public int fadeWaitTime;
	public float footDist;		//distance below player to check for ground
	public bool waterUpgrade;
	public float waterTime; 	//frames you're allowed to spend on water
	public float waterTimeMax; 	//frames you're allowed to spend on water
	public bool isRespawning;
	
	private MonoBehaviour footC;
	private MonoBehaviour boardC;
	private MonoBehaviour ragdollC;
	
	// Use this for initialization
	void Start () {
		spawnPoint = player.position;
		waterTime = 0;
		isRespawning = false;
		
		footC = gameObject.GetComponent<FootController>();
		boardC = gameObject.GetComponent<BoardController>();
		ragdollC = gameObject.GetComponent<RagdollController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isRespawning){
			RaycastHit hit;
			bool touchingWater = false;
			Debug.DrawRay(transform.position, -Vector3.up, Color.magenta);
			if (Physics.Raycast (player.position, -Vector3.up, out hit, footDist/*, ~100000000*/)){				//if (hit.distance<=footDist){
				//print ("hit distance:" +hit.distance);
				/*bool isNoke = false;
				Transform par = hit.transform;
				while(par!=null){
						if (par.gameObject.tag == "Player"){isNoke = true; break;}
						par = par.parent;
					}
					if ((!isNoke) && (hit.transform.tag != "Grind") && (hit.transform.tag != "Water") && 
						(hit.transform.tag != "NoRespawn") && (hit.transform.tag != "Respawn"))	{*/
					if(hit.transform.tag == "Terrain"){
						spawnPoint = player.position;
					}
					
					if ((hit.transform.tag == "Water") && (!waterUpgrade)){
						touchingWater = true;
					}
				//}
			}
			if (touchingWater && footC.enabled == true){
				if (waterTime < waterTimeMax)
					waterTime += 1;
				else {
					waterTime = 0;
					Respawn();
				}
			}
			else if(waterTime > 0){
				waterTime -= 1;
			}
		}
	}
	
	IEnumerator Wait(){
		yield return new WaitForSeconds(fadeWaitTime);
	}
	
	void OnTriggerEnter (Collider other){
		//Debug.Log("hit respawner");
		//GetComponent <Animator>().renderer.enabled = false; //Is .renderer needed?
		if(other.transform.tag == "Respawn"){
			Respawn();
		}
	}
	
	public void Respawn(){
		//Debug.log ("respawning");
		isRespawning = true;
		mainCamera.SendMessage("flash");
		//StartCoroutine(Wait());
		//mainCamera.SendMessage("fadeIn");
		player.position = spawnPoint;
		isRespawning = false;
	}
}
