using UnityEngine;
using System;

[Serializable]
public class HoverboardThrusters
{
	public Transform FrontThruster
	{
		get { return frontThruster; }
	}

	public Transform BackThruster
	{
		get { return backThruster; }
	}

	public HoverboardThrusters(Transform hostTransform, Corners corners)
	{
		Vector3 frontOffset = Vector3.zero;
		frontOffset.z = corners.TopFrontLeft.z;
		Vector3 backOffset = -frontOffset;
		CreateThruster(hostTransform, frontOffset, "Front Thruster");
		CreateThruster(hostTransform, backOffset, "Back Thruster");
	}

	private Transform CreateThruster(Transform hostTransform, Vector3 offset, string name)
	{
		GameObject thruster = new GameObject();
		thruster.name = name;
		thruster.transform.parent = hostTransform;
		thruster.transform.localPosition = offset;
		return thruster.transform;
	}

	[HideInInspector] private Transform frontThruster;
	[HideInInspector] private Transform backThruster;
}