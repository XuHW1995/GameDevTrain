/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-26 14:59:02
* Des: 
*******************************************************************************/

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

//[Serializable]
public class DicKey
{
    public int keyA;
    public string keyName;
}

//[Serializable]
public class DicValue
{
    public int valueA;
    public string ValueName;
}

[Serializable]
public class TestMono2 : TestMono2Father
{
    public int A;
    public bool B;
    public DicKey key;
    public DicValue value;
    
    [NonSerialized, OdinSerialize]
    public Dictionary<DicKey, DicValue> dicTest;
}