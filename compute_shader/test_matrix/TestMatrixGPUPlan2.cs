using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMatrixGPUPlan2 : MonoBehaviour
{
    public int Matrix_Row = 4;
    public int Matrix_Col = 4;
    public ComputeShader MatrixMultipCompute2;

    private ComputeBuffer rawBuffer;
    private ComputeBuffer resultBuffer;

    private int kernel;

    struct Multip
    {
        public float a;//乘数a
        public float b;
        public float multi;//a * b的结果
    }

    Multip[] inputRaw;
    float[] outputBuffer;//使用方案2，可以不用原子操作，所以可以用float


    // Start is called before the first frame update
    void Start()
    {
        int rawCount = Matrix_Row * Matrix_Col * Matrix_Row;
        inputRaw = new Multip[rawCount];//4 * 4 * 4  16组 每组4线程  64个数据源
        int index = 0;
        /*
                                       * 0,1,2,3,4,5,6 ....
                                       * 0,1,2,3,4,5,6 ....
                                       * 0,1,2,3,4,5,6 ....
                                       * 0,1,2,3,4,5,6 ....
                                       * ......
                                       
                                       */
        for (int i = 0; i < Matrix_Row; i++)
        {
            int next = 0;
            for (int j = 0; j < Matrix_Row * Matrix_Col; j++)
            {
                float a = j % Matrix_Row;
                inputRaw[index].a = a;
                inputRaw[index].b = next;
                inputRaw[index].multi = 0;//相乘结果初始0
                if (a + 1 == Matrix_Col)
                {
                    next++;
                }
                index++;
            }
        }

        int resultCount = Matrix_Row * Matrix_Col;
        outputBuffer = new float[resultCount];

        int sizeRawBuffer = sizeof(float) * 3;
        rawBuffer = new ComputeBuffer(rawCount, sizeRawBuffer);
        rawBuffer.SetData(inputRaw);

        resultBuffer = new ComputeBuffer(resultCount, sizeof(float));
        resultBuffer.SetData(outputBuffer);

        kernel = MatrixMultipCompute2.FindKernel("CSMain");
        MatrixMultipCompute2.SetBuffer(kernel, "inputRawData", rawBuffer);
        MatrixMultipCompute2.SetBuffer(kernel, "outputBuffer", resultBuffer);
    }

    string resultStr;
    public bool Print = false;
    public Text UIResult;

    // Update is called once per frame
    void Update()
    {
        MatrixMultipCompute2?.Dispatch(kernel, Matrix_Row * Matrix_Col, 1, 1);
        //阻塞
        resultBuffer.GetData(outputBuffer);

        if (!Print)
            return;

        resultStr = "";
        for (int i = 0; i < outputBuffer.Length; i++)
        {
            resultStr += outputBuffer[i] + "  ";
            if (i > 0 && (i + 1) % Matrix_Row == 0)
            {
                resultStr += "\n";
            }
        }
        Debug.Log(resultStr);
        UIResult.text = resultStr;
    }

    private void OnDestroy()
    {
        rawBuffer?.Release();
        resultBuffer?.Release();
    }
}
