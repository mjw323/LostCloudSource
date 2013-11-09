using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph : MonoBehaviour {
	
	
	public List<Vertex> vertices;
	int verticesCount;
	
	public Vertex playerLocation;
	
	// Use this for initialization
	void Start () {
		vertices = new List<Vertex>();
		verticesCount = vertices.Count;
		
		FindAllVertices();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FindAllVertices(){
		GameObject[] gameObjects;
		gameObjects = GameObject.FindGameObjectsWithTag("Vertex");
		foreach (GameObject go in gameObjects){
			vertices.Add(go.GetComponent<Vertex>());
		}
		print("vertices.count = " + vertices.Count);
	}
	
	public void LoadAdjacentVertices(){
		
		print ("Graph.LoadAdjacentVertices() called");
		foreach(Vertex v in vertices){
			
			//load area if the player's vertex has connection to v
			if((playerLocation.HasConnectionTo(v) && v.HasConnectionTo(playerLocation)) || (playerLocation.id.Equals( v.id))){
				v.gameObject.SetActive(true);
			}
			//unload if not adjacent
			else{
				v.gameObject.SetActive(false);
			}
		}
	}
}
