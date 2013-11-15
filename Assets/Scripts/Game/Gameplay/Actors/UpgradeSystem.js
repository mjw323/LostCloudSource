
var HasPlayerGottenUpgrade : boolean;
var Player : Transform;

function Start(){
	HasPlayerGottenUpgrade = false;
	
}


function OnCollisionEnter(collision : Collision){
	if(collision.gameObject.name == "Upgrade"){
		HasPlayerGottenUpgrade = true;
		Destroy(collision.gameObject);
	}

}