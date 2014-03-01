using UnityEngine;
using System.Collections;

public class YorexNodeDebug : MonoBehaviour {
	private Transform[] kids;

	void DrawCircle(Vector3 cen, float rad, Color col){
		int qual = 8;
		for(int i = 0; i < qual; i++){
			Debug.DrawLine(cen + (Quaternion.AngleAxis(360*i/qual,Vector3.up) * Vector3.right * rad),
					cen + (Quaternion.AngleAxis(360*(i+1)/qual,Vector3.up) * Vector3.right * rad), col, 1.0f, false);
		}
	}

	// Use this for initialization
	void Start () {
	kids = GetComponentsInChildren<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		Color col = Color.cyan;
		foreach (Transform kid in kids){
			if (kid != this.transform){
			col = Color.cyan;
			if(kid.childCount == 0){col = Color.red;}
			DrawCircle(kid.position,4f,col);
			Debug.DrawLine(kid.position,kid.parent.position,col,1.0f,false);
		}
		}
	}
}
