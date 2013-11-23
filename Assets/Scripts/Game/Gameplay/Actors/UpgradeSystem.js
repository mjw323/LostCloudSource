
var HasPlayerGottenUpgrade : boolean;
var Player : Transform;
var MainCamera : Camera;
var Enemy : GameObject;
var FadeWaitTime : int;



function Start(){
	HasPlayerGottenUpgrade = false;
}


function OnCollisionEnter(collision : Collision){
	if(collision.gameObject.name == "Upgrade"){
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
		Player.rigidbody.isKinematic = false;
		navAgent = Enemy.GetComponent(NavMeshAgent);
		navAgent.enabled = true;
		navAgent.speed = 12;
}