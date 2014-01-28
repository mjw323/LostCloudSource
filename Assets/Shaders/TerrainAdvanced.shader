//*************************************************
// TerrainAdvanced.cg
//
// Main Terrain shader for LostCloud
//
// Kyle Small '13
// ks347@drexel.edu
//
//*************************************************
Shader "LostCloud/TerrainAdvanced" {
    Properties {
        _Mix ("Mix Color",COLOR) = (1.0,1.0,1.0,1.0)
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

        float _BlendScale;
        float _Scale0;
        float _Scale1;
        float _Scale2;
        float _Scale3;
        float _Scale4;

        float _Ymax;
        float _invRange;
		float _HeightOffset;
		
        sampler2D _Ground0;
        sampler2D _Ground1;
        sampler2D _Ground2;
        sampler2D _Ground3;
        sampler2D _Wall;
        sampler2D _Ramp;

        float _RimPower;
        float4 _RimColor;

        float4 _Mix;

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
            float height = (_Ymax - (IN.worldPos.y + _HeightOffset) * _BlendScale) * _invRange;

            // Wrap height
            if(height < 0.0)
               height = -height;
            
            if(height > 1.0)
               height = 1.0 - fmod(height,1.0);

            float4 weights = float4( saturate( 1.0f - abs(height - 0) * 4.0 ),
                                     saturate( 1.0f - abs(height - 0.30) * 4.0),
                                     saturate( 1.0f - abs(height - 0.60) * 4.0),
                                     saturate( 1.0f - abs(height - 0.90) * 4.0));

            half4 c1 = tex2D(_Ground0, IN.worldPos.xz * _Scale0) * weights.w +
                       tex2D(_Ground1, IN.worldPos.xz * _Scale1) * weights.z +
                       tex2D(_Ground2, IN.worldPos.xz * _Scale2) * weights.y +
                       tex2D(_Ground3, IN.worldPos.xz * _Scale3) * weights.x;
            
            c1 *= _Mix;

            half4 c2 = tex2D(_Wall, IN.worldPos.xy * _Scale4);
			
            half3 projnormal = TransformBasisProject(IN.worldNormal,_X, _Y, _Z);
            half4 blendedColor = TriNormalBlend(projnormal,c1,c2,c2);

            o.Albedo = blendedColor.rgb;
            o.Alpha = blendedColor.a;

            half rim = 1.0 - saturate( dot( normalize(IN.viewDir), projnormal ) );
            o.Emission = _RimColor.rgb * pow( rim, _RimPower );
        }

        ENDCG
    }

    Fallback "Diffuse"
}