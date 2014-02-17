using UnityEngine;

public class TestRendered : MonoBehaviour
{	
	public bool Visible;
	public Camera Eyes;

	void Update()
	{
		if (renderer.IsVisibleFrom(Eyes)) {
			Visible = true;
		}
		else {
			Visible = false;
		}
	}
}