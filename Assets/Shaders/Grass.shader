Shader "LostCloud/Grass" {
	Properties {
		_GrassTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
		LOD 300
        
        // Draw double-sided
        //Cull Off
        //ZWrite Off
		//AlphaTest Greater 0.2

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _GrassTex;

		struct Input {
			float2 uv_GrassTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_GrassTex, IN.uv_GrassTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
