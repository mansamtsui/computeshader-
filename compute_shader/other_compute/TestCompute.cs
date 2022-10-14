using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 这个例子是 新建一个数组，这个数组里的值在GPU中进行一次计算，并把结果保存回CPU，最终输出出来。
public class TestCompute : MonoBehaviour
{

    //ComputeShader对象定义，需要给它赋值一个computeShader的shader资源
    public ComputeShader computeShader;

    //ComputeBuffer, c#端与gpu数据通信的容器，我们组织好需要计算的数据（本例中是inOutBuffer），装在这个buffer里面，然后把这个buffer塞到
    //computeShader里面，为gpu计算做数据准备
    private ComputeBuffer buffer;

    //我们总共有多少个数据
    public int count = 128;

    //表示我们要计算的方法在computeShader里的索引，这个索引在绑定buffer时会用到
    private int kernal;

    //我们自己定义的数据，用这个对象把我们要的数据装起来，然后塞给ComputeBuffer
    //注意，数组里面的数据必须是blittable 类型的数据，可以认为必须是c#基础类型，或者由基础类型组成的struct类型
    //byte, sbyte, short, ushort, int, uint, long, ulong, single, double 这些是blittable
    MixData[] inOutBuffer;
    struct MixData
    {
        public int myVal;
        public float myFloat;
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"是否支持compute shader:{SystemInfo.supportsComputeShaders}");

        //准备我们自己的数据
        inOutBuffer = new MixData[count];
        for (int i = 0; i < count; i++)
        {
            inOutBuffer[i].myFloat = 1.0f;
            inOutBuffer[i].myVal = i;
        }
        //这里表示我们要塞的数据MixData的总长度，可以看到，MixData由一个int和一个float组成，长度是8
        int stride = sizeof(int) + sizeof(float);
        //初始化ComputeBuffer
        buffer = new ComputeBuffer(count, stride);
        //把我们准备好的数据塞给Buffer里
        buffer.SetData(inOutBuffer);

        //找到GPU真正执行的方法在computeShader里的索引
        kernal = computeShader.FindKernel("CSMain");
        Debug.Log($"Kernal:{kernal}");
        //把我们之前准备好的buffer数据塞给computeShader，这样就建立了一个gpu到cpu的数据连接，gpu在计算时
        //会使用当前这个buffer里的数据。
        //注意：下面方法中的第二个参数 必须与 shader 里对应的那个 buffer 的变量名一模一样
        computeShader.SetBuffer(kernal, "inOutBuffer", buffer);
        foreach (var val in inOutBuffer)
        {
            Debug.Log($"before : index:{val.myVal};value:{val.myFloat}");
        }
    }

    bool iswitch1 = false;
    bool iswitch2 = false;
    // Update is called once per frame
    void Update()
    {

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current[Key.F].isPressed)
#else
        if (Input.GetKeyDown(KeyCode.F))
#endif
        {
            iswitch1 = true;
            iswitch2 = true;
        }



#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current[Key.T].isPressed && iswitch1)
#else
        if (Input.GetKeyDown(KeyCode.T))
#endif
        {
            iswitch1 = false;

            //c#层触发computeShader进行gpu核心计算
            //第一个参数是相应的方法索引，弟2，3，4个参数分别代表着线程组的x,y,z，它分别对应computeShader中的
            //thread_group_x,thread_group_y和thread_group_z
            computeShader.Dispatch(kernal, 2, 2, 1);
        }

#if ENABLE_INPUT_SYSTEM
        if(Keyboard.current[Key.C].isPressed && iswitch2)
#else
        if (Input.GetKeyDown(KeyCode.C))
#endif
        {
            iswitch2 = false;

            //从Buffer中拿到完整数据，装入inOutBuffer这个对象中
            buffer.GetData(inOutBuffer);
            foreach (var val in inOutBuffer)
            {
                Debug.Log($"after : index:{val.myVal};value:{val.myFloat}");
            }
        }
    }

    private void OnDestroy()
    {
        //释放Buffer
        buffer?.Release();

        //Dispose Buffer
        buffer?.Dispose();
    }
}
