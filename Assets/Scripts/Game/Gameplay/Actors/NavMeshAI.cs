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
		if (Player.GetComponent<UpgradeSystem>().HasPlayerGottenNextUpgrade){
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
