using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class meleeAttackSurfaceInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool surfaceEnabled = true;

	public string surfaceName;

	[Space]
	[Header ("Weapon Throw Settings")]
	[Space]

	public bool useOffsetTransformOnWeaponThrow;
	public Transform offsetTransformOnWeaponThrow;

	public bool useOffsetDistanceOnWeaponThrow;
	public float offsetDistanceOnWeaponThrow;

	public bool disableInstaTeleportOnThisSurface;

	[Space]
	[Header ("Attach To Surface On Throw Settings")]
	[Space]

	public bool setAttachMeleeWeaponOnSurfaceValue;
	public bool attachMeleeWeaponOnSurfaceValue;

	[Space]
	[Header ("Durability Settings")]
	[Space]

	public bool ignoreDurability;
	public float extraDurabilityMultiplier;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnSurfaceDetected;
	public UnityEvent eventOnSurfaceDeteceted;

	[Space]
	[Header ("Events On Throw/Return Weapon Settings")]
	[Space]

	public bool useEventOnThrowWeapon;
	public UnityEvent eventOnThrowWeapon;

	public bool useEventOnReturnWeapon;
	public UnityEvent eventOnReturnWeapon;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEvent;
	public List<string> remoteEventNameList = new List<string> ();

	[Space]

	public bool useRemoteEventOnWeapon;
	public List<string> remoteEventOnWeaponNameList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool ignoreSurfaceActive;

	string originalSurfaceName;



	void Start ()
	{
		originalSurfaceName = surfaceName;
	}

	public string getSurfaceName ()
	{
		return surfaceName;
	}

	public bool isSurfaceEnabled ()
	{
		return surfaceEnabled && !ignoreSurfaceActive;
	}

	public void setNewSurfaceName (string newSurfaceName)
	{
		surfaceName = newSurfaceName;
	}

	public void setOriginalSurfaceName ()
	{
		setNewSurfaceName (originalSurfaceName);
	}

	public void setIgnoreSurfaceActiveState (bool state)
	{
		ignoreSurfaceActive = state;

		if (showDebugPrint) {
			print ("ignore surface active " + state);
		}
	}

	public void setUseRemoteEventState (bool state)
	{
		useRemoteEvent = state;
	}

	public void setUseRemoteEventOnWeaponState (bool state)
	{
		useRemoteEventOnWeapon = state;
	}

	public void checkEventOnSurfaceDetected ()
	{
		if (useEventOnSurfaceDetected) {
			eventOnSurfaceDeteceted.Invoke ();
		}
	}

	public void checkEventOnThrowWeapon ()
	{
		if (useEventOnThrowWeapon) {
			eventOnThrowWeapon.Invoke ();

			if (showDebugPrint) {
				print ("event on throw weapon activated " + gameObject.name);
			}
		}
	}

	public void checkEventOnReturnWeapon ()
	{
		if (useEventOnReturnWeapon) {
			eventOnReturnWeapon.Invoke ();

			if (showDebugPrint) {
				print ("event on return weapon activated " + gameObject.name);
			}
		}
	}
}