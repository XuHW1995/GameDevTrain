using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObjectPositionUpdateSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool followObjectActive = true;

	public bool followPosition = true;
	public bool followRotation = true;

	public Transform objectToFollow;
	public Transform mainTransform;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool objectToFollowLocated;

	bool initialized;


	void Start ()
	{
		initializeComponents ();
	}

	void initializeComponents ()
	{
		if (!initialized) {
			if (mainTransform == null) {
				mainTransform = transform;
			}

			objectToFollowLocated = objectToFollow != null;

			initialized = true;
		}
	}

	void Update ()
	{
		if (followObjectActive && objectToFollowLocated) {
			if (followPosition && followRotation) {
				mainTransform.SetPositionAndRotation (objectToFollow.position, objectToFollow.rotation);
			} else {
				if (followPosition) {
					mainTransform.position = objectToFollow.position;
				}

				if (followRotation) {
					mainTransform.rotation = objectToFollow.rotation;
				}
			}
		}
	}

	public void setFollowObjectActiveState (bool state)
	{
		followObjectActive = state;
	}

	public void setObjectToFollow (Transform newObject)
	{
		objectToFollow = newObject;
	}

	public void setEnabledState (bool state)
	{
		enabled = state;
	}

	public void setObjectToFollowFromEditor (Transform newObject)
	{
		setObjectToFollow (newObject);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Set Object To Follow From Editor", gameObject);
	}
}
