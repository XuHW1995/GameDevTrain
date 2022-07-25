using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Person
{
    public int age;
    public string name;
    public MovementAblitiy movementAblitiy;

    public override string ToString()
    {
        return $"{name} is {age} old, has {movementAblitiy.ToString()}";
    }
}

public class MovementAblitiy
{
    public float moveSpeed;
    public bool moveAble;

    public override string ToString()
    {
        return $"{moveAble} movespeed = {moveSpeed}";
    }
}

public class DictionaryInspectorTest : SerializedMonoBehaviour
{

    public Dictionary<int, Person> personMap = new Dictionary<int, Person>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("打印person")]
    public void DebugPerson()
    {
        foreach (var personKeyValue in personMap)
        {
            Debug.Log(personKeyValue.Value.ToString());
        }
    }
}
