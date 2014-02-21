using UnityEngine;
using System.Collections;

public class NavMeshAI : MonoBehaviour {
	

	public GameObject Player;
	public Vector3 lastKnownPlayerPosition;
	public GameObject HidePosition;
	public Camera Eyes;
	public bool Leaping;
	public float WaitTime;
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
		else if (state == 5){
			curious();
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
		GetComponent<NavMeshAgent>().speed = 6;
		Wait ();
		this.transform.Rotate (0,Time.deltaTime*10,0);

	}

	void search(){
		//Move around player's area slowly
		GetComponent<NavMeshAgent>().speed = 9;
		look ();

		//Move around code
	}

	void chase(){
		//Speed up and chase player in any direction until the enemy loses sight of the player
		//Leap towards the player occasionally
		
		GetComponent<NavMeshAgent>().speed = 12;
		look ();

		if (!Leaping && (Vector3.Magnitude(Player.transform.position - this.transform.position) < 100)){
			GetComponent<NavMeshAgent>().destination = Player.transform.position;
		}

		if (Random.Range(1,500) == 1 || Leaping){
			if (!Leaping){
				Debug.Log("Leap!");
			}
			if (Vector3.Magnitude(Player.transform.position - this.transform.position) < 100){
				leap();
			}
		}

		//Can't find her
		if (Player.GetComponentInChildren<TestRendered>().Visible == false){
			state = 4;
		
		}

	}

	void alerted(){
		//Move to last known spot of the player, then stop rotate for 5 seconds, and then move to search
		look ();
		GetComponent<NavMeshAgent>().destination = lastKnownPlayerPosition;
		this.transform.Rotate (0,Time.deltaTime*30,0);
	}

	void curious(){
		//Move closer to last heard hoverboard spot slowly, then stop, rotate after 5 seconds, return to move and search
		look ();
	}

	void look(){
		if (Player.GetComponentInChildren<TestRendered>().Visible == true){
			state = 3;
			lastKnownPlayerPosition = Player.transform.position;
		}
	}
	
	void leap(){

		float distance;
		distance = Vector3.Magnitude(Player.transform.position-this.transform.position);
		GetComponent<NavMeshAgent>().destination = Player.transform.position + (Player.rigidbody.velocity * (distance/GetComponent<NavMeshAgent>().speed));
		if (distance > leapDistance){
			Leaping = true;
			GetComponent<NavMeshAgent>().speed = 200;
		}
		else{
			Debug.Log("OK done!!");
			Leaping = false;
		}
	
	}

	IEnumerator Wait(){
		yield return new WaitForSeconds(WaitTime);
	}

}
