using UnityEngine;
using System.Collections;

public class YorexSight : MonoBehaviour {
	public Camera yorexEyes;
	private bool seen = false;
	private bool saw = false;

	// Use this for initialization
	void OnWillRenderObject () {
		if (Camera.current == yorexEyes){seen = true;}
	}
	
	public  bool Visible(){
			return saw;
	}
	
	// Update is called once per frame
	void Update () {
		saw = seen;
		seen = false;
	}
}
