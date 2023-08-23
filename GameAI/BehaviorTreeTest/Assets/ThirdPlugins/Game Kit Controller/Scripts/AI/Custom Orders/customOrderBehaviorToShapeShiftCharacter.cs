using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToShapeShiftCharacter : customOrderBehavior
{
	[Header ("Main Settings")]
	[Space]

	public bool changeEnabled = true;

	[Space]
	[Header ("Generic Character Settings")]
	[Space]

	public bool setRegularCharacter;

	public string customCharacterNameToConfigure;

	public override void activateOrder (Transform character)
	{
		if (!changeEnabled) {
			return;
		}

		if (character == null) {
			return;
		}

		playerComponentsManager currentPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			customCharacterControllerManager currentCustomCharacterControllerManager = currentPlayerComponentsManager.getCustomCharacterControllerManager ();

			if (currentCustomCharacterControllerManager != null) {
				if (setRegularCharacter) {
					if (currentCustomCharacterControllerManager.isCustomCharacterControllerActive ()) {
						currentCustomCharacterControllerManager.disableCustomCharacterControllerState ();
					}
				} else {
					currentCustomCharacterControllerManager.setCustomCharacterControllerState (customCharacterNameToConfigure);
				}
			}
		}
	}
}
