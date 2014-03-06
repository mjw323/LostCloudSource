//*************************************************
// LostCloud.cginc
//
// Various helper functions for LostCloud shaders
//
// Kyle Small '13
// ks347@drexel.edu
//
//*************************************************
#ifndef LOSTCLOUD_CGINC
#define LOSTCLOUD_CGINC

#include "UnityCG.cginc"

// Blends 3 colors using a normal direction
inline half4 TriNormalBlend(half3 n, half4 a, half4 b, half4 c) {
	return (1 - n.x) * ((1 - n.z) * a + (n.z) * b) + (n.x) * c;
}

// Transform a vector using a new basis
inline float3 TransformBasis( float3 v, float x, float y, float z) {
	return normalize(v * float3(x, y, z));
}

// Transform a vector using a new basis, with each component in the range [0,1]
inline float3 TransformBasisProject( float3 v, float x, float y, float z) {
	float3 vt = normalize(v * float3(x, y, z));
	return saturate(pow(vt*1.5, 4));
}

#endif