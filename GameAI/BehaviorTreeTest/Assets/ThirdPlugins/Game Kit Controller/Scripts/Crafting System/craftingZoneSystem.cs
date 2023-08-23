using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class craftingZoneSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string tagToCheck;
	public bool setCraftingZoneEnabledState;
	public bool craftingZoneEnabledState;

	GameObject currentPlayer;

	void OnTriggerEnter (Collider col)
	{
		checkTriggerInfo (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		checkTriggerInfo (col, false);
	}

	public void checkTriggerInfo (Collider col, bool isEnter)
	{
		if (!setCraftingZoneEnabledState) {
			return;
		}

		if (!col.gameObject.CompareTag (tagToCheck)) {
			return;
		}

		if (isEnter) {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				craftingSystem currentCraftingSystem = currentPlayerComponentsManager.getCraftingSystem ();

				if (currentCraftingSystem != null) {
					currentCraftingSystem.setPlacementActivePausedState (!craftingZoneEnabledState);
				}
			}
		} else {
			currentPlayer = col.gameObject;

			playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (currentPlayerComponentsManager != null) {
				craftingSystem currentCraftingSystem = currentPlayerComponentsManager.getCraftingSystem ();

				if (currentCraftingSystem != null) {
					currentCraftingSystem.setOriginalPlacementActivePausedState ();
				}
			}
		}
	}
}
