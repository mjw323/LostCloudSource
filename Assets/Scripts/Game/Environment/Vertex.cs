using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex : MonoBehaviour
{
	public List<Vertex> connections;
	public string id;
	
	void Start(){
		id = transform.name;
	}
	
	public bool HasConnectionTo(Vertex v){
		if (connections.Contains(v))
			return true;
		else
			return false;
	}
	
	//Unload mesh (and later, world objects)
	public void LoadArea(){
		
	}
	
	//Not working :(
	public void UnloadArea(){
		print ("Vertex.UnloadArea() called");
		GameObject.Destroy(transform.parent);
	}
}
