#ifndef HLSL_TEXTURE_THREEDEE_SDF
#define HLSL_TEXTURE_THREEDEE_SDF


#include "Assets/Hedronizer/SDFInclude/hedronizer_variables.hlsl"

Texture3D<float> _SDF;
SamplerState sampler_SDF;

float sample_sdf(float3 position){
    

    // float d = _SDF.Load(((position - _Origin).xzy) / _Scale);
    // float d = _SDF[uint3(position - _Origin).xzy];
    //normalizes world position according to _Scale (puts them in texture coordinates)
    // multiplies fetched distance back to world coordinates
    float d = _SDF.SampleLevel(sampler_SDF, (position).xyz / _Scale, 0) * _Scale;
    return d - _Isovalue;

}

#endif
