
    var dif : Shader;
    
    function Awake (){
       camera.SetReplacementShader(dif,"RenderType");
    }
    
    function OnDestroy (){
       camera.ResetReplacementShader();
    }