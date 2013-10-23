using UnityEngine;
using System.Collections;

public class PlayerHoverController : MonoBehaviour {
	[SerializeField] Hover m_hover;

	void Update()
	{
		m_hover.Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
	}
}