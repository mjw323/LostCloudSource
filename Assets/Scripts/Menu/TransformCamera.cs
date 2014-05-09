using UnityEngine;
using System.Collections;

public class TransformCamera : MonoBehaviour {
	public float translateSpeed = 0;
	public float rotateSpeed = 0;

	private Vector3 camPosition;
	private Quaternion camRotation;

	// Use this for initialization
	void Start () {
		camPosition = transform.position;
		camRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward*Time.deltaTime*translateSpeed);
		transform.Rotate(Vector3.down, Time.deltaTime*rotateSpeed);
	}

	public void Reset(){
		transform.position = camPosition;
		transform.rotation = camRotation;
	}
}
