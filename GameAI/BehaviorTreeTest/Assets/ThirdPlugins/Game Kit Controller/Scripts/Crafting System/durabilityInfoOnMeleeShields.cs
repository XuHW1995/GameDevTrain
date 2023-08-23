using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class durabilityInfoOnMeleeShields : durabilityInfo
{
	public override void updateDurabilityAmountState ()
	{
		base.updateDurabilityAmountState ();


	}

	public override void setFullDurabilityState ()
	{
		base.setFullDurabilityState ();


	}

	public override void getMainCharacterGameObject ()
	{
		meleeShieldObjectSystem currentMeleeShieldObjectSystem = GetComponent<meleeShieldObjectSystem> ();
		
		if (currentMeleeShieldObjectSystem != null) {
			mainCharacterGameObject = currentMeleeShieldObjectSystem.getCurrentCharacter ();
		}
	}
}
