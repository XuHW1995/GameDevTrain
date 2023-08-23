using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIHidePositionsManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]
	public List<Transform> hidePositionList = new List<Transform> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public float gizmoRadius;


	//add a new waypoint
	public void addNewWayPoint ()
	{
		Vector3 newPosition = transform.position;

		if (hidePositionList.Count > 0) {
			newPosition = hidePositionList [hidePositionList.Count - 1].position + hidePositionList [hidePositionList.Count - 1].forward;
		}

		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = (hidePositionList.Count + 1).ToString ();

		hidePositionList.Add (newWayPoint.transform);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Added new position to hide ", gameObject);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (showGizmo) {
			for (int i = 0; i < hidePositionList.Count; i++) {
				if (hidePositionList [i] != null) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere (hidePositionList [i].position, gizmoRadius);

					Gizmos.color = Color.white;
					Gizmos.DrawLine (hidePositionList [i].position, transform.position);
				}
			}
		}
	}
}