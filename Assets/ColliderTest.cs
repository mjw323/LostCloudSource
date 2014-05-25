using UnityEngine;
using System.Collections;

public class ColliderTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	   private void OnTriggerEnter(Collider other)
    {
		Debug.Log ("COLLIDER ENTER");
    }
	
	   private void OnCollisionStay(Collision other)
    {
		Debug.Log ("COLLIsION STAY");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
