using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carryPhysicallyObjectFromInventory : objectOnInventory
{
	[Header ("Main Settings")]
	[Space]

	public grabPhysicalObjectSystem mainGrabPhysicalObjectSystem;


	public override void carryPhysicalObjectFromInventory (GameObject currentPlayer)
	{
		mainGrabPhysicalObjectSystem.setCurrentPlayer (currentPlayer);

		grabObjects currentGrabObjects = currentPlayer.GetComponent<grabObjects> ();

		if (currentGrabObjects != null) {
			currentGrabObjects.getClosestPhysicalObjectToGrab ();
			currentGrabObjects.grabObject ();
		}
	}
}
