using UnityEngine;
using System.Collections;

public class PauseMenuOptions : MonoBehaviour {

	public GameObject pauseMenuController;

	public void Message(string value){

		if (value.Contains("x")){
			//Extract the number from the string (0, 2, 4, 8)
			string amt = value.Substring(0, 1);
			int amount = int.Parse(amt);
			AntiAliasing(amount);
		}

		// Handle different quality settings
		else if (value == "Low") {
			 QualitySettings.SetQualityLevel(1, true);
		} else if (value == "Medium") {
			 QualitySettings.SetQualityLevel(2, true);
		} else if (value == "High") {
			 QualitySettings.SetQualityLevel(3, true);
		} else if (value == "Ultra") {
			 QualitySettings.SetQualityLevel(4, true);
		} 
		// Handle vertical sync
		else if (value == "On") {
			 QualitySettings.vSyncCount = 1;
		} else if (value == "Off") {
			 QualitySettings.vSyncCount = 0;
		} 
		// Back button
		else if (value == "Back") {
			pauseMenuController.GetComponent<PauseMenu>().LoadPauseMenu();
		} else {
			print("ERROR: The message: " + value + " is not recognized.  See the Message function in Options.js");
		}
	}

	void AntiAliasing(int amount){
		QualitySettings.antiAliasing = amount;
	}

	void Quit(){
		//pauseMenuController.LoadPauseMenu();
	}
}
