using UnityEngine;
using System.Collections;

public class NavMeshAI : MonoBehaviour {
	

	public GameObject Player;
	public Vector3 lastKnownPlayerPosition;
	public Vector3 RandomMoveLocation;
	public GameObject HidePosition;
	public bool Flying;
	public bool Wandering;
	public float WaitTime;
	public float leapDistance;
	public int state = 0;
	private GameObject[] gos;
	private GameObject[] RandomMove;
	private NavMeshAgent navAgent;
    private HeatVisionCamera Eyes;
	private Vector3 JumpTarget;
	public float JumpCountdown = 6f;
	private float JumpCountdownCurrent = 0f;
	public float WanderCountdown = 12f;
	private float WanderCountdownCurrent = 0f;
	public float risingSpeed = 35f;
	public float flyingSpeed = 30f;
	public float landingSpeed = 40f;
	public float glideHeight = 120f;
	public float landAnimDist = 0f; //distance he moves while playing his landing animation
	
	public float mySpeed = 12f;
	private Vector3 NewRandomNode;
	
	public float jumpAnim = 0.5f;
	private float jumpAnimCur;

	Vector3 downoffset = new Vector3(0,-2,0);
	Vector3 rightoffset = new Vector3(3,0,0);
	Vector3 upoffset = new Vector3(0,2,0);
	Vector3 leftoffset = new Vector3(-3,0,0);
	
	private bool seen = false;
	
	private Framing shakeCam;
	
	public Vector3 fleePos = new Vector3(120f,220f,1220f);
	public bool rise = false; //used for camera follow
	private float riseTime = 1f;
	private float riseTimer = 1f;


