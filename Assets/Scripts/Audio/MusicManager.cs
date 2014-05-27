using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip currentClip;

	public AudioSource audio;
	
	[HideInInspector] public AudioClip daySong;
	[HideInInspector] public AudioClip nightSong;
	private TimeOfDay theTime;

	private float audio1Volume = 1.0f;

	//static MusicManager instance;

	void Start(){
		audio.clip = currentClip;
		audio.Play ();
		
		theTime = GameObject.FindWithTag("GameController").GetComponent<TimeOfDay>();
	}
	
	void Update(){
		if (theTime.IsDay && currentClip!=daySong){
			PlaySong (daySong);
		}
		if (!theTime.IsDay && currentClip!=nightSong){
			PlaySong (nightSong);
		}
	}

	public void PlaySong(AudioClip newClip){
		StartCoroutine (DoPlaySong(newClip));
	}
	
	IEnumerator FadeOut(){
		while(audio1Volume > 0.1){
			audio1Volume -= 0.2f * Time.deltaTime;
		//	print("volume = " + audio1Volume);
			audio.volume = audio1Volume;
			//Camera.mainCamera.GetComponent<AudioSource>().volume = audio1Volume;
			yield return null;
		}
	}

	IEnumerator FadeIn(){
		while (audio1Volume < 1) {
			audio1Volume += 0.2f * Time.deltaTime;
			audio.volume = audio1Volume;
			yield return null;
		}
	}

	private IEnumerator DoPlaySong(AudioClip newClip){
		if (!string.Equals (newClip.name, currentClip.name)) {
			yield return StartCoroutine(FadeOut ());
			currentClip = newClip;
			if (audio1Volume <= 0.2f) {
				audio.clip = newClip;
				audio.Play ();
				yield return StartCoroutine(FadeIn ());
			}
		}
	}

	//For use if we want MusicManager to be singleton
	/*
	public static MusicManager GetInstance(){
		if (instance == null) {
			GameObject musicMgr = new GameObject("Music Manager");
			instance = musicMgr.AddComponent<MusicManager>();

			instance.audio = musicMgr.AddComponent<AudioSource>();

			instance.currentClip = 
		}
		return instance;
	}
	*/
}
