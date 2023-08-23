using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehavior : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool orderEnabled = true;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;


	public virtual void activateOrder (Transform character)
	{
	
	}

	public virtual void activateOrder (Transform character, Transform orderOwner)
	{

	}

	public virtual Transform getCustomTarget (Transform character, Transform orderOwner)
	{


		return null;
	}

	public virtual bool checkConditionToShowOrderButton (Transform character)
	{

		return false;
	}
}