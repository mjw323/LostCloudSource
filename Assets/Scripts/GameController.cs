using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
		PlayerData data = new PlayerData();

		//data.location = location;
		//data.upgrade1 = upgrade1;
		// etc...
		bf.Serialize(file, data);
		file.Close();
	}

	public void Load(){
		if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();
			//location = data.location;
			//upgrade1 = data.upgrade1;
			//etc...
		}
	}
}

[Serializable]
class PlayerData{
	//insert values here that we want to save. upgrades, time of day, location, etc

}