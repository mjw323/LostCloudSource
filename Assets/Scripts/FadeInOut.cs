using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {
	
	public Texture2D fadeOutTexture;
	public float fadeSpeed;
	public int drawDepth;
	
	private float alpha;
	private int fadeDir;
	
	private bool reverse = false;
	private GameObject caller;
	private string callMessage;

	// Use this for initialization
	void Start () {
		fadeSpeed = 0.3f;
		drawDepth = -1000;
		alpha = 1.0f;
		fadeDir = -1;
		fadeIn ();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnGUI(){
		alpha += fadeDir * fadeSpeed * Time.deltaTime;	
		alpha = Mathf.Clamp01(alpha);	
		
		if (alpha>=1f && reverse){
				reverse = false; fadeIn ();
				caller.SendMessage(callMessage);
		}
 
		Color alphaColor = GUI.color;
		alphaColor.a = alpha;
		GUI.color = alphaColor;
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);	
	}
	
	void fadeIn(){
		fadeDir = -1;
	}
	
	void fadeOut(){
		fadeDir = 1;
	}
	
	public void fadeOutIn(GameObject call, string str){ //give me an object and a message too
		fadeDir = 1;
		reverse = true;
		
		caller = call;
		callMessage = str;
	}
}
