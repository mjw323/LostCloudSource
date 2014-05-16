using UnityEngine;
using System.Collections;

public class MoonCameraFacing : MonoBehaviour
{
	public Transform m_Camera;
	
	void Update()
	{
		//transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back,
		//                 m_Camera.transform.rotation * Vector3.forward);

		transform.LookAt(m_Camera);
	}
}