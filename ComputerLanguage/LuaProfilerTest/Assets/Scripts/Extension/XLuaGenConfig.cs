using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


//系统 和 引擎 与lua的交互扩展
public static class XLuaGenConfig
{
    [LuaCallCSharp]
    public static List<Type> lua_call_cs_list = new List<Type>()
    {
        typeof(UnityEngine.Object),
        typeof(System.Type),
        typeof(LuaMB),
        typeof(UnityEngine.MonoBehaviour),
        typeof(UnityEngine.Component ),
        typeof(UnityEngine.GameObject ),
        typeof(UnityEngine.Transform ),
        typeof(UnityEngine.Application),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.SceneManagement.LoadSceneMode),
        typeof(UnityEngine.SceneManagement.SceneManager),
        typeof(UnityEngine.SceneManagement.Scene),
        typeof(UnityEngine.RectTransform),
        typeof(UnityEngine.Vector4),
        typeof(UnityEngine.GL),
        typeof(UnityEngine.Color),
        typeof(UnityEngine.Camera),
        typeof(UnityEngine.CameraClearFlags),
        typeof(UnityEngine.EventSystems.EventSystem),
        typeof(UnityEngine.EventSystems.StandaloneInputModule ),
        typeof(UnityEngine.Canvas),
        typeof(UnityEngine.Sprite),
        typeof(UnityEngine.Texture2D),
        typeof(UnityEngine.Rect),
        typeof(UnityEngine.PlayerPrefs),
        typeof(UnityEngine.TextAsset),
        typeof(UnityEngine.UI.Text),
        typeof(UnityEngine.UI.Slider),
        typeof(UnityEngine.Font),
        typeof(UnityEngine.TextAnchor),
        typeof(UnityEngine.AssetBundle),
        typeof(UnityEngine.AssetBundleManifest),
        typeof(UnityEngine.Input),
        //typeof(System.IO.File),
        typeof(UnityEngine.UI.Toggle),
        typeof(UnityEngine.Time),
        typeof(WaitForSeconds),
        typeof(WWW),
        typeof(UnityEngine.PlayerPrefs),
    };
    [CSharpCallLua]
    public static List<Type> cs_call_lua_list = new List<Type>()
    {
        typeof(UnityEngine.Events.UnityAction),
        typeof(UnityEngine.Events.UnityAction<string>),
        typeof(Action),
        typeof(Action<LuaTable>),
        typeof(UnityEngine.Events.UnityAction<bool>)
    };

    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()
   {
       
        new List<string>(){ "UnityEngine.Input", "IsJoystickPreconfigured","System.String"},
        new List<string>(){ "UnityEngine.MonoBehaviour", "runInEditMode"},
        new List<string>(){ "UnityEngine.Texture2D", "alphaIsTransparency"},
        new List<string>(){ "UnityEngine.UI.Text", "OnRebuildRequested"},
        new List<string>(){ "UnityEngine.WWW", "movie"},
    };                                           

}