using UnityEngine;
using System.Collections;

public class EndingStuff : MonoBehaviour {
	private UpgradeSystem noke;
	private bool ending = false;

	// Use this for initialization
	void Start () {
		noke = GameObject.FindWithTag ("Player").GetComponent<UpgradeSystem> ();
		

		for (int i=0; i<this.transform.childCount; i+=1){ //put the kids to sleep
			this.transform.GetChild(i).gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!ending && noke.UpgradesFound()>=3){
			ending = true;
			for (int i=0; i<this.transform.childCount; i+=1){ //wake up, kids!!! the game's gonna end!!!
				this.transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}
}
