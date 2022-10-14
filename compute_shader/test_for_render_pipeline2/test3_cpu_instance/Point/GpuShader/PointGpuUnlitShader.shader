
Shader "Unlit/PointGpuUnlitShader"
{
    Properties
    {
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
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 positionWS : TEXCOORD0;
            };

            #if SHADER_TARGET >= 45
                StructuredBuffer<float3> _Positions;
            #endif

            float _Step;

            //也可以通过 #pragma instancing_options procedural:ConfigureProcedural调用   使用unity_InstanceID 来获取
            //由于没有使用gpu instancing 技术，所以通过uint iId : SV_InstanceID 来获取

            void ConfigureProcedural (uint id) 
			{
				#if SHADER_TARGET >= 45
                    float3 position = _Positions[id];

                    unity_ObjectToWorld = 0.0;
                    unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0); //位置
                    unity_ObjectToWorld._m00_m11_m22 = _Step; //缩放
                #endif
            }

			// SV_InstanceID：通过该id获取StructuredBuffer中对应的位置
            //相当于每一个被渲染的物体，去获取compute shader计算的数据列表中，第几个，因为SV_InstanceID也是批量实例化出来的第几个物体
            // 相当于 unity_InstanceID
            v2f vert (appdata v, uint iId : SV_InstanceID)
            {
                ConfigureProcedural(iId);

                v2f o;
                o.positionCS = UnityObjectToClipPos(v.vertex);
                o.positionWS = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = saturate(i.positionWS * 0.5 + 0.5);
                return col;
            }
            ENDCG
        }
    }
}
