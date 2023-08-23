using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addForceToObjectSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool affectToPlayerOnlyOnAIr;
	public bool affectOnlyOnParagliderActive;

	public float forceAmountOnParaglider;

	public float forceAmountCharacters;
	public float forceAmountRegularObjects;

	public float forceAmountVehicles;

	public ForceMode forceModeCharacters;
	public ForceMode forceModeRegularObjects;

	public ForceMode forceModeVehicles;

	public bool addForceInUpdate;

	[Space]
	[Header ("Detection Settings")]
	[Space]

	public bool checkPlayerEnabled = true;
	public string playerTag = "Player";

	public bool checkVehiclesEnabled;

	public bool useLayerToCheck;
	public LayerMask layerToCheck;

	[Space]
	[Header ("Wind/Air Settings")]
	[Space]

	public bool checkWindObjectStateEnabled;

	public bool ignoreForcesOnVehicleIfWindActive;

	public Transform windDirectionTransform;

	public float windForce;

	public bool updateWindDirectionOnUpdate;

	[Space]
	[Header ("Debug")]
	[Space]

	public List<rigidbodyInfo> rigidbodyInfoList = new List<rigidbodyInfo> ();

	public bool objectsDetected;

	public bool pauseUpdateForces;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform forceDirection;


	Vector3 forceDirectionForward;

	rigidbodyInfo currentRigidbodyInfo;


	void Update ()
	{
		if (pauseUpdateForces) {
			return;
		}

		if (updateWindDirectionOnUpdate) {
			if (objectsDetected) {
				forceDirectionForward = forceDirection.forward;

				for (int i = 0; i < rigidbodyInfoList.Count; i++) {

					currentRigidbodyInfo = rigidbodyInfoList [i];

					if (currentRigidbodyInfo.mainRigidbody != null) {
						if (currentRigidbodyInfo.isPlayer) {
							if (affectToPlayerOnlyOnAIr) {
								bool currentPlayerOnAir = !currentRigidbodyInfo.mainExternalControllerBehavior.isCharacterOnGround ();

								if (currentPlayerOnAir) {
									if (affectOnlyOnParagliderActive) {
										currentRigidbodyInfo.mainExternalControllerBehavior.updateExternalForceActiveState (forceDirectionForward, forceAmountOnParaglider);
									} else {
										currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountCharacters, forceModeCharacters);
									}
								}
							} else {
								currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountCharacters, forceModeCharacters);
							}
						} else if (currentRigidbodyInfo.isVehicle) {
							if (!currentRigidbodyInfo.ignoreForcesOnVehicleIfWindActive) {
								currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountVehicles, forceModeVehicles);
							}
						} else {
							currentRigidbodyInfo.mainRigidbody.AddForce (forceDirectionForward * forceAmountRegularObjects, forceModeRegularObjects);
						}

						if (updateWindDirectionOnUpdate) {
							if (currentRigidbodyInfo.applyWindOnObject) {
								currentRigidbodyInfo.mainWindOnObjectState.setWindDirectionValues (windDirectionTransform.forward);

								currentRigidbodyInfo.mainWindOnObjectState.setWindForceValue (windForce);
							}
						}
					}
				}
			}
		}
	}

	public void setPauseUpdateForcesState (bool state)
	{
		pauseUpdateForces = state;
	}

	public void setWindDetectedStateOnAllObjects (bool state)
	{
		if (updateWindDirectionOnUpdate) {
			for (int i = 0; i < rigidbodyInfoList.Count; i++) {

				currentRigidbodyInfo = rigidbodyInfoList [i];

				if (currentRigidbodyInfo.mainRigidbody != null) {
				
					if (currentRigidbodyInfo.applyWindOnObject) {
						currentRigidbodyInfo.mainWindOnObjectState.setWindDetectedState (state);
					}
				}
			}
		}
	}

	public void addNewObject (GameObject newObject)
	{
		if (useLayerToCheck) {
			bool checkObjectResult = false;

			if ((1 << newObject.layer & layerToCheck.value) == 1 << newObject.layer) {
				checkObjectResult = true;
			}

			if (!checkObjectResult) {
				return;
			}
		}

		if (!checkPlayerEnabled) {
			if (newObject.CompareTag (playerTag)) {
				return;
			}
		}
			
		bool isVehicle = false;

		if (checkVehiclesEnabled) {
			GameObject currentVehicle = applyDamage.getVehicle (newObject);

			if (currentVehicle != null) {
				isVehicle = true;

				newObject = currentVehicle;
			}
		}

		Rigidbody mainRigidbody = newObject.GetComponent<Rigidbody> ();

		if (mainRigidbody != null) {
			for (int i = 0; i < rigidbodyInfoList.Count; i++) {
				if (rigidbodyInfoList [i].mainObject == newObject) {
					return;
				}
			}

			rigidbodyInfo newRigidbodyInfo = new rigidbodyInfo ();

			newRigidbodyInfo.mainObject = newObject;
			newRigidbodyInfo.mainRigidbody = mainRigidbody;

			if (newObject.CompareTag (playerTag)) {
				newRigidbodyInfo.isPlayer = true;

				playerComponentsManager currentPlayerComponentsManager = newObject.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					externalControllerBehavior currentExternalControllerBehavior = currentPlayerComponentsManager.getParagliderSystem ();

					if (currentExternalControllerBehavior != null) {

						newRigidbodyInfo.mainExternalControllerBehavior = currentExternalControllerBehavior;

						newRigidbodyInfo.mainExternalControllerBehavior.setExternalForceActiveState (true);
					}
				}
			}

			newRigidbodyInfo.isVehicle = isVehicle;

			if (checkWindObjectStateEnabled) {
				windOnObjectState currentWindOnObjectState = newObject.GetComponent<windOnObjectState> ();

				if (currentWindOnObjectState != null) {
					newRigidbodyInfo.mainWindOnObjectState = currentWindOnObjectState;

					currentWindOnObjectState.setWindDetectedState (true);
					
					currentWindOnObjectState.setWindDirectionValues (windDirectionTransform.forward);

					currentWindOnObjectState.setWindForceValue (windForce);

					newRigidbodyInfo.applyWindOnObject = true;

					if (isVehicle) {
						if (ignoreForcesOnVehicleIfWindActive) {
							newRigidbodyInfo.ignoreForcesOnVehicleIfWindActive = true;
						}
					}
				}
			}

			rigidbodyInfoList.Add (newRigidbodyInfo);

			objectsDetected = true;
		}
	}

	public void removeObject (GameObject objectToRemove)
	{
		if (useLayerToCheck) {
			bool checkObjectResult = false;

			if ((1 << objectToRemove.layer & layerToCheck.value) == 1 << objectToRemove.layer) {
				checkObjectResult = true;
			}

			if (!checkObjectResult) {
				return;
			}
		}

		if (checkVehiclesEnabled) {
			GameObject currentVehicle = applyDamage.getVehicle (objectToRemove);

			if (currentVehicle != null) {
				objectToRemove = currentVehicle;
			}
		}

		for (int i = rigidbodyInfoList.Count - 1; i >= 0; i--) {
			if (rigidbodyInfoList [i] == null) {
				rigidbodyInfoList.RemoveAt (i);
			}
		}

		if (rigidbodyInfoList.Count == 0) {
			objectsDetected = false;
		}

		for (int i = 0; i < rigidbodyInfoList.Count; i++) {
			currentRigidbodyInfo = rigidbodyInfoList [i];

			if (currentRigidbodyInfo.mainObject == objectToRemove) {

				if (currentRigidbodyInfo.isPlayer) {
					if (affectOnlyOnParagliderActive && currentRigidbodyInfo.mainExternalControllerBehavior != null) {
						currentRigidbodyInfo.mainExternalControllerBehavior.setExternalForceActiveState (false);
					} 
				}

				if (checkWindObjectStateEnabled) {
					if (currentRigidbodyInfo.applyWindOnObject) {
						currentRigidbodyInfo.mainWindOnObjectState.setWindDetectedState (false);

						currentRigidbodyInfo.mainWindOnObjectState.setWindDirectionValues (Vector3.zero);

						currentRigidbodyInfo.mainWindOnObjectState.setWindForceValue (0);
					}
				}

				rigidbodyInfoList.RemoveAt (i);

				if (rigidbodyInfoList.Count == 0) {
					objectsDetected = false;
				}

				return;
			}
		}
	}

	public void removeAllObjects ()
	{
		for (int i = rigidbodyInfoList.Count - 1; i >= 0; i--) {
			if (rigidbodyInfoList [i] == null) {
				rigidbodyInfoList.RemoveAt (i);
			}
		}

		for (int i = 0; i < rigidbodyInfoList.Count; i++) {
			removeObject (rigidbodyInfoList [i].mainObject);
		}

		rigidbodyInfoList.Clear ();

		objectsDetected = false;
	}

	public void setWindForceValue (float newValue)
	{
		windForce = newValue;
	}

	[System.Serializable]
	public class rigidbodyInfo
	{
		public string Name;
		public bool isPlayer;
		public GameObject mainObject;
		public Rigidbody mainRigidbody;

		public externalControllerBehavior mainExternalControllerBehavior;

		public windOnObjectState mainWindOnObjectState;

		public bool isVehicle;

		public bool applyWindOnObject;

		public bool ignoreForcesOnVehicleIfWindActive;
	}
}
