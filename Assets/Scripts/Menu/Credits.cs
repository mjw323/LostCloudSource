using UnityEngine;
using System.Collections;

public class CreditsScript : MonoBehaviour {

	public GameObject mainMenuController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Back(){
		mainMenuController.GetComponent<MainMenu>().LoadMainMenu();
	}
}
