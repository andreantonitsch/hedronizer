#ifndef HLSL_TEST_SDF
#define HLSL_TEST_SDF


uniform float _Time;

// float3 opTwist(float3 p, float intensity)
// {
//     float k = intensity; // or some other amount
//     float c = cos(k*p.z);
//     float s = sin(k*p.z);
//     float2x2  m = {c,-s,s,c};
//     float3  q = float3(mul(m, p.xy),p.z);
//     return q;
// }

float opSmoothUnion( float d1, float d2, float k )
{
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) - k*h*(1.0-h);
}

float sdTorus( float3 p, float2 t )
{
  float2 q = float2(length(p.xy)-t.x,p.z);
  return length(q)-t.y;
}

float sample_sdf(float3 position, float3 origin){
    
    float3 p = position + origin.xyz ;
    float t = sdTorus(p - float3(4.0, 4.0, 4.0), float2 (0.9, 0.3));
    float s =  length(p -  float3(4.0 + sin(_Time), 5.0, 4.0)) - 0.5; 
    return opSmoothUnion(t, s, 0.3 + sin(_Time  * 0.2));
}

// float sample_sdf(float3 position){

//     float3 p = position + origin.xyz - float3(4.0, 4.0, 4.0);
//     p = opTwist(p, sin(_Time) * 20.0);
//     float s = sdTorus(p, float2 (1.0, 0.3));
//     // sphere at origin with radius 10
//     return  s;
// }

// float sample_sdf(float3 position){

//     float d1 =  length(position + origin.xyz -  float3(4.0, 4.0, 4.0)) - 1.5; 
//     float d2 =  length(position + origin.xyz -  float3(4.0 + sin(_Time), 5.0, 4.0)) - 1.5; // * abs(sin(_Time)) -0.5; 
//     float d3 =  length(position + origin.xyz -  float3(4.0, 6.25, 4.0)) - 0.5; 
//     // sphere at origin with radius 10
//     return   min(d1, min(d3,d2));
// }




#endif
