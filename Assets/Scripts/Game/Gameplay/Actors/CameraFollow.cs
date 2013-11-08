using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
	public float distance;
	public float height;
	public float heightDamping;
	public float rotationDamping;
	public Transform target;
	private float xAngle = 0.0f;
	private float yAngle = 0.0f;

	void Update()
	{
		xAngle = Input.GetAxis ("RightStickH")*60;
		yAngle = Input.GetAxis ("RightStickV")*30;
		float rotationAngle = Mathf.LerpAngle(transform.eulerAngles.y+(xAngle), target.eulerAngles.y+(yAngle), rotationDamping * Time.deltaTime);
		Quaternion rotation = Quaternion.Euler(0, rotationAngle, 0);
		Vector3 newPosition = target.position;
		newPosition -= rotation * Vector3.forward * distance;
		newPosition.y = Mathf.Lerp(transform.position.y, target.position.y + height, heightDamping * Time.deltaTime);
		transform.position = newPosition;
		transform.LookAt(target);
	}
}