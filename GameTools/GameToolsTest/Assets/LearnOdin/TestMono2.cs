/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-26 14:59:02
* Des: 
*******************************************************************************/

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

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

// public class TestMono2 : TestMono2Father
// {
//     public int A;
//     public bool B;
//     public DicKey key;
//     public DicValue value;
//     
//     [NonSerialized, OdinSerialize]
//     public Dictionary<DicKey, DicValue> dicTest;
// }

public enum TestEnum : long
{
    A,
    B,
}

[ShowOdinSerializedPropertiesInInspector]
public class TestMono2 : MonoBehaviour, ISerializationCallbackReceiver, ISupportsPrefabSerialization
{
    public GameObject obj;
    public int A;
    public bool B;
    public DicKey key;
    public DicValue value;
    
    [NonSerialized, OdinSerialize]
    public Dictionary<DicKey, DicValue> dicTest;
    
    [SerializeField, HideInInspector]
    private SerializationData serializationData;

    SerializationData ISupportsPrefabSerialization.SerializationData { get { return this.serializationData; } set { this.serializationData = value; } }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
    }
}