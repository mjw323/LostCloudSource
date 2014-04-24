using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// List of GUI elements
	public GameObject mainMenuElements;
	public GameObject continueElements;
	public GameObject newGameElements;
	public GameObject optionsElements;
	public GameObject creditsElements;

	// Enables the main menu
	void LoadMainMenu() {
		// Disable everything
		// Enable the elements we need
		mainMenuElements.SetActive(true);
	}

	public void LoadScene(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	public void LoadCredits()
	{
		
	}

	//Quits game, won't work in WebPlayer builds
	public void QuitGame()
	{
		Application.Quit();
	}

}
