using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BadGuy
{
    public void BadMethod()
    {

    }
}

public class Student
{
    public int age;
    public string name;
}

public class Csharp : MonoBehaviour
{
    public const int stu = 100;
    // Start is called before the first frame update
    void Start()
    {
        short st = -1000;
        ushort us = 1000;
        
        Debug.Log(Convert.ToString(st, 2));
        BadGuy bg = new BadGuy();
        bg.BadMethod();

        Student s = new Student();
        //常量不可以赋值
        //stu = 200;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
