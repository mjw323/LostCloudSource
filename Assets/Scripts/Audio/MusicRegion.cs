using UnityEngine;
using System.Collections;

public class MusicRegion : MonoBehaviour {

	public AudioClip daySong;
	public AudioClip nightSong;

	private MusicManager mgr;
	
	// Use this for initialization
	void Start () {
		mgr = GameObject.Find ("Music Manager").GetComponent<MusicManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider c) {
		//print("SoundManager.cs collided");
		if (c.gameObject.tag.Equals("Player") || c.gameObject.tag.Equals("Board")){
			//print ("SoundManager.cs found player");
			//If the music playing is the same as the one we just collided with, no need to change clip or restart song
			if (!string.Equals(daySong.name, mgr.currentClip.name)){

				//refer to the MusicManager class to change songs
				mgr.PlaySong(daySong);
			}
		}
	}
}
