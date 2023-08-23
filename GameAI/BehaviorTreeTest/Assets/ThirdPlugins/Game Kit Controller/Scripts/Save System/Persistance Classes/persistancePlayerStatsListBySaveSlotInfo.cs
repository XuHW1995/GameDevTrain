using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class persistancePlayerStatsListBySaveSlotInfo
{
	public int saveNumber;
	public List<persistancePlayerStatInfo> playerStatsList = new List<persistancePlayerStatInfo> ();
}

[System.Serializable]
public class persistancePlayerStatInfo
{
	public int playerID;
	public List<persistanceStatInfo> statsList = new List<persistanceStatInfo> ();
}

[System.Serializable]
public class persistanceStatInfo
{
	public float currentValue;

	public float extraCurrentValue;

	public bool currentBoolState;

	public persistanceStatInfo (persistanceStatInfo obj)
	{
		currentValue = obj.currentValue;

		currentValue = obj.extraCurrentValue;

		currentBoolState = obj.currentBoolState;
	}

	public persistanceStatInfo ()
	{
		
	}
}