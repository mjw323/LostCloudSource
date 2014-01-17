using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class HoverboardSensors
{
	public Transform FrontRightSensor
	{
		get { return frontRightSensor; }
	}

	public Transform FrontLeftSensor
	{
		get { return frontLeftSensor; }
	}

	public Transform BackRightSensor
	{
		get { return backRightSensor; }
	}

	public Transform BackLeftSensor
	{
		get { return backLeftSensor; }
	}

	public Transform[] Sensors
	{
		get { return sensors; }
	}

	public HoverboardSensors(Transform hostTransform, Corners corners)
	{
		frontRightSensor = CreateSensor(hostTransform, corners.BottomFrontRight, "Front-Right Sensor");
		frontLeftSensor = CreateSensor(hostTransform, corners.BottomFrontLeft, "Front-Left Sensor");
		backRightSensor = CreateSensor(hostTransform, corners.BottomBackRight, "Back-Right Sensor");
		backLeftSensor = CreateSensor(hostTransform, corners.BottomBackLeft, "Back-Left Sensor");

		sensors = new Transform[4];
		sensors[0] = frontRightSensor;
		sensors[1] = frontLeftSensor;
		sensors[2] = backRightSensor;
		sensors[3] = backLeftSensor;

		sensorHits = new RaycastHit[sensors.Length];
	}

	private Transform CreateSensor(Transform hostTransform, Vector3 offset, string name)
	{
		GameObject sensor = new GameObject();
		sensor.name = name;
		sensor.transform.parent = hostTransform;
		sensor.transform.localPosition = hostTransform.InverseTransformPoint(offset);
		return sensor.transform;
	}

	[HideInInspector] private Transform frontRightSensor;
	[HideInInspector] private Transform frontLeftSensor;
	[HideInInspector] private Transform backRightSensor;
	[HideInInspector] private Transform backLeftSensor;

	[HideInInspector] private Transform[] sensors;
	[HideInInspector] private RaycastHit[] sensorHits; 
}