using UnityEngine;
using System.Collections;

public class WhiteFlash : MonoBehaviour {
	
	public Texture2D fadeOutTexture;
	public int drawDepth;
	
	private float alpha = 0f;

	// Use this for initialization
	void Start () {
		drawDepth = -1000;
	}
	
	// Update is called once per frame
	void Update () {
		if (alpha > 0.01f) {
			Debug.Log ("flashin'");
		} else {
			alpha = 0f;
		}
		alpha += (0f - alpha) * .1f;
	}
	
	void OnGUI(){
		//alpha += fadeDir * fadeSpeed * Time.deltaTime;	
		//alpha = Mathf.Clamp01(alpha);	
 
		Color alphaColor = GUI.color;
		alphaColor.a = Mathf.Sqrt(alpha);
		GUI.color = alphaColor;
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);	
	}
	
	void flash(){
		alpha = 1f;
	}

}
