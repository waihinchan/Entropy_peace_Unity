#ifndef WATERVARIABLESINCLUDE
#define WATERVARIABLESINCLUDE
TEXTURE2D(_WaterDepthMap); SAMPLER(sampler_WaterDepthMap_linear_clamp);
float _MaxDepth;
float _DebugWorldToScreenUVParam;
float4 _DepthCamParams;
float _Height;
float2 _DebugRefelectionDistortion;
#define WATER_PI            3.14159265359f
#define WATER_PI_TWO_PI        6.28318530718f 
//in case of UNITY_PI not avaliable
float _Gravity;
float _CustomWaterDepth;
float4 _DebugColor1;
float4 _DebugColor2;
SAMPLER(sampler_ScreenTextures_linear_clamp);
TEXTURE2D(_SurfaceMap); SAMPLER(sampler_SurfaceMap);
TEXTURE2D(_CameraDepthTexture);
float4 _DitherPattern_TexelSize;//Vector4(1 / width, 1 / height, width, height) this is set by unity
TEXTURE2D(_DitherPattern); SAMPLER(sampler_DitherPattern);
TEXTURE2D(_FoamMap); SAMPLER(sampler_FoamMap);
TEXTURE2D(_AbsorptionScatteringRamp); SAMPLER(sampler_AbsorptionScatteringRamp);
TEXTURE2D(_PlanarReflectionTexture);
TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture_linear_clamp);
#endif