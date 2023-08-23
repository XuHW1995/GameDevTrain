using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class consumableInventoryPrefabCreationSystem : inventoryPrefabCreationSystem
{
	public override void createInventoryPrefabObject ()
	{
		generalPickup currentGeneralPickup = GetComponent<generalPickup> ();

		inventoryObject currentInventoryObject = GetComponentInChildren<inventoryObject> ();

		string newName = currentInventoryObject.inventoryObjectInfo.Name;

		currentGeneralPickup.setConsumableName (newName);

		currentGeneralPickup.setObjectTakenAsPickupMessage (newName + " x -AMOUNT-");

		currentGeneralPickup.setObjectTakenAsInventoryMessage (newName + " x -AMOUNT- Stored");
	}
}
