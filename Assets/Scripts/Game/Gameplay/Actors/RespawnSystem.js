var SpawnPoint : Vector3;
var Player : Transform;
var MainCamera : Camera;
var FadeWaitTime : int;
var FootDist : float;  //distance below player to check for ground



function Start(){
	SpawnPoint = Player.position;
}

function Update(){
	var hit : RaycastHit;
		if (Physics.Raycast (Player.position, -Vector3.up*FootDist, hit)) {
			if ((hit.transform.tag!="Grind") && (hit.transform.tag!="Water") && (hit.transform.tag!="NoRespawn")){
				SpawnPoint = Player.position;
			}
		}
}



function Respawn()
	{
		MainCamera.SendMessage("fadeOut");
		yield WaitForSeconds(FadeWaitTime);
		MainCamera.SendMessage("fadeIn");
		Player.position = SpawnPoint;
	}