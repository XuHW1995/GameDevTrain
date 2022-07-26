/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-26 14:49:25
* Des: 
*******************************************************************************/

// 需要导入命名空间
using System.IO;
using System.Xml.Serialization;
using OdinAttributeLearn;
using UnityEditor;
using UnityEngine;

// 先定义一个类：
public class Student2
{
    public string m_Name = "仑仑";
}

public class SerializeDemo : MonoBehaviour 
{
    void Start()
    {
        // 序列化：
        Student2 student = new Student2();
        // 第一要确定储存的位置
        // Application.dataPath就是unity中的Asset文件夹 
        StreamWriter sw = new StreamWriter(Application.dataPath + "/XHWSerializeDemo.xml");
        // 第二要对什么类型序列化
        XmlSerializer xs = new XmlSerializer(typeof(Student2));
        // 第三对对象序列化
        xs.Serialize(sw, student);
        // 关闭流
        sw.Close();
        // 刷新Asset文件
        AssetDatabase.Refresh();
        // 运行程序会在Asset目录下看到Demo.xml文件
 
 
        // 反序列化
        // 第一确定对哪个文件进行反序列化
        StreamReader sr = new StreamReader(Application.dataPath + "/XHWSerializeDemo.xml");
        // 第二要确定对什么类型反序列化
        XmlSerializer xs1 = new XmlSerializer(typeof(Student2));
        // 第三反序列化
        Student2 student1 = xs1.Deserialize(sr) as Student2;
        // 关闭流
        sr.Close();
        // 会在控制台看到 “仑仑”
        print(student1.m_Name);
    }
}