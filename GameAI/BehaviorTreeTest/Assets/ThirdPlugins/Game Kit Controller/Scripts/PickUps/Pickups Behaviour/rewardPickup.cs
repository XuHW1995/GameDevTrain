using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rewardPickup : pickupType
{
	public objectExperienceSystem mainObjectExperienceSystem;

	public override bool checkIfCanBePicked ()
	{
		GameObject character = gameObject;

		if (finderIsPlayer) {
			character = player;

			canPickCurrentObject = true;

			amountTaken = mainPickupObject.amount;
		} 

		return canPickCurrentObject;
	}

	public override void confirmTakePickup ()
	{
		if (finderIsPlayer) {
			mainObjectExperienceSystem.sendExperienceToPlayer (player);
		} 

		if (useCustomPickupMessage) {
			showPickupTakenMessage (amountTaken);
		} else {
			showPickupTakenMessage ("Rewards Obtained x " + amountTaken);
		}

		mainPickupObject.playPickupSound ();

		mainPickupObject.removePickupFromLevel ();
	}
}