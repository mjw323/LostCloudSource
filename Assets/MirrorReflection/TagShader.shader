        // One shader, renders things white.
    Shader "Custom/RenderWhite" {
        SubShader {
            Tags { "RenderType"="RenderWhite" "MyTag" = "MyTagWhite" }
            Pass {
                Fog { Mode Off }        
                Color (1,1,1,1)
            }
        }
    }
     
    