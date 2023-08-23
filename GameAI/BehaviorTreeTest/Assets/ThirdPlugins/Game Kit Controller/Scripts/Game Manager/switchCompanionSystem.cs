using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class switchCompanionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool switchCompanionEnabled = true;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventToSetCharacterAsAI;
	public UnityEvent eventToSetCharacterAsPlayer;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController playerControllerManager;
	public playerCharactersManager mainPlayerCharactersManager;

	public friendListManager mainfriendListManager;


	public void activateEventToSetCharacterAsAI ()
	{
		eventToSetCharacterAsAI.Invoke ();
	}

	public void activateEventToSetCharacterAsPlayer ()
	{
		eventToSetCharacterAsPlayer.Invoke ();
	}

	public void inputSwitchToNextCharacter ()
	{
		if (!switchCompanionEnabled) {
			return;
		}

		if (!playerControllerManager.isPlayerMenuActive () &&
		    (!playerControllerManager.isUsingDevice () || playerControllerManager.isPlayerDriving ()) &&
		    !playerControllerManager.isGamePaused ()) {

			mainPlayerCharactersManager.inputSetNextCharacterToControl ();
		}
	}
}
