Shader "LostCloud/TriplanarToonBump"
{
    Properties
    {
        _X("X-axis Blend Weight", Range(0,1)) = 1
        _Y("Y-axis Blend Weight", Range(0,1)) = 1
        _Z("Z-axis Blend Weight", Range(0,1)) = 1
        _Scale("Texture Scaling", Range(1,20)) = 1
        _NScale("NormalMap Scaling", Range(1,20)) = 1
        _Tex1 ("Ground", 2D) = "white" {}
        _Nmap1 ("Ground NormalMap", 2D) = "black" {}
        _Tex2 ("Wall 1", 2D) = "white" {}
        _Nmap2 ("Wall 1 NormalMap", 2D) = "black" {}
        _Tex3 ("Wall 2", 2D) = "white" {}
        _Nmap3 ("Wall 2 NormalMap", 2D) = "black" {}
        _RimPower( "Rim Power", Range( 0.5, 8.0 ) ) = 3.0
        _RimColor( "Rim Color", Color ) = ( 0.26, 0.19, 0.16, 0.0 )
        _Ramp( "Ramp", 2D ) = "gray" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
            
        LOD 300

        CGPROGRAM
        #pragma surface SurfMain TriplanarRamp vertex:vert
        #pragma target 3.0

        struct Input
        {
            half3 worldPos;
            float3 viewDir;
            fixed3 worldNormal;
            fixed3 norm;
            INTERNAL_DATA
        };

        fixed _X;
        fixed _Y;
        fixed _Z;

        float _Scale;
        float _NScale;

        sampler2D _Tex1;
        sampler2D _Tex2;
        sampler2D _Tex3;
        sampler2D _Nmap1;
        sampler2D _Nmap2;
        sampler2D _Nmap3;        
        sampler2D _Ramp;

        float _RimPower;
        float4 _RimColor;

        half4 LightingTriplanarRamp( SurfaceOutput s, half3 lightDir, half atten ) {
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
            half nscale = 1.0 / _NScale;

            fixed4 c1 = tex2D(_Tex1, IN.worldPos.xz * scale);
            fixed4 c2 = tex2D(_Tex2, IN.worldPos.xy * scale);
            fixed4 c3 = tex2D(_Tex3, IN.worldPos.zy * scale);
            
            fixed3 n1 = UnpackNormal(tex2D(_Nmap1, IN.worldPos.xz * nscale));
            fixed3 n2 = UnpackNormal(tex2D(_Nmap2, IN.worldPos.xy * nscale));
            fixed3 n3 = UnpackNormal(tex2D(_Nmap3, IN.worldPos.zy * nscale));

            fixed3 nWNormal = normalize(IN.norm * fixed3(_X, _Y, _Z));
            fixed3 projnormal = saturate(pow(nWNormal*1.5, 4));

            fixed3 n = (1 - projnormal.x) * ((1 - projnormal.z) * n1 + (projnormal.z) * n2) + (projnormal.x) * n3;
            half4 blendedColor = (1 - projnormal.x) * ((1 - projnormal.z) * c1 + (projnormal.z) * c2) + (projnormal.x) * c3;

            o.Albedo = blendedColor.rgb;
            o.Alpha = blendedColor.a;
            o.Normal = n;

            half rim = 1.0 - saturate( dot( normalize(IN.viewDir), n ) );
            o.Emission = _RimColor.rgb * pow( rim, _RimPower );
        }

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.norm = normalize(v.normal);
        }

        ENDCG
    }

    Fallback "Diffuse"
}