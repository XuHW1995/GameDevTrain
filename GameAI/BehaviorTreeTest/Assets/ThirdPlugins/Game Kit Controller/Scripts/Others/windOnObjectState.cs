using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windOnObjectState : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool windDetectEnabled = true;

	public Vector3 windDirection;

	public float windForce;

	public bool ignoreApplyOtherForcesOnObject;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool windDetected;


	public void setWindDetectedState (bool state)
	{
		if (!windDetectEnabled) {
			return;
		}

		windDetected = state;
	}

	public void setWindDirectionValues (Vector3 newValues)
	{
		windDirection = newValues;
	}

	public void setWindForceValue (float newValue)
	{
		windForce = newValue;
	}

	public bool isWindDetected ()
	{
		return windDetected;
	}

	public Vector3 getwindDirection ()
	{
		if (windDetected) {
			return windDirection;
		}

		return Vector3.zero;
	}

	public float getWindForce ()
	{
		if (windDetected) {
			return windForce;
		}

		return 0;
	}
}
