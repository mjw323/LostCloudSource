//Monster Teleporting to move Monster to positions closer to the player if they're too far away.

var Player : Transform;
var Enemy : Transform;
var DistanceNeededToTeleport: int;
var DistanceNeededToReactivate: int;
var Active : boolean;

function Update () {
	if(Vector3.Magnitude(Player.position - this.transform.position) < DistanceNeededToTeleport && Active){
		Enemy.position = new Vector3(this.transform.position.x,this.transform.position.y,this.transform.position.z);
		Active = false;
	}
	
	if(Vector3.Magnitude(Player.position - this.transform.position) > DistanceNeededToReactivate ){
		Active = true;
	}
	


}