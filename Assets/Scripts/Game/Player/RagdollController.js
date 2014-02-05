#pragma strict
//script from john gonzalez
//xenosmash.com

 
var boneRig : Component[];//used to store the rigidbody bones of our ragdoll
 
function Start () 
{
 
boneRig = gameObject.GetComponentsInChildren (Rigidbody); //grabs the rigidbodies of our bones to start
 
}
 
function killRagdoll () 
{
 
	for (var ragdoll : Rigidbody in boneRig) 
	{
	 	ragdoll.isKinematic = false; //disables the animated ragdoll and turns it into just a regular ragdoll with no movement
	}
	GetComponent(Animator).enabled=false; //disables the mecanim animator so our character no longer has any animations playing
}