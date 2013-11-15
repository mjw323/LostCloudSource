﻿//Monster Teleporting to move Monster to positions closer to the player if they're too far away.

public var Player : Transform;
public var Enemy : GameObject;
var DistanceNeededToTeleport : int;
var DistanceNeededToReactivate : int;
var Active : boolean;

function Update () {
	if(Player.GetComponent(UpgradeSystem).HasPlayerGottenUpgrade){
		if(Vector3.Magnitude(Player.position - this.transform.position) < DistanceNeededToTeleport && Active){
			navAgent = Enemy.GetComponent(NavMeshAgent);
			navAgent.enabled = false;
			Enemy.transform.position = this.transform.position;
			navAgent.enabled = true;
		
		
			Active = false;
		}
	}
	
	if(Vector3.Magnitude(Player.position - this.transform.position) > DistanceNeededToReactivate ){
		Active = true;
	}
	


}