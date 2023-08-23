using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleFireWeaponsConditionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool conditionCheckEnabled;

	public bool checkIfCurrentWeapon;

	public bool checkIfAimingWeapon;

	public bool checkIfThirdPersonActive;
	public bool checkIfFirstPersonActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnConditionComplete;

	public UnityEvent eventOnConditionNotComplete;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public IKWeaponSystem mainIKWeaponSystem;

	public void checkIfConditionComplete ()
	{
		if (!conditionCheckEnabled) {
			return;
		}

		bool conditionResult = checkConditionResult ();

		setConditionResult (conditionResult);
	}

	bool checkConditionResult ()
	{
		bool conditionResult = false;

		bool currentConditionState = true;

		if (checkIfCurrentWeapon) {
			if (!mainIKWeaponSystem.isCurrentWeapon ()) {
				currentConditionState = false;
			}

		}

		if (checkIfAimingWeapon) {
			if (!mainIKWeaponSystem.isAimingWeapon ()) {
				currentConditionState = false;
			}
		}
			
		if (checkIfThirdPersonActive) {
			if (mainIKWeaponSystem.isFirstPersonActive ()) {
				currentConditionState = false;
			}
		}

		if (checkIfFirstPersonActive) {
			if (!mainIKWeaponSystem.isFirstPersonActive ()) {
				currentConditionState = false;
			}
		}

		conditionResult = currentConditionState;

		return conditionResult;
	}

	public void setConditionResult (bool state)
	{
		if (showDebugPrint) {
			print ("Condition result: " + state);
		}

		if (state) {
			eventOnConditionComplete.Invoke ();
		} else {
			eventOnConditionNotComplete.Invoke ();
		}
	}
}