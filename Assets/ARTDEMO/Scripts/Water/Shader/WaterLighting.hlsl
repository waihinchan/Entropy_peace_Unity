#ifndef WATERLIGHTINGINCLUDE
#define WATERLIGHTINGINCLUDE
//Soft Shadows
float SoftShadows(float3 screenUV, float3 positionWS) 
//COPY from boatAttack. we don't care how this shadow looks like(although it will make a good effect)
//TODO write a note in shadow guide.
//simple note：
//by giving a jitter Noise map , and move the uv from this map servals time to sample the shadow map. and combine them with a weight.
{
    float2 jitterUV = screenUV.xy * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
	float shadowAttenuation = 0;
	
	uint loop = 4;
	float loopDiv = 1.0 / loop;
	for (uint i = 0u; i < loop; ++i)
    {
#ifndef _STATIC_WATER
        jitterUV += frac(float2(_Time.x, -_Time.z));
#endif
        float3 jitterTexture = SAMPLE_TEXTURE2D(_DitherPattern, sampler_DitherPattern, jitterUV + i * _ScreenParams.xy).xyz * 2 - 1;
	    float3 lightJitter = positionWS + jitterTexture.xzy * 2;
	    shadowAttenuation += SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, TransformWorldToShadowCoord(lightJitter));
	}
    return BEYOND_SHADOW_FAR(TransformWorldToShadowCoord(positionWS * 1.1)) ? 1.0 : shadowAttenuation * loopDiv;
}
float CalculateFresnelTerm(float3 normalWS, float3 viewDirectionWS)
{   
    
    return saturate(pow(1.0 - dot(normalWS, viewDirectionWS), 3)); 
    //simply get the viewdir and normal and return the fresnel.
}
float3 SampleReflections(float3 normalWS, float3 viewDirectionWS, float2 screenUV, float roughness)
{
    float3 reflection = 0;

    half2 reflectionUV = screenUV + normalWS.zx * _DebugRefelectionDistortion;
    // reflectionUV = screenUV;
    reflection += SAMPLE_TEXTURE2D_LOD(_PlanarReflectionTexture, sampler_ScreenTextures_linear_clamp, reflectionUV, 6 * roughness).rgb;//planar reflection
    
    
    return reflection;
}
#endif