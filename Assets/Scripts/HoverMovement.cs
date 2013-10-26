// HoverMovement.cs
// Author: Zach Greer
using UnityEngine;
using System.Collections;

// Keeps the attached rigidbody afloat and orients it to be parallel with the ground.
[RequireComponent (typeof(Rigidbody))]
public class HoverMovement : MonoBehaviour {
	public float forwardPower;
	public float steerPower;
	public float hoverHeight;
	public float minHeight;
	public float stability; // Distance in meters that the terrain samplers are from the corners of the board's BoxCollider.

	public float raycastOffset;
	public float slerpStep;

	private float jumpPower; // measures how long jump was held, 0 - 1
	public float jumpLength = 3.0f; //how many seconds to build maximum jump
	public float jumpForce = 20.0f; //The max force applied when jumping

	Transform[] samplers;
	RaycastHit hit;
	Vector3[] normals;
	Vector3 averageNormal;
	Vector3 newForward;

	// Uses a temporary BoxCollider to discern the dimensions of the board.
	Vector3 CalculateBoardDimensions() {
		bool createdBoxCollider = false;
		BoxCollider boxCollider = GetComponent<BoxCollider>();
		if (!boxCollider) {
			boxCollider = gameObject.AddComponent<BoxCollider>();
			createdBoxCollider = true;
		}
		Vector3 boxDimensions = new Vector3(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y, boxCollider.size.z * transform.localScale.z);
		if (createdBoxCollider)
			Destroy(boxCollider);
		return boxDimensions;
	}

	Vector3[] CalculateSamplerTransforms(Vector3 boxDimensions) {
		Vector3[] samplerPositions = new Vector3[5];
		samplerPositions[0] = new Vector3(transform.position.x - boxDimensions.x / 2, transform.position.y - boxDimensions.y / 2, transform.position.z + boxDimensions.z / 2 + stability);
		samplerPositions[1] = new Vector3(transform.position.x + boxDimensions.x / 2, transform.position.y - boxDimensions.y / 2, transform.position.z + boxDimensions.z / 2 + stability);
		samplerPositions[2] = new Vector3(transform.position.x - boxDimensions.x / 2, transform.position.y - boxDimensions.y / 2, transform.position.z - boxDimensions.z / 2 - stability);
		samplerPositions[3] = new Vector3(transform.position.x + boxDimensions.x / 2, transform.position.y - boxDimensions.y / 2, transform.position.z - boxDimensions.z / 2 - stability);
		samplerPositions[4] = new Vector3(transform.position.x, transform.position.y - boxDimensions.y / 2, transform.position.z);
		return samplerPositions;
	}

	void CreateSamplers(Vector3[] samplerPositions) {
		samplers = new Transform[5];
		for (int i = 0; i < samplers.Length; i++) {
			GameObject sampler = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sampler.name = "Terrain Sampler " + (i + 1);
			sampler.transform.parent = transform;
			sampler.transform.localPosition = transform.InverseTransformPoint(samplerPositions[i]);
			Destroy(sampler.GetComponent<MeshRenderer>());
			Destroy(sampler.GetComponent<Collider>());
			samplers[i] = sampler.transform;
		}
	}

	void UpdateNormals() {
		for (int i = 0; i < samplers.Length; i++) {
			if (Physics.Raycast(samplers[i].position, -samplers[i].up, out hit, hoverHeight + raycastOffset)) {
				normals[i] = hit.normal;
				Debug.DrawLine(samplers[i].position, hit.point, Color.yellow);
				Debug.DrawRay(hit.point, hit.normal, Color.green);
			}
		}
		averageNormal = (normals[0] + normals[1] + normals[2] + normals[3] + normals[4]) / 5;
		Debug.DrawRay(transform.position, averageNormal, Color.red);
	}

	void ApplyForces() {
		rigidbody.AddForce(-Physics.gravity * rigidbody.mass * Mathf.Pow(hoverHeight / hit.distance, 2));
	}

	void Reorient() {
		newForward = Vector3.Cross(averageNormal.normalized, transform.right);
		transform.up = averageNormal.normalized;
		transform.forward = newForward;
	}

	public void Move(float forward, float steer, bool jump) {
		if (jump){jumpPower = Mathf.Clamp(jumpPower+(Time.deltaTime)/jumpLength,0.0f,1.0f);}
		else{rigidbody.AddForce(transform.up * jumpForce * Mathf.Sqrt(jumpPower));}
		rigidbody.AddForce(transform.forward * forward * forwardPower * (1 + (0.25f * jumpPower)));
		rigidbody.AddTorque(transform.up * steer * steerPower * (0.5f + ((1 - jumpPower)/1)));
	}

	void Awake() {
		CreateSamplers(CalculateSamplerTransforms(CalculateBoardDimensions()));
		normals = new Vector3[5];
	}

	void FixedUpdate() {
		UpdateNormals();
		ApplyForces();
		//Reorient();
	}

	void OnDrawGizmos() {
		if (samplers == null)
			return;
		foreach (Transform sampler in samplers)
			Gizmos.DrawSphere(sampler.position, 0.05f);
	}
}