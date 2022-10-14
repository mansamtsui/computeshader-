using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMatrix : MonoBehaviour
{
    public int Matrix_Row = 4;//����
    public int Matrix_Col = 4;//����

    public List<List<float>> MultiplierA;//����
    public List<List<float>> MultiplierB;//����

    public List<List<float>> Result;//���


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
        resultIndex = 0;//������
        for(int i = 0; i < Matrix_Row; i++)
        {
            rowA = i;//Aÿ�еݹ����
            add = 0f;
            col = 0;
            for (int j =0; j <Matrix_Col * Matrix_Col; j++)
            {
                rowB = j % Matrix_Col;//Bÿ�еݹ����
                add += MultiplierA[rowA][rowB] * MultiplierB[rowB][col];
                if (rowB + 1 == Matrix_Col)//�Ѿ����һ��
                {
                    //Debug.Log($"�ڼ��������{resultIndex} {rowA} {rowB}   ========  ����ǣ�{add}");
                    Result[rowA][resultIndex%Matrix_Row] = add;
                    col++;//ÿ�ж�����һ�Σ���ʼ��һ��
                    add = 0f;
                    resultIndex++;//����4�Σ�����������ŵ�������
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


    //�鿴���
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
