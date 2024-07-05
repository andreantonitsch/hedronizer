#ifndef HLSL_TEXTURE_THREEDEE_SDF
#define HLSL_TEXTURE_THREEDEE_SDF


#include "./SDFInclude/hedronizer_variables.hlsl"

Texture3D<float> _SDF;
SamplerState sampler_SDF;

float sample_sdf(float3 position){
    

    // float d = _SDF.Load(((position - _Origin).xzy) / _Size);
    // float d = _SDF[uint3(position - _Origin).xzy];
    float d = _SDF.SampleLevel(sampler_SDF, (position).xzy / _Size, 0) * _Size;
    return d - _Isovalue;

}

#endif
