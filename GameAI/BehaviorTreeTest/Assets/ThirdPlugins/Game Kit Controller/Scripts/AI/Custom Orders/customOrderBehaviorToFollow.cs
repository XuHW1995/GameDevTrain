using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToFollow : customOrderBehavior
{
	public override void activateOrder (Transform character)
	{


	}


	public override Transform getCustomTarget (Transform character, Transform orderOwner)
	{
		return orderOwner;
	}
}
