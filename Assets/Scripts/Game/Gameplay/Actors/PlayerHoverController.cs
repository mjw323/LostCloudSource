using UnityEngine;
using System.Collections;

public class PlayerHoverController : MonoBehaviour {
	[SerializeField] Hover m_hover;

	void Update()
	{
		m_hover.Move(
				//Mathf.Sign(Input.GetAxis("Vertical"))*Mathf.Pow(Mathf.Abs(Input.GetAxis("Vertical")),1/2),
				Input.GetAxis("Vertical"),
				Input.GetAxis("Horizontal"),
				Input.GetButton("Jump"));
	}
}