using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class externalStatsManager : MonoBehaviour
{
	public playerStatsSystem mainPlayerStatsSystem;

	public bool statsManagerEnabled = true;

	string temporalStatName;

	public void setTemporalStatName (string statName)
	{
		if (statsManagerEnabled) {
			temporalStatName = statName;
		}
	}

	public void updatePlayerStateOfTemporalStat (float statExtraValue)
	{
		if (statsManagerEnabled) {
			if (temporalStatName != "") {
				updateStatValueExternally (temporalStatName, statExtraValue);

				temporalStatName = "";
			}
		}
	}

	public void updateStatValueExternally (string statName, float newValue)
	{
		if (statsManagerEnabled) {
			mainPlayerStatsSystem.updateStatValueExternally (statName, newValue);
		}
	}

	public void updateStatValueExternally (int statIndex, float newValue)
	{
		if (statsManagerEnabled) {
			mainPlayerStatsSystem.updateStatValueExternally (statIndex, newValue);
		}
	}
}
