using UnityEngine;
using System.Collections;

public class MusicRegion : MonoBehaviour {

	public AudioClip daySong;
	public AudioClip nightSong;
	
	private TimeOfDay theTime;

	private MusicManager mgr;
	
	// Use this for initialization
	void Start () {
		theTime = GameObject.FindWithTag("GameController").GetComponent<TimeOfDay>();
		mgr = GameObject.FindWithTag ("MusicManager").GetComponent<MusicManager>();
		for (int i=0;i<this.transform.childCount;i+=1){
			MusicRegion g = this.transform.GetChild(i).gameObject.AddComponent("MusicRegion") as MusicRegion;
			g.daySong = daySong;
			g.nightSong = nightSong;
			
		}
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
				mgr.daySong = daySong;
				mgr.nightSong = nightSong;
				
				if (theTime.IsDay){
				//refer to the MusicManager class to change songs
					mgr.PlaySong(daySong);
				}else{
					mgr.PlaySong(nightSong);
				}
			}
		}
	}
}
