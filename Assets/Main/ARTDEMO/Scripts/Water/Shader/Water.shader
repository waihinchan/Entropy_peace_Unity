Shader "Custom/Sea"
{
    Properties
    {   
        _Height("WaveHeight",Range(1,10)) = 1
        _DebugRefelectionDistortion("DebugRefelectionDistortion",Vector) = (0,0,0,0)
        _DitherPattern("DitherPattern",2D) = "Grey"{}
        _SurfaceMap("SurfaceMap",2D) = "White"{}
        _FoamMap("_FoamMap",2D) = "black"{}
        _CustomWaterDepth("CustomWaterDepth", Float) = 0
        _Gravity("Gravity", Float) = 9.8 //custom gravity
        _DebugColor1("DebugColor1",Color) = (1,1,1,1)
        _DebugColor2("DebugColor2",Color) = (1,1,1,1)
        _DebugWorldToScreenUVParam("DebugWorldToScreenUVParam",Float) = 0.015 
        //this stuff is for convert the worldposition to the screen uv.
        //if want a function, can do mul(matrix,worldpos)
        //matrix = preBakeDepthCam projection matrix;.
    }
    SubShader
    {
        Tags {"RenderType" = "Transparent" "Queue"="Transparent-100" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="4.5"}
        LOD 100

        ZWrite ON
        ZTest Less
        Blend SrcAlpha OneMinusSrcAlpha
        //todo : this canbe one zero,bacause alpha always 1. and all the refraction and reflection are in combine. so that we can make a fake transparent water.

        Pass
        {
            Name "SimpleWaterPass"

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            

            // Lightweight Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex WaterVertex
            #pragma fragment WaterFragment
            #include "Assets/Main/ARTDEMO/Scripts/Water/Shader/WaterLitForwardPass.hlsl"




            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    // CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}

