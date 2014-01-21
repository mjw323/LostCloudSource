//*************************************************
// Terrain.cg
//
// Main Terrain shader for LostCloud
//
// Kyle Small '13
// ks347@drexel.edu
//
//*************************************************
Shader "LostCloud/Terrain" {
	Properties
    {
        _X("X-axis Blend Weight", Range(0,1)) = 1
        _Y("Y-axis Blend Weight", Range(0,1)) = 1
        _Z("Z-axis Blend Weight", Range(0,1)) = 1

        _RimPower( "Rim Power", Range( 0.5, 8.0 ) ) = 3.0
        _RimColor( "Rim Color", Color ) = ( 0.26, 0.19, 0.16, 0.0 )
        _Ramp( "Ramp", 2D ) = "gray" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
            
        LOD 300

        CGPROGRAM
        #pragma surface SurfMain Terrain
        #pragma target 3.0
		#include "LostCloud.cginc"
		
        struct Input
        {
            float3 worldPos;
            float3 viewDir;
            float3 worldNormal;
        };

        half _X;
        half _Y;
        half _Z;

        float _Scale1;
        float _Scale2;
        float _Scale3;

        float _Ymax;
        float _invRange;
        float _TexWidth;
		float _HeightOffset;
		
        sampler2D _Ground;
        sampler2D _Wall1;
        sampler2D _Wall2;
        sampler2D _Ramp;

        float _RimPower;
        float4 _RimColor;

        half4 LightingTerrain( SurfaceOutput s, half3 lightDir, half atten ) {
            half diff = dot( s.Normal, lightDir ) * 0.5 + 0.5;
            half3 ramp = tex2D( _Ramp, float2(diff)).rgb;

            half4 c;
            c.rgb = s.Albedo * diff * _LightColor0.rgb * ramp * ( atten * 1.5 );
            c.a = s.Alpha;
            
            return c;
        }

        void SurfMain(Input IN, inout SurfaceOutput o)
        {
            float height = 1.0 - (_Ymax - ((_HeightOffset + IN.worldPos.y) * _Scale1)) * _invRange;
            float2 uv = half2(sign(IN.worldPos.x) * fmod(IN.worldPos.x,_TexWidth)/1024.0,sign(IN.worldPos.z) * fmod(IN.worldPos.z,_TexWidth)/1024.0);

            half4 c1 = tex2D(_Ground, float2(height,sqrt(IN.worldPos.x + IN.worldPos.z)));

            half4 c2 = tex2D(_Wall1, IN.worldPos.xy * _Scale2);
            half4 c3 = tex2D(_Wall2, IN.worldPos.yz * _Scale3);
			
            half3 projnormal = TransformBasisProject(IN.worldNormal,_X, _Y, _Z);
            half4 blendedColor = TriNormalBlend(projnormal,c1,c2,c3);

            o.Albedo = blendedColor.rgb;
            o.Alpha = blendedColor.a;

            half rim = 1.0 - saturate( dot( normalize(IN.viewDir), projnormal ) );
            o.Emission = _RimColor.rgb * pow( rim, _RimPower );
        }

        ENDCG
    }

    Fallback "Diffuse"
}