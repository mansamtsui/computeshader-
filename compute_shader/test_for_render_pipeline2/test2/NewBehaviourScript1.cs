using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewBehaviourScript1 : MonoBehaviour
{
    // public Transform pointTransform;
    [SerializeField, Range(10, 200)]
    public int resolution = 200;
    // Transform[] points;
    ComputeBuffer positionsBuffer;
    [SerializeField]
    ComputeShader computeShader = default;
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        scaleId = Shader.PropertyToID("_Scale"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");
    [SerializeField]
    Material material = default;
    [SerializeField]
    Mesh mesh = default;
    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }
    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetVector(scaleId, new Vector4(step, 1f / step));
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }
    private void Update()
    {
        UpdateFunctionOnGPU();
    }
}