using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remotePlayerNavmeshOverrideSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool activateNavmesh;

	[Space]
	[Header ("Navmesh Target Settings")]
	[Space]

	public Transform navmeshTargetTransform;

	[Space]

	public bool useWaypointList;

	public List<Transform> waypointList = new List<Transform> ();

	[Space]
	[Header ("Components")]
	[Space]

	public remotePlayerNavmeshOverride mainRemotePlayerNavmeshOverride;


	public void setRemoteNavmeshState ()
	{
		setRemoteNavmeshStateExternally (activateNavmesh);
	}

	public void setRemoteNavmeshStateExternally (bool state)
	{
		if (state) {
			activateRemoteNavmesh ();
		} else {
			disableRemoteNavmesh ();
		}
	}

	public void activateRemoteNavmesh ()
	{
		if (mainRemotePlayerNavmeshOverride == null) {
			mainRemotePlayerNavmeshOverride = FindObjectOfType<remotePlayerNavmeshOverride> ();
		}

		if (mainRemotePlayerNavmeshOverride != null) {
			mainRemotePlayerNavmeshOverride.setPlayerNavMeshEnabledState (true);

			if (useWaypointList) {
				mainRemotePlayerNavmeshOverride.setPlayerNavMeshWaypointTargetPosition (waypointList);
			} else {
				mainRemotePlayerNavmeshOverride.setPlayerNavMeshTransformTargetPosition (navmeshTargetTransform);
			}
		}
	}

	public void disableRemoteNavmesh ()
	{
		if (mainRemotePlayerNavmeshOverride == null) {
			mainRemotePlayerNavmeshOverride = FindObjectOfType<remotePlayerNavmeshOverride> ();
		}

		if (mainRemotePlayerNavmeshOverride != null) {
			mainRemotePlayerNavmeshOverride.setPlayerNavMeshEnabledState (false);

			if (useWaypointList) {
				mainRemotePlayerNavmeshOverride.setPlayerNavMeshWaypointTargetPosition (null);
			}
		}
	}
}
