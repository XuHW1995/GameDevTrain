using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generalStatComponent : statType
{
	[Header ("Custom Settings")]
	[Space]

	public int currentStatAmount;


	public override void eventToInitializeStat (float newValue)
	{
		currentStatAmount = (int)newValue;
	}

	public override void eventToIncreaseStat (float newValue)
	{
		currentStatAmount += (int)newValue;
	}

	public override void eventToUseStat (float newValue)
	{
		currentStatAmount -= (int)newValue;
	}

	public override void eventToAddAmount (float newValue)
	{
		currentStatAmount += (int)newValue;
	}

	public override void eventToInitializeBoolStat (bool newValue)
	{

	}

	public override void eventToActivateBoolStat (bool newValue)
	{

	}

	public override void eventToInitializeStatOnComponent ()
	{

	}

	public override void eventToInitializeBoolStatOnComponent ()
	{

	}

	public override void updateStatAmountWithoutUpdatingStatManager (int statId, float amount)
	{
		currentStatAmount = (int)amount;
	}
}

