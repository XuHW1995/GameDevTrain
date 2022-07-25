/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-25 17:27:00
* Des: 
*******************************************************************************/

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class OdinAttributeTest : SerializedMonoBehaviour
{
     [BoxGroup("第一组")]
     [InfoBox("The AssetList attribute work on both lists of UnityEngine.Object types and UnityEngine.Object types, but have different behaviour.")]
     [AssetList]
     [InlineEditor(InlineEditorModes.LargePreview)]
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
     
     // Now, anywhere you declare it, myStr will now dynamically know the name of its parent
     public Student OneStudent;
}