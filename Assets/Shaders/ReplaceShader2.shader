        // Replacement shader, any shader in your scene that has a matching MyTag to one of these subshaders will use the subshader defined here.
    // In this case;
    // It'll render anything with MyTag RenderWhite in red.
    // And anything with MyTag RenderBlack in green.
    // And because there isn't a subshader with MyTag RenderGrey, it won't render that material at all in this render pass.
    Shader "Custom2/ReplacementShader" {
        SubShader {
            Tags { "RenderType"="Opaque" "Player" = "Default" }
            Pass {
                Fog { Mode Off }
                Color (0,0,1,1)
            }
        }
        SubShader {
            Tags { "RenderType"="TransparentCutout" "Player" = "TargetTag" }
            Pass {
                Fog { Mode Off }
                Color (1,0,0,1)
            }

            Pass {
                Blend One One
                BlendOp Add

                CGPROGRAM

                #include "UnityCG.cginc"

                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #pragma glsl
                #pragma fragmentoption ARB_precision_hint_fastest

                struct v2f {
                    float4 position : POSITION;
                    float4 color : COLOR;
                };

                sampler2D _MainTex;

                half4 frag( v2f IN ) : COLOR {


                    return IN.color;
                }

                v2f vert(appdata_full v) {
                    v2f o = (v2f)0;

                    o.position = mul(UNITY_MATRIX_MVP,v.vertex);
                    o.color = tex2D(_MainTex,float2(1.0,0.0));

                    return o;
                }

                ENDCG
            }
        }
       
        SubShader {
            Tags { "RenderType"="RenderWhite" "Player" = "MyTagBlack" }
            Pass {
                Fog { Mode Off }
                Color (0,1,0,1)
            }
        }
    }