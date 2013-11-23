//Controls Monster AI movement and leaping

var Player : Transform;
var Leaping : boolean;
var leapDistance : float;


leapDistance = 40.0f;

function Update(){
	
	if (Player.GetComponent(UpgradeSystem).HasPlayerGottenUpgrade){
		if (!Leaping && (Vector3.Magnitude(Player.position - this.transform.position) < 100)){
			GetComponent(NavMeshAgent).destination = Player.position;
		}
	
		if (Random.Range(1,500) == 1 || Leaping){
			if (!Leaping){
				Debug.Log("Leap!");
			}
			if (Vector3.Magnitude(Player.position - this.transform.position) < 100){
				leap();
			}
		}
	}
}

function leap(){
	var distance : float;
	distance = Vector3.Magnitude(Player.position-this.transform.position);
	GetComponent(NavMeshAgent).destination = Player.position + (Player.rigidbody.velocity * (distance/GetComponent(NavMeshAgent).speed));
	if (distance > leapDistance){
		Leaping = true;
		GetComponent(NavMeshAgent).speed = 200;
	}
	else{
		Debug.Log("OK done!!");
		Leaping = false;
		GetComponent(NavMeshAgent).speed = 12;
	}
}