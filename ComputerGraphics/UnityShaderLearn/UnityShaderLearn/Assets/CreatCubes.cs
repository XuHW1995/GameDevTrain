using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatCubes : MonoBehaviour
{
    public GameObject cubePrefab;
    public float creatRange = 100;
    
    //初始化调用生成方块函数
    void Start()
    {
        CreatCubesIn100();
    }
    
    //在100米范围内依次生成方块，间隔为2米
    public void CreatCubesIn100()
    {
        for (int i = 0; i < creatRange; i += 2)
        {
            for (int j = 0; j < creatRange; j += 2)
            {
                for (int k = 0; k < creatRange; k += 2)
                {
                    Instantiate(cubePrefab, new Vector3(i, j, k), Quaternion.identity);
                }
            }
        }
    }
}
