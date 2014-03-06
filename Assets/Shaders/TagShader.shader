        // One shader, renders things white.
    Shader "Custom/RenderWhite" {
        SubShader {
            Tags { "RenderType"="TheTarget" "MyTag" = "TargetTag" }
            Pass {
                Fog { Mode Off }        
                Color (0,1,1,1)
            }
        }
    }
     
    