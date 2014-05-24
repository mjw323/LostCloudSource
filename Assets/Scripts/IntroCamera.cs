using UnityEngine;
using System.Collections;

public class IntroCamera : MonoBehaviour {
	private bool havePlayed=false;
	private bool playing = false;
	private Transform target;
	public float startWait = 3f;

	public float cameraSpeed = 5f;
	public GameObject text;
	private int onNode = 0;

	// Use this for initialization
	void Start () {
		target = GameObject.FindWithTag ("IntroStartNode").transform;
		//text = this.transform.GetChild (0).gameObject;
		
		Color nc = text.renderer.material.color;
		nc.a = 0f;
		text.renderer.material.color = nc;
	}
	
	// Update is called once per frame
	void Update () {
		if (!havePlayed) {
			havePlayed = true;
			this.transform.position = target.position;
			this.transform.LookAt(target.GetChild(0));
			playing = true;
			this.gameObject.GetComponent<DynamicCamera>().enabled = false;
			}
		if (playing) {
			if (startWait > 0f){startWait -= Time.deltaTime;}
			else{
				if (target.childCount<2){
					playing = false;
					this.gameObject.GetComponent<DynamicCamera>().enabled = true;
				}else{
					if (Vector3.Magnitude(target.GetChild (1).position - this.transform.position)>(cameraSpeed*Time.deltaTime)){
						this.transform.position += Vector3.Normalize(target.GetChild (1).position - this.transform.position)*cameraSpeed*Time.deltaTime;
						this.transform.LookAt(
								target.GetChild (0).position //start
							+ ((target.GetChild (1).GetChild (0).position - target.GetChild (0).position) //amount to move by
						   * (Vector3.Magnitude(this.transform.position-target.position)/Vector3.Magnitude(target.GetChild (1).position-target.position))) //ratio 
							);
					}else{
						this.transform.position = target.GetChild (1).position;
						target = target.GetChild (1);
						this.transform.LookAt(target.GetChild (0).position);
						onNode += 1;
					}
				}
			}
			
			}
		if ((startWait<=0f) && onNode <2 && playing){
				Color nc = text.renderer.material.color;
				nc.a += (1f - nc.a)*Time.deltaTime*.66f;
				text.renderer.material.color = nc;
				}
		else{
			Color nc = text.renderer.material.color;
			nc.a += (0f - nc.a)*Time.deltaTime*.66f;
			text.renderer.material.color = nc;
		}
	}
}
