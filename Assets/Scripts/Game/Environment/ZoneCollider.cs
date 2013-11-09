using UnityEngine;
using System.Collections;

public class ZoneCollider : MonoBehaviour {
	
	private Vertex m_colliderLocation;
	private GameObject m_player;
	private Collider myCollider;
	
	void OnTriggerEnter(Collider collider){
		myCollider = collider;
		if (collider.tag.Equals(m_player.tag)){
			Graph graph = m_player.GetComponent<Graph>();
			
			graph.playerLocation = m_colliderLocation;
			graph.LoadAdjacentVertices();
		}
	}
	
	// Use this for initialization
	void Start () {
		m_player = GameObject.FindGameObjectWithTag("Player");
		m_colliderLocation = transform.parent.GetComponent<Vertex>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
