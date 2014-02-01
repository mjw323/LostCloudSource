
var HasPlayerGottenUpgrade : boolean;
var Player : Transform;
var MainCamera : Camera;
var Enemy : GameObject;
var Sun : GameObject;
var Moon : GameObject;
var FadeWaitTime : int;



function Start(){
	HasPlayerGottenUpgrade = false;
}


function OnCollisionEnter(collision : Collision){
	if(collision.gameObject.tag == "Upgrade"){
		HasPlayerGottenUpgrade = true;
		Player.rigidbody.isKinematic = true;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.speed = 0;
		navAgent.enabled = false;
		MainCamera.SendMessage("fadeOut");
		StartCoroutine(CollisionExit());
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
		}
	}

function CollisionExit()
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