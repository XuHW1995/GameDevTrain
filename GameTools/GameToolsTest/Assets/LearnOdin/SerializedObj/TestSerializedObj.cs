/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-27 10:45:33
* Des: 
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

//[Serializable]
public class PlayerData
{
    public string name;
    public Vector3 pos;
    public int hp;

    //public Dictionary<int, string> itemName;
    public Int2intDic itemCount;


    public override string ToString()
    {
        return $"名字 = {name} pos = {pos} hp = {hp}";
    }
}

public class TestSerializedObj: MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    public string dataPath;

    [ShowInInspector]
    private int _testIntField;

    
    [ShowInInspector]
    [OnValueChanged("testIntFieldValueChange")]
    public int testIntField
    {
        get
        {
            return _testIntField;
        }
        set
        {
            _testIntField = value;
        }
    }

    public void testIntFieldValueChange()
    {
        Debug.Log("testIntField 值改变，改为" + testIntField);
    }
    
    public void Awake()
    {
        dataPath = Application.dataPath + "/";
    }

    [Button("SETDATA")]
    public void SETDATA()
    {
        dataPath = Application.dataPath + "/";
        playerData = new PlayerData();
        playerData.name = name;
        playerData.pos = transform.position;
        playerData.hp = 60;

        playerData.itemCount = new Int2intDic();
        playerData.itemCount.Add(1, 10);
        playerData.itemCount.Add(2, 20);
    }

    [Button("序列化此对象")]
    public void TestSerializedThisObj()
    {
        byte[] bytes = SerializationUtility.SerializeValue(this.playerData, DataFormat.Binary);
        File.WriteAllBytes(dataPath + $"{name}.byte", bytes);
    }
    
    [Button("反序列化此对象,并创建新对象")]
    public void TestDeSerializedThisObj()
    {
        if (!File.Exists(dataPath + $"{name}.byte")) return; // No state to load
	
        byte[] bytes = File.ReadAllBytes(dataPath + $"{name}.byte");

        GameObject newObj = Instantiate(this.gameObject);
        newObj.GetComponent<TestSerializedObj>().playerData = SerializationUtility.DeserializeValue<PlayerData>(bytes, DataFormat.Binary);
    }

    [Button("複製一份自己")]
    public void test()
    {
        
         GameObject A = Instantiate(gameObject);
        
         Debug.Log(A.GetComponent<TestSerializedObj>().playerData.ToString());
         Debug.Log("Datapath = " + A.GetComponent<TestSerializedObj>().dataPath);
    }
}