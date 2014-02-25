/*
 * PROPRIETARY INFORMATION.  This software is proprietary to
 * Side Effects Software Inc., and is not to be reproduced,
 * transmitted, or disclosed in any way without written permission.
 *
 * Produced by:
 *      Side Effects Software Inc
 *		123 Front Street West, Suite 1401
 *		Toronto, Ontario
 *		Canada   M5J 2M2
 *		416-504-9876
 *
 * COMMENTS:
 * 
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using HAPI;

[ ExecuteInEditMode ]
public class HAPI_Instance: MonoBehaviour
{	
#if UNITY_EDITOR
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Public
	
	public int 			prInstancePointNumber { get { return myInstancePointNumber; } set { myInstancePointNumber = value; } }
	public GameObject 	prObjectToInstantiate { get { return myObjectToInstantiate; } set { myObjectToInstantiate = value; } }
	public bool 		prTransformChanged { get { return myTransformChanged; } set { myTransformChanged = value; } }

	public HAPI_Instancer prInstancer { get { return myInstancer; } set { myInstancer = value; } }


	public HAPI_Instance()
	{
		reset();
	}

	~HAPI_Instance()
	{

	}
	
	public void reset()
	{
		myInstancePointNumber = -1;
		myTransformChanged = false;
		myInstancer = null;

	}
	
	public void Awake()
	{
		myLastLocalToWorld = transform.localToWorldMatrix;
	}		

	public virtual void Update()
	{
		
		Matrix4x4 local_to_world = transform.localToWorldMatrix;
		
		if ( local_to_world == myLastLocalToWorld )
			return;
						
		myLastLocalToWorld = local_to_world;
		myTransformChanged = true;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Private

	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Serialized Private Data
	
	[SerializeField] private int			myInstancePointNumber;
	[SerializeField] private GameObject		myObjectToInstantiate;
	[SerializeField] private Matrix4x4		myLastLocalToWorld;
	[SerializeField] private bool			myTransformChanged;
	[SerializeField] private HAPI_Instancer myInstancer;
	
#endif // UNITY_EDITOR
}
