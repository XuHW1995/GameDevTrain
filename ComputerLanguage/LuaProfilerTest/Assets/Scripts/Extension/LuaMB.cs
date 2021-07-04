using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using UnityEngine.EventSystems;

[System.Serializable]
public class InjectionGameObject
{
    public string name;
    public GameObject value;
}

[System.Serializable]
public class InjectionAttribute
{
    public string name;
    public System.Object value;
}

class CallLuaFunction{
    LuaTable m_lt;
    public CallLuaFunction(LuaTable lt)
    {
        m_lt = lt;
    }
    ~CallLuaFunction()
    {
        m_lt = null;
    }
    Dictionary<string, Action<LuaTable>> luaFunctionCache = new Dictionary<string, Action<LuaTable>>();

    public void Add(string luafuncName, bool cover = false) {
        if (luaFunctionCache.ContainsKey(luafuncName))
        {
            if (cover)
            {
                luaFunctionCache.Remove(luafuncName);
            }
            else
            {
                return;
            }
        }
        luaFunctionCache.Add(luafuncName, m_lt.Get<Action<LuaTable>>(luafuncName));
    }
    public void Call(string luafunc) {
        if (luaFunctionCache.ContainsKey(luafunc))
        {
            if (luaFunctionCache[luafunc] != null)
                luaFunctionCache[luafunc](m_lt);
        }
    }
    public void clear()
    {
        luaFunctionCache.Clear();
        luaFunctionCache = null;
    }
}

[LuaCallCSharp]
public class LuaMB : MonoBehaviour
{
    [HideInInspector]
    public string luaFilePath;

    public InjectionGameObject[] Gos;

    public BaseEventData eventData;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    CallLuaFunction clf;
    //关联的lua脚本
    public LuaTable lb;
    //Lua层初始化参数
    public LuaTable args;

    void Awake()
    {
        int pos = luaFilePath.LastIndexOf('/');
        if (pos < 0) pos = -1;
        string luaclassname = luaFilePath.Substring(pos + 1);
        string createSrc = string.Format(@"
			return function(cs)
				local Class = require('{0}')
                local o = Class(cs,'{1}')
				assert(o.bLuaBehaviour == true, 'expect a LuaBehaviour')
				return o
			end
		    ", luaFilePath, luaclassname);
        var createLuaObjectFunction = (LuaFunction)XLuaMgr.instance.GUEnv.DoString(createSrc)[0] as LuaFunction;
        lb = createLuaObjectFunction.Call(this)[0] as LuaTable;

        //
        clf = new CallLuaFunction(lb);

        clf.Add("Awake");
        clf.Add("Start");
        clf.Add("Update");
        clf.Add("OnDestroy");

        clf.Call("Awake");
    }

    // Use this for initialization
    void Start()
    {
        clf.Call("Start");
    }

    // Update is called once per frame
    void Update()
    {
        clf.Call("Update");

        if (Time.time - LuaMB.lastGCTime > GCInterval)
        {
            XLuaMgr.instance.GUEnv.Tick();
            LuaMB.lastGCTime = Time.time;
        }
    }

    public void CallLuaFunction(string funcName)
    {
        clf.Add(funcName);
        
        clf.Call(funcName);
    }

    [XLua.BlackList]
    public void SetEventData(BaseEventData bed)
    {
        eventData = bed;
    }

    void OnDestroy()
    {
        clf.Call("OnDestroy");
        clf.clear();
        Gos = null;
    }
}
