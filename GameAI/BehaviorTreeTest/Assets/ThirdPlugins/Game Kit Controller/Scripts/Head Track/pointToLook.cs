using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointToLook : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool pointToLookEnabled = true;

	[Space]
	[Header ("Component")]
	[Space]

	public Transform pointToLookTransform;

	[Space]
	[Space]

	[TextArea (3, 10)] public string explanation = "Put this component in an object with a trigger on it and the layer 'Ignore Raycast', " +
	                                               "in order to be detected by the main player camera system as target to look";

	public Transform getPointToLookTransform ()
	{
		if (pointToLookTransform == null) {
			pointToLookTransform = transform;
		}

		return pointToLookTransform;
	}

	public void setPointToLookEnabledState (bool state)
	{
		pointToLookEnabled = state;
	}

	public bool isPointToLookEnabled ()
	{
		return pointToLookEnabled;
	}
}
