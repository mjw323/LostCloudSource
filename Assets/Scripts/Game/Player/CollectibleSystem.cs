using UnityEngine;
using System.Collections;

public class CollectibleSystem : MonoBehaviour {
	//public int upgradeCount;
	//public int upgradesFound = 0;
	private GameObject[] upgrades;

	// Use this for initialization
	void Start () {
		upgrades = GameObject.FindGameObjectsWithTag ("Collectible");
	}

	/*void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Collectible") {
			Collectible c = col.gameObject.GetComponent<Collectible>();
			if (!c.collected){
				c.collected = true;
				upgradesFound += 1;
			}
		}
	}*/

	public float UpgradeCompletion () {
		float count = 0f;
		for (int i = 0; i < upgrades.Length; i+= 1) {
			if (upgrades[i].GetComponent<Activatable>().Activated()){count = 1f;}
		}
		Debug.Log (count / (float)upgrades.Length);
		return count / (float)upgrades.Length;
	}
}
