//Controls Monster AI movement and leaping

var target : Transform;
var Leaping : boolean;
var leapDistance : float;


leapDistance = 25.0f;

function Update(){
	
	if (!Leaping && (Vector3.Magnitude(target.position - this.transform.position) < 50)){
		GetComponent(NavMeshAgent).destination = target.position;
	}
	
	if (Random.Range(1,500) == 1 || Leaping){
		if (!Leaping){
			Debug.Log("Leap!");
		}
	if (Vector3.Magnitude(target.position - this.transform.position) < 50){
		leap();
		}
	}
	

}

function leap(){
	var distance : float;
	distance = Vector3.Magnitude(target.position-this.transform.position);
	GetComponent(NavMeshAgent).destination = target.position + (target.rigidbody.velocity * (distance/GetComponent(NavMeshAgent).speed));
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