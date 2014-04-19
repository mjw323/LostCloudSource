using UnityEngine;
using System.Collections;

public class SoundMachine : MonoBehaviour {
	private UpgradeSystem noke;
	private Activatable activator;
	private AudioCone[] awaves;
	private bool pActive = false;
	public float soundTime = 4f;
	private float soundTimeC = 0f;
	
	private ParticleSystem[] particles;
	
	public bool destroyed = false; //use this to swap the model and particles when it gets wrecked
	
	// Use this for initialization
	void Start () {
		noke = GameObject.FindWithTag ("Player").GetComponent<UpgradeSystem> ();
		activator = GetComponent<Activatable> ();
		awaves = GetComponentsInChildren<AudioCone>();
		
		particles = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem part in particles){part.enableEmission = false;}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (destroyed){
				foreach (ParticleSystem part in particles){part.enableEmission = true;}
				activator.setActive(true);
			foreach (AudioCone fx in awaves){
				fx.intensity = 0f;
			}
		}
		else{
			if (activator.Activated () && !pActive){ //on the frame it's activated, start playing sound machine fx
				soundTimeC = soundTime;
			}
			
		if (soundTimeC > 0f){ //play thru sound machine fx
			soundTimeC -= Time.deltaTime;
			foreach (AudioCone fx in awaves){
				fx.intensity += (1f - fx.intensity) * 0.4f;
			}
		}
		else{
			foreach (AudioCone fx in awaves){
				fx.intensity += (0f - fx.intensity) * 0.2f;
			}
		}
			
		if (noke.HasPlayerGottenNextUpgrade == activator.Activated ()) { //make it activatable at night
			activator.setActive(!activator.Activated());
		}

		pActive = activator.Activated ();
		}


	}
}
