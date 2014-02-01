
var HasPlayerGottenUpgrade : boolean;
var Player : Transform;
var MainCamera : Camera;
var Enemy : GameObject;
var Sun : GameObject;
var Moon : GameObject;
var FadeWaitTime : int;



function Start(){
	HasPlayerGottenUpgrade01 = false;
	HasPlayerGottenUpgrade02 = false;
	HasPlayerGottenUpgrade03 = false;
	HasPlayerGottenUpgrade04 = false;
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
		if(other.gameObject.name == "StartSphere" && HasPlayerGottenUpgrade == true){
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
		MainCamera.Skybox.material = "Night1 Skybox";
		
		Player.rigidbody.isKinematic = false;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.enabled = true;
		navAgent.speed = 12;
}

function DayTime()
	{
		yield WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Sun.active = false;
		Moon.active = true;
		
		//Set Render Settings and Fog
		RenderSettings.fog = enabled;
		RenderSettings.fogColor = new Color(1,.98,.706);
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogDensity = .01;
		RenderSettings.fogStartDistance = 0;
		RenderSettings.fogEndDistance = 1200;
		RenderSettings.ambientLight = new Color(.2,.2,.2);
		MainCamera.Skybox.material = "Clouds3 Skybox";
		
		Player.rigidbody.isKinematic = false;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.speed = 0;
		navAgent.enabled = false;
}