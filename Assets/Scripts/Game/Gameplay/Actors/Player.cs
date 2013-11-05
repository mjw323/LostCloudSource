using UnityEngine;

[AddComponentMenu ("Scripts/Character/Player")]
[RequireComponent (typeof(FootMovement))]
public class Player : MonoBehaviour
{
	[HideInInspector] private FootMovement m_footMovement;

	public void MoveTowards(Vector3 direction)
	{
		m_footMovement.MoveTowards(direction);
	}

	void Awake()
	{
		m_footMovement = GetComponent<FootMovement>();
	}
}