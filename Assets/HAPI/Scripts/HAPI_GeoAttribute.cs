﻿/*
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
using System.Collections;

public class HAPI_GeoAttribute : ScriptableObject
{
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Public Enums

	public enum Type
	{
		UNDEFINED = -1,
		BOOL,
		INT,
		FLOAT,
		MAX
	}

	public enum Preset
	{
		UNDEFINED = -1,
		POSITION,
		UV,
		NORMAL,
		COLOR,
		MAX
	}

	public enum SpecialPaintMode
	{
		COLOUR,
		MAX
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Public Properties

	public string prName
	{
		get
		{
			return myName;
		}
		set
		{
			myName = value;
		}
	}
	public Type prType
	{
		get
		{
			return myType;
		}
		set
		{
			if ( myType == Type.UNDEFINED )
				return;

			if ( myType == value || value <= Type.UNDEFINED || value >= Type.MAX )
				return;

			if ( myType == Type.BOOL || myType == Type.INT )
			{
				myFloatPaintValue = new float[ myTupleSize ];
				myFloatMins = new float[ myTupleSize ];
				myFloatMaxes = new float[ myTupleSize ];
				for ( int i = 0; i < myTupleSize; ++i )
				{
					myFloatPaintValue[ i ] = (float) myIntPaintValue[ i ];
					myFloatMins[ i ] = (float) myIntMins[ i ];
					myFloatMaxes[ i ] = (float) myIntMaxes[ i ];
				}
				myIntPaintValue = null;
				myIntMins = null;
				myIntMaxes = null;

				myFloatData = new float[ myIntData.Length ];
				for ( int i = 0; i < myIntData.Length; ++i )
					myFloatData[ i ] = (float) myIntData[ i ];
				myIntData = null;
			}
			else if ( myType == Type.FLOAT )
			{
				myIntPaintValue = new int[ myTupleSize ];
				myIntMins = new int[ myTupleSize ];
				myIntMaxes = new int[ myTupleSize ];
				for ( int i = 0; i < myTupleSize; ++i )
					if ( value == Type.BOOL )
					{
						myIntPaintValue[ i ] = (int) myFloatPaintValue[ i ] > 0 ? 1 : 0;
						myIntMins[ i ] = 0;
						myIntMaxes[ i ] = 1;
					}
					else
					{
						myIntPaintValue[ i ] = (int) myFloatPaintValue[ i ];
						myIntMins[ i ] = (int) myFloatMins[ i ];
						myIntMaxes[ i ] = (int) myFloatMaxes[ i ];
					}
				myFloatPaintValue = null;
				myFloatMins = null;
				myFloatMaxes = null;

				myIntData = new int[ myFloatData.Length ];
				for ( int i = 0; i < myFloatData.Length; ++i )
					if ( value == Type.BOOL )
						myIntData[ i ] = (int) myFloatData[ i ] > 0 ? 1 : 0;
					else
						myIntData[ i ] = (int) myFloatData[ i ];
				myFloatData = null;
			}

			myType = value;
		}
	}
	public int prTupleSize
	{
		get
		{
			return myTupleSize;
		}
		set
		{
			if ( myType == Type.UNDEFINED )
				return;

			if ( myTupleSize == value || value < 1 || value > 5 )
				return;

			int new_tuple_size = value;

			if ( myType == Type.BOOL || myType == Type.INT )
			{
				int[] new_paint_value = new int[ new_tuple_size ];
				int[] new_mins = new int[ new_tuple_size ];
				int[] new_maxes = new int[ new_tuple_size ];
				int[] new_data = new int[ myVertexCount * new_tuple_size ];

				int min_tuple_size = Mathf.Min( myTupleSize, new_tuple_size );
				for ( int i = 0; i < min_tuple_size; ++i )
				{
					new_paint_value[ i ] = myIntPaintValue[ i ];
					new_mins[ i ] = myIntMins[ i ];
					new_maxes[ i ] = myIntMaxes[ i ];
				}
				for ( int i = min_tuple_size; i < new_tuple_size; ++i )
				{
					if ( myType == Type.BOOL )
					{
						new_paint_value[ i ] = 1;
						new_mins[ i ] = 0;
						new_maxes[ i ] = 1;
					}
					else if ( myType == Type.INT )
					{
						new_paint_value[ i ] = myDefaultIntPaintValue;
						new_mins[ i ] = myDefaultIntMin;
						new_maxes[ i ] = myDefaultIntMax;
					}
				}

				for ( int i = 0; i < myVertexCount; ++i )
					for ( int j = 0; j < min_tuple_size; ++j )
						new_data[ i * new_tuple_size + j ] = myIntData[ i * myTupleSize + j ];

				myIntPaintValue = new_paint_value;
				myIntMins = new_mins;
				myIntMaxes = new_maxes;
				myIntData = new_data;
				myTupleSize = new_tuple_size;
				myPaintMode = Mathf.Min( myPaintMode, (int) SpecialPaintMode.MAX + new_tuple_size );
			}
			else if ( myType == Type.FLOAT )
			{
				float[] new_paint_value = new float[ new_tuple_size ];
				float[] new_mins = new float[ new_tuple_size ];
				float[] new_maxes = new float[ new_tuple_size ];
				float[] new_data = new float[ myVertexCount * new_tuple_size ];

				int min_tuple_size = Mathf.Min( myTupleSize, new_tuple_size );
				for ( int i = 0; i < min_tuple_size; ++i )
				{
					new_paint_value[ i ] = myFloatPaintValue[ i ];
					new_mins[ i ] = myFloatMins[ i ];
					new_maxes[ i ] = myFloatMaxes[ i ];
				}
				for ( int i = min_tuple_size; i < new_tuple_size; ++i )
				{
					new_paint_value[ i ] = myDefaultFloatPaintValue;
					new_mins[ i ] = myDefaultFloatMin;
					new_maxes[ i ] = myDefaultFloatMax;
				}

				for ( int i = 0; i < myVertexCount; ++i )
					for ( int j = 0; j < min_tuple_size; ++j )
						new_data[ i * new_tuple_size + j ] = myFloatData[ i * myTupleSize + j ];

				myFloatPaintValue = new_paint_value;
				myFloatMins = new_mins;
				myFloatMaxes = new_maxes;
				myFloatData = new_data;
				myTupleSize = new_tuple_size;
				myPaintMode = Mathf.Min( myPaintMode, (int) SpecialPaintMode.MAX + new_tuple_size );
			}
		}
	}

	public int prPaintMode
	{
		get
		{
			return myPaintMode;
		}
		set
		{
			myPaintMode = Mathf.Min( value, (int) SpecialPaintMode.MAX + myTupleSize );
		}
	}

	public int[] prIntPaintValue { get { return myIntPaintValue; } set { myIntPaintValue = value; } }
	public int[] prIntMins { get { return myIntMins; } set { myIntMins = value; } }
	public int[] prIntMaxes { get { return myIntMaxes; } 
		set { if ( myType != Type.BOOL ) myIntMaxes = value; } }
	public float[] prFloatPaintValue { get { return myFloatPaintValue; } set { myFloatPaintValue = value; } }
	public float[] prFloatMins { get { return myFloatMins; } set { myFloatMins = value; } }
	public float[] prFloatMaxes { get { return myFloatMaxes; } set { myFloatMaxes = value; } }

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Public Methods

	public HAPI_GeoAttribute()
	{
		reset();
	}

	public void reset()
	{
		myName = "NO_NAME";

		myType = Type.UNDEFINED;
		myTupleSize = 1;
		myVertexCount = 0;

		myPaintMode = (int) SpecialPaintMode.COLOUR;

		myIntPaintValue = null;
		myIntMins = null;
		myIntMaxes = null;
		myIntData = null;

		myFloatPaintValue = null;
		myFloatMins = null;
		myFloatMaxes = null;
		myFloatData = null;
	}

	public void init( Mesh mesh, Preset preset )
	{
		switch ( preset )
		{
			case Preset.POSITION:
			{
				init(
					mesh, HAPI.HAPI_Constants.HAPI_ATTRIB_POSITION, Type.FLOAT,
					HAPI.HAPI_Constants.HAPI_POSITION_VECTOR_SIZE );
				break;
			}
			case Preset.UV:
			{
				init(
					mesh, HAPI.HAPI_Constants.HAPI_ATTRIB_UV, Type.FLOAT,
					HAPI.HAPI_Constants.HAPI_UV_VECTOR_SIZE );
				break;
			}
			case Preset.NORMAL:
			{
				init(
					mesh, HAPI.HAPI_Constants.HAPI_ATTRIB_NORMAL, Type.FLOAT,
					HAPI.HAPI_Constants.HAPI_NORMAL_VECTOR_SIZE );
				break;
			}
			case Preset.COLOR:
			{
				init(
					mesh, HAPI.HAPI_Constants.HAPI_ATTRIB_COLOR, Type.FLOAT,
					HAPI.HAPI_Constants.HAPI_COLOR_VECTOR_SIZE );
				break;
			}
			default: return; // Throw error.
		}
	}

	public void init( Mesh mesh, string name, Type type, int tuple_size )
	{
		if ( tuple_size <= 0 )
			return; // Throw error.

		if ( type <= Type.UNDEFINED || type >= Type.MAX )
			return; // Throw error.

		reset();
		myName = name;
		myType = type;
		myTupleSize = tuple_size;
		myVertexCount = mesh.vertexCount;

		if ( type == Type.BOOL || type == Type.INT )
		{
			myIntPaintValue = new int[ tuple_size ];
			myIntMins = new int[ tuple_size ];
			myIntMaxes = new int[ tuple_size ];
			myIntData = new int[ mesh.vertexCount * tuple_size ];

			for ( int i = 0; i < tuple_size; ++i )
				if ( type == Type.BOOL )
				{
					// These are hard coded because...well, BOOLs.
					myIntPaintValue[ i ] = 1;
					myIntMins[ i ] = 0;
					myIntMaxes[ i ] = 1;
				}
				else if ( type == Type.INT )
				{
					myIntPaintValue[ i ] = myDefaultIntPaintValue;
					myIntMins[ i ] = myDefaultIntMin;
					myIntMaxes[ i ] = myDefaultIntMax;
				}
		}
		else if ( type == Type.FLOAT )
		{
			myFloatPaintValue = new float[ tuple_size ];
			myFloatMins = new float[ tuple_size ];
			myFloatMaxes = new float[ tuple_size ];
			myFloatData = new float[ mesh.vertexCount * tuple_size ];

			for ( int i = 0; i < tuple_size; ++i )
			{
				myFloatPaintValue[ i ] = myDefaultFloatPaintValue;
				myFloatMins[ i ] = myDefaultFloatMin;
				myFloatMaxes[ i ] = myDefaultFloatMax;
			}
		}
	}

	// -----------------------------------------------------------------------
	// Representation

	public Color[] getColorRepresentation()
	{
		if ( myType == Type.UNDEFINED )
			return null; // Throw error.

		Color[] colors = new Color[ myVertexCount ];

		if ( myPaintMode == (int) SpecialPaintMode.COLOUR )
		{
			for ( int i = 0; i < myVertexCount; ++i )
			{
				colors[ i ].r = 1.0f;
				colors[ i ].g = 1.0f;
				colors[ i ].b = 1.0f;
				colors[ i ].a = 1.0f;

				if ( myType == Type.BOOL || myType == Type.INT )
					for ( int j = 0; j < Mathf.Min( 3, myTupleSize ); ++j )
						colors[ i ][ j ] =
							Mathf.InverseLerp( 
								myIntMins[ j ], myIntMaxes[ j ], 
								myIntData[ i * myTupleSize + j ] );
				else if ( myType == Type.FLOAT )
					for ( int j = 0; j < Mathf.Min( 3, myTupleSize ); ++j )
						colors[ i ][ j ] =
							Mathf.InverseLerp( 
								myFloatMins[ j ], myFloatMaxes[ j ], 
								myFloatData[ i * myTupleSize + j ] );
			}
		}
		else
		{
			int component_index = myPaintMode - (int) SpecialPaintMode.MAX;

			for ( int i = 0; i < myVertexCount; ++i )
			{
				colors[ i ].r = 1.0f;
				colors[ i ].g = 1.0f;
				colors[ i ].b = 1.0f;
				colors[ i ].a = 1.0f;

				if ( myType == Type.BOOL || myType == Type.INT )
					for ( int j = 0; j < 3; ++j )
						colors[ i ][ j ] =
							Mathf.InverseLerp( 
								myIntMins[ component_index ],
								myIntMaxes[ component_index ], 
								myIntData[ i * myTupleSize + component_index ] );
				else if ( myType == Type.FLOAT )
					for ( int j = 0; j < 3; ++j )
						colors[ i ][ j ] =
							Mathf.InverseLerp( 
								myFloatMins[ component_index ],
								myFloatMaxes[ component_index ], 
								myFloatData[ i * myTupleSize + component_index ] );
			}
		}

		return colors;
	}

	// -----------------------------------------------------------------------
	// Paint

	public void paint( int vertex_index, float paint_factor )
	{
		if ( vertex_index <= 0 || vertex_index >= myVertexCount )
			return; // Throw error.

		int start_comp_index = 0;
		int end_comp_index = myTupleSize;

		if ( myPaintMode >= (int) SpecialPaintMode.MAX )
		{
			int component_index = myPaintMode - (int) SpecialPaintMode.MAX;
			start_comp_index = component_index;
			end_comp_index = component_index + 1;
		}

		for ( int i = start_comp_index; i < end_comp_index; ++i )
			if ( myType == Type.BOOL )
				myIntData[ vertex_index * myTupleSize + i ] += (int) Mathf.Sign( paint_factor ) * myIntPaintValue[ i ];
			else if ( myType == Type.INT )
			{
				myIntData[ vertex_index * myTupleSize + i ] +=
					(int) ( paint_factor * (float) myIntPaintValue[ i ] );
				myIntData[ vertex_index * myTupleSize + i ] =
					Mathf.Clamp( myIntData[ vertex_index * myTupleSize + i ], 
					myIntMins[ i ], myIntMaxes[ i ] );
			}
			else if ( myType == Type.FLOAT )
			{
				myFloatData[ vertex_index * myTupleSize + i ] +=
					paint_factor * myFloatPaintValue[ i ];
				myFloatData[ vertex_index * myTupleSize + i ] =
					Mathf.Clamp( myFloatData[ vertex_index * myTupleSize + i ],
					myFloatMins[ i ], myFloatMaxes[ i ] );
			}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Private

	[SerializeField] private string myName;

	[SerializeField] private Type myType;
	[SerializeField] private int myTupleSize;
	[SerializeField] private int myVertexCount;

	[SerializeField] private int myPaintMode;

	[SerializeField] private int[] myIntPaintValue;
	[SerializeField] private int[] myIntMins;
	[SerializeField] private int[] myIntMaxes;
	[SerializeField] private int[] myIntData;
	
	[SerializeField] private float[] myFloatPaintValue;
	[SerializeField] private float[] myFloatMins;
	[SerializeField] private float[] myFloatMaxes;
	[SerializeField] private float[] myFloatData;

	// -----------------------------------------------------------------------
	// Constants
	private const int myDefaultIntPaintValue = 1;
	private const int myDefaultIntMin = 0;
	private const int myDefaultIntMax = 10;
	private const float myDefaultFloatPaintValue = 0.1f;
	private const float myDefaultFloatMin = 0.0f;
	private const float myDefaultFloatMax = 1.0f;
}
