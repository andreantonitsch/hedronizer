#ifndef HLSL_FACET_CONSUME_FUNCTION
#define HLSL_FACET_CONSUME_FUNCTION

struct Vertex {
    float4 position;
    float4 normal;
    //float4 color;
    //float4 v1;

};
struct Facet{
    Vertex vertices[3];
};

StructuredBuffer<Facet> _Data;

void GetFacetPosition_float(float vertexID, out float4 position){
    position = _Data[int(vertexID) / 3].vertices[int(vertexID) % 3].position;
}


void GetFacetNormalInterp_float(float vertexID, out float3 normal){
    normal.xyz = _Data[int(vertexID) / 3].vertices[int(vertexID) % 3].normal.xyz;
}

void GetFacetNormalFlat_float(float vertexID, out float3 normal){
    // normal.xyz = _Data[int(vertexID) / 3].vertices[int(vertexID) % 3].normal.xyz;
    normal.xyz = _Data[int(vertexID) / 3].vertices[ 0].normal.xyz;
    normal.xyz += _Data[int(vertexID) / 3].vertices[1].normal.xyz;
    normal.xyz += _Data[int(vertexID) / 3].vertices[2].normal.xyz;
    normal.xyz /= 3.0;
}

// void GetFacetColor_float(float vertexID, out float3 color){
//     color = _Data[int(vertexID) / 3].vertices[int(vertexID) % 3].color;
// }

#endif
