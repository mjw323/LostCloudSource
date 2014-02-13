using UnityEngine;
using System.Collections;

public class RayCasting : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		Vector3 forward = transform.TransformDirection(Vector3.forward) ;
		Vector3 downoffset = new Vector3(0,-2,3);
		Vector3 rightoffset = new Vector3(3,0,3);
		Vector3 upoffset = new Vector3(0,2,3);
		Vector3 leftoffset = new Vector3(-3,0,3);

		RaycastHit hit;
		if(Physics.SphereCast(gameObject.transform.position, 3, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
			transform.LookAt(hit.transform);
			Debug.DrawRay(transform.position,transform.forward*10,Color.green);

		if(Physics.SphereCast(gameObject.transform.position + upoffset, 1, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
			transform.LookAt(hit.transform);
		Debug.DrawRay(transform.position + upoffset,transform.forward*10,Color.green);

		if(Physics.SphereCast(gameObject.transform.position + downoffset, 1, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
			transform.LookAt(hit.transform);
		Debug.DrawRay(transform.position + downoffset,transform.forward*10,Color.green);

		if(Physics.SphereCast(gameObject.transform.position + leftoffset, 1, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
			transform.LookAt(hit.transform);
		Debug.DrawRay(gameObject.transform.position + leftoffset,transform.forward*10,Color.green);

		if(Physics.SphereCast(gameObject.transform.position + rightoffset, 1, transform.forward, out hit, 20000) && hit.transform.gameObject.tag == "Player")
			transform.LookAt(hit.transform);
		Debug.DrawRay(gameObject.transform.position + rightoffset,transform.forward*10,Color.green);
		}
}
