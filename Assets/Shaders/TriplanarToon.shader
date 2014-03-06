Shader "LostCloud/TriplanarToon"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        
        _TexXZ ("Ground (XZ Plane)", 2D) = "white" {}
        _TexXY ("Wall 1 (XY Plane)", 2D) = "white" {}
        _TexZY ("Wall 2 (ZY Plane)", 2D) = "white" {}
		
        _TileU("TileU", Float) = 20
        _TileV("TileV", Float) = 20
        
        _NX("NX", Range(0,1)) = 1
        _NY("NY", Range(0,1)) = 1
        _NZ("NZ", Range(0,1)) = 1
        
		_Ramp( "Ramp", 2D ) = "gray" {}
		_RimColor( "Rim Color", Color ) = ( 0.26, 0.19, 0.16, 0.0 )
		_RimPower( "Rim Power", Range( 0.5, 8.0 ) ) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
            
        LOD 300

        CGPROGRAM
        #pragma surface SurfMain TriplanarRamp

        struct Input
        {
            half3 worldPos;
            float3 viewDir;
            fixed3 worldNormal;
        };

        sampler2D _TexXZ;
        sampler2D _TexXY;
        sampler2D _TexZY;
        
        sampler2D _Ramp;

		float _RimPower;

        fixed4 _Color;
		float4 _RimColor;

        fixed _NX;
        fixed _NY;
        fixed _NZ;
        
        half _TileU;
        half _TileV;        

		half4 LightingTriplanarRamp( SurfaceOutput s, half3 lightDir, half atten ) {
            // Standard Lambertian Diffuse Term
			half NdotL = dot( s.Normal, lightDir );

			// Scale diffuse term from [-1,1] -> [0,1]
			half diff = NdotL * 0.5 + 0.5;

			// Rim Color gradient lookup
			half3 ramp = tex2D( _Ramp, float2(diff)).rgb;

            // Calculate final color
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * ( atten * 2 );
			c.a = s.Alpha;
			
			return c;
		}

        void SurfMain(Input IN, inout SurfaceOutput o)
        {
        	// Tiling
            half2 scale = half2(_TileU, _TileV);

            // Color Lookups
            fixed4 c = tex2D(_TexXZ, IN.worldPos.xz/scale);
            fixed4 c1 = tex2D(_TexXY, IN.worldPos.xy/scale);
            fixed4 c2 = tex2D(_TexZY, IN.worldPos.zy/scale);
			
            // Transform normal into space defined by basis vectors NX, NY, NZ
            fixed3 nWNormal = normalize(IN.worldNormal * fixed3(_NX, _NY, _NZ));

            // Normal projected for for interpolation between colors
            fixed3 projnormal = saturate(pow(nWNormal*1.5, 4));

            // Interpolate between the texels
            half4 result = lerp(c, c1, projnormal.z);
            result = lerp(result, c2, projnormal.x);

            // Modulate texture color with base color
			o.Albedo = result.rgb * _Color.rgb;
            o.Alpha = result.a * _Color.a;
			
            // V . N = 0 at edges of object (ie cos() = 0) 
			half rim = 1.0 - saturate( dot( normalize(IN.viewDir), IN.worldNormal ) );

			// Emmisive color, applied before light integration
			o.Emission = _RimColor.rgb * pow( rim, _RimPower );
        }
        
        ENDCG
    }

    Fallback "Diffuse"
}