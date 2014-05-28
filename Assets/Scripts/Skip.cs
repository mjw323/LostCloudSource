using UnityEngine;
using System.Collections;

[AddComponentMenu("Debug/Skip")]
public class Skip : MonoBehaviour
{
	public void SkipDayOne()
	{
		UpdateTransform();
		targetTransform.position = skipNode1.position;
	}

	public void SkipDayTwo()
	{
		UpdateTransform();
		targetTransform.position = skipNode2.position;
	}

	public void SkipDayThree()
	{
		UpdateTransform();
		targetTransform.position = skipNode3.position;
	}

	public void SkipNight()
	{
		UpdateTransform();
		targetTransform.position = skipNode0.position;
	}
	
	public void SkipEnding()
	{
		UpdateTransform();
		targetTransform.position = endNode.position;
		GameObject.FindWithTag("GameController").GetComponent<GameController>().SkipToEnd();
	}

	[SerializeField] private Transform player;
	[SerializeField] private Transform skipNode0;
	[SerializeField] private Transform skipNode1;
	[SerializeField] private Transform skipNode2;
	[SerializeField] private Transform skipNode3;
	[SerializeField] private Transform endNode;
	[SerializeField] private UpgradeCinematic upgradeCinematic;

	private void UpdateTransform()
	{
		if (player.parent.tag == "Yorex") {
			targetTransform = player.parent;
		} else {
			targetTransform = player;
		}
	}
	
	public void NightTime(){
				upgradeCinematic.Play();
	}

	// If the player is riding the hoverboard, this will be assigned to the Hoverboard's transform.
	private Transform targetTransform;
}