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
	private GameObject[] gos;
	private NavMeshAgent navAgent;
	private Vector3 JumpTarget;
	public float JumpCountdown = 6f;
	private float JumpCountdownCurrent = 0f;
	
	public float mySpeed = 12f;
	
	public float jumpAnim = 0.5f;
	private float jumpAnimCur;

	Vector3 downoffset = new Vector3(0,-2,0);
	Vector3 rightoffset = new Vector3(3,0,0);
	Vector3 upoffset = new Vector3(0,2,0);
	Vector3 leftoffset = new Vector3(-3,0,0);
	
	private bool seen = false;


	
	void Start(){
		leapDistance = 40.0f;
		gos = GameObject.FindGameObjectsWithTag("YorexNode");
		navAgent = this.GetComponent<NavMeshAgent>();
		
		jumpAnimCur = jumpAnim;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Eyes.transform.GetComponent<SearchCamera>().CanSee() != seen ){
				seen = !seen;
				Debug.Log ("Noke visibility is now "+seen);
		}

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
	
	public void StartAI(){
		
		navAgent.enabled = false;
		Vector3 targetPos = FindJumpNode (Player.transform.position);
		this.transform.position = targetPos;
		//Debug.Log ("I'm going to "+targetPos+", and ended up at "+this.transform.position);
		navAgent.enabled = true;
	}
	
	public void Jump(){
		
		navAgent.enabled = false;
		Vector3 targetPos = FindJumpNode (Player.transform.position);
		this.transform.position = targetPos;
		//Debug.Log ("I'm going to "+targetPos+", and ended up at "+this.transform.position);
		navAgent.enabled = true;
	}


	void hide(){
		//Monster is turned off and reset to a hidden location;

		GetComponent<NavMeshAgent>().speed = 0;
		GetComponent<NavMeshAgent>().enabled = false;
		this.transform.position = HidePosition.transform.position;
	}

	void idle(){
		//Stand still, rotate a random number of degrees every three seconds, raycast for player
		
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
		
		GetComponent<NavMeshAgent>().speed = mySpeed;
		JumpCountdownCurrent = JumpCountdown;
		look ();

		
		GetComponent<NavMeshAgent>().destination = Player.transform.position;
		
		
		if (navAgent.isOnOffMeshLink){
			if (jumpAnimCur>0f){
				jumpAnimCur -= Time.deltaTime; 
				navAgent.speed = 0f;
			}
			else{
				navAgent.speed = mySpeed;
			}
		}
		else{jumpAnimCur = jumpAnim;}
		
		RaycastHit hit;
		//Can't find her
		if (!Player.GetComponentInChildren<YorexSight>().Visible()){
			Debug.Log ("distance is "+Vector3.Magnitude (this.transform.position - Player.transform.position)+"/75, wall between us is "+Physics.Raycast (this.transform.position,Player.transform.position-this.transform.position, out hit, 75f,  ~ (1 << 11)));
			if(Vector3.Magnitude (this.transform.position - Player.transform.position) > 75f || 
				Physics.Raycast (this.transform.position,Player.transform.position-this.transform.position, out hit, 75f,  ~ (1 << 11))
				){
				Debug.Log ("changed states as a result");
				state = 4;
			}
			//else
		}
	}

	void alerted(){
		//Move to last known spot of the player, then stop rotate for 5 seconds, and then move to search
		look ();
		GetComponent<NavMeshAgent>().destination = lastKnownPlayerPosition;
		this.transform.Rotate (0,Time.deltaTime*30,0);
		
		if (!Player.GetComponentInChildren<YorexSight>().Visible()){
				JumpCountdownCurrent -= Time.deltaTime;
				//Debug.Log ("Jump Countdown Timer:" + JumpCountdownCurrent);
			}
		if (JumpCountdownCurrent < 0f){
			Jump ();	
		}
	}

	void curious(){
		//Move closer to last heard hoverboard spot slowly, then stop, rotate after 5 seconds, return to move and search
		look ();
	}

	void look(){
		if (Player.GetComponentInChildren<YorexSight>().Visible() 
			//&& Physics.Raycast (this.transform.position,Player.transform.position-this.transform.position, out hit, 75f,  ~ (1 << 11))
			){
			if (state!=3){Debug.Log ("saw player, started chasing");}
			state = 3;
			lastKnownPlayerPosition = Player.transform.position;
		}
	}

	IEnumerator Wait(){
		yield return new WaitForSeconds(WaitTime);
	}
	
	Vector3 FindJumpNode(Vector3 position){
		position.y = 0;
        GameObject closest = null;
        float distance = Mathf.Infinity;
        foreach (GameObject go in gos) {
            Vector3 diff = /*go.transform.TransformPoint(*/go.transform.position - /*this.transform.TransformPoint(*/position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                closest = go;
                distance = curDistance;
            }
        }
		
		//Debug.Log (closest.transform.position+", parent: "+closest.transform.parent.position);
		
		RaycastHit hit;
		Vector3 nodePoint;
		if (closest.transform.parent.tag != "YorexNode"){nodePoint = /*closest.transform.TransformPoint(*/closest.transform.position;}
		else{nodePoint = /*closest.transform.parent.TransformPoint(*/closest.transform.parent.position;}
		
		RaycastHit nodeHit;
		if (Physics.Raycast (new Vector3(nodePoint.x,300f,nodePoint.z),-Vector3.up,out nodeHit)){
		//Debug.Log ("point "+nodeHit.point+", distance "+nodeHit.distance+", collider "+nodeHit.collider);
		//Debug.Log ("returning "+nodeHit.point+", which was found from "+nodePoint);
        return nodeHit.point; 
		}
		else{
			//Debug.Log ("didn't find anything"); 
			return Vector3.zero;}
		
    }

}
