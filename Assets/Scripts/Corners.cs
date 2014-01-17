using UnityEngine;
using System.Collections;

public class Corners : MonoBehaviour
{
	public Vector3 TopFrontLeft
	{
		get
		{
			float x = -boxCollider.bounds.extents.x;
			float y = boxCollider.bounds.extents.y;
			float z = boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}	
	}

	public Vector3 TopFrontRight
	{
		get
		{
			float x = boxCollider.bounds.extents.x;
			float y = boxCollider.bounds.extents.y;
			float z = boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}	
	}

	public Vector3 TopBackLeft
	{
		get
		{
			float x = -boxCollider.bounds.extents.x;
			float y = boxCollider.bounds.extents.y;
			float z = -boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}
	}

	public Vector3 TopBackRight
	{
		get
		{
			float x = boxCollider.bounds.extents.x;
			float y = boxCollider.bounds.extents.y;
			float z = -boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}
	}

	public Vector3 BottomFrontLeft
	{
		get
		{
			float x = -boxCollider.bounds.extents.x;
			float y = -boxCollider.bounds.extents.y;
			float z = boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}
	}

	public Vector3 BottomFrontRight
	{
		get
		{
			float x = boxCollider.bounds.extents.x;
			float y = -boxCollider.bounds.extents.y;
			float z = boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}
	}

	public Vector3 BottomBackRight
	{
		get
		{
			float x = boxCollider.bounds.extents.x;
			float y = -boxCollider.bounds.extents.y;
			float z = -boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}
	}

	public Vector3 BottomBackLeft
	{
		get
		{
			float x = -boxCollider.bounds.extents.x;
			float y = -boxCollider.bounds.extents.y;
			float z = -boxCollider.bounds.extents.z;
			return new Vector3(x, y, z);
		}
	}

	private void Awake()
	{
		addedCollider = false;
		boxCollider = GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = gameObject.AddComponent<BoxCollider>();
			addedCollider = true;
		}
	}

	private void Start()
	{
		if (addedCollider)
		{
			boxCollider.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (addedCollider)
		{
			Destroy(boxCollider);	
		}
	}

	private BoxCollider boxCollider;
	private bool addedCollider;
}