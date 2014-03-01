using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadScene(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	public void LoadCredits()
	{
		
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
