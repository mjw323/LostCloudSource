//Monster Teleporting to move Monster to positions closer to the player if they're too far away.

public var Player : Transform;
public var Enemy : GameObject;
var DistanceNeededToTeleport : int;
var DistanceNeededToReactivate : int;
var Active : boolean;

function Update () {
	if(Vector3.Magnitude(Player.position - this.transform.position) < DistanceNeededToTeleport && Active){
		
		var instance : GameObject = Instantiate(Enemy, Vector3(this.position.x,this.position.y,this.position.z), transform.rotation);
		
		Active = false;
	}
	
	if(Vector3.Magnitude(Player.position - this.transform.position) > DistanceNeededToReactivate ){
		Active = true;
	}
	


}