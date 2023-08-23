using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(GKCUIManager))]
public class GKCUIManagerEditor : Editor
{
	SerializedProperty managerEnabled;
	SerializedProperty UIElementInfoList;

	SerializedProperty objectSearcherName;

	SerializedProperty objectSearchResultList;

	SerializedProperty searchObjectsActive;

	GKCUIManager manager;

	SerializedProperty currentArrayElement;

	bool expanded;

	GUIStyle buttonStyle = new GUIStyle ();

	void OnEnable ()
	{
		managerEnabled = serializedObject.FindProperty ("managerEnabled");
		UIElementInfoList = serializedObject.FindProperty ("UIElementInfoList");
		objectSearcherName = serializedObject.FindProperty ("objectSearcherName");

		objectSearchResultList = serializedObject.FindProperty ("objectSearchResultList");

		searchObjectsActive = serializedObject.FindProperty ("searchObjectsActive");

		manager = (GKCUIManager)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.BeginVertical (GUILayout.Height (30));

		buttonStyle = new GUIStyle (GUI.skin.button);

		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fontSize = 12;

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (managerEnabled);

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("UI Info List", "window");
		showUIElementInfoList (UIElementInfoList);
		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.BeginVertical ("UI Object Searcher", "window");
		EditorGUILayout.PropertyField (objectSearcherName, new GUIContent ("Name To Search"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Search Objects By Name")) {
			manager.showObjectsBySearchName ();

			if (objectSearchResultList.arraySize > 0) {
				objectSearchResultList.isExpanded = true;
			}
		}

		if (searchObjectsActive.boolValue) {
			if (GUILayout.Button ("Clear Results")) {
				manager.clearObjectsSearcResultList ();
			}

			EditorGUILayout.Space ();

			GUILayout.BeginVertical ("UI Object To Search Result List", "window");
			showObjectSearchResultList (objectSearchResultList);
			GUILayout.EndVertical ();
		}

		GUILayout.EndVertical ();

		EditorGUILayout.Space ();

		GUILayout.EndVertical ();
		if (GUI.changed) {
			serializedObject.ApplyModifiedProperties ();
		}
	}

	void showUIElementInfoList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {

			EditorGUILayout.Space ();

			GUILayout.Label ("Number of Elements: " + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Object")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Expand All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = true;
				}
			}
			if (GUILayout.Button ("Collapse All")) {
				for (int i = 0; i < list.arraySize; i++) {
					list.GetArrayElementAtIndex (i).isExpanded = false;
				}
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Enable All")) {
				manager.enableAllUIElement ();
			}
			if (GUILayout.Button ("Disable All")) {
				manager.disableAllUIElement ();
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				expanded = false;
				GUILayout.BeginHorizontal ();
				GUILayout.BeginHorizontal ("box");

				EditorGUILayout.Space ();

				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.BeginVertical ();

					currentArrayElement = list.GetArrayElementAtIndex (i);

					string elementName = currentArrayElement.FindPropertyRelative ("Name").stringValue;

					if (currentArrayElement.FindPropertyRelative ("elementActive").boolValue) {
						elementName += " - Active";
					} else {
						elementName += " - Inactive";
					}
						
					EditorGUILayout.PropertyField (currentArrayElement, new GUIContent (elementName), false);

					if (currentArrayElement.isExpanded) {
						expanded = true;
						showUIElementInfoListElement (currentArrayElement);
					}

					EditorGUILayout.Space ();

					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				if (expanded) {
					GUILayout.BeginVertical ();
				} else {
					GUILayout.BeginHorizontal ();
				}
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
				}
				if (GUILayout.Button ("v")) {
					if (i >= 0) {
						list.MoveArrayElement (i, i + 1);
					}
				}
				if (GUILayout.Button ("^")) {
					if (i < list.arraySize) {
						list.MoveArrayElement (i, i - 1);
					}
				}

				if (GUILayout.Button ("o", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.selectObjectByIndex (i);

						return;
					}
				}

				if (GUILayout.Button ("+", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.enableUIElement (list.GetArrayElementAtIndex (i).FindPropertyRelative ("Name").stringValue);

						return;
					}
				}

				if (GUILayout.Button ("-", buttonStyle)) {
					if (!Application.isPlaying) {
						manager.disableUIElement (list.GetArrayElementAtIndex (i).FindPropertyRelative ("Name").stringValue);

						return;
					}
				}

				if (expanded) {
					GUILayout.EndVertical ();
				} else {
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndHorizontal ();
			}
		}       
	}

	void showUIElementInfoListElement (SerializedProperty list)
	{
		GUILayout.BeginVertical ("box");
		GUILayout.BeginVertical ("Main Settings", "window");
		EditorGUILayout.PropertyField (list.FindPropertyRelative ("Name"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("useMoreNameTagsToSearch"));

		if (list.FindPropertyRelative ("useMoreNameTagsToSearch").boolValue) {
			showSimpleList (list.FindPropertyRelative ("moreNameTagsToSearchList"));
		}

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("UIGameObject"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("eventToEnableUIGameObject"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField (list.FindPropertyRelative ("envetToDisableUIGameObject"));

		GUILayout.EndVertical ();
		GUILayout.EndVertical ();
	}


	void showObjectSearchResultList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		GUILayout.Label ("Number of Results: " + list.arraySize);

		EditorGUILayout.Space ();

		for (int i = 0; i < list.arraySize; i++) {
			expanded = false;
			GUILayout.BeginHorizontal ();
			GUILayout.BeginHorizontal ("box");

			EditorGUILayout.Space ();

			if (i < list.arraySize && i >= 0) {
				EditorGUILayout.BeginVertical ();

				GUILayout.Label (list.GetArrayElementAtIndex (i).stringValue, EditorStyles.boldLabel, GUILayout.MaxWidth (200));

				EditorGUILayout.Space ();

				GUILayout.EndVertical ();
			}
			GUILayout.EndHorizontal ();
			if (expanded) {
				GUILayout.BeginVertical ();
			} else {
				GUILayout.BeginHorizontal ();
			}

			if (GUILayout.Button ("o", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.selectObjectByName (list.GetArrayElementAtIndex (i).stringValue);

					return;
				}
			}

			if (GUILayout.Button ("+", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.enableUIElement (list.GetArrayElementAtIndex (i).stringValue);

					return;
				}
			}

			if (GUILayout.Button ("-", buttonStyle)) {
				if (!Application.isPlaying) {
					manager.disableUIElement (list.GetArrayElementAtIndex (i).stringValue);

					return;
				}
			}

			if (expanded) {
				GUILayout.EndVertical ();
			} else {
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndHorizontal ();
		}
	}


	void showSimpleList (SerializedProperty list)
	{
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
			list.isExpanded = !list.isExpanded;
		}

		EditorGUILayout.Space ();

		if (list.isExpanded) {
			EditorGUILayout.Space ();

			GUILayout.Label ("Number Of Transforms: \t" + list.arraySize);

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add")) {
				list.arraySize++;
			}
			if (GUILayout.Button ("Clear")) {
				list.arraySize = 0;
			}
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			for (int i = 0; i < list.arraySize; i++) {
				GUILayout.BeginHorizontal ();
				if (i < list.arraySize && i >= 0) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), new GUIContent ("", null, ""), false);
				}
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("x")) {
					list.DeleteArrayElementAtIndex (i);
					return;
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndHorizontal ();
			}
		}       
	}
}
#endif