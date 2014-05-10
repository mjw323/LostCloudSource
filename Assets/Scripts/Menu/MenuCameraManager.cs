using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuCameraManager : MonoBehaviour {

	public List<TransformCamera> cameras;

	// Use this for initialization
	void Start () {
		StartCoroutine(SwitchCameras ());
	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator SwitchCameras(){
		int i = 0;
		while(true){

			//disable camera[i];
			cameras[i].GetComponent<Camera>().enabled = true;
			yield return new WaitForSeconds(6);
			foreach (TransformCamera camera in cameras){
				camera.Reset();
			}
			//enabled camera[i + (1%lengthoflist)];
			/*if (i + (1%cameras.Count) >= cameras.Count){
				i = 0;
			}*/

			//cameras[i+(1%cameras.Count)].GetComponent<Camera>().enabled = true;
			cameras[i].GetComponent<Camera>().enabled = false;
			i = (i + 1)%cameras.Count;
			print (i);

		}
	}
}
