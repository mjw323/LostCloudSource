using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {
	void Start() {
		float[] distances = new float[32];
		distances[10] = 10000f;
		camera.layerCullDistances = distances;
	}
}