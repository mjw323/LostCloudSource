using UnityEngine;
using System.Collections;

public class SoundMachine : MonoBehaviour {
	private UpgradeSystem noke;
	private Activatable activator;

	// Use this for initialization
	void Start () {
		noke = GameObject.FindWithTag ("Player").GetComponent<UpgradeSystem> ();
		activator = GetComponent<Activatable> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (noke.HasPlayerGottenNextUpgrade == activator.Activated ()) {
			activator.setActive(!activator.Activated());
		}
	}
}