	void Start(){
		leapDistance = 40.0f;
		gos = GameObject.FindGameObjectsWithTag("YorexNode");
		RandomMove = GameObject.FindGameObjectsWithTag ("RandomMove");
		navAgent = this.GetComponent<NavMeshAgent>();
		Eyes = this.GetComponentInChildren<HeatVisionCamera> ();

		animator = GetComponentInChildren<Animator>();
		animator.applyRootMotion = false; //we turn off root motion because of some of the funky stuff fly animations do.

		speedId = Animator.StringToHash("Speed");
		glidingId = Animator.StringToHash("Gliding");
		
		jumpAnimCur = jumpAnim;
		
		shakeCam = GameObject.FindWithTag ("MainCamera").GetComponent<Framing>();
		
		beaconParticles = GetComponentInChildren<ParticleSystem>();

        if (beaconParticles != null)
        {
            beaconParticles.enableEmission = false;
            beaconParticles.renderer.material.renderQueue = 4000;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Eyes.CanSee() != seen ){
				seen = !seen;
				Debug.Log ("Noke visibility is now "+seen);
		}
		if (state != 6) {
			//Debug.Log ("Current yorex state: "+state);
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
		else if (state == 6){
			//Debug.Log ("flying...");
			flying();
		}

		////////animator stuff
		if (GetComponent<NavMeshAgent> ().enabled) {
			animator.SetFloat (speedId, GetComponent<NavMeshAgent> ().speed);
		} 
		animator.SetBool (glidingId, Flying);
		
		////particle stuff
        if (beaconParticles != null)
        {
            if (state == 6 && Vector3.Magnitude(this.transform.position - Player.transform.position) > 100f)
            {
                //beaconParticles.enableEmission = true;
            }
            else
            {
                beaconParticles.enableEmission = false;
            }
        }
	}
	
	public void StartAI(){
		state = 1;
		navAgent.enabled = false;
		Vector3 targetPos = FindJumpNode (Player.transform.position);
		Vector3 startDir = (Player.transform.position - targetPos);
		startDir.y = 0f;
		this.transform.position = targetPos + (Vector3.up*100f) + (Quaternion.AngleAxis(-20, Vector3.up) * Vector3.Normalize (startDir) * 200f);
		this.transform.LookAt (Player.transform.position,Vector3.up);
		//Debug.Log ("I'm going to "+targetPos+", and ended up at "+this.transform.position);
		navAgent.speed = mySpeed;
		navAgent.enabled = true;
		Jump ();
	}
	
	public void DayFlee(){
		JumpTarget = fleePos;
		navAgent.enabled = false;
		Flying = true;
		state = 6;
	}
	
	public void Jump(){
		Debug.Log ("looking to jump");
		JumpTarget = FindJumpNode (Player.transform.position) + (Vector3.up*GetComponent<NavMeshAgent> ().height/2f);
		Vector3 moveDir = this.transform.position - JumpTarget; //move landing spot towards starting so he has space to land
			moveDir.y = 0f;
			JumpTarget += (Vector3.Normalize(moveDir)*landAnimDist);
		if (Vector3.Magnitude(JumpTarget - this.transform.position) > flyingSpeed){
		navAgent.enabled = false;
		Flying = true;
		state = 6;
			
		rise = true;
		riseTimer = riseTime;
		Debug.Log ("jumping to "+JumpTarget);
		}
		//this.transform.position = targetPos;
		//Debug.Log ("I'm going to "+targetPos+", and ended up at "+this.transform.position);
		//navAgent.enabled = true;
	}

	void flying(){
		if (riseTimer > 0f){riseTimer -= Time.deltaTime;}
		else{rise = false;}
		
		navAgent.enabled = false;
		Flying = true;
		/*if (!Flying) { //something's been turned off, go back to ground movement
			navAgent.enabled = true;
			state = 2;
		} else {*/
		Vector3 distLeft = JumpTarget - this.transform.position; //see how close we are to destination
		Vector3 lookDir = distLeft;

		distLeft.y = 0;

		////////////////////////rising/landing////////////////////
		//Debug.Log("time left for fly: "+(Vector3.Magnitude (distLeft) / flyingSpeed)+", time left for land: "+(Mathf.Abs(JumpTarget.y - this.transform.position.y) / landingSpeed));
		if ((Vector3.Magnitude (distLeft) / flyingSpeed) <= (Mathf.Abs(JumpTarget.y - this.transform.position.y) / landingSpeed)) { //if we're in range where we should start landing
			//Debug.Log("landing");
				this.transform.position += Vector3.up * Mathf.Sign (JumpTarget.y - this.transform.position.y) * landingSpeed * Time.deltaTime;
			} else {
			this.transform.position += Vector3.up * Mathf.Sign (glideHeight - this.transform.position.y) * risingSpeed * Time.deltaTime; //otherwise, rise up toward glide height
			lookDir.y = glideHeight - this.transform.position.y;
			}

		this.transform.rotation = Quaternion.LookRotation (Vector3.RotateTowards(this.transform.forward,distLeft,1.0f * Time.deltaTime, 0.0f)); // rotate towards destination

			////////////////////////moving/stopping////////////////////
		if (Vector3.Magnitude (distLeft) <= flyingSpeed * Time.deltaTime) { //if we're close enough to the target to get there this frame, get there
				Debug.Log ("Landed!");	
				navAgent.enabled = true;
				state = 2;
				Flying = false;
				this.transform.position = JumpTarget;

				this.transform.LookAt(JumpTarget,Vector3.up);
				
				float distToPlayer = Vector3.Magnitude(this.transform.position - Player.transform.position);
				shakeCam.ShakeScreen(
					2f - (1.5f * Mathf.Min (1f,Mathf.Abs ((distToPlayer/200f)))), 
					1f - (1f * Mathf.Min (1f,Mathf.Abs ((distToPlayer/300f))))
				);
			
			} else { //otherwise, fly towards it
			this.transform.position += Vector3.Normalize (distLeft) * flyingSpeed * Time.deltaTime;
			}
		//}
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
		Wandering = true;
		MoveAround ();

	}

	void search(){
		//Move around player's area slowly
		GetComponent<NavMeshAgent>().speed = 9;
		look ();
		//Move around code
		Wandering = true;
		MoveAround ();
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
		if (!seen){
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
	}

	void curious(){
		//Move closer to last heard hoverboard spot slowly, then stop, rotate after 5 seconds, return to move and search
		look ();
	}

	void flyReady(){
		if (!seen) {
			JumpCountdownCurrent -= Time.deltaTime;
			//Debug.Log ("Flying in: " + JumpCountdownCurrent);
		} else {
			JumpCountdownCurrent = JumpCountdown;
		}
		if (JumpCountdownCurrent < 0f){
			Jump ();
			JumpCountdownCurrent = JumpCountdown;
		}
	}

	void look(){
		flyReady ();
		if (seen){
			if (state!=3){Debug.Log ("saw player, started chasing");}
			state = 3;
			lastKnownPlayerPosition = Player.transform.position;
		}
	}
	
	
	Vector3 GetRandomNode(){
		bool[] canGo = new bool[4];
		int i;
		float pDiddy = Mathf.Infinity;
		int goNode = 0;
		int safeZones = 0;
		for(i = 0; i < RandomMove.Length; i+=1){
			if(Physics.Raycast(RandomMove[i].transform.position, -Vector3.up,10f)){
			canGo[i] = true;
				safeZones += 1;
			if (Vector3.Magnitude (RandomMove[i].transform.position - Player.transform.position) < pDiddy){
						pDiddy = Vector3.Magnitude (RandomMove[i].transform.position - Player.transform.position);
						goNode = i;
				}
			}
			else{
				canGo[i] = false;
			}
		}
		if (safeZones > 0){
		if (Random.Range (0f,1f)<.5f){return RandomMove[goNode].transform.position;}
		else{
			goNode = Random.Range (0,3);
			while(!canGo[goNode]){
				goNode = Random.Range (0,3);
			}
				
			return RandomMove[goNode].transform.position;	
		}
		}else{return this.transform.position;}

	}
	
	void MoveAround(){
		if(Wandering = true){
		navAgent.enabled = true;
		RaycastHit hit;
		if (!seen) {
			WanderCountdownCurrent -= Time.deltaTime;
			float moveDistance = (this.transform.position - NewRandomNode).sqrMagnitude;
			//Debug.Log (moveDistance);
			if((moveDistance) < 10f){
				navAgent.speed = 0;	
				}
			//Debug.Log ("Moving randomly in" + WanderCountdownCurrent);
		} else {
			WanderCountdownCurrent = WanderCountdown;
		}
		if (WanderCountdownCurrent <= 0f){
			NewRandomNode = GetRandomNode();
			navAgent.destination = NewRandomNode;
			WanderCountdownCurrent = WanderCountdown*(Random.Range (.5f,1f));
			}	
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

	//Animator BS
	[HideInInspector] private Animator animator;
	[HideInInspector] private int glidingId;
	[HideInInspector] private int speedId;
	
	[HideInInspector] new private ParticleSystem beaconParticles;

}
