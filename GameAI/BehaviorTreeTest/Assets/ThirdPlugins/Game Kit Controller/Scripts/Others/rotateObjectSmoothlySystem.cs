using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateObjectSmoothlySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool rotateObjectSystemEnabled = true;

	public Transform transformToRotate;

	public bool avoidRotationIfAlreadyInProcess;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool rotationInProcess;

	[Space]
	[Header ("Rotation State List Settings")]
	[Space]

	public List<objectRotationStateInfo> objectRotationStateInfoList = new List<objectRotationStateInfo> ();

	objectRotationStateInfo currentObjectRotationStateInfo;

	Coroutine rotationCoroutine;


	public void rotateObjectToRandomRotationState ()
	{
		if (!rotateObjectSystemEnabled) {
			return;
		}

		if (avoidRotationIfAlreadyInProcess) {
			if (rotationInProcess) {
				return;
			}
		}

		stopRotateObjectToRotationCoroutine ();

		int randomIndex = Random.Range (0, objectRotationStateInfoList.Count - 1);

		rotationCoroutine = StartCoroutine (rotateObjectToRotationCoroutine (objectRotationStateInfoList [randomIndex].Name));
	}

	public void rotateObjectToRotation (string rotationName)
	{
		if (!rotateObjectSystemEnabled) {
			return;
		}

		if (avoidRotationIfAlreadyInProcess) {
			if (rotationInProcess) {
				return;
			}
		}

		stopRotateObjectToRotationCoroutine ();

		rotationCoroutine = StartCoroutine (rotateObjectToRotationCoroutine (rotationName));
	}

	public void stopRotateObjectToRotationCoroutine ()
	{
		if (rotationCoroutine != null) {
			StopCoroutine (rotationCoroutine);
		}

		rotationInProcess = false;
	}

	IEnumerator rotateObjectToRotationCoroutine (string rotationName)
	{
		rotationInProcess = true;

		bool isNewRotation = true;

		int rotationStateIndex = -1;

		for (int i = 0; i < objectRotationStateInfoList.Count; i++) {
			if (objectRotationStateInfoList [i].Name.Equals (rotationName)) {
				if (objectRotationStateInfoList [i].isCurrentRotation) {
					isNewRotation = false;
				} else {
					objectRotationStateInfoList [i].isCurrentRotation = true;
				}

				rotationStateIndex = i;

				currentObjectRotationStateInfo = objectRotationStateInfoList [i];
			} else {
				objectRotationStateInfoList [i].isCurrentRotation = false;
			}
		}

		bool setRotationResult = false;

		if ((isNewRotation || currentObjectRotationStateInfo.ignoreIfCurrentRotation) && rotationStateIndex > -1) {
			setRotationResult = true;
		}

		if (setRotationResult) {

			if (transformToRotate == null) {
				transformToRotate = transform;
			}

			float rotationSpeed = currentObjectRotationStateInfo.rotationSpeed;

			float translateTimer = 0;

			float rotationTimer = 0;

			bool targetReached = false;

			float angleDifference = 0;

			Quaternion targetRotation = Quaternion.identity;

			Vector3 targetEuler = currentObjectRotationStateInfo.targetEuler;

			if (currentObjectRotationStateInfo.addRotationToCurrentValues) {
				Quaternion rotationAmount = Quaternion.Euler (targetEuler);
				targetRotation = transformToRotate.rotation * rotationAmount;

			} else {
				targetRotation = Quaternion.Euler (targetEuler);
			}

			if (showDebugPrint) {
				print ("target rotation " + targetEuler);
			}

			while (!targetReached) {
				translateTimer += Time.deltaTime / rotationSpeed;

				transformToRotate.rotation = Quaternion.Lerp (transformToRotate.rotation, targetRotation, translateTimer);

				rotationTimer += Time.deltaTime;

				angleDifference = Quaternion.Angle (transformToRotate.rotation, targetRotation);

				if (rotationTimer > 5) {
					targetReached = true;
				}

				if (angleDifference < 0.001f) {
					targetReached = true;
				}

				yield return null;
			}

			transformToRotate.rotation = targetRotation;
		}

		rotationInProcess = false;
	}

	[System.Serializable]
	public class objectRotationStateInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public Vector3 targetEuler;
		public float rotationSpeed;
	
		public bool addRotationToCurrentValues;

		public bool ignoreIfCurrentRotation = true;

		[Space]
		[Header ("Debug")]
		[Space]

		public bool isCurrentRotation;
	}
}
