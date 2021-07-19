Shader "XHW/HelloShader"
{
    Properties
    {
        
        [Header(hahahahhha)]_MyColor("TestColorAttr", Color) = (1, 1, 1, 1)
        [HideInInspector]_MyInt("TestIntAttr", int) = 1
        _MyFloat("TestFloatAttr", float) = 0.3
        _MyRange("TestRangeAttr", range(0, 3)) = 0.2
        [IntRange]_MyIntRange("TestIntRange", range(0, 3)) = 0.2
        [PowerSlider(10)]_MyPowerRange("TestPowerSlider", range(0, 3)) = 0.2
        [Toggle]_MyToggle("TestToggleAttr", int) = 1
        [Enum(UnityEngine.Rendering.CullMode)]_MyEnum("TestEnumAttr", int) = 1

        _MyVector4("TestVector4Attr", vector) = (1,2,3,4)
        _MyTexture2D("Test2dTexture", 2D) = "white"{}
        [Normal]_MyNormalMap2D("TestNormalMap", 2D) = "gray"{}
        _MyTexture3d("Test3dTexture", 3d) = ""{}
        _MyCubeMapTexture("TestCubeMap", cube) = ""{}

    }
    //123
    SubShader
    {
        Pass
        {
            CGPROGRAM
            //顶点着色函数声明
            #pragma vertex vert
            #pragma fragment frag

            struct appData
            {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                                float2 uv:TEXCOORD;
            };
            
            v2f vert(appData v)
            {
                v2f fData;
                fData.pos = UnityObjectToClipPos(v.vertex);
                fData.uv = v.uv;
                return fData;
            }

            fixed checker(float2 uv)
            {
                float2 repeatUV = uv * 10;
                float2 c = floor(repeatUV)/2;
                float checker = frac(c.x + c.y) * 2;
                return checker;
            }
            
            fixed4 _MyColor;
            float4 frag(v2f i):SV_Target
            {
                return checker(i.uv);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "EditorName"
}
