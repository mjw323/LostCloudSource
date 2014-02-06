using UnityEngine;
using System.Collections;

public class MonsterTeleport : MonoBehaviour {

	public Transform Player;
	public GameObject Enemy;
	public int DistanceNeededToTeleport;
	public int DistanceNeededToReactivate;
	public bool Active;
	public NavMeshAgent navAgent;
	
	// Update is called once per frame
	void Update () {

		if(Player.gameObject.GetComponent<UpgradeSystem>().HasPlayerGottenNextUpgrade){
			if(Vector3.Magnitude(Player.position - this.transform.position) < DistanceNeededToTeleport && Active){
				navAgent = Enemy.GetComponent<NavMeshAgent>();
				navAgent.enabled = false;
				Enemy.transform.position = this.transform.position;
				navAgent.enabled = true;
				
				Active = false;
			}
		}
		
		if(Vector3.Magnitude(Player.position - this.transform.position) > DistanceNeededToReactivate ){
			Active = true;
		}
	
	}
}
