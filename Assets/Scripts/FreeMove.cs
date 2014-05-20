using UnityEngine;
using System.Collections;


public class FreeMove : MonoBehaviour {
	
	void Start()
	{

	}
	
	void Update()
	{
		float stickX = Input.GetAxis("Horizontal");
		float stickY = Input.GetAxis("Vertical");
		Vector3 stickDirection = new Vector3(stickX, 0f, stickY);

		this.transform.Translate(stickDirection* 2, Space.World);

		if(Input.GetButton ("Jump")){
			this.transform.Translate (Vector3.up, Space.World);
		}

		if(Input.GetButton ("Grind")){
			this.transform.Translate (Vector3.down, Space.World);
		}
	
	}

}