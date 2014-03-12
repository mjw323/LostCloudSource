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
                Color (0,0,1,1) // Render it red rather than white!
            }
        }
        SubShader {
            Tags { "RenderType"="TransparentCutout" "Player" = "TargetTag" }
            Pass {
                Fog { Mode Off }
                Color (1,0,0,1) // Render it red rather than white!
				SetTexture [__RenderTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}
            }
        }
       
        SubShader {
            Tags { "RenderType"="RenderWhite" "Player" = "MyTagBlack" }
            Pass {
                Fog { Mode Off }
                Color (0,1,0,1) // Render it green rather than black!
            }
        }
    }