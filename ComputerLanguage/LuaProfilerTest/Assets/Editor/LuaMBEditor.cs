using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(LuaMB))]
public class LuaMBEditor : Editor {

	private string Relative(string luaPath) {
		string pattern = @"^Assets/Resources/Lua/(.+)\.lua.txt$";
		//string pattern = @"^Assets/Resources/Lua/(.+)\.lua$";
		Match match = Regex.Match(luaPath, pattern);
		if (match.Success) {
			return match.Groups[1].Value;
		} else {
			return null;
		}
	}
	
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.Space();
		DropAreaGUI();
	}

	public void DropAreaGUI() {
        LuaMB myTarget = (LuaMB)target;
		Event evt = Event.current;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("luaFilePath", GUILayout.Width(80));
		Rect luaFilePathRect = EditorGUILayout.GetControlRect(GUILayout.Width(250));
		EditorGUI.TextField(luaFilePathRect, myTarget.luaFilePath);

		switch (evt.type) {
		case EventType.DragUpdated:
		case EventType.DragPerform:
			if (!luaFilePathRect.Contains(evt.mousePosition))
				return;
			
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if (evt.type == EventType.DragPerform) {
				string luaPath = Relative(DragAndDrop.paths[0]);

				if (luaPath != null) {
					DragAndDrop.AcceptDrag();
                    myTarget.luaFilePath = luaPath;
				}
			}
			break;
		}
        EditorGUILayout.EndHorizontal();
	}
}
