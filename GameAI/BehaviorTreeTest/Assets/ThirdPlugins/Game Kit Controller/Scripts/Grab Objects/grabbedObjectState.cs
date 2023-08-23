﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class grabbedObjectState : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool removeComponentOnDropObject = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool insideZeroGravityRoom;
	public bool grabbed;

	public bool carryingObjectPhysically;

	public GameObject currentHolder;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventsOnGrabbedState;
	public UnityEvent eventOnGrabObject;
	public UnityEvent eventOnDropObject;

	[Space]
	[Header ("Components")]
	[Space]

	public zeroGravityRoomSystem currentZeroGravityRoom;


	public void setGrabbedState (bool state)
	{
		grabbed = state;

		if (useEventsOnGrabbedState) {
			if (grabbed) {
				eventOnGrabObject.Invoke ();
			} else {
				eventOnDropObject.Invoke ();
			}
		}
	}

	public bool isGrabbed ()
	{
		return grabbed;
	}

	public void setCurrentHolder (GameObject current)
	{
		currentHolder = current;
	}

	public GameObject getCurrentHolder ()
	{
		return currentHolder;
	}

	public void setInsideZeroGravityRoomState (bool state)
	{
		insideZeroGravityRoom = state;
	}

	public bool isInsideZeroGravityRoom ()
	{
		return insideZeroGravityRoom;
	}

	public void setCurrentZeroGravityRoom (zeroGravityRoomSystem gravityRoom)
	{
		currentZeroGravityRoom = gravityRoom;
	}

	public zeroGravityRoomSystem getCurrentZeroGravityRoom ()
	{
		return currentZeroGravityRoom;
	}

	public void checkGravityRoomState ()
	{
		if (insideZeroGravityRoom) {
			currentZeroGravityRoom.setObjectInsideState (gameObject);
		}
	}

	public void setCarryingObjectPhysicallyState (bool state)
	{
		carryingObjectPhysically = state;
	}

	public bool isCarryingObjectPhysically ()
	{
		return carryingObjectPhysically;
	}

	public void removeGrabbedObjectComponent ()
	{
		if (removeComponentOnDropObject) {
			Destroy (this);
		}
	}
}
