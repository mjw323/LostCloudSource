Shader "LostCloud/CharacterToonBump" {
	Properties {
		_Base ("Base (RGB)", 2D) = "white" {}
		_NormalMap ("NormalMap", 2D) = "bump" {}
        _Scale("Texture Scaling", Range(1,20)) = 1
        _RimPower( "Rim Power", Range( 0.5, 8.0 ) ) = 3.0
        _RimColor( "Rim Color", Color ) = ( 0.26, 0.19, 0.16, 0.0 ) 
		_Ramp("Color Ramp", 2D) = "white" {}
	}

	SubShader {
        Tags {"RenderType"="TransparentCutout" "Queue"="Transparent"}
         
        LOD 300
        Cull Off
        Alphatest Greater 0.2
        
        CGPROGRAM
        #pragma surface SurfMain ToonRamp
        #pragma target 3.0

        struct Input
        {
            half3 worldPos;
            float2 uv_Base;
            float2 uv_NormalMap;
            float3 viewDir;
            fixed3 worldNormal;
            INTERNAL_DATA
        };

        float _Scale;

        sampler2D _Base;
        sampler2D _NormalMap;       
        sampler2D _Ramp;

        float _RimPower;
        float4 _RimColor;

        half4 LightingToonRamp( SurfaceOutput s, half3 lightDir, half atten ) {
            half diff = dot( s.Normal, lightDir ) * 0.5 + 0.5;
            half3 ramp = tex2D( _Ramp, float2(diff)).rgb;

            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ramp * ( atten * 2 );
            c.a = s.Alpha;
            
            return c;
        }

        void SurfMain(Input IN, inout SurfaceOutput o)
        {
            half scale = 1.0 / _Scale;

            fixed4 baseColor = tex2D(_Base, IN.uv_Base * scale);            
            fixed3 normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap * scale));

            o.Albedo = baseColor.rgb;
            o.Alpha = baseColor.a;
            o.Normal = normal;

            half rim = 1.0 - saturate( dot( normalize(IN.viewDir), normal) );
            o.Emission = _RimColor.rgb * pow( rim, _RimPower );
        }

        ENDCG
    }

	FallBack "Diffuse"
}
