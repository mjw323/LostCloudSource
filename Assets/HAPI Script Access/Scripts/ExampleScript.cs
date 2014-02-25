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
using System.Runtime.Serialization;
using System.Collections;
using HAPI;

[ExecuteInEditMode]
public class ExampleScript : MonoBehaviour 
{
#if UNITY_EDITOR
	public int parmIndex;
	public string[] parmNames;

	[SerializeField] private HAPI_AssetAccessor asset;
	[SerializeField] private string parmName;
	[SerializeField] private int parmSize;
	[SerializeField] private HAPI_AssetAccessor.ParmType parmType;
	[SerializeField] private int[] parmIntValue;
	[SerializeField] private float[] parmFloatValue;
	[SerializeField] private string[] parmStringValue;
	
	ExampleScript()
	{
		Debug.Log( "ExampleScript: CONSTRUCTOR" );
		EditorApplication.playmodeStateChanged += playmodeStateChanged;
	}
	
	~ExampleScript()
	{
		Debug.Log( "ExampleScript: DESTRUCTOR" );
		EditorApplication.playmodeStateChanged -= playmodeStateChanged;
	}
	
	public void playmodeStateChanged()
	{
		Debug.Log( "ExampleScript: playmodeStateChanged - " + EditorApplication.isPlayingOrWillChangePlaymode );
	}

	public void OnApplicationQuit()
	{
		Debug.Log( "ExampleScript: OnApplicationQuit" );
	}

	public void Reset()
	{
		Debug.Log( "ExampleScript: Reset" );
	}

	public void Awake()
	{
		Debug.Log( "ExampleScript: Awake" );
	}
	
	public void Start() 
	{
		Debug.Log( "ExampleScript: Start" );

		parmIndex = 0;
		parmNames = null;
		asset = null;
		parmName = "";
		parmSize = 0;
		parmType = HAPI_AssetAccessor.ParmType.INVALID;
		parmIntValue = null;
		parmFloatValue = null;
		parmStringValue = null;

		// If the game object has a HAPI_Asset component then get
		// the parameters for this asset and set the selected
		// parameter to be the asset's first parameter
		asset = HAPI_AssetAccessor.getAssetAccessor( gameObject );
		if ( asset != null )
		{
			Debug.Log( "Asset name: " + asset.prName );
			parmNames = asset.getParameters();
		}
		SetSelectedParameter();
	}
	
	public void Update() 
	{
		//Debug.Log( "ExampleScript: Update" );
	}
	
	public void OnEnable()
	{
		Debug.Log( "ExampleScript: OnEnable" );
	}
	
	public void OnDisable()
	{
		Debug.Log( "ExampleScript: OnDisable" );
	}
	
	public void OnScene()
	{
		//Debug.Log( "ExampleScript: OnScene" );
	}
	
	public void OnGUI()
	{
		//Debug.Log( "ExampleScript: OnGUI" );
	}

	// Set the currently selected parameter and retrieve its values
	public void SetSelectedParameter()
	{
		try
		{
			parmName = parmNames[ parmIndex ];
			parmSize = asset.getParmSize( parmName );
			parmType = asset.getParmType( parmName );
			parmIntValue = null;
			parmFloatValue = null;
			parmStringValue = null;

			if ( parmType == HAPI_AssetAccessor.ParmType.INT )
			{
				parmIntValue = new int[ parmSize ];

				for ( int i = 0; i < parmSize; i++ )
				{
					parmIntValue[ i ] = asset.getParmIntValue( parmName, i );
				}
			}
			else if ( parmType == HAPI_AssetAccessor.ParmType.FLOAT )
			{
				parmFloatValue = new float[ parmSize ];
				
				for ( int i = 0; i < parmSize; i++ )
				{
					parmFloatValue[ i ] = asset.getParmFloatValue( parmName, i );
				}
			}
			else if ( parmType == HAPI_AssetAccessor.ParmType.STRING )
			{
				parmStringValue = new string[ parmSize ];
				
				for ( int i = 0; i < parmSize; i++ )
				{
					parmStringValue[ i ] = asset.getParmStringValue( parmName, i );
				}
			}
		}
		catch ( HAPI_Error err )
		{
			Debug.LogError( err.ToString() );
		}
	}

	// Set the value of the currently selected parameter
	public void SetParameterValue()
	{
		try
		{
			for ( int i = 0; i < parmSize; i++ )
			{
				if ( parmType == HAPI_AssetAccessor.ParmType.INT )
					asset.setParmIntValue( parmName, i, parmIntValue[ i ] );
				else if ( parmType == HAPI_AssetAccessor.ParmType.FLOAT )
					asset.setParmFloatValue( parmName, i, parmFloatValue[ i ] );
				else if ( parmType == HAPI_AssetAccessor.ParmType.STRING )
					asset.setParmStringValue( parmName, i, parmStringValue[ i ] );
			}
		}
		catch ( HAPI_Error err )
		{
			Debug.LogError( err.ToString() );
		}
	}

	// Set up the UI for the currently selected parameter
	public void GetParameterGUI()
	{
		try
		{
			for ( int i = 0; i < parmSize; i++ )
			{
				if ( parmType == HAPI_AssetAccessor.ParmType.INT )
					parmIntValue[ i ] = EditorGUILayout.IntField( parmIntValue[ i ] );
				else if ( parmType == HAPI_AssetAccessor.ParmType.FLOAT )
					parmFloatValue[ i ] = EditorGUILayout.FloatField( parmFloatValue[ i ] );
				else if ( parmType == HAPI_AssetAccessor.ParmType.STRING )
					parmStringValue[ i ] = EditorGUILayout.TextField( parmStringValue[ i ] );
			}
		}
		catch ( HAPI_Error err )
		{
			Debug.LogError( err.ToString() );
		}
	}
#endif // UNITY_EDITOR
}
