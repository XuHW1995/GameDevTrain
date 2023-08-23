using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeShieldObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useObjectDurabilityOnBlock;

	public float durabilityUsedOnBlock = 1;

	[Space]
	[Header ("Debug")]
	[Space]

	public GameObject currentCharacter;

	[Space]
	[Header ("Components")]
	[Space]

	public durabilityInfo mainDurabilityInfo;


	public void setCurrentCharacter (GameObject newObject)
	{
		currentCharacter = newObject;
	}

	public GameObject getCurrentCharacter ()
	{
		return currentCharacter;
	}

	public bool checkDurabilityOnBlockWithShield (float extraMultiplier)
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.addOrRemoveDurabilityAmountToObjectByName (-durabilityUsedOnBlock * extraMultiplier, false);

			if (mainDurabilityInfo.durabilityAmount <= 0) {
				return true;
			}
		}

		return false;
	}

	public void updateDurabilityAmountState ()
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.updateDurabilityAmountState ();
		}
	}

	public float getDurabilityAmount ()
	{
		if (useObjectDurabilityOnBlock) {
			return mainDurabilityInfo.getDurabilityAmount ();
		}

		return -1;
	}

	public void initializeDurabilityValue (float newAmount)
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.initializeDurabilityValue (newAmount);
		}
	}

	public void repairObjectFully ()
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.repairObjectFully ();
		}
	}

	public void breakFullDurabilityOnCurrentWeapon ()
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.breakFullDurability ();
		}
	}

	public void setUseObjectDurabilityOnBlockValue (bool newValue)
	{
		useObjectDurabilityOnBlock = newValue;
	}

	public void setDurabilityUsedOnBlockValue (float newValue)
	{
		durabilityUsedOnBlock = newValue;
	}

	public void setUseObjectDurabilityOnBlockValueFromEditor (bool newValue)
	{
		setUseObjectDurabilityOnBlockValue (newValue);

		updateComponent ();
	}

	public void setDurabilityUsedOnBlockValueFromEditor (float newValue)
	{
		setDurabilityUsedOnBlockValue (newValue);

		updateComponent ();
	}

	public void setObjectNameFromEditor (string newName)
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.setObjectNameFromEditor (newName);
		}
	}

	public void setDurabilityAmountFromEditor (float newValue)
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.setDurabilityAmountFromEditor (newValue);
		}
	}

	public void setMaxDurabilityAmountFromEditor (float newValue)
	{
		if (useObjectDurabilityOnBlock) {
			mainDurabilityInfo.setMaxDurabilityAmountFromEditor (newValue);
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Updating melee shield object info" + gameObject.name, gameObject);
	}
}
