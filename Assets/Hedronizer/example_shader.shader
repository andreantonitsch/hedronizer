Shader "ExampleShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            struct Vertex {
                float4 position;
                float4 normal;
                float4 color;
                //float4 v1;
            
            };
            struct Facet{
                Vertex vertices[3];
            };

            StructuredBuffer<Facet> _Data;
            uniform float4x4 _ObjectToWorld;

            v2f vert(uint vertexID: SV_VertexID)
            {
                v2f o;
                Facet facet = _Data[vertexID / 3];
                Vertex vertex = facet.vertices[vertexID % 3.0];

                float4 pos = mul(_ObjectToWorld, vertex.position);

                o.pos = mul(UNITY_MATRIX_VP, pos);
                
                o.color= fixed4(vertex.color);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}