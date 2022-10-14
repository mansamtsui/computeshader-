Shader "test_compute_for_pipeline/NoiseBall2"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Cull Off

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma instancing_options procedural:ConfigureProcedural
            #pragma editor_sync_compilation



            float4x4 _LocalToWorld;
            float4x4 _WorldToLocal;
            half4 _Color;

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<float4> _PositionBuffer;
            StructuredBuffer<float4> _NormalBuffer;

#endif


            void ConfigureProcedural()
            {
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                unity_ObjectToWorld = _LocalToWorld;
                unity_WorldToObject = _WorldToLocal;
#endif
            }


            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                uint vid : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                    // UNITY_VERTEX_OUTPUT_STEREO // VR、立体眼镜支持
            };
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);


#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                uint id = unity_InstanceID * 3 + input.vid;//?? 为什么是*3 再加   因为这里是在vert阶段获取instance id，在ConfigureProcedural获取就可以直接用unity_InstanceID
                input.vertex.xyz = _PositionBuffer[id].xyz;
                input.normal = _NormalBuffer[id].xyz;
#endif

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
                output.positionCS = vertexInput.positionCS;
                output.color = _Color;//float4(saturate(vertexInput.positionWS * 0.5f + 0.5f), 1);
                return output;
            }
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
            // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            return input.color;
            }
            ENDHLSL

        }


        /*
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"


        #if defined(INSTANCING_ON)// || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
            #define ENABLE_INSTANCING
        #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                uint vid : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)


            float4x4 _LocalToWorld;
            float4x4 _WorldToLocal;


#if defined(ENABLE_INSTANCING)

            StructuredBuffer<float4> _PositionBuffer;
            StructuredBuffer<float4> _NormalBuffer;

#endif


            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.

#if defined(ENABLE_INSTANCING)

               

                uint id = unity_InstanceID * 3 + v.vid;

                v.vertex.xyz = _PositionBuffer[id].xyz;
                v.normal = _NormalBuffer[id].xyz;
#endif

                o.vertex = mul(UNITY_MATRIX_VP, mul(_LocalToWorld, float4(v.vertex.xyz, 1.0)));// UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i); // necessary only if any instanced properties are going to be accessed in the fragment Shader.

                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            }
            ENDCG
        }
        */
    }
}
