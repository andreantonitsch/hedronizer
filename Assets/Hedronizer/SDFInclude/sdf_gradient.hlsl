#ifndef HLSL_SDF_GRADIENT
#define HLSL_SDF_GRADIENT

#include "Assets/Hedronizer/SDFInclude/hedronizer_variables.hlsl"
#include "Assets/Hedronizer/SDFInclude/texture_threedee_sdf.hlsl"

float3 sample_sdf_gradient(float3 position){

    float h = _GradientH;

    float f = sample_sdf(position);
    float3 deltas = float3(sample_sdf(position + float3(h, 0.0, 0.0)) - f,
                           sample_sdf(position + float3(0.0, h, 0.0)) - f,
                           sample_sdf(position + float3(0.0, 0.0, h)) - f
     );

    return normalize(deltas / h);

}

float4 sample_sdf_gradient(float3 position, float radius){

    float h = _GradientH;

    float f = sample_sdf(position) - radius;
    float3 deltas = float3(sample_sdf(position + float3(h, 0.0, 0.0)) - radius - f ,
                           sample_sdf(position + float3(0.0, h, 0.0)) - radius - f ,
                           sample_sdf(position + float3(0.0, 0.0, h)) - radius - f 
     );

    return float4(normalize(deltas / h), f);

}


#endif
