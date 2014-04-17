using UnityEngine;
using System.Collections;

public class AudioCone : MonoBehaviour {
	public float scrollSpeed = -3f;
	public float intensity = 0f;

	// Use this for initialization
	void Start () {
		//
	}
	
	// Update is called once per frame
	void Update () {
		float offset = Time.time *scrollSpeed;
		renderer.material.SetTextureOffset ("_MainTex", new Vector2(0,offset));
		renderer.material.SetColor ("_Color", new Vector4(1f,1f,1f,intensity));

		if (intensity > 0f){ //scale only grows, and snaps to 0 when intensity is 0
		this.transform.localScale = new Vector3(
				Mathf.Max(Mathf.Sqrt(intensity)*2.5f,transform.localScale.x),
				Mathf.Max(Mathf.Sqrt(intensity)*2.5f,transform.localScale.y),
				Mathf.Max(Mathf.Pow(intensity,2f)*10f,transform.localScale.z)
				);
		}else{
			this.transform.localScale = Vector3.zero;
		}
	}
}
