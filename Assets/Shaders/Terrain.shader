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
        _Range("Blend Height Offset", Float) = 1
        _HeightRamp("Height color gradient", 2D) = "white" {}
        _Tex1a ("Ground High", 2D) = "white" {}
        _Scale1("Texture Scaling", Range(1,200)) = 1
        _Tex1b ("Ground Low", 2D) = "white" {}
        _Scale2("Texture Scaling", Range(1,200)) = 1
        _Tex2 ("Wall 1", 2D) = "white" {}
        _Scale3("Texture Scaling", Range(1,200)) = 1
        _Tex3 ("Wall 2", 2D) = "white" {}
        _Scale4("Texture Scaling", Range(1,200)) = 1
        _RimPower( "Rim Power", Range( 0.5, 8.0 ) ) = 3.0
        _RimColor( "Rim Color", Color ) = ( 0.26, 0.19, 0.16, 0.0 )
        _Ramp( "Ramp", 2D ) = "gray" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
            
        LOD 300
        Cull Back

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
        float _Scale4;
        float _Range;
        
        sampler2D _Tex1a;
        sampler2D _Tex1b;
        sampler2D _Tex2;
        sampler2D _Tex3;
        sampler2D _HeightRamp;       
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
            half scale1 = 1.0 / _Scale1;
            half scale2 = 1.0 / _Scale2;
            half scale3 = 1.0 / _Scale3;
            half scale4 = 1.0 / _Scale4;

            half4 blendColor = tex2D(_HeightRamp,float2(saturate(1.0 - ((_Range - max(-IN.worldPos.y,IN.worldPos.y))/_Range))));

            half4 ca = tex2D(_Tex1a, IN.worldPos.xz * scale1);
            half4 cb = tex2D(_Tex1b, IN.worldPos.xz * scale2);

            half4 c1 = lerp(ca,cb,saturate(_Range - IN.worldPos.yyy)) * blendColor; 
            half4 c2 = tex2D(_Tex2, IN.worldPos.xy * scale3);
            half4 c3 = tex2D(_Tex3, IN.worldPos.yz * scale4);
            
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