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
    SubShader
    {
        Tags { "RenderType"="Opaque" }
            
        LOD 300

        CGPROGRAM
        #pragma multi_compile TRIPLANAR_ON TRIPLANAR_OFF
        #pragma multi_compile BUMPMAP_ON BUMPMAP_OFF
        #pragma multi_compile HGRAD_ON HGRAD_OFF
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
        
        sampler2D _Ground0Bump;
        sampler2D _Ground1Bump;
        sampler2D _Ground2Bump;
        sampler2D _Ground3Bump;

        sampler2D _Wall;
        sampler2D _Ramp;

        half4 LightingTerrain( SurfaceOutput s, half3 lightDir, half atten ) {
            half diff = dot( s.Normal, lightDir ) * 0.5 + 0.5;

            half4 c;
            c.rgb = s.Albedo * diff * _LightColor0.rgb * ( atten * 1.5 );
            c.a = s.Alpha;
            
            return c;
        }

        void SurfMain(Input IN, inout SurfaceOutput o)
        {
            float y = IN.worldPos.y + _HeightOffset;
            float _Ymin = _Ymax - (1.0/_invRange);

            // Wrap height
            if(y < _Ymin)
               y = _Ymax - fmod(_Ymin,y);
            
            if(y > _Ymax)
               y = _Ymin + fmod(y,_Ymax);//1.0 - fmod(height,1.0);

            float height = 1.0 - (_Ymax - y) * _invRange;

            float4 weights = float4( saturate( 1.0f - abs(height - 0) * 4.0 ),
                                     saturate( 1.0f - abs(height - 0.30) * 4.0),
                                     saturate( 1.0f - abs(height - 0.60) * 4.0),
                                     saturate( 1.0f - abs(height - 0.90) * 4.0));

            half4 c1 = tex2D(_Ground0, IN.worldPos.xz * _Scale0) * weights.x +
                       tex2D(_Ground1, IN.worldPos.xz * _Scale1) * weights.y +
                       tex2D(_Ground2, IN.worldPos.xz * _Scale2) * weights.z +
                       tex2D(_Ground3, IN.worldPos.xz * _Scale3) * weights.w;
        
        #ifdef BUMPMAP_ON
            // TODO
        #endif


        // Modulate height colors
        #ifdef HGRAD_ON
            half4 heightRamp = tex2D(_Ramp, float2(1.0 - (_Ymax - IN.worldPos.y) * _invRange,0.0));
            c1 = c1 * heightRamp + c1;
        #endif

            half4 blendedColor = c1;
            half3 projnormal = TransformBasisProject(IN.worldNormal,_X, _Y, _Z);

        #ifdef TRIPLANAR_ON 
            half4 c2 = tex2D(_Wall, IN.worldPos.xy * _Scale4);
            blendedColor = TriNormalBlend(projnormal,c1,c2,c2);
        #endif

            o.Albedo = blendedColor.rgb;
            o.Alpha = blendedColor.a;
        }

        ENDCG
    }

    Fallback "Diffuse"
}