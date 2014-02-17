using UnityEngine;
using System.Collections;

public class CollectibleSystem : MonoBehaviour {
	public int upgradeCount;
	public int upgradesFound = 0;

	// Use this for initialization
	void Start () {
		upgradeCount = GameObject.FindGameObjectsWithTag ("Collectible").Length ();
	}

	void OnCollisionEnter(Collision col){
		if (col.tag == "Collectible") {
			Collectible c = col.gameObject.GetComponent<Collectible>();
			if (!c.collected){
				c.collected = true;
				upgradesFound += 1;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
