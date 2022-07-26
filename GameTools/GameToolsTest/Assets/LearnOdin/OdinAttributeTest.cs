/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-25 17:27:00
* Des: 
*******************************************************************************/

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Examples;
using Sirenix.Serialization;
using UnityEngine;

namespace OdinAttributeLearn
{
     public enum PersonType
     {
          china,
          us,
     }
          
     public class Person
     {
          [HideInInspector]
          public PersonType PersonType;
     }
          
     [Serializable]
     public class Student: Person
     {
          [InfoBox(@"@""This member's parent property is called "" + $property.Parent.NiceName")]
          public string name;
     
          public int age;
     
          [AssetList]
          [InlineButton("ShowStudentName", "显示学生名字")]
          public List<GameObject> res;
     
          public void ShowStudentName()
          {
               Debug.Log("name = " + name);
          }
     }
     
     /// <summary>
     /// 复合属性
     /// </summary>
     [IncludeMyAttributes]
     [BoxGroup("第一组")]
     [InfoBox("The AssetList attribute work on both lists of UnityEngine.Object types and UnityEngine.Object types, but have different behaviour.")]
     [AssetList]
     [InlineEditor(InlineEditorModes.LargePreview)]
     public class MyObjectAttributeGroup: Attribute{}
     
     public class OdinAttributeTest : SerializedMonoBehaviour
     {
          [MyObjectAttributeGroup]
          public GameObject Prefab;
     
          private int intValueField;
          [BoxGroup("第一组")]
          [OdinSerialize()]
          private int IntValueFieldSave
          {
               get { return intValueField;}
               set { intValueField = value;}
          }
     
          [BoxGroup("第一组")]
          [ShowInInspector()]
          private int intValueFieldNotSave;
     
          [SerializeField, BoxGroup("第二组")] 
          public Dictionary<int, int> intMaps = new Dictionary<int, int>();
     
          [BoxGroup("第二组/1")]
          public bool boolValue;
          [BoxGroup("第二组/2"), Range(-10, 10)]
          public float floatValue;
     
          [BoxGroup("第三组")]
          [OnValueChanged("OnIntValueChangeHandler")]
          [ValidateInput("CheckValueValidate", "值得大于10才行", InfoMessageType.Warning)]
          public int valueChangeEvent;
     
          public bool CheckValueValidate(int value)
          {
               return value > 10;
          }
          
          [Button("OnIntValueChangeHandler")]
          [BoxGroup("第三组")]
          public void OnIntValueChangeHandler()
          {
               Debug.Log("XHW valueChangeEvent change");
          }
          
          public Student OneStudent;
          
          // It is generally recommended to use the OnStateUpdate attribute to control the state of properties
          [OnStateUpdate("@#(exampleList).State.Expanded = $value.HasFlag(ExampleEnum.UseStringList)")]
          public ExampleEnum exampleEnum;

          public List<string> exampleList;

          [OnStateUpdate("@#(exampleList).State.Expanded = ($value == 10 ?  true : false)")]
          public int stateInt;
          
          [Flags]
          public enum ExampleEnum
          {
               None,
               UseStringList = 1 << 0,
               // ...
          }

          public void EnumFlag()
          {
               exampleEnum.HasFlag(ExampleEnum.None);
          }

          public PreloadObj TestPreloadObj;
     }
}