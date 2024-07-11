Shader "Custom/TerrainShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
   }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma target 5.0
            #include "UnityCG.cginc"


            struct Vertex {
                float4 position;
                float4 normal;
                float4 color;
                //float4 v1;
            
            };
            struct Facet{
                Vertex vertices[3];
            };
    

            #if !defined(SHADER_TARGET_SURFACE_ANALYSIS)
            //#ifdef SHADER_API_D3D11		
            // has position, normals and color for each vertex
            // indexed with  vertex id * 3 + 0, vertex id * 3 + 1, vertex id * 3 + 2
           uniform StructuredBuffer<Facet> _Data;
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint id : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4x4 _ObjectToWorld;

            v2f vert (appdata v)
            {
                v2f o;

                #if !defined(SHADER_TARGET_SURFACE_ANALYSIS)
                    Facet facet = _Data[v.id / 3];
                    Vertex vertex = facet.vertices[v.id % 3];
                    v.vertex.xyzw = float4(vertex.position.xyz, 1.0);
                    o.normal = vertex.normal;
                #endif
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex = mul(UNITY_MATRIX_VP, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.normal.xyz =  vertex.normal;
                //d.color.xyzw = fixed4(vertex.color);  s


                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
