using UnityEngine;
using System.Collections;

/* 
 * this mofo is an ubiquitous component for things noke can activate
 * when it's not activated, it'll show a prompt when noke's near it and off her board. if she pushes the activate button,
 * it gets activated and turns off.
 * 
 * it wants a prefab for the button card it shows (Assets/Prefabs/upgrades/controllerButton)

 */

public class Activatable : MonoBehaviour {

	private Transform noke; 
	public float yOffset = 2.0f; // height from origin to draw button prompt
	public GameObject button;
	private GameObject myButton;
	private MeshRenderer buttonDraw;
	private Transform camera;
	private float activeDistance = 3.0f;
	private bool activated = false;
	public string message = "";
	public float messageAttachment = 0f;

	// Use this for initialization
	void Start () {
		noke = GameObject.FindWithTag ("Player").transform;
		camera = GameObject.FindWithTag ("MainCamera").transform;

		myButton = Instantiate (button, this.transform.position + (Vector3.up * yOffset), Quaternion.identity) as GameObject;
		myButton.transform.parent = this.transform;
		buttonDraw = myButton.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		Color color = buttonDraw.materials [0].color;
		float dist = Vector3.Magnitude (noke.position - this.transform.position);
		if (dist < activeDistance && !activated && noke.GetComponent<FootMovement>().enabled) {
			buttonDraw.materials [0].SetColor("_Color", new Color(color.r,color.g,color.b,color.a + ((1f - color.a) * 0.3f))); //fade in button
			if (Input.GetButton("Activate")){
				activated = true;
				if (message!=""){noke.SendMessage(message,messageAttachment,SendMessageOptions.RequireReceiver);}
			}

		} else {
			buttonDraw.materials [0].SetColor("_Color", new Color(color.r,color.g,color.b,color.a + ((0f - color.a) * 0.1f))); //fade out button
		}

		myButton.transform.rotation = Quaternion.LookRotation (camera.position - myButton.transform.position) * Quaternion.Euler(0, -90, 0); //button rotates toward camera
	}

	public bool Activated(){ // find out if this has been activated
		return activated;
	}

	public void Deactivate(){ // call this to deactivate
		activated = false;
	}

	public void setActive(bool setto){ // call this to deactivate
		activated = setto;
	}
}
