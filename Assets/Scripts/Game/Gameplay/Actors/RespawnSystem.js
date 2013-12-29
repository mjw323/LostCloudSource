var SpawnPoint : Vector3;
var Player : Transform;
var MainCamera : Camera;
var FadeWaitTime : int;
var FootDist : float;  //distance below player to check for ground
var WaterUpgrade : boolean ;
var WaterTime : float; //frames you're allowed to spend on water
var WaterTimeMax : float; //frames you're allowed to spend on water
var Respawning : boolean;

function Start(){
	SpawnPoint = Player.position;
	WaterTime = 0;
	Respawning = false;
}

function Update(){
	if (!Respawning){
	var hit : RaycastHit;
	var touchingWater : boolean;
	touchingWater = false;
		if (Physics.Raycast (Player.position, -Vector3.up, hit)) {
		if (hit.distance<=FootDist){
			if ((hit.transform.tag!="Grind") && (hit.transform.tag!="Water") && (hit.transform.tag!="NoRespawn") && (hit.transform.tag!="Respawn")){
				SpawnPoint = Player.position;
				//Debug.Log(hit.transform.tag);
			}
			if ((hit.transform.tag=="Water") && (!WaterUpgrade)){
				touchingWater = true;
			}
			}
		}
	if (touchingWater){
		if (WaterTime<WaterTimeMax){WaterTime += 1;}
		else{WaterTime = 0; Respawn();}
	}
	else{
		if (WaterTime > 0){WaterTime -= 1;}
	}
	}
}

function OnTriggerEnter (other : Collider) {
	Debug.Log(other.transform);
		if(other.transform.tag == "Respawn"){
			Respawn();
		}
	}

function Respawn()
	{
		//Player.rigidbody.isKinematic = true;
		Debug.Log("respawning...");
		Respawning = true;
		MainCamera.SendMessage("fadeOut");
		yield WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Player.position = SpawnPoint;
		Respawning = false;
		//Player.rigidbody.isKinematic = false;
	}