/*
	Simple Lake Water Shader
	Kyle Small '13
	openglsdl@gmail.com
*/

Shader "LostCloud/Water" { 
	Properties {
		_BaseColor ("Base Water color", COLOR)  = ( .54, .95, .99, 0.5)	
		_RefractionColor ("Refraction color", COLOR)  = ( .54, .95, .99, 0.5)
		_FoamColor ("Foam Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
		_FoamOffset ("Foam Distance Offset", Range(0.0,1.0)) = 0.55
		_Reflectivity ("Water Reflectivity", Range(0.1,1.0)) = 0.5
		_FresnelFactor ("Fresnel Reflection/Refraction scale", Range(0.0,1)) = 0.25
		_NormalMap ("Ripple NormalMap", 2D) = "black" {}
		_RippleScale ("Ripple Size", Range(0.1,1000.0)) = 10.0
		_RippleSpeed ("Ripple Animation Speed", Range(0.0,0.1)) = 0.1
		_FlowMap ("Flow Map (NOT IMPLEMENTED)", 2D) = "black" {}
	}

	CGINCLUDE
		
		#include "UnityCG.cginc"
		
		struct v2f 
		{
			float4 position : SV_POSITION;
			float4 normal : TEXCOORD0;
			float4 viewDir : TEXCOORD1;
			float4 screenPos : TEXCOORD2;
			float4 worldPos : TEXCOORD3;
		};
	
		// Flow/Ripple Textures
		sampler2D _NormalMap;
		sampler2D _FlowMap;

		// Auto-Gen'ed Textures
		sampler2D _ReflectionTex;
		sampler2D _GrabTexture;
		sampler2D _CameraDepthTexture;

		// Wave info
		uniform float3 waveMapMax;
		uniform float3 waveMapSize;
		sampler2D _waveMapTexture;

		// Hoverboard
		uniform float4 _BoardPosition;
		uniform float4 _BoardDirection;
		uniform float4 _BoardVelocity;
		
		// Sun info
		uniform float4 _SunDirection;
		uniform float4 _SunColor;

		// Colors to use
		uniform float4 _BaseColor;
		uniform float4 _RefractionColor;
		uniform float4 _FoamColor;
		
		// Various Magic Numbers
		uniform float _FoamOffset;
		uniform float _Reflectivity;
		uniform float _FresnelFactor;
		uniform float _RippleScale;
		uniform float _RippleSpeed;

		v2f vert(appdata_full v)
		{		
			v2f o = (v2f)0;

			o.worldPos = mul(_Object2World,v.vertex);
			o.position = mul(UNITY_MATRIX_MVP,v.vertex);

			// Copy of screen space position for use in fragment shader
			o.screenPos = o.position;

			o.normal = float4(v.normal,0.0);
			o.viewDir = float4(WorldSpaceViewDir(v.vertex),1.0);

			return o;
		}

		half4 frag( v2f IN ) : COLOR
		{
			// Screen space position for sampling reflection/refraction textures
			float4 screenPos = ComputeScreenPos(IN.screenPos);
			screenPos.xy = screenPos.xy / screenPos.w;
			float4 foamPos = screenPos;
			
			// Board Waves
			float tx = (waveMapMax.x - IN.worldPos.x) * (1.0/waveMapSize.x);
			float tz = (waveMapMax.z - IN.worldPos.z) * (1.0/waveMapSize.z);
			float4 offset = tex2D(_waveMapTexture,float2(tx,tz));

			// Waves
			//float2 fromBoard = normalize(IN.worldPos.xz - _BoardPosition.xz);
			//float d = distance(IN.worldPos.xz,_BoardPosition.xz);
			//float phi = dot(fromBoard,normalize(_BoardVelocity.xz));
			//float2 offset = saturate(-0.9 - phi) * _Time.x * 100.0 * saturate(length(_BoardVelocity) - 2.0) * saturate(length(_BoardVelocity) - 2.0) * float2(sin(phi), cos(phi)) * 1.0 / (d*d*d);

			//offset += wake.xy;

			// Add some variance to screen position to simulate ripples
			float4 waveMap = tex2D(_NormalMap,float2(_Time.xz*_RippleSpeed) + ((offset + IN.worldPos.xz)/_RippleScale));
			screenPos += float4(0.05 * UnpackNormal(waveMap),0.0);

			// View space depth
			float depth = LinearEyeDepth(pow(tex2D(_CameraDepthTexture,screenPos.xy).r,_FresnelFactor));
			depth = depth * _ProjectionParams.w * (_ProjectionParams.z * 0.005);
			depth = min(1.0,depth);

			// Very simplistic approx. for fresnel term
			float4 v = normalize(IN.viewDir);
			float4 n = normalize(IN.normal);
			float F = dot(v,n);

			// Blended reflection/refraction color based on view angle and depth below 
			// lake surface
			half4 refColor = (1 - F) * lerp(_RefractionColor,tex2D(_ReflectionTex,screenPos.xy),depth) + F * lerp(_RefractionColor,tex2D(_GrabTexture,float2(screenPos.x,1.0 - screenPos.y)),1 - depth);
			
			// DEBUG: Draw some gridlines :)
			//half4 lines = half4(0.0);
			//if(fmod(sign(IN.worldPos.x) * IN.worldPos.x,10.0) < 0.1 || fmod(sign(IN.worldPos.z) * IN.worldPos.z,10.0) < 0.1) {
			//	lines = half4(1.0,0.0,0.0,1.0);
			//}

			// DEBUG: Draw some Circles :)
			//half4 circles = half4(0.0);
			//if(fmod(d,2.0) < 0.1 || fmod(d,2.0) < 0.1) {
			//	circles = half4(0.0,1.0,0.0,1.0);
			//}
			
			// Final color
			float foamFactor = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, foamPos.xy)));
			foamFactor = saturate(foamFactor - foamPos.w);
			float4 foam = max(0.0,1 - (foamFactor + _FoamOffset)) * _FoamColor;

			half4 final = foam + (depth * _BaseColor + refColor) * _Reflectivity;
			final.a = 1.0;

			return final;
		}
				
	ENDCG

	Subshader 
	{ 
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		
		Lod 500
		ColorMask RGB
		
		// Renders objects below the water to a render texture
		GrabPass { }
		
		Pass {
				Blend SrcAlpha OneMinusSrcAlpha
				ZTest LEqual
				ZWrite Off
				Cull Off
				
				CGPROGRAM
				
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#pragma glsl
				#pragma fragmentoption ARB_precision_hint_fastest	
							  			
				ENDCG
		}
	}

	Fallback "Transparent/Diffuse"
}