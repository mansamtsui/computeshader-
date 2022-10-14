using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestComputeRt : MonoBehaviour
{
    public Texture inputTex;//Դ��ͼ�������ڼ�����Ҷ�ֵ
    public ComputeShader computeShader;
    public RawImage image;//��һ��UI����ʾ

    public RenderTexture t;//���������rt������ʹ��Ԥ������RT��Դ�ļ�����Ҫtemp���������д��rt��sm 5.0���ϲ�֧��

    void Start()
    {
        t = new RenderTexture(inputTex.width, inputTex.height, 24);
        t.enableRandomWrite = true;
        t.Create();
        image.texture = t;
        image.SetNativeSize();

        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(kernel, "inputTexture", inputTex);
        computeShader.SetTexture(kernel, "Result", t);
        computeShader.Dispatch(kernel, inputTex.width / 8, inputTex.height / 8, 1);
    }
}
