using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGroupMgr : MonoBehaviour
{
    public static EnemyGroupMgr instance;

    public CharCtr[] enemyList;

    public int index = 0;
    public CharCtr GetOneEnemy()
    {
        index++;
        if (index > enemyList.Length - 1)
        {
            index = 0;
        }
        //index = Random.Range(0, enemyList.Length);
        CharCtr enemy = enemyList[index];
        return enemy;
    }
    
    public void Start()
    {
        EnemyGroupMgr.instance = this;
        enemyList = GetComponentsInChildren<CharCtr>();
    }

    [ContextMenu("ceshi引用")]
    public void Test()
    {
        A aobj = new A();
        aobj.aInt = 2;
        
        GCHandle handle = GCHandle.Alloc(aobj, GCHandleType.WeakTrackResurrection);
        IntPtr addr = GCHandle.ToIntPtr(handle);
        Debug.Log("Out ChangeA before" + aobj.aInt + " hashcode = " + aobj.GetHashCode() + "addr = " + $"0x{addr.ToString("X")}");
        
        ChangeA(aobj);

        Debug.Log("Out ChangeA after" +aobj.aInt + " hashcode = " + aobj.GetHashCode() + "addr = " + $"0x{addr.ToString("X")}");

        ChangeARef(ref aobj);
        
        Debug.Log("Out ChangeARef after" +aobj.aInt + " hashcode = " + aobj.GetHashCode() + "addr = " + $"0x{addr.ToString("X")}");
    }

    [ContextMenu("ceshistring引用")]
    public void TestString()
    {
        string str1 = "XHW";
        ChangeStr(str1);
        
        Debug.Log(str1);

        ChangeStrRef(ref str1);
        
        Debug.Log(str1);
    }

    public void ChangeStr(string str)
    {
        //引用类型对象按值传递，传递的是对象的地址副本
        
        //str 的+操作相当于  把地址副本 指向了新的内存，但是原地址指向的内存位置的数据不会被修改
        //只是因为str 的 + 操作本身的特性而已
        str = str + "change";
        
        Debug.Log("传值的方式传参" + str);
    }
    
    public void ChangeStrRef(ref string str)
    {
        //引用类型对象按引用传递，传递的是对象的地址本身
        str = str + "ChangeStrRef";
        
        Debug.Log("传引用的方式传参" + str);
    }
    
    public void ChangeA(A aparm)
    {
        aparm.aInt = 3;
        GCHandle handle = GCHandle.Alloc(aparm, GCHandleType.WeakTrackResurrection);
        IntPtr addr = GCHandle.ToIntPtr(handle);
        Debug.Log("In ChangeA" + aparm.aInt + " hashcode" + aparm.GetHashCode()+ "addr = " + $"0x{addr.ToString("X")}");
        
        aparm = new A();
        aparm.aInt = 888;
        
        GCHandle handle2 = GCHandle.Alloc(aparm, GCHandleType.WeakTrackResurrection);
        IntPtr addr2 = GCHandle.ToIntPtr(handle);
        Debug.Log("In ChangeA 改变地址引用后" + aparm.aInt + " hashcode" + aparm.GetHashCode()+ "addr = " + $"0x{addr2.ToString("X")}");

    }

    public void ChangeARef(ref A aparm)
    {
        aparm.aInt = 666;
        
        GCHandle handle = GCHandle.Alloc(aparm, GCHandleType.WeakTrackResurrection);
        IntPtr addr = GCHandle.ToIntPtr(handle);
        Debug.Log("In ChangeARef" + aparm.aInt + " hashcode" + aparm.GetHashCode()+ "addr = " + $"0x{addr.ToString("X")}");

        aparm = new A();
        aparm.aInt = 999;
        GCHandle handle2 = GCHandle.Alloc(aparm, GCHandleType.WeakTrackResurrection);
        IntPtr addr2 = GCHandle.ToIntPtr(handle2);
        Debug.Log("In ChangeARef" + aparm.aInt + " hashcode" + aparm.GetHashCode()+ "addr = " + $"0x{addr2.ToString("X")}");

    }
    
    public class A
    {
        public int aInt;
    }
}
