using UnityEngine;
using XLua;
using System.IO;

[LuaCallCSharp]
[Hotfix]
//XLua模块封装
public class XLuaMgr {
    private LuaEnv _GUEnv;
    public LuaEnv GUEnv
    {
        get { return _GUEnv; }
    }

    static private XLuaMgr _instance;
    static public XLuaMgr instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new XLuaMgr();
                _instance.init();
            }
            return _instance;
        }
    }
    //
    void init()
    {
        Debug.Assert(_GUEnv == null);
        _GUEnv = new LuaEnv();

        _GUEnv.AddLoader(CustomLuaLoader);
    }

    /* 自定义lua加载器
     * 参数：lua文件的 path，有两种来源：1、来自LuaMBEditor（LuaMB记录的lua路径） 2、来自*.lua.txt文件代码中的require "xxx/xxx"
     * 注意：为了方便，参数path是"Lua/*.lua.txt"中的*代替的部分，所以CustomLuaLoader中需要重新组合成合法的路径名
     */
    byte[] CustomLuaLoader(ref string path)
    {
        byte[] ret;
        string resourcesPath;
        string fullPathDownload;
        //合成resources路径
        resourcesPath = Path.Combine("Lua/", path + ".lua");
        //合成下载路径
        fullPathDownload = Path.Combine(Application.persistentDataPath, resourcesPath + ".txt");
        //fullPathDownload = Path.Combine(Application.persistentDataPath, resourcesPath);
        //优先读取下载路（热更新能工作的基本条件）
        if (File.Exists(fullPathDownload))
        {
            //由于下载的lua代码并不打包成AssetsBundle，所以可以直接用File.ReadAllBytes读取。
            ret = File.ReadAllBytes(fullPathDownload);
        }
        else
        {
            TextAsset ta = Resources.Load<TextAsset>(resourcesPath);
            ret = ta == null ? null : ta.bytes;
            //ret = File.ReadAllBytes(resourcesPath);
        }

        //判断是否加密，如果是加密则解密
        //todo...

        return ret;
    }


    //
    [BlackList]
    public void Dispose()
    {
        if (_instance != null)
        {
            _instance = null;
        }
        if (_GUEnv != null)
        {
            _GUEnv.Dispose();
        }
    }
}
