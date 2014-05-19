using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightMapController : MonoBehaviour {
	public int lightMapNumber = 45;
	public string lightmapFolder = "Lightmaps/";
	public string dayLightFolder = "Day/";
	public string nightLightFolder = "Night/";
	
	private LightmapData[] dayLightmapData;
	private LightmapData[] nightLightmapData;
	//private List<Texture2D> nightTest;

	// Use this for initialization
	void Start () {
		InitLightmaps();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 if (Input.GetKeyDown(KeyCode.N)){
			SwitchToNight();
		}
		if (Input.GetKeyDown(KeyCode.M)){
			SwitchToDay();
		}
		*/
	}
	
	public void SwitchToDay(){
		Debug.Log ("switching to day");
		LightmapSettings.lightmaps = dayLightmapData;
	}
	
	public void SwitchToNight(){
		Debug.Log ("switching to night");
		LightmapSettings.lightmaps = nightLightmapData;
	}
	
	private void InitLightmaps( )
	{
    	dayLightmapData = new LightmapData[lightMapNumber];
		nightLightmapData = new LightmapData[lightMapNumber];

    	for(int i = 0 ; i < lightMapNumber ; i++ ){
			Debug.Log (lightmapFolder + dayLightFolder + "LightmapFar-" + i.ToString());
        	dayLightmapData[i] = new LightmapData();
			nightLightmapData[i] = new LightmapData();
			
			//nightTest.Add (Resources.Load( lightmapFolder + nightLightFolder + "LightmapFar-" + i.ToString(), typeof(Texture2D)) as Texture2D);
			
			dayLightmapData[i].lightmapFar = Resources.Load( lightmapFolder + dayLightFolder + "LightmapFar-" + i.ToString(), typeof(Texture2D)) as Texture2D;
			nightLightmapData[i].lightmapFar = Resources.Load( lightmapFolder + nightLightFolder + "LightmapFar-" + i.ToString(), typeof(Texture2D)) as Texture2D;
		}

    LightmapSettings.lightmaps = dayLightmapData;
	}
}
