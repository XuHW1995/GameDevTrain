using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToAttack : customOrderBehavior
{
	[Header ("Custom Settings")]
	[Space]

	public List<string> tagToLocate = new List<string> ();

	public bool checkIfTargetOnLockOnViewEnabled = true;


	public override void activateOrder (Transform character)
	{


	}


	public override Transform getCustomTarget (Transform character, Transform orderOwner)
	{
		if (canAIAttack (character)) {

			if (checkIfTargetOnLockOnViewEnabled) {
				playerComponentsManager currentPlayerComponentsManager = orderOwner.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					playerCamera currentPlayerCamera = currentPlayerComponentsManager.getPlayerCamera ();

					if (currentPlayerCamera != null) {
						if (currentPlayerCamera.isPlayerLookingAtTarget ()) {
							Transform currentTarget = currentPlayerCamera.getCurrentTargetToLook ();

							if (currentTarget != null) {
								if (applyDamage.isCharacter (currentTarget.gameObject)) {
									return  currentTarget;
								}
							}
						}
					}
				}
			}
				
			return getClosestEnemy (orderOwner);
		}

		return null;
	}

	public bool canAIAttack (Transform AIFriend)
	{
		bool canAttack = false;

		findObjectivesSystem currentFindObjectivesSystem = AIFriend.GetComponentInChildren<findObjectivesSystem> ();

		if (currentFindObjectivesSystem != null) {
			if (currentFindObjectivesSystem.attackType != findObjectivesSystem.AIAttackType.none) {
				canAttack = true;
			}
		}

		return canAttack;
	}

	public Transform getClosestEnemy (Transform centerPointTransform)
	{
		Vector3 centerPosition = centerPointTransform.position;

		List<GameObject> fullEnemyList = new List<GameObject> ();

		GameObject closestEnemy;

		for (int i = 0; i < tagToLocate.Count; i++) {
			GameObject[] enemiesList = GameObject.FindGameObjectsWithTag (tagToLocate [i]);

			fullEnemyList.AddRange (enemiesList);
		}

		List<GameObject> closestEnemyList = new List<GameObject> ();

		for (int j = 0; j < fullEnemyList.Count; j++) {
			if (!applyDamage.checkIfDead (fullEnemyList [j])) {
				closestEnemyList.Add (fullEnemyList [j]);
			}
		}

		if (closestEnemyList.Count > 0) {
			float distance = Mathf.Infinity;

			int index = -1;

			for (int j = 0; j < closestEnemyList.Count; j++) {
				float currentDistance = GKC_Utils.distance (closestEnemyList [j].transform.position, centerPosition);

				if (currentDistance < distance) {
					distance = currentDistance;
					index = j;
				}
			}

			if (index != -1) {
				closestEnemy = closestEnemyList [index];

				return closestEnemy.transform;
			}
		}

		return null;
	}
}
