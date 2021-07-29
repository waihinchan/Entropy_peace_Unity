#ifndef GERSTMERWAVEINCLUDE
#define GERSTMERWAVEINCLUDE

struct WaveStruct
{
	float3 position;
	float3 tangent;
    float3 bitangent;
};
//basically from https://developer.nvidia.com/gpugems/gpugems/part-i-natural-effects/chapter-1-effective-water-simulation-physical-models
struct Wave 
{
	// float amplitude;
    float steepness;
	float2 direction; //they seems to use a radius as a direction. make sense!
	float wavelength;
    // float speed;
	// half2 origin; //this is for circle wave?
	// half omni;
};

// #if defined(USE_STRUCTURED_BUFFER) //struct buffer kind of allow us pack a stuct into the buffer.
// StructuredBuffer<Wave> _WaveDataBuffer;
// #else
// half4 waveData[20]; //kind of stroge all the data into this stuff. so far we do it manually
// #endif
float getWaveDirection(float2 direction,float2 planePos){ //use the xz to get the rotate
    float2 d = normalize(direction);
    return dot(d,planePos);
}
float3 GerstnerWave(Wave wave, float2 worldPos,inout float3 tangent,inout float3 bitangent){
    float graivty = 9.8;
    float steepness = wave.steepness;
    float2 direction = wave.direction;
    float L = _Height * wave.wavelength;
    float k = 2 * WATER_PI_TWO_PI / L;
    float c = sqrt( _Gravity / k);
    float amplitude = steepness / k;
    float time = _Time.y;
    // time = 0;
    float moveParam = k * (getWaveDirection(direction,worldPos) - time * c );  // k * (dx * x + dz * z - _Time.y *c);
    float2 d = normalize(direction);
    float x = d.x * amplitude * cos(moveParam);
    float z = d.y * amplitude * cos(moveParam); //y is z
    float y = amplitude * sin(moveParam);
    //caculate the Partial Derivative
    //for example:
    // float dx_moveParamTerm = k * d.x;
    // float dz_moveParamTerm = k * d.z; 
    //x: -d.x * (steepness / k) * sin(moveParam) * dx_moveParamTerm = -d.x * d.x * steepness * sin(moveParam)
    //y: -(steepness / k) * cos(moveParam) * dx_moveParamTerm = steepness * d.x * cos(moveParam)  
    //z: -d.y * (steepness / k) * sin(moveParam) * dx_moveParamTerm = -d.y * d.x * steepness * sin(moveParam)
    //caculate the Partial Derivative
    //we can merage the term and make it simple;
    tangent   += float3(-d.x * d.x * steepness * sin(moveParam), steepness * d.x * cos(moveParam), -d.x * d.y * steepness * sin(moveParam));
    bitangent += float3(-d.y * d.x * steepness * sin(moveParam), steepness * d.y * cos(moveParam), -d.y * d.y * steepness * sin(moveParam));
    float3 returnPos = float3(x,y,z);
    return returnPos;

}
inline WaveStruct SimpleGerstnerWave(float3 worldPos){
    float3 pos = worldPos;
    //temp hard coding here
    Wave waveA = (Wave)0;
    Wave waveB = (Wave)0;
    Wave waveC = (Wave)0;
    Wave waveD = (Wave)0;
    waveA.steepness = 0.25;
    waveA.direction = float2(3,2);
    waveA.wavelength = 20;
    waveB.steepness = 0.15;
    waveB.direction = float2(5,3);
    waveB.wavelength = 10;
    waveC.steepness = 0.1;
    waveC.direction = float2(4,-3);
    waveC.wavelength = 30;
    waveD.steepness = 0.3;
    waveD.direction = float2(-2,1);
    waveD.wavelength = 5;
    //temp hard coding here
    WaveStruct resultWave = (WaveStruct)0;
    float3 tangent = float3(1,0,0);
    float3 bitangent= float3(0,0,1);
    pos += GerstnerWave(waveA,worldPos.xz,tangent,bitangent);
    pos += GerstnerWave(waveB,worldPos.xz,tangent,bitangent);
    pos += GerstnerWave(waveC,worldPos.xz,tangent,bitangent);
    pos += GerstnerWave(waveD,worldPos.xz,tangent,bitangent);
    resultWave.position = pos;
    
    resultWave.tangent = tangent;
    resultWave.bitangent = bitangent;
    return resultWave;
}
#endif