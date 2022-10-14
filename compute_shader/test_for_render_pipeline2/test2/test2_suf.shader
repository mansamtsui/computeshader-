Shader "test_compute_for_pipeline_2/test2222"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma instancing_options procedural:ConfigureProcedural
            #pragma editor_sync_compilation
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<float3> _Positions;
            #endif
            float2 _Scale;
            //在vert阶段前，会先调用这个方法，顶点运算用到的矩阵数据更新。相当于通过控制转换矩阵实现物体的顶点变换，从而渲染出变换的效果
            void ConfigureProcedural()
            {
                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float3 position = _Positions[unity_InstanceID];
                unity_ObjectToWorld = 0.0f;
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0f);
                unity_ObjectToWorld._m00_m11_m22 = _Scale.x;

                unity_WorldToObject = 0.0f;
                unity_WorldToObject._m03_m13_m23_m33 = float4(-position, 1.0f);
                unity_WorldToObject._m00_m11_m22 = _Scale.y;
                #endif
            }
            struct Attributes
            {
                float4 positionOS   : POSITION;
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
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.color = float4(saturate(vertexInput.positionWS * 0.5f + 0.5f), 1);
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
    }
    FallBack "Diffuse"
}