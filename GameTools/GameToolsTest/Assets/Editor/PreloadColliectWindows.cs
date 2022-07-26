/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-26 12:05:45
* Des: 
*******************************************************************************/

using System;
using UnityEditor;
using UnityEngine;

public class PreloadColliectWindows: EditorWindow
{
    
    private Editor editor;
    [MenuItem("Preload/BattlePreload", false, 1)]
    public static void ShowHistoryWindow()
    {
        var window = EditorWindow.GetWindow<PreloadColliectWindows>("PreloadColliectWindows");
        // 直接根据ScriptableObject构造一个Editor
        window.editor = Editor.CreateEditor(ScriptableObject.CreateInstance<PreloadObj>());
    }

    public void OnGUI()
    {
        this.editor.OnInspectorGUI();
    }
}