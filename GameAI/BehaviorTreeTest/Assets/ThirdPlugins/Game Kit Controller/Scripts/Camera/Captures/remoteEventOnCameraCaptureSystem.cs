using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteEventOnCameraCaptureSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkObjectFoundOnCaptureEnabled = true;

	public LayerMask layermaskToUse;

	public float maxRaycastDistance = 20;

	public bool sendObjectOnSurfaceDetected;
	public GameObject objectToSendOnSurfaceDetected;

	[Space]
	[Header ("Raycast Detection Settings")]
	[Space]

	public bool useCapsuleRaycast;

	public float capsuleCastRadius;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform cameraTransform;


	RaycastHit hit;

	RaycastHit[] hits;

	public void checkCapture ()
	{
		if (checkObjectFoundOnCaptureEnabled) {
			if (Physics.Raycast (cameraTransform.position, cameraTransform.forward, out hit, maxRaycastDistance, layermaskToUse)) {
				if (useCapsuleRaycast) {

					Vector3 currentRayOriginPosition = cameraTransform.position;

					Vector3 currentRayTargetPosition = hit.point;

					float distanceToTarget = GKC_Utils.distance (currentRayOriginPosition, currentRayTargetPosition);
					Vector3 rayDirection = currentRayOriginPosition - currentRayTargetPosition;
					rayDirection = rayDirection / rayDirection.magnitude;

					Debug.DrawLine (currentRayTargetPosition, (rayDirection * distanceToTarget) + currentRayTargetPosition, Color.red, 2);

					Vector3 point1 = currentRayOriginPosition - rayDirection * capsuleCastRadius;
					Vector3 point2 = currentRayTargetPosition + rayDirection * capsuleCastRadius;

					hits = Physics.CapsuleCastAll (point1, point2, capsuleCastRadius, rayDirection, 0, layermaskToUse);

					for (int i = 0; i < hits.Length; i++) {
						GameObject currentSurfaceGameObjectFound = hits [i].collider.gameObject;

						checkObjectDetected (currentSurfaceGameObjectFound);
					}
				} else {
					checkObjectDetected (hit.collider.gameObject);
				}
			}
		}
	}

	void checkObjectDetected (GameObject objectDetected)
	{
		GameObject character = applyDamage.getCharacterOrVehicle (objectDetected);

		if (character != null) {
			objectDetected = character;
		}

		if (objectDetected != null) {

			eventObjectFoundOnCaptureSystem currentEventObjectFoundOnCaptureSystem = objectDetected.GetComponent<eventObjectFoundOnCaptureSystem> ();

			if (currentEventObjectFoundOnCaptureSystem != null) {
				currentEventObjectFoundOnCaptureSystem.callEventOnCapture ();

				if (sendObjectOnSurfaceDetected) {
					currentEventObjectFoundOnCaptureSystem.callEventOnCaptureWithGameObject (objectToSendOnSurfaceDetected);
				}
			}
		}
	}

	public void setNewCameraTransform (Transform newCamera)
	{
		cameraTransform = newCamera;
	}
}