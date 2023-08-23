using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof(remoteVehicleNavmeshOverride))]
public class remoteVehicleNavmeshOverrideEditor : Editor
{
	remoteVehicleNavmeshOverride manager;

	void OnEnable ()
	{
		manager = (remoteVehicleNavmeshOverride)target;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUILayout.Label ("EDITOR BUTTONS", EditorStyles.boldLabel);
	
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Set Vehicle Navmesh Target")) {
			manager.setVehicleNavMeshTargetPosition ();
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Remove Vehicle Navmesh Target")) {
			manager.removeVehicleNavmeshTarget ();
		}

		EditorGUILayout.Space ();
	}
}
#endif