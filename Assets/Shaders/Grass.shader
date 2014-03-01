Shader "LostCloud/Grass" {
	Properties {
		_GrassTex ("Base (RGB)", 2D) = "white" {}
		_AlphaCuttoff("Alpha Cuttoff", Range(0.0,0.9)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="Transparent" }
		LOD 300
        
        // Draw double-sided
        Cull Off
        ZWrite On
        AlphaTest Greater [_AlphaCuttoff]
		
		CGPROGRAM
		#pragma surface GrassSurf GrassFrag vertex:GrassVert
		#pragma target 3.0

		sampler2D _GrassTex;

		struct Input {
			float2 uv_GrassTex;
		};
		
		half4 LightingGrassFrag( SurfaceOutput s, half3 lightDir, half atten ) { 
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ( atten * 2 );
            c.a = s.Alpha;
            
            return c;
        }

		void GrassSurf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_GrassTex, IN.uv_GrassTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		void GrassVert(inout appdata_full v) {
          v.normal = float3(0.0,1.0,0.0);
          
          if(v.texcoord.y > 0.9) {
          	v.vertex.x += (-0.30 + 0.55 * sin(20 * _Time.x));
          }
      	}
      	
		ENDCG
	} 
	FallBack "Diffuse"
}
