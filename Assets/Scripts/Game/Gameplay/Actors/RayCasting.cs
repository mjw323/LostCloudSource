using UnityEngine;
using System.Collections;

public class RayCasting : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		Vector3 forward = transform.TransformDirection(Vector3.forward) ;
		if(Physics.Raycast(transform.position, transform.forward,  50)){
			Debug.DrawRay(transform.position, transform.forward*5, Color.green);
		}

		RaycastHit  hit;                                                                                                                                                
		if(Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 500)) {                                                                            
			if(hit.collider.gameObject.tag == "Player"){                             
				Debug.DrawRay(transform.position, transform.forward*5, Color.red);    
				transform.LookAt(hit.transform);
			}                                                                                                                                                       
		}

	
	}
}
