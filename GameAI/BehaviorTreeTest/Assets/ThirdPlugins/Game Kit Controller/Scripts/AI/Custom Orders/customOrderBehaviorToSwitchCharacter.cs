using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToSwitchCharacter : customOrderBehavior
{
	public playerCharactersManager mainPlayerCharactersManager;

	public override void activateOrder (Transform character)
	{


	}


	public override Transform getCustomTarget (Transform character, Transform orderOwner)
	{
		return character;
	}


	public override bool checkConditionToShowOrderButton (Transform character)
	{
		if (character == null) {
			return false;
		}

		if (mainPlayerCharactersManager.isCharacterInList (character.gameObject)) {
			return true;
		}

		return false;
	}
}