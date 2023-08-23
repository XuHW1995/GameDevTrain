using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToWait : customOrderBehavior
{
	public override void activateOrder (Transform character)
	{


	}


	public override Transform getCustomTarget (Transform character, Transform orderOwner)
	{
		return orderOwner;
	}
}