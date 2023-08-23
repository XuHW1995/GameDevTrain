using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteVehicleNavmeshOverride : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool targetIsFriendly;

	public bool targetIsObject;

	[Space]
	[Header ("Components")]
	[Space]

	public vehicleAINavMesh mainVehicleAINavMesh;

	public Transform targetTranform;


	public void setVehicleNavMeshTargetPosition ()
	{
		if (targetTranform == null) {
			targetTranform = transform;
		}

		mainVehicleAINavMesh.follow (targetTranform);

		mainVehicleAINavMesh.setTargetType (targetIsFriendly, targetIsObject);
	}

	public void removeVehicleNavmeshTarget ()
	{
		mainVehicleAINavMesh.removeTarget ();
	}
}
