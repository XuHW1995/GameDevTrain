#ifndef X_UNLIT_PASS_INCLUDED
#define X_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/XCommon.hlsl"

float4 XUnlitPassVertex(float3 positionOS : POSITION) : SV_POSITION
{
    float3 positionWS = TransformObjectToWorld(positionOS);
    return TransformWorldToHClip(positionWS);   
}

CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor; 
CBUFFER_END

float4 XUnlitPassFragment() : SV_TARGET
{
    return _BaseColor;
}

#endif