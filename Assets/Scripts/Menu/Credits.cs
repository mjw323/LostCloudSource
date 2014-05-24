using UnityEngine;
using System.Collections;

public class Credits:MonoBehaviour {

	public GameObject mainMenuController;

	void Back(){
		mainMenuController.GetComponent<MainMenu>().LoadMainMenu();
	}
}
