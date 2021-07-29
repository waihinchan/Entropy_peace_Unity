#ifndef WATERLITFORWARDPASSINCLUDE
#define WATERLITFORWARDPASSINCLUDE
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
#include "Assets/Main/ARTDEMO/Scripts/Water/Shader/WaterVariables.hlsl"
#include "Assets/Main/ARTDEMO/Scripts/Water/Shader/GerstnerWave.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/Main/ARTDEMO/Scripts/Water/Shader/WaterLighting.hlsl"

struct WaterVertexInput // vert struct
{
    float4	vertex 					: POSITION;		// vertex positions
	float2	texcoord 				: TEXCOORD0;	// local UVs
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct WaterVertexOutput // fragment struct
{   
    float4	uv 						: TEXCOORD0;	// Geometric UVs stored in xy, and world(pre-waves) in zw
    float3	posWS					: TEXCOORD1;	// world position of the vertices
    float3 	normal 					: NORMAL;		// vert normals
    float4	shadowCoord				: TEXCOORD2;	// for ssshadows
    float3 	viewDir 				: TEXCOORD3;	// view direction
    float3	preWaveSP 				: TEXCOORD4;	// screen position of the verticies before wave distortion
	float4	clipPos					: SV_POSITION;
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};
float WaterTextureDepth(float3 worldPos){ //Vertical depth
    float depth =1 - SAMPLE_TEXTURE2D_LOD(_WaterDepthMap,sampler_WaterDepthMap_linear_clamp,worldPos.xz * _DebugWorldToScreenUVParam + 0.5, 0).r; 
    //DX11\12 is 1 - 0, mean 1 is near, 0 is far.
    // one minus mean more deep the value more greater.
    return clamp((depth * (_MaxDepth + _DepthCamParams.x) - _DepthCamParams.x),0,_MaxDepth);
}

WaterVertexOutput WaveVertexOperations(WaterVertexOutput input){
    float time = _Time.y;
    // time = 0;
    input.normal = float3(0,1,0); //init normal. a plane's normal always to a UP vector

    // input.fogFactorNoise.y = ( ( noise( (input.posWS.xz * 0.5) + time) + noise( ( input.posWS.xz * 1) + time)) * 0.25 - 0.5) + 1; 
    //they use a fog, so far we don't add this

    //the details uv.
    // noise is from another package. we use keijiro sama ' package
    float noise = ClassicNoise(input.posWS.xz * 1.0f + time);
    input.uv.xy = input.posWS.xz * 0.2 +  noise * 0.2 + time * 0.01f; //just keep things simple!
    input.uv.zw = input.posWS.xz * 0.15 +  noise * 0.1 - time * 0.02f; //the other direction

    	// Detail UVs


    float4 screenUV = ComputeScreenPos(TransformWorldToHClip(input.posWS));//ATTENTION: so far this uv is before deformation.
	screenUV.xyz /= screenUV.w; //Homogeneous division manually

    // // shallows mask
    float waterDepth = WaterTextureDepth(input.posWS); 
    //this return a value from 0 - 1 pointing the how depp from water to ground
    input.posWS.y += pow(saturate((-waterDepth + 1.5) * 0.4), 2); 
    //the shallow place, the water higher.

    WaveStruct finalWave = SimpleGerstnerWave(input.posWS); //sigma all the "Wave"
    input.posWS = finalWave.position + (step(1,_Height) * _Height) / 2; //TODO: the deeper place should have more wave.
    input.normal = normalize(cross(finalWave.bitangent,finalWave.tangent));
    //basically the same . the different is how to get the Wave. if we can use a compute Shader!?
    //also they use a depth to control the wave. we need more observation to modify it.
#ifdef SHADER_API_PS4
    input.posWS.y -= 0.5;
#endif
	// float4 waterFX = SAMPLE_TEXTURE2D_LOD(_WaterFXMap, sampler_ScreenTextures_linear_clamp, screenUV.xy, 0);
	// input.posWS.y += waterFX.w * 2 - 1; //sofar we don't have this stuff.. don't know what it's. but not effect the result much.
    input.clipPos = TransformWorldToHClip(input.posWS );
	input.shadowCoord = ComputeScreenPos(input.clipPos);
    input.viewDir = SafeNormalize(_WorldSpaceCameraPos - input.posWS); 
    input.preWaveSP = screenUV.xyz; // pre-displaced screenUVs

    float distanceBlend = saturate(abs(length((_WorldSpaceCameraPos.xz - input.posWS.xz) * 0.0005)) - 0.25); //we can just use this params to blend the far water normal
	input.normal = lerp(input.normal, float3(0, 1, 0), distanceBlend);
    return input;
}
WaterVertexOutput WaterVertex(WaterVertexInput v)
{   
    //init
    WaterVertexOutput o = (WaterVertexOutput)0;
	UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    //init
    o.uv.xy = v.texcoord; //get the uv.
    o.posWS = TransformObjectToWorld(v.vertex.xyz); //object space to world space vertex. //all the caculate start at in the world space.
	o = WaveVertexOperations(o);
    return o;
}

float GetDpethDifference(float4 screenPosition,float2 uv){ //this is from URP tutorial.
    float rawD = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_ScreenTextures_linear_clamp, uv); //这个是原始深度
    float d = LinearEyeDepth(rawD,_ZBufferParams); 
    float currentDepth = d;
    return d - (screenPosition.w + _CustomWaterDepth);

}
float2 DistortionUVs(float depth, float3 normalWS)
{
    float3 viewNormal = mul((float3x3)GetWorldToHClipMatrix(), -normalWS).xyz; //use the normal ws to clip space ws

    return viewNormal.xz * saturate((depth) * 0.005); //the more depth the distor harder!
}
float3 Scattering(float depth)
{
	return SAMPLE_TEXTURE2D(_AbsorptionScatteringRamp, sampler_AbsorptionScatteringRamp, float2(depth, 0.33)).rgb;
}
float3 Absorption(float depth)
{
	return SAMPLE_TEXTURE2D(_AbsorptionScatteringRamp, sampler_AbsorptionScatteringRamp, float2(depth, 0.0h)).rgb;
}
float3 Refraction(float2 distortion, float depth)
{
	float3 output = SAMPLE_TEXTURE2D_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, distortion, depth * 0.25).rgb;
    //base on the depth to determin the LOD
	output *= Absorption(depth);
	return output;
}
float4 WaterFragment(WaterVertexOutput IN) : SV_Target{
    UNITY_SETUP_INSTANCE_ID(IN);
    float3 screenUV = IN.shadowCoord.xyz / IN.shadowCoord.w;//displacement screen UVs
    // half4 waterFX = SAMPLE_TEXTURE2D(_WaterFXMap, sampler_ScreenTextures_linear_clamp, IN.preWaveSP.xy); //custom render pass, add it in future.

    float3 positionVS = TransformWorldToView(IN.posWS);

    float depthDifference = GetDpethDifference(IN.clipPos,screenUV); //sofar we use a simple depth
    float verticalDepth = WaterTextureDepth(IN.posWS);
    IN.uv.xy = floor(IN.uv.xy * 50) / 50;
    IN.uv.zw = floor(IN.uv.xy * 50) / 50;
    float2 detailNormal1 = SAMPLE_TEXTURE2D(_SurfaceMap, sampler_SurfaceMap, IN.uv.xy).xy * 2 - 1;
	float2 detailNormal2 = SAMPLE_TEXTURE2D(_SurfaceMap, sampler_SurfaceMap, IN.uv.zw).xy * 2 - 1;
	float2 detailNormal = (detailNormal1 + detailNormal2 * 0.5) * verticalDepth; //the deeper water have more wave normal
    IN.normal += float3(detailNormal.x, 0, detailNormal.y) * 0.1;
	// IN.normal += float3(1-waterFX.y, 0.5h, 1-waterFX.z) - 0.5;
	IN.normal = normalize(IN.normal);

    
	Light mainLight = GetMainLight(TransformWorldToShadowCoord(IN.posWS));
    float shadow = SoftShadows(screenUV, IN.posWS); //CUSTOM FUNCTION from WaterLighting
    float3 GI = SampleSH(IN.normal);

    // FAKE SSS
    float3 directLighting = dot(mainLight.direction, float3(0, 1, 0)) * mainLight.color;
    // directLighting += saturate(pow(dot(IN.viewDir, -mainLight.direction) * IN.additionalData.z, 3)) * 5 * mainLight.color;
    directLighting += saturate( pow( dot(IN.viewDir, -mainLight.direction), 3)) * 5 * mainLight.color; 
    //they use a wave height to scale the hight light
    //this is kind of like a the more height of the wave the more hight light it get. so far we skip this.
    float3 sss = directLighting * shadow + GI;
    // FAKE SSS
    
    // Foam
	float3 foamMap = SAMPLE_TEXTURE2D(_FoamMap, sampler_FoamMap,  IN.uv.zw).rgb; //r=thick, g=medium, b=light 
    //use the noise uv to sample some foam
    //TODO add this when we support multi depth
	float depthEdge = saturate(depthDifference * 10);  //rescale the depth difference
	float depthAdd = saturate(1 - depthDifference * 4) * 0.5;
	float edgeFoam = saturate((1 - min(depthDifference, verticalDepth) * 0.5 - 0.25) + depthAdd) * 1;
    // edgeFoam = (depthDifference) * (1 - verticalDepth) + verticalDepth * (1 - depthDifference);
    
    // compare here is the shallow or edge. if here is edge, the depthDifference should be less than verticalDepth
    // (because the object in the middle of water should have some verticalDepth)
    // if the object is the shallow, if there is no object on the shallow, the verticalDepth should less than the depth difference 
    // because all the depth object are other the water
    // if there is a object in the shallow. not only the shallow should has foam, but alos the object;
    // so if we use a verticalDepth it will make everything foam. so we multi a depthDifference, so that the object at shallow has foam too.
    // (if no object the depth difference should be very small)
	float waveFoam = saturate(IN.posWS.y   * 0.5); // wave tips //the higher wave has more wave foam.
	// float foamBlendMask = max(max(waveFoam, edgeFoam), waterFX.r * 2); //TODO ADD waterFX to render
    float foamBlendMask = max(waveFoam, edgeFoam); //
    // foamBlendMask = edgeFoam;
    // compare if here is a wave. so kind of like the waveFoam will be very big. so that it can figure out if here is the wave or object or shallow
	float3 foamBlend = SAMPLE_TEXTURE2D(_AbsorptionScatteringRamp, sampler_AbsorptionScatteringRamp, float2(foamBlendMask, 0.70)).rgb;
    // float3 foamBlend = SAMPLE_TEXTURE2D(_AbsorptionScatteringRamp, sampler_AbsorptionScatteringRamp, float2(depthDifference, 0.70)).rgb; //the stuff i used before
	float foamMask = saturate(length(foamMap * foamBlend) * 2 );
	// Foam lighting
	float foam = foamMask.xxx * (mainLight.shadowAttenuation * mainLight.color + GI);

	// Distortion for refraction
	float2 distortion = DistortionUVs(depthDifference, IN.normal); //so far we use a depth difference
	distortion = screenUV.xy + distortion;// use the wave displacement uv + noise
    distortion = floor(distortion * 100) / 100;
    // we make things simple!
	// float d = depth.x;
	// depth.xz = AdjustedDepth(distortion, IN.additionalData);
	// distortion = depth.x < 0 ? screenUV.xy : distortion;
	// depth.x = depth.x < 0 ? d : depth.x;
    // we make things simple!


    // Fresnel
	float fresnelTerm = saturate( CalculateFresnelTerm( IN.normal, SafeNormalize(_WorldSpaceCameraPos - IN.posWS)));
    // float fresnelTerm = CalculateFresnelTerm(float3(0,1,0), IN.viewDir.xyz);
	//return fresnelTerm.xxxx;

    BRDFData brdfData;
    float alpha = 1;
    InitializeBRDFData(float3(0, 0, 0), 0, float3(1, 1, 1), 0.95, alpha, brdfData);
    //albedo = 0, metalic = 0, specular = 1, smoothness =0.95,alpha,brdfData
	float3 spec = DirectBDRF(brdfData, IN.normal, mainLight.direction, IN.viewDir) * shadow * mainLight.color; 
    //default specularHighlightsOff is false
    //actually this is diffuse..
    //but like albedo is zero..so only we have the spec term
#ifdef _ADDITIONAL_LIGHTS
    uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, IN.posWS);
        spec += LightingPhysicallyBased(brdfData, light, IN.normal, IN.viewDir); //if this too bright?
        sss += light.distanceAttenuation * light.color;
    }
#endif
    sss *= Scattering(depthDifference * 1/ _MaxDepth);
    // Reflections
    screenUV = floor(screenUV * 100) / 100;
	float3 reflection = SampleReflections(IN.normal, IN.viewDir.xyz, screenUV.xy, 0.0);

	// Refraction
	float3 refraction = Refraction(distortion, depthDifference * 1 / _MaxDepth);
    float3 comp = lerp( lerp( refraction, reflection, fresnelTerm)  + sss + spec, foam, foamMask);
    // float ttt = SAMPLE_TEXTURE2D_LOD(_WaterDepthMap,sampler_WaterDepthMap_linear_clamp,float2(1,1) * 2 + 0.5, 0).r;  //just want to check the water depth map
    return saturate(float4(comp,1));
}






























#endif