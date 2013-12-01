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
	} 


	CGINCLUDE
		
		#include "UnityCG.cginc"
		
		struct v2f 
		{
			float4 position : SV_POSITION;
			float4 normal : TEXCOORD0;
			float4 viewDir : TEXCOORD1;
			float4 screenPos : TEXCOORD2;
		};

		// Auto-Gen'ed Textures
		sampler2D _ReflectionTex;
		sampler2D _RefractionTex;
		sampler2D _CameraDepthTexture;

		// Colors to use
		uniform float4 _BaseColor;
		uniform float4 _RefractionColor;
		uniform float4 _FoamColor;
		
		// Various Magic Numbers
		uniform float _FoamOffset;
		uniform float _Reflectivity;
		uniform float _FresnelFactor;

		v2f vert(appdata_full v)
		{		
			v2f o = (v2f)0;

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
			half4 refColor = (1 - F) * lerp(_RefractionColor,tex2D(_ReflectionTex,screenPos.xy),depth) + F * lerp(_RefractionColor,tex2D(_RefractionTex,screenPos.xy),1 - depth);

			// Final color
			float foamFactor = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, screenPos.xy)));
			foamFactor = saturate(foamFactor - IN.screenPos.w);
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
		GrabPass { "_RefractionTex" }
		
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