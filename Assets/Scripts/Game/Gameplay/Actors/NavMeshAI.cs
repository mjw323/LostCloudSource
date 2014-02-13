using UnityEngine;
using System.Collections;

public class NavMeshAI : MonoBehaviour {
	

	public Transform Player;
	public bool Leaping;
	public float leapDistance;

	void Start(){
		leapDistance = 40.0f;
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 forward = transform.TransformDirection(Vector3.forward) ;
		Vector3 downoffset = new Vector3(0,-2,0);
		Vector3 rightoffset = new Vector3(3,0,0);
		Vector3 upoffset = new Vector3(0,2,0);
		Vector3 leftoffset = new Vector3(-3,0,0);
		
		RaycastHit hit;
		if (Player.GetComponent<UpgradeSystem>().HasPlayerGottenNextUpgrade){
			if(Physics.SphereCast(gameObject.transform.position, 3, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
				transform.LookAt(hit.transform);
				Debug.DrawRay(transform.position,transform.forward*10,Color.green);
		
			if(Physics.SphereCast(gameObject.transform.position + upoffset, 3, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
				transform.LookAt(hit.transform);
				Debug.DrawRay(transform.position + upoffset,transform.forward*10,Color.green);
		
			if(Physics.SphereCast(gameObject.transform.position + downoffset, 3, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
				transform.LookAt(hit.transform);
				Debug.DrawRay(transform.position + downoffset,transform.forward*10,Color.green);
		
			if(Physics.SphereCast(gameObject.transform.position + leftoffset, 3, transform.forward + leftoffset, out hit, 20000) && hit.transform.gameObject.tag == "Player")
				transform.LookAt(hit.transform);
				Debug.DrawRay(gameObject.transform.position + leftoffset,transform.forward*10,Color.green);
		
			if(Physics.SphereCast(gameObject.transform.position + rightoffset, 3, transform.forward + rightoffset, out hit, 20000) && hit.transform.gameObject.tag == "Player")
				transform.LookAt(hit.transform);
				Debug.DrawRay(gameObject.transform.position + rightoffset,transform.forward*10,Color.green);


			if (!Leaping && (Vector3.Magnitude(Player.position - this.transform.position) < 100)){
				GetComponent<NavMeshAgent>().destination = Player.position;
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
	
	void leap(){

		float distance;
		distance = Vector3.Magnitude(Player.position-this.transform.position);
		GetComponent<NavMeshAgent>().destination = Player.position + (Player.rigidbody.velocity * (distance/GetComponent<NavMeshAgent>().speed));
		if (distance > leapDistance){
			Leaping = true;
			GetComponent<NavMeshAgent>().speed = 200;
		}
		else{
			Debug.Log("OK done!!");
			Leaping = false;
			GetComponent<NavMeshAgent>().speed = 12;
		}
	
	}
}
