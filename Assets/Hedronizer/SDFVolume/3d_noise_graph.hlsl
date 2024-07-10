#ifndef HLSL_THREEDEE_NOISE
#define HLSL_THREEDEE_NOISE

#include "noise.hlsl"


void  CubeNoise_float(float3 p, out float value){

    value =  snoise(p);
}

#endif
