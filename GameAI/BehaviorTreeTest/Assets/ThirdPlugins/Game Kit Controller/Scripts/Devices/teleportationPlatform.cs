using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class teleportationPlatform : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool teleportEnabled = true;
	public Transform platformToMove;
	public LayerMask layermask;

	public bool useButtonToActivate;

	[Space]
	[Header ("Check Layer Settings")]
	[Space]

	public bool checkObjectLayerToTeleport;
	public LayerMask objectLayerToTeleport;

	public bool ignoreVehiclesToTeleport;

	public float vehicleRadiusMultiplier = 0.5f;

	[Space]
	[Header ("Rotation Settings")]
	[Space]

	public bool setObjectRotation;
	public bool setFullObjectRotation;

	public bool adjustPlayerCameraRotationIfPlayerTeleported;

	public Transform objectRotationTransform;

	[Space]
	[Header ("Gravity Settings")]
	[Space]

	public bool setGravityDirection;

	public setGravity setGravityManager;

	[Space]
	[Header ("Debug")]
	[Space]

	public GameObject objectInside;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool callEventOnTeleport;
	public UnityEvent eventOnTeleport;
	public bool callEventOnEveryTeleport;

	[Space]
	[Header ("Components")]
	[Space]

	public teleportationPlatform platformToMoveManager;

	bool platformToMoveManagerLocated;

	bool eventCalled;

	RaycastHit hit;
	grabbedObjectState currentGrabbedObject;


	void OnTriggerEnter (Collider col)
	{
		if (!teleportEnabled) {
			return;
		}

		if (objectInside != null) {
			return;
		}

		GameObject currentObject = col.gameObject;

		if (checkObjectLayerToTeleport) {
			if ((1 << currentObject.layer & objectLayerToTeleport.value) != 1 << currentObject.layer) {
				return;
			}
		}

		if (ignoreVehiclesToTeleport) {
			if (applyDamage.isVehicle (currentObject)) {
				return;
			}
		}

		GameObject objectToTeleport = applyDamage.getCharacterOrVehicle (currentObject);

		if (objectToTeleport != null) {
			currentObject = objectToTeleport;
		}

		Rigidbody currentRigidbody = currentObject.GetComponent<Rigidbody> ();

		if (currentRigidbody == null) {
			return;
		}


		if (currentObject == null) {
			return;
		}
			
		objectInside = currentObject;

		//if the object is being carried by the player, make him drop it
		currentGrabbedObject = objectInside.GetComponent<grabbedObjectState> ();

		if (currentGrabbedObject != null) {
			if (currentGrabbedObject.isCarryingObjectPhysically ()) {
				objectInside = null;

				return;
			} else {
				GKC_Utils.dropObject (currentGrabbedObject.getCurrentHolder (), objectInside);
			}
		}

		if (!useButtonToActivate) {
			activateTeleport ();
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (objectInside != null && col.gameObject == objectInside) {
			objectInside = null;
			currentGrabbedObject = null;
		}
	}

	public void removeObjectInside ()
	{
		objectInside = null;
	}

	void activateDevice ()
	{
		if (!teleportEnabled) {
			return;
		}

		if (useButtonToActivate && objectInside != null) {
			activateTeleport ();
		}
	}

	public void activateTeleport ()
	{
		if (!platformToMoveManagerLocated) {
			if (platformToMove != null) {
				platformToMoveManager = platformToMove.GetComponent<teleportationPlatform> ();
			}

			platformToMoveManagerLocated = platformToMoveManager != null;
		}

		if (platformToMoveManagerLocated) {
			platformToMoveManager.sendObject (objectInside);

			removeObjectInside ();

			if (callEventOnTeleport) {
				if (!eventCalled || callEventOnEveryTeleport) {
					eventOnTeleport.Invoke ();
					eventCalled = true;
				}
			}
		}
	}

	public void sendObject (GameObject objectToMove)
	{
		Vector3 targetPosition = transform.position + transform.up * 0.3f;

		if (Physics.Raycast (transform.position + transform.up * 2, -transform.up, out hit, Mathf.Infinity, layermask)) {
			targetPosition = hit.point;

			objectInside = objectToMove;
		}
			
		bool objectIsVehicle = applyDamage.isVehicle (objectToMove);

		if (objectIsVehicle) {
			float vehicleRadius = applyDamage.getVehicleRadius (objectToMove);

			if (vehicleRadius != -1) {
				targetPosition += objectToMove.transform.up * vehicleRadius * vehicleRadiusMultiplier;
			}
		}

		objectToMove.transform.position = targetPosition;
			
		if (setObjectRotation) {
			if (setFullObjectRotation) {
				objectToMove.transform.rotation = objectRotationTransform.rotation;
			} else {
				float rotationAngle = Vector3.SignedAngle (objectToMove.transform.forward, objectRotationTransform.forward, objectToMove.transform.up);
				
				objectToMove.transform.Rotate (objectToMove.transform.up * rotationAngle);
			}

			bool objectIsCharacter = applyDamage.isCharacter (objectToMove);

			if (objectIsCharacter) {
				if (adjustPlayerCameraRotationIfPlayerTeleported) {
					playerController currentPlayerController = objectToMove.GetComponent<playerController> ();

					if (currentPlayerController != null) {
						currentPlayerController.getPlayerCameraGameObject ().transform.rotation = objectToMove.transform.rotation;
					}
				}
			}
		}

		if (setGravityDirection && setGravityManager != null) {
			Collider currentCollider = objectToMove.GetComponent<Collider> ();

			if (currentCollider != null) {
				setGravityManager.checkTriggerType (currentCollider, true);
			}
		}
	}

	public void setTeleportEnabledState (bool state)
	{
		teleportEnabled = state;
	}
}