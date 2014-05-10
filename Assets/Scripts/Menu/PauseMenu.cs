using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public GameObject pauseMenu;
	public GameObject controller;
	public GameObject optionsMenu;
	public GameObject mainCamera; //maybe not needed
	
	private bool paused = false;
	
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
	}
	
	// Pauses/resumes game when "Esc" is pressed
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)){
			if(paused == false){
				PauseGame();
			}
			else{
				ResumeGame();
			}
		}
	}
	
	//Displays elements for pause menu;
	public void LoadPauseMenu(){
		Disable();
		pauseMenu.SetActive(true);
	}
	
	//Displays elements for options menu
	public void OptionsMenu(){
		Disable();
		optionsMenu.SetActive(true);
	}
	
	public void Save(){
		//Implement saving here
	}

	private void Quit(){
		Application.Quit();
		Application.LoadLevel(0);
	}

	private void Disable(){
		pauseMenu.SetActive(false);
		optionsMenu.SetActive(false);
	}

	public void PauseGame(){
		paused = true;
		Time.timeScale = 0;
		Screen.lockCursor = false;
		//Disable anything else we need here
		//Disable mouselook and joystick camera control here
	}

	public void ResumeGame(){
		paused = false;
		Time.timeScale = 1;
		Screen.lockCursor = true;
		//Re-enable any script we previously disabled.

		Disable();
	}
	
}
