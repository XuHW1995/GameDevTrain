/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-27 11:56:29
* Des: 
*******************************************************************************/

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class TestDrawer: OdinValueDrawer<MyStruct>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect = EditorGUILayout.GetControlRect();

        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        MyStruct value = this.ValueEntry.SmartValue;
        GUIHelper.PushLabelWidth(20);
        value.X = EditorGUI.Slider(rect.AlignLeft(rect.width * 0.5f), "X", value.X, 0, 1);
        value.Y = EditorGUI.Slider(rect.AlignRight(rect.width * 0.5f), "Y", value.Y, 0, 1);
        GUIHelper.PopLabelWidth();

        this.ValueEntry.SmartValue = value;
    }
}