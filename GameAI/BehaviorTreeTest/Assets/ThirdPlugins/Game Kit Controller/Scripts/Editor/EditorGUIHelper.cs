using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace GameKitController.Editor
{
	public class EditorGUIHelper
	{
		public static void showAudioElementList (SerializedProperty list)
		{
			GUILayout.BeginVertical ();

			EditorGUILayout.Space ();

			var buttonStyle = new GUIStyle (GUI.skin.button);

			buttonStyle.fontStyle = FontStyle.Bold;
			buttonStyle.fontSize = 12;

			if (GUILayout.Button ("Show/Hide " + list.displayName, buttonStyle)) {
				list.isExpanded = !list.isExpanded;
			}

			EditorGUILayout.Space ();

			if (list.isExpanded) {

				EditorGUILayout.Space ();

				GUILayout.Label ("Number of " + list.type + ": " + list.arraySize);

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add " + list.type)) {
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

				for (int i = 0; i < list.arraySize; i++) {
					bool expanded = false;
					GUILayout.BeginHorizontal ();
					GUILayout.BeginHorizontal ("box");

					EditorGUILayout.Space ();

					if (i < list.arraySize && i >= 0) {
						EditorGUILayout.BeginVertical ();
						EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i), false);
						if (list.GetArrayElementAtIndex (i).isExpanded) {
							if (list.type == typeof (audioSourceInfo).Name)
								showAudioSourceInfoListElement (list.GetArrayElementAtIndex (i));
							else
								showAudioElementListElement (list.GetArrayElementAtIndex (i));
							expanded = true;
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
					if (expanded) {
						GUILayout.EndVertical ();
					} else {
						GUILayout.EndHorizontal ();
					}
					GUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.Space ();
			GUILayout.EndVertical ();
		}

		private static void showAudioElementListElement (SerializedProperty list)
		{
			GUILayout.BeginVertical ("box");

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioPlayMethod"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioEventName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("clip"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSourceName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSource"));

			GUILayout.EndVertical ();
		}

		private static void showAudioSourceInfoListElement (SerializedProperty list)
		{
			GUILayout.BeginVertical ("box");

			EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSourceName"));
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("audioSource"));

			GUILayout.EndVertical ();
		}
	}
}
