using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestComputeRt : MonoBehaviour
{
    public Texture inputTex;//源贴图，被用于计算其灰度值
    public ComputeShader computeShader;
    public RawImage image;//拿一个UI来显示

    public RenderTexture t;//计算出来的rt，不能使用预创建的RT资源文件，需要temp创建可随机写的rt，sm 5.0以上才支持

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
