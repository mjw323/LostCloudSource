using UnityEngine;
using System.Collections;

public class EndingStuff : MonoBehaviour {
	private Transform player;
	[SerializeField] private GameObject yorex;
	[SerializeField] private GameObject noke;
	[SerializeField] private GameObject endCam;
	private Animator yorexAnim;
	private Animator nokeAnim;
	private int sceneStage = 0;
	private float waitAfterClimbTime = 9f;
	private float waitAfterLaunchTime = 4.9f;
	public float timeAfterEndToFade = 6f;
	private float waitedTime = 0f;
	public float flySpeed = 6f;
	private Interactive interactive;
	


    private void Awake()
    {
        interactive = GetComponent<Interactive>();
    }

    private void OnApplicationQuit()
    {
        interactive.OnInteracted -= OnInteracted;
    }

	// Use this for initialization
	void Start () {
		interactive.OnInteracted += OnInteracted;
		
		player = GameObject.FindWithTag ("Player").transform;
		
		yorexAnim = yorex.GetComponent<Animator>();
		nokeAnim = noke.GetComponent<Animator>();
		
		downId = Animator.StringToHash("GetDown");
		climbId = Animator.StringToHash("Climb");
		
		noke.SetActive(false);
	}
	
	/*
	private void OnTriggerEnter(Collider other)
    {
        // Can only collide with the player, but we need to find the root
        GetComponent<Interactive>().setFoot(other.transform.root.GetComponentInChildren<FootController>());
        GetComponent<Interactive>().enabled = true;
		Debug.Log ("collision enter");
    }*/
	
	private void OnInteracted()
    {
		if (sceneStage<2){
        	sceneStage = 2;
			GameObject.FindWithTag("MainCamera").GetComponent<FadeInOut>().fadeOutIn(this.gameObject,"SwitchNoke");
		}
    }
	
	private void SwitchNoke(){ //called by camera fade
		player.gameObject.SetActive(false);
		noke.SetActive(true);
		nokeAnim.SetBool(climbId,true);
		sceneStage = 3;
		
		GameObject.FindWithTag("MainCamera").SetActive(false);
		endCam.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	switch(sceneStage){
		case 0:
			if (Vector3.Magnitude(yorex.transform.position - player.position)<30f){
					sceneStage+=1; 
					yorexAnim.SetBool(downId,true);
			}
			break;
		case 3:
			waitedTime += Time.deltaTime;
			if (waitedTime > waitAfterClimbTime){
				sceneStage = 4;
				yorexAnim.SetBool(downId,false);
				waitedTime = 0f;
			}
			break;
		case 4:
			waitedTime += Time.deltaTime;
			if (waitedTime > waitAfterLaunchTime){
				waitedTime = 0f;
				sceneStage = 5;
			}
			break;
		case 5:
			yorex.transform.position += flySpeed*Time.deltaTime*Vector3.forward;
			if (waitedTime > timeAfterEndToFade){
				waitedTime = 0f;
				sceneStage = 6;
			}
			break;
		}
	}
	
	[HideInInspector] private int downId;
	[HideInInspector] private int climbId;
}
