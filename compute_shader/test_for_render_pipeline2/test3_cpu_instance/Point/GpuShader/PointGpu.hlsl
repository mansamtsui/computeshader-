#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3> _Positions;
#endif

float _Step;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float3 position = _Positions[unity_InstanceID];

		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
		unity_ObjectToWorld._m00_m11_m22 = _Step;
	#endif
}

/*

添加一个 void ShaderGraphFunction_float函数到 PointGPU 。
它有两个 float3 的参数 In 和 Out 。这个函数简单的将输入分配给了输出。
参数名称按惯例大写，因为它们将对应于shader graph中使用的输入和输出标签。
*/

void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}

void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}