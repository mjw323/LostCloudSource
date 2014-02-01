
var HasPlayerGottenNextUpgrade : boolean;
var HasPlayerGottenUpgrade01 : boolean;
var	HasPlayerGottenUpgrade02 : boolean;
var	HasPlayerGottenUpgrade03 : boolean;
var	HasPlayerGottenUpgrade04 : boolean;
var Player : Transform;
var MainCamera : Camera;
var Enemy : GameObject;
var Sun : GameObject;
var Moon : GameObject;
var DaySky : Material;
var NightSky : Material;
var FadeWaitTime : int;



function Start(){
	HasPlayerGottenUpgrade01 = false;
	HasPlayerGottenUpgrade02 = false;
	HasPlayerGottenUpgrade03 = false;
	HasPlayerGottenUpgrade04 = false;
	HasPlayerGottenNextUpgrade = false;
	
}


function OnCollisionEnter(collision : Collision){
	
	if(collision.gameObject.name == "Upgrade01"){
		HasPlayerGottenUpgrade01 = true;
	}
	
	if(collision.gameObject.name == "Upgrade02"){
		HasPlayerGottenUpgrade02 = true;
	}
	
	if(collision.gameObject.name == "Upgrade03"){
		HasPlayerGottenUpgrade03 = true;
	}
	
	if(collision.gameObject.name == "Upgrade04"){
		HasPlayerGottenUpgrade04 = true;
	}
	
	
	if(collision.gameObject.tag == "Upgrade"){
		HasPlayerGottenNextUpgrade = true;
		Player.rigidbody.isKinematic = true;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.speed = 0;
		navAgent.enabled = false;
		MainCamera.SendMessage("fadeOut");
		StartCoroutine(NightTime());
		Destroy(collision.gameObject);
	}
	
	

}

function OnTriggerEnter (other : Collider) {
		if(other.gameObject.name == "StartSphere" && HasPlayerGottenNextUpgrade == true){
			Player.rigidbody.isKinematic = true;
			navAgent = Enemy.GetComponent(NavMeshAgent);
			navAgent.speed = 0;
			navAgent.enabled = false;
			MainCamera.SendMessage("fadeOut");
			StartCoroutine(DayTime());
		}
	}

function NightTime()
	{
		yield WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Sun.active = false;
		Moon.active = true;
		
		//Set Render Settings and Fog
		RenderSettings.fog = enabled;
		RenderSettings.fogColor = new Color(.051,.051,.098);
		RenderSettings.fogMode = FogMode.ExponentialSquared;
		RenderSettings.fogDensity = .0035;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		RenderSettings.ambientLight = new Color(.075,.075,.09);
		RenderSettings.skybox = NightSky;
		
		Player.rigidbody.isKinematic = false;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.enabled = true;
		navAgent.speed = 12;
}

function DayTime()
	{
		yield WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Sun.active = true;
		Moon.active = false;
		HasPlayerGottenNextUpgrade = false;
		
		
		//Set Render Settings and Fog
		RenderSettings.fog = enabled;
		RenderSettings.fogColor = new Color(1,.98,.706);
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogDensity = .01;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		RenderSettings.ambientLight = new Color(.2,.2,.2);
		RenderSettings.skybox = DaySky;
		
		Player.rigidbody.isKinematic = false;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.speed = 0;
		navAgent.enabled = false;
}