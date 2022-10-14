using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMatrix : MonoBehaviour
{
    public int Matrix_Row = 4;//行数
    public int Matrix_Col = 4;//列数

    public List<List<float>> MultiplierA;//乘数
    public List<List<float>> MultiplierB;//乘数

    public List<List<float>> Result;//结果


    // Start is called before the first frame update
    void Start()
    {
        declaMatrix(out MultiplierA);
        declaMatrix(out MultiplierB);
        declaMatrix(out Result);
    }

    void declaMatrix(out List<List<float>> datalist)
    {
        datalist = new List<List<float>>(Matrix_Row);
        for (int i = 0; i < Matrix_Row; i++)
        {
            var row = new List<float>(Matrix_Col);
            for (int j = 0; j < Matrix_Col; j++)
            {
                row.Add(j);
            }
            datalist.Add(row);
        }
    }

    int resultIndex;
    int rowA;
    float add;
    int col;
    int rowB;
    void cpuCalc()
    {
        resultIndex = 0;//结果序号
        for(int i = 0; i < Matrix_Row; i++)
        {
            rowA = i;//A每行递归序号
            add = 0f;
            col = 0;
            for (int j =0; j <Matrix_Col * Matrix_Col; j++)
            {
                rowB = j % Matrix_Col;//B每列递归序号
                add += MultiplierA[rowA][rowB] * MultiplierB[rowB][col];
                if (rowB + 1 == Matrix_Col)//已经算好一列
                {
                    //Debug.Log($"第几个结果：{resultIndex} {rowA} {rowB}   ========  结果是：{add}");
                    Result[rowA][resultIndex%Matrix_Row] = add;
                    col++;//每行都乘了一次，开始下一列
                    add = 0f;
                    resultIndex++;//乘了4次，结果出来，放到这个序号
                }
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        cpuCalc();
        print();
    }


    //查看结果
    public Text uiMultiplierA;
    public Text uiMultiplierB;
    public Text uiResult;
    public bool Print = false;
    string tidyPrintStr(List<List<float>> list)
    {
        string result = "";
        for (int i = 0; i < list.Count; i++)
        {
            var row = list[i];
            for (int j = 0; j < row.Count; j++)
            {
                string sep = j + 1 == row.Count ? "\n" : "  ";
                result += row[j] + sep;

            }
        }
        return result;
    }

    private void print()
    {
        if (!Print) return;
        uiResult.text = tidyPrintStr(Result);
        uiMultiplierA.text = tidyPrintStr(MultiplierA);
        uiMultiplierB.text = tidyPrintStr(MultiplierB);
    }
}
