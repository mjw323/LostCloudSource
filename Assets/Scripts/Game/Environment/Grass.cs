using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Grass : MonoBehaviour {
    private MeshFilter mf = null;
    private MeshRenderer mr = null;
    private GameObject[] grass;
    private const int GRASS_COUNT = 32;

	// Use this for initialization
	void Start() {
        //InitializeGrass();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void InitializeGrass() {
        Vector3 offset = Vector3.zero;
        grass = new GameObject[GRASS_COUNT * 3];

        for (int idx = 0; idx < GRASS_COUNT * 3; idx+=3)
        {
            grass[idx] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            grass[idx].name = "GrassChild_" + (idx / 3) + "_a";
            grass[idx].transform.parent = transform;
            grass[idx].transform.position = transform.position + offset;
            grass[idx].renderer.castShadows = false;
            grass[idx].renderer.receiveShadows = false;
            grass[idx].renderer.material = renderer.material;

            grass[idx + 1] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            grass[idx + 1].name = "GrassChild_" + (idx / 3) + "_b";
            grass[idx + 1].transform.parent = transform;
            grass[idx + 1].transform.position = transform.position + offset;
            grass[idx + 1].transform.Rotate(Vector3.up, 45.0f);
            grass[idx + 1].renderer.castShadows = false;
            grass[idx + 1].renderer.receiveShadows = false;
            grass[idx + 1].renderer.material = renderer.material;

            grass[idx + 2] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            grass[idx + 2].name = "GrassChild_" + (idx / 3) + "_c";
            grass[idx + 2].transform.parent = transform;
            grass[idx + 2].transform.position = transform.position + offset;
            grass[idx + 2].transform.Rotate(Vector3.up, -45.0f);
            grass[idx + 2].renderer.castShadows = false;
            grass[idx + 2].renderer.receiveShadows = false;
            grass[idx + 2].renderer.material = renderer.material;

            offset.x += (2.0f * Mathf.Cos(Mathf.Repeat(idx, 360.0f)));
            offset.z += (2.0f * Mathf.Sin(Mathf.Repeat(idx, 360.0f)));
        }
    }
}
