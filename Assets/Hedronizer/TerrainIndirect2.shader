Shader "Instanced/TerrainShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {

        Pass {

            Tags {"LightMode"="ForwardBase"}

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            //#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma target 4.5

            #include "UnityCG.cginc"
            //#include "UnityLightingCommon.cginc"
            //#include "AutoLight.cginc"

            sampler2D _MainTex;
            float4x4 _ObjectToWorld;

            #if SHADER_TARGET >= 45
    
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
    
           #endif

            struct appdata{
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                fixed4 color : COLOR;
                uint id : SV_VertexID;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
                //float3 ambient : TEXCOORD1;
                //float3 diffuse : TEXCOORD2;
                //float3 color : TEXCOORD3;
              //  SHADOW_COORDS(4)
            };


            v2f vert (appdata v, uint id : SV_VertexID)
            {
                v2f o;
            #if SHADER_TARGET >= 45
                Facet facet = _Data[id / 3];
                Vertex vertex = facet.vertices[id % 3];
                v.vertex.xyzw = float4(vertex.position.xyz, 1.0);
                v.normal = vertex.normal;
                v.color = vertex.color;
            #else
               v.vertex.xyzw = float4(0.0,0.0,0.0, 1.0);
           #endif


                float3 localPosition = v.vertex.xyz;
                float3 worldPosition = localPosition;
                float3 worldNormal = v.normal;

                //float3 ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                //float3 ambient = ShadeSH9(float4(worldNormal, 0.0f));
                //float3 diffuse = (ndotl * _LightColor0.rgb);
                //float3 color = v.color;

                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv_MainTex = v.texcoord;
                //o.ambient = ambient;
                //o.diffuse = diffuse;
                //o.color = color;
 
                //TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed shadow = SHADOW_ATTENUATION(i);
                fixed4 albedo = tex2D(_MainTex, i.uv_MainTex);
                //float3 lighting = i.diffuse * shadow + i.ambient;
                //fixed4 output = fixed4(albedo.rgb * i.color * lighting, albedo.w);
                //UNITY_APPLY_FOG(i.fogCoord, output);
                //return output;
                return albedo;
            }

            ENDHLSL
        }
    }
}