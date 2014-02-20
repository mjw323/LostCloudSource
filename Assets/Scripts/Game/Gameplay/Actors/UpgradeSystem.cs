﻿using UnityEngine;
using System.Collections;



public class UpgradeSystem : MonoBehaviour {

	public bool DebugUpgrades;
	public bool HasPlayerGottenNextUpgrade;
	public Transform Player;
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
	
	Player.gameObject.GetComponent<Hover>().canGlide = false;
	Player.gameObject.GetComponent<Hover>().canGrind = false;
	Player.gameObject.GetComponent<Hover>().canWater = false;
		
	if(DebugUpgrades == true){
		Player.gameObject.GetComponent<Hover>().canGlide = true;
		Player.gameObject.GetComponent<Hover>().canGrind = true;
		Player.gameObject.GetComponent<Hover>().canWater = true;
	}
	
	}
	
	void GotBigUpgrade(float index){
		gettingUpgrade = index;
	
		if(index == 3){
		//Start Final Cutscene
	}
	
	
		HasPlayerGottenNextUpgrade = true;
		Player.rigidbody.isKinematic = true;
		navAgent = Enemy.GetComponent<NavMeshAgent>();
		navAgent.speed = 0;
		navAgent.enabled = false;
		Enemy.GetComponent<NavMeshAI>().state = 3;
		MainCamera.SendMessage("fadeOut");
		StartCoroutine(NightTime());
		//Destroy(collision.gameObject);
		
	}

	void ActivateSoundMachine(){
		if(HasPlayerGottenNextUpgrade){
			Player.rigidbody.isKinematic = true;
			HasPlayerGottenNextUpgrade = false;
			Enemy.GetComponent<NavMeshAI>().state = 0;
			MainCamera.SendMessage("fadeOut");
			StartCoroutine(DayTime());

			if(gettingUpgrade == 0){
				Player.gameObject.GetComponent<Hover>().canGlide = true;
			}
			
			if(gettingUpgrade == 1){
				Player.gameObject.GetComponent<Hover>().canGrind = true;
			}
			
			if(gettingUpgrade == 2){
				Player.gameObject.GetComponent<Hover>().canWater = true;
			}
		}
	}

	IEnumerator NightTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
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
		
		Player.rigidbody.isKinematic = false;
		navAgent.enabled = true;
		navAgent.speed = 12;
	}

	IEnumerator DayTime(){
		yield return new WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
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
		
		Player.rigidbody.isKinematic = false;
		navAgent.speed = 0;
		navAgent.enabled = false;
	}
}
