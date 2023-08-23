using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeWeaponOnInventory : objectOnInventory
{
	[Header ("Custom Settings")]
	[Space]

	public meleeWeaponPickup mainMeleeWeaponPickup;

	public bool checkEventsOnChangeEquipState = true;


//	string weaponName;

	public override void eventOnPickObject (GameObject currentPlayer)
	{
//		weaponName = mainMeleeWeaponPickup.weaponName;


	}

	public override void eventOnDropObject (GameObject currentPlayer)
	{
//		weaponName = mainMeleeWeaponPickup.weaponName;


	}

	public override bool setObjectEquippedStateOnInventory (GameObject currentPlayer, bool state)
	{
//		weaponName = mainMeleeWeaponPickup.weaponName;

		if (checkEventsOnChangeEquipState) {
			checkRemoteEvents (currentPlayer);

			checkRemoteEventsOnSetObjectEquipState (currentPlayer, state);
		}

		return true;
	}
}