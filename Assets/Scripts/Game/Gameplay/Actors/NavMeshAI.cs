using UnityEngine;
using System.Collections;

public class NavMeshAI : MonoBehaviour {
	

	public Transform Player;
	public Vector3 lastKnownPlayerPosition;
	public GameObject HidePosition;
	public Camera Eyes;
	public bool Leaping;
	public float leapDistance;
	public int state = 0;


	Vector3 downoffset = new Vector3(0,-2,0);
	Vector3 rightoffset = new Vector3(3,0,0);
	Vector3 upoffset = new Vector3(0,2,0);
	Vector3 leftoffset = new Vector3(-3,0,0);

	void Start(){
		leapDistance = 40.0f;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (state == 0){
			hide();
		}
		else if (state == 1){
			idle();
		}
		else if (state == 2){
			search();
		}
		else if (state == 3){
			chase();
		}
		else if (state == 4){
			alerted();
		}
	}

	void hide(){
		//Monster is turned off and reset to a hidden location;

		GetComponent<NavMeshAgent>().speed = 0;
		GetComponent<NavMeshAgent>().enabled = false;
		this.transform.position = HidePosition.transform.position;
	}

	void idle(){
		//Teleport close to player, stand still, rotate a random number of degrees every three seconds, raycast for player

		look ();
	}

	void search(){
		//Move around player's area slowly, raycast for player
		GetComponent<NavMeshAgent>().speed = 10;
		look ();
	}

	void chase(){
		//Speed up and chase player in any direction until the enemy loses sight of the player
		//Leap towards the player occasionally

		look ();

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

	void alerted(){
		//Move to last known spot of the player, then stop rotate for 5 seconds, and then move to search
	}

	void look(){
		if (Player.GetComponentInChildren<TestRendered>().Visible == true){
			Debug.Log ("FOUND YOU!");
			state = 3;
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
			GetComponent<NavMeshAgent>().speed = 20;
		}
	
	}
}
