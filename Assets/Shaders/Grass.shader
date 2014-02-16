Shader "LostCloud/Grass" {
	Properties {
		_GrassTex ("Base (RGB)", 2D) = "white" {}
		_AlphaCuttoff("Alpha Cuttoff", Range(0.0,0.9)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
		LOD 300
        
        // Draw double-sided
        Cull Off
        ZWrite On
        AlphaTest Greater [_AlphaCuttoff]
		
		CGPROGRAM
		#pragma surface GrassVert GrassFrag
		#pragma target 3.0

		sampler2D _GrassTex;

		struct Input {
			float2 uv_GrassTex;
		};
		
		half4 LightingGrassFrag( SurfaceOutput s, half3 lightDir, half atten ) { 
            half4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            
            return c;
        }

		void GrassVert(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_GrassTex, IN.uv_GrassTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
