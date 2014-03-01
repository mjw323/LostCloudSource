using UnityEngine;
using System.Collections;

public class UpgradeParticleController : MonoBehaviour {
	private Activatable activator;
	public GameObject particle;
	// Use this for initialization
	void Start () {
		activator = GetComponent<Activatable> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (activator.Activated ()) {
			particle.GetComponent<ParticleSystem>().enableEmission = false;

			Component[] partics = particle.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem p in partics){
				p.enableEmission = false;
			}
		}
	}
}
