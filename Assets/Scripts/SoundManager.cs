using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip song;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider c) {
		print("collided");
		if (c.gameObject.tag.Equals("Player") || c.gameObject.tag.Equals("Board")){
			print ("found player");
			Camera.mainCamera.GetComponent<AudioSource>().clip = song;
			Camera.mainCamera.GetComponent<AudioSource>().Play();
		}
	}
}
