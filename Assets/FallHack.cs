using UnityEngine;
using System.Collections;

public class FallHack : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.transform.GetComponent<CharacterController>().isGrounded){
			//Debug.Log("characater controller is not grounded");
			//if (!jumping){
			Debug.Log("checkin for falling thru world");
			RaycastHit hit = new RaycastHit();
			Physics.Raycast(this.transform.position,Vector3.up,out hit);
			if (hit.transform!=null){
				Debug.Log("there's a dood above me");
			if (hit.transform.tag=="Terrain"){
					Debug.Log("he's terrrein");
				this.transform.Translate(10f*Vector3.up);
			}}
		}//}
	}
}
