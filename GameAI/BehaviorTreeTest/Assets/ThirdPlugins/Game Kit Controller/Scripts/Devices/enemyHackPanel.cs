﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class enemyHackPanel : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public GameObject hackPanel;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingDevice;

	[Space]
	[Header ("Components")]
	[Space]

	public hackTerminal hackTerinalManager;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent startHackFunction = new UnityEvent ();
	public UnityEvent correctHackedlyFunction = new UnityEvent ();
	public UnityEvent incorrectlyHackedFunction = new UnityEvent ();

	void Start ()
	{
		if (hackTerinalManager == null) {
			hackTerinalManager = hackPanel.GetComponent<hackTerminal> ();
		}
	}

	//activates a function to activate the hacking of a turret, but it can be used to any other type of device
	public void activateHackPanel (bool state)
	{
		usingDevice = state;

		if (usingDevice) {
			if (startHackFunction.GetPersistentEventCount () > 0) {
				startHackFunction.Invoke ();
			}

			hackTerinalManager.activeHack ();
		} else {
			hackTerinalManager.stopHacking ();
		}
	}

	public void setCorrectlyHackedState ()
	{
		setHackResult (true);
	}

	public void setIncorrectlyHackedState ()
	{
		setHackResult (false);
	}

	//send the hack result to the enemy
	public void setHackResult (bool state)
	{
		if (state) {
			if (correctHackedlyFunction.GetPersistentEventCount () > 0) {
				correctHackedlyFunction.Invoke ();
			}
		} else {
			if (incorrectlyHackedFunction.GetPersistentEventCount () > 0) {
				incorrectlyHackedFunction.Invoke ();
			}
		}
	}

	//close the hack panel once the enemy has been hacked
	public void disablePanelHack ()
	{
		StartCoroutine (disablePanelHackCoroutine ());
	}

	IEnumerator disablePanelHackCoroutine ()
	{
		yield return new WaitForSeconds (1);

		hackTerinalManager.moveHackTerminal (false);
	}
}