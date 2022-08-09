/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-27 11:55:26
* Des: 
*******************************************************************************/

using System;
using UnityEngine;

[Serializable] // The Serializable attributes tells Unity to serialize fields of this type.
public struct MyStruct
{
    public float X;
    public float Y;
}

public class TestDrwaMonoObj: MonoBehaviour
{
    public MyStruct MyStruct;
}