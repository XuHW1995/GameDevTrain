using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventObjectFoundOnRaycastSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool checkObjectsEnabled = true;

	public Transform raycastTransform;
	public float rayDistanceToCheckObjectFound;
	public LayerMask layerToCheckObjectFound;

	public bool checkObjectsOnUpdate;

	[Space]
	[Header ("Type of Event Settings")]
	[Space]

	public bool checkEventsOnFoundCaptureSystem = true;

	public bool sendObjectOnSurfaceDetected;
	public GameObject objectToSendOnSurfaceDetected;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool checkEventsOnRemoteEventSystem;

	public string remoteEventToCall;

	public bool useRemoteEventList;

	public List<string> removeEventNameList = new List<string> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventToCallObjecObjectDetected;
	public UnityEvent eventToCallOnObjectDetected;


	RaycastHit hit;

	GameObject currentObjectDetected;

	GameObject previousObjectDetected;

	bool raycastTransformLocated;


	void Start ()
	{
		if (checkObjectsOnUpdate) {
			activateRaycastDetectionOnUpdate ();
		}
	}

	public void activateRaycastDetectionOnUpdate ()
	{
		StartCoroutine (checkObjectWithRaycastCoroutine ());
	}

	IEnumerator checkObjectWithRaycastCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			yield return waitTime;

			checkObjectWithRaycast ();
		}
	}

	public void checkObjectWithRaycast ()
	{
		if (checkObjectsEnabled) {
			if (showDebugPrint) {
				print ("checkObjectWithRaycast called");
			}

			if (!raycastTransformLocated) {

				raycastTransformLocated = raycastTransform != null;
			}

			if (raycastTransformLocated) {
				if (Physics.Raycast (raycastTransform.position, raycastTransform.forward, out hit, rayDistanceToCheckObjectFound, layerToCheckObjectFound)) {

					currentObjectDetected = hit.collider.gameObject;

					if (showDebugPrint) {
						print ("Object Detected On Raycast " + currentObjectDetected.name);
					}

					if (currentObjectDetected != previousObjectDetected) {

						previousObjectDetected = currentObjectDetected;

						if (checkEventsOnFoundCaptureSystem) {
							if (showDebugPrint) {
								print ("Checking if event found on capture system");
							}

							eventObjectFoundOnCaptureSystem currentEventObjectFoundOnCaptureSystem = currentObjectDetected.GetComponent<eventObjectFoundOnCaptureSystem> ();

							if (currentEventObjectFoundOnCaptureSystem != null) {
								if (showDebugPrint) {
									print ("currentEventObjectFoundOnCaptureSystem detected");
								}

								currentEventObjectFoundOnCaptureSystem.callEventOnCapture ();

								if (sendObjectOnSurfaceDetected) {
									currentEventObjectFoundOnCaptureSystem.callEventOnCaptureWithGameObject (objectToSendOnSurfaceDetected);
								}
							}
						}

						if (checkEventsOnRemoteEventSystem) {
							remoteEventSystem currentRemoteEventSystem = currentObjectDetected.GetComponent<remoteEventSystem> ();

							if (currentRemoteEventSystem != null) {
								if (useRemoteEventList) {
									for (int i = 0; i < removeEventNameList.Count; i++) {
										currentRemoteEventSystem.callRemoteEvent (removeEventNameList [i]);
									}
								} else {
									currentRemoteEventSystem.callRemoteEvent (remoteEventToCall);
								}
							}
						}

						if (useEventToCallObjecObjectDetected) {
							eventToCallOnObjectDetected.Invoke ();
						}
					}
				} else {
					if (currentObjectDetected != null) {
						currentObjectDetected = null;

						previousObjectDetected = null;
					}
				}
			}
		}
	}

	public void setRaycastTransform (Transform newObject)
	{
		raycastTransform = newObject;
	}

	public void setCheckObjectsEnabledState (bool state)
	{
		checkObjectsEnabled = state;
	}
}
