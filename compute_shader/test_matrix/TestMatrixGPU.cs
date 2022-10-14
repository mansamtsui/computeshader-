using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMatrixGPU : MonoBehaviour
{
    public Text uiResult;

    public int Matrix_Row = 4;//行数
    public int Matrix_Col = 4;//列数

    public ComputeShader MatrixMultipCompute;

    private ComputeBuffer rawBuffer;
    private ComputeBuffer resultBuffer;

    private int kernal;

    struct Multip
    {
        public float a;
        public float b;
    }
    Multip[] intputRaw;
    int[] outputBuffer;//方案1，使用原子操作需要int

    // Start is called before the first frame update
    void Start()
    {
        int rawCount = Matrix_Row * Matrix_Col * Matrix_Row;
        intputRaw = new Multip[rawCount];// 4 * 4 * 4  16组 每组4线程
        int index = 0;
        for (int i = 0; i < Matrix_Row; i++)
        {
            int next = 0;
            for(int j = 0; j < Matrix_Row * Matrix_Col; j++)
            {
                float a = j % Matrix_Row;
                intputRaw[index].a = a;
                intputRaw[index].b = next;
                if (a + 1 == Matrix_Col)
                {
                    next++;
                }
                index++;
            }
        }

        int resultCount = Matrix_Row * Matrix_Col;
        outputBuffer = new int[resultCount];
        
        int sizeRaw = sizeof(float) * 2;
        rawBuffer = new ComputeBuffer(rawCount, sizeRaw);
        rawBuffer.SetData(intputRaw);

        //resultBuffer = new ComputeBuffer(resultCount, sizeof(float));
        //resultBuffer.SetData(outputBuffer);

        kernal = MatrixMultipCompute.FindKernel("CSMain");
        MatrixMultipCompute.SetBuffer(kernal, "inputRawData", rawBuffer);
        //MatrixMultipCompute.SetBuffer(kernal, "outputBuffer", resultBuffer);
    }

    string resultStr;
    public bool Print = false;
    // Update is called once per frame
    void Update()
    {
        resultBuffer?.Release();

        int resultCount = Matrix_Row * Matrix_Col;
        for (int i = 0; i < resultCount; i++)
        {
            outputBuffer[i] = 0;
        }
        resultBuffer = new ComputeBuffer(resultCount, sizeof(int));
        resultBuffer.SetData(outputBuffer);
        MatrixMultipCompute.SetBuffer(kernal, "outputBuffer", resultBuffer);

        MatrixMultipCompute.Dispatch(kernal, Matrix_Row * Matrix_Col, 1, 1);

        resultBuffer.GetData(outputBuffer);

        if (!Print)
            return;
        resultStr = "";
        for(int i = 0; i < outputBuffer.Length; i++)
        {
            resultStr += outputBuffer[i] + "  ";
            if (i > 0 && (i + 1) % Matrix_Row == 0)
            {
                resultStr += "\n";
            }
        }
        //foreach(var f in outputBuffer)
        //{
        //    Debug.Log(f);
        //}
        uiResult.text = resultStr;
    }

    private void OnDestroy()
    {
        rawBuffer?.Release();
        resultBuffer?.Release();
    }
}
