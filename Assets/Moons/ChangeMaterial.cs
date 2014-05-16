using UnityEngine;
using System.Collections;

public class ChangeMaterial : MonoBehaviour {
	private UpgradeSystem noke;

	public Material cresMoon;
	public Material waxMoon;
	public Material fullMoon;

	// Use this for initialization
	void Start () {
		noke = GameObject.FindWithTag ("Player").GetComponent<UpgradeSystem> ();
	}

	void Update () {
		//Debug.Log(noke.UpgradesFound());
		if (noke.UpgradesFound()==0){ 
			renderer.sharedMaterial = cresMoon;
		} else if (noke.UpgradesFound()==1){ 
			renderer.sharedMaterial = waxMoon;
		} else if (noke.UpgradesFound()>=2){ 
			renderer.sharedMaterial = fullMoon;
		}
	}
}
