using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip otherClip;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag.Equals("Player")){
			//Camera.mainCamera.GetComponent<AudioSource>().clip = blah;
		}
	}
}
