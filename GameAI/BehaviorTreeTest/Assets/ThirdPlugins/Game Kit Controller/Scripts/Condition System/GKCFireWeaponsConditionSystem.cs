using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCFireWeaponsConditionSystem : GKCConditionInfo
{
	[Header ("Custom Settings")]
	[Space]

	public bool checkIfCarryinWeapon;

	public bool checkIfCarryingWeaponByName;
	public string weaponCarriedName;

	public bool checkBothConditionsOnCoroutineUpdate;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool currentConditionCompleteChecked;
	public bool currentConditionNotCompleteChecked;

	playerWeaponsManager mainPlayerWeaponsManager;

	bool mainPlayerWeaponsManagerLocated;


	public override void checkIfConditionComplete ()
	{
		if (!checkIfPlayerAssigned ()) {
			return;
		}

		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);
	}

	public override void checkFunctionOnTriggerExit ()
	{
		base.checkFunctionOnTriggerExit ();


		mainPlayerWeaponsManager = null;

		mainPlayerWeaponsManagerLocated = false;

		currentConditionNotCompleteChecked = false;

		currentConditionCompleteChecked = false;
	}

	public override void checkConditionOnUpdateState ()
	{
		bool conditionResult = checkConditionResult ();

		if (checkBothConditionsOnCoroutineUpdate) {
			if (conditionResult) {
				if (!currentConditionCompleteChecked) {
					setConditionResult (true);

					currentConditionCompleteChecked = true;

					currentConditionNotCompleteChecked = false;
				}
			} else {
				if (!currentConditionNotCompleteChecked) {
					setConditionResult (false);

					currentConditionNotCompleteChecked = true;

					currentConditionCompleteChecked = false;
				}
			}
		} else {
			if (conditionResult) {
				setConditionResult (true);
			}
		}
	}

	bool checkConditionResult ()
	{
		bool conditionResult = false;

		if (!mainPlayerWeaponsManagerLocated) {
			playerComponentsManager mainPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				mainPlayerWeaponsManager = mainPlayerComponentsManager.getPlayerWeaponsManager ();

				mainPlayerWeaponsManagerLocated = mainPlayerWeaponsManager != null;
			}
		}

		if (mainPlayerWeaponsManagerLocated) {
			bool currentConditionState = true;

			if (checkIfCarryinWeapon) {
				if (!mainPlayerWeaponsManager.isPlayerCarringWeapon ()) {
					currentConditionState = false;
				}

				if (checkIfCarryingWeaponByName) {
					if (currentConditionState) {
						string currentWeaponName = mainPlayerWeaponsManager.getCurrentWeaponName ();

						if (currentWeaponName != null && currentWeaponName != "") {
							if (!currentWeaponName.Equals (weaponCarriedName)) {
								currentConditionState = false;
							}
						} else {
							currentConditionState = false;
						}
					}
				}
			}

			conditionResult = currentConditionState;
		}

		return conditionResult;
	}
}