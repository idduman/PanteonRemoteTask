Shader "Custom/VertexColor"
{
    Properties
    {
            _Alpha("Alpha", range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Pass
        {
            ColorMaterial AmbientAndDiffuse
        }
    }
}
