using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateCharacterGravityTriggerSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool updateGravityEnabled = true;

	public string playerTag = "Player";

	public bool ignoreUpdateGravityIfCharacterHasDifferentDirection;

	public float maxGravityAngleDifference;

	public bool ignoreRecalculateSurface;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform gravityDirectionTransform;


	void OnTriggerEnter (Collider col)
	{
		if (!updateGravityEnabled) {
			return;
		}

		checkTriggerType (col, true);
	}

	void OnTriggerExit (Collider col)
	{
		if (!updateGravityEnabled) {
			return;
		}

		checkTriggerType (col, false);
	}

	public void checkTriggerType (Collider col, bool isEnter)
	{
		if (col == null) {
			return;
		}

		if (col.isTrigger) {
			return;
		}

		checkObjectOnGravitySystem (col.gameObject, isEnter);
	}

	public void checkObjectOnGravitySystem (GameObject objectToCheck, bool isEnter)
	{
		//if the player is not driving, stop the gravity power

		bool checkResult = objectToCheck.CompareTag (playerTag);

		if (checkResult) {
			gravitySystem currentGravitySystem = objectToCheck.GetComponent<gravitySystem> ();

			if (currentGravitySystem != null) {
				if (gravityDirectionTransform == null) {
					gravityDirectionTransform = transform;
				}

				bool updateGravityResult = true;

				if (ignoreUpdateGravityIfCharacterHasDifferentDirection) {
					if (currentGravitySystem.getCurrentNormal () != gravityDirectionTransform.up) {
						float currentAngleDifference = Vector3.Angle (currentGravitySystem.getCurrentNormal (), gravityDirectionTransform.up);

						if (Mathf.Abs (currentAngleDifference) > maxGravityAngleDifference) {

							updateGravityResult = false;
						}
					}
				}

				if (updateGravityResult) {
					currentGravitySystem.setUpdateCurrentNormalByExternalTransformState (isEnter, gravityDirectionTransform, ignoreRecalculateSurface);
				}
			}
		}
	}
}
