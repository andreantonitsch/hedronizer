Shader "Custom/DataShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members normal,vertex)
//#pragma exclude_renderers d3d11

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 5.0

        struct Vertex {
            float4 position;
            float4 normal;
            float4 color;
            //float4 v1;
        
        };
        struct Facet{
            Vertex vertices[3];
        };

        sampler2D _MainTex;

        struct Input
        {
            fixed4 color;
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        uniform float4x4 _ObjectToWorld;

        #if !defined(SHADER_TARGET_SURFACE_ANALYSIS)
        //#ifdef SHADER_API_D3D11		
        // has position, normals and color for each vertex
        // indexed with  vertex id * 3 + 0, vertex id * 3 + 1, vertex id * 3 + 2
        StructuredBuffer<Facet> _Data;
        #else
        #endif

        struct appdata{
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            // float4 texcoord1 : TEXCOORD1;
            // float4 texcoord2 : TEXCOORD2;
            //float4 tangent : TANGENT;
            //fixed4 color : COLOR;
            uint id : SV_VertexID;
        };

        
        void setup()
        {
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            unity_ObjectToWorld = _ObjectToWorld;
            // unity_WorldToObject = unity_ObjectToWorld;
        #endif
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata v, out Input d) {
            UNITY_INITIALIZE_OUTPUT(Input, d);
            
            #if !defined(SHADER_TARGET_SURFACE_ANALYSIS)
            //#ifdef SHADER_API_D3D11
                Facet facet = _Data[v.id / 3];
                Vertex vertex = facet.vertices[v.id %3];
                v.vertex.xyzw = vertex.position;
                v.normal.xyz =  vertex.normal;
                d.color.xyzw = fixed4(vertex.color);    
            #else
                v.vertex.xyzw = float4(v.id * 0.1, v.id  * 0.2, v.id  * 0.3, 1.0);
                v.normal.xyz =  float3(0.0, 1.0, 0.0);
                d.color.xyzw = fixed4(_Color.xyz, 1.0); 
            #endif

        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            ///fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = IN.color.xyz;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
