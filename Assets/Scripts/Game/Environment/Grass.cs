using UnityEngine;
using System.Collections;

[ExecuteInEditMode,System.Serializable]
public class Grass : MonoBehaviour {
    public Material grassMaterial;

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "grass_icon.png", true);
    }
}
