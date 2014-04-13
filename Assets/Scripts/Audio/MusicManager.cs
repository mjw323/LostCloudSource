using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip song;

	private float audio1Volume = 1.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider c) {
		//print("SoundManager.cs collided");
		if (c.gameObject.tag.Equals("Player") || c.gameObject.tag.Equals("Board")){
			//print ("SoundManager.cs found player");
			//If the music playing is the same as the one we just collided with, no need to change clip or restart song
			if (!string.Equals(song.name, Camera.mainCamera.GetComponent<AudioSource>().clip.name)){
				Camera.mainCamera.GetComponent<AudioSource>().clip = song;
				Camera.mainCamera.GetComponent<AudioSource>().Play();
			}

		}
	}

	void FadeOut(){
		if(audio1Volume > 0.1)
		{
			audio1Volume -= 0.1f * Time.deltaTime;
			Camera.mainCamera.GetComponent<AudioSource>().volume = audio1Volume;
		}
	}
}
