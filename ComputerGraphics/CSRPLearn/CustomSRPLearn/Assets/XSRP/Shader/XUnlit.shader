Shader "XShader/XUnlit"
{
    Properties
    {
        _BaseColor("BaseColor", color) = (1.0, 1.0, 1.0, 1.0)
    }
    
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit"}
            HLSLPROGRAM

            #pragma multi_compile_instancing
            #pragma vertex XUnlitPassVertex
            #pragma fragment XUnlitPassFragment
            #pragma enable_d3d11_debug_symbols
            
            #include "XUnlitPass.hlsl"
            ENDHLSL
        }
    }
}