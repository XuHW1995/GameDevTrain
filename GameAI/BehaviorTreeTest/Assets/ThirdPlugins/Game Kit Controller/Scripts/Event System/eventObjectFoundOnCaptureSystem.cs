using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventObjectFoundOnCaptureSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool eventEnabled = true;

	public bool callEventMultipleTimes;

	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool eventTriggered;
	public bool eventTriggeredWithGameObject;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToCallOnCapture;

	public bool useEventToSendObject;

	public eventParameters.eventToCallWithGameObject eventToSendObject;

	public void callEventOnCapture ()
	{
		if (eventEnabled && !eventTriggered) {
			eventToCallOnCapture.Invoke ();

			if (showDebugPrint) {
				print ("Event Called on Capture");
			}

			if (!callEventMultipleTimes) {
				eventTriggered = true;
			}
		}
	}

	public void callEventOnCaptureWithGameObject (GameObject newObject)
	{
		if (eventEnabled) {
			if (useEventToSendObject) {
				if (newObject == null) {
					return;
				}

				if (!eventTriggeredWithGameObject) {
					if (showDebugPrint) {
						print ("Event Called on Capture With GameObject");
					}

					eventToSendObject.Invoke (newObject);

					if (!callEventMultipleTimes) {
						eventTriggeredWithGameObject = true;
					}
				}
			}
		}
	}
}