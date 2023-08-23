using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToHide : customOrderBehavior
{
	AIHidePositionsManager mainAIHidePositionsManager;

	public override void activateOrder (Transform character)
	{


	}


	public override Transform getCustomTarget (Transform character, Transform orderOwner)
	{
		return getClosestHidePosition (character);
	}

	public Transform getClosestHidePosition (Transform AIFriend)
	{
		if (mainAIHidePositionsManager == null) {
			mainAIHidePositionsManager = FindObjectOfType<AIHidePositionsManager> ();
		}

		if (mainAIHidePositionsManager != null) {
			if (mainAIHidePositionsManager.hidePositionList.Count > 0) {
				float distance = Mathf.Infinity;

				int index = -1;

				for (int j = 0; j < mainAIHidePositionsManager.hidePositionList.Count; j++) {
					float currentDistance = GKC_Utils.distance (AIFriend.position, mainAIHidePositionsManager.hidePositionList [j].position);

					if (currentDistance < distance) {
						distance = currentDistance;

						index = j;
					}
				}

				return mainAIHidePositionsManager.hidePositionList [index];
			}
		}

		return null;
	}
}
