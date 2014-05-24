using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// List of GUI elements
	public GameObject mainMenuElements;
	public GameObject optionsElements;
	public GameObject creditsElements;

	void Start()
	{	
		print ("calling LoadMainMenu");
		LoadMainMenu();
	}
	// Enables the main menu
	public void LoadMainMenu()
	{
		Disable();
		mainMenuElements.SetActive(true);
	}

	public void NewGame()
	{
		Disable ();
		Application.LoadLevel ("Island");
	}

	public void LoadCredits()
	{
		Disable();
		creditsElements.SetActive(true);
	}
	
	public void LoadOptions()
	{
		Disable();
		optionsElements.SetActive(true);
	}

	//Quits game, won't work in WebPlayer builds
	public void QuitGame()
	{
		Application.Quit();
		Application.LoadLevel (0);
	}

	private void Disable()
	{
		mainMenuElements.SetActive(false);
		optionsElements.SetActive(false);
		creditsElements.SetActive (false);
	}
}
