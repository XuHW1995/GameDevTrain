using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateRigidbodyStateInsideRigidbodySystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool detectObjectsEnabled = true;
	public LayerMask layerMask;

	public bool useTagList;
	public List<string> tagList = new List<string> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool objectsOnList;

	public List<GameObject> gameObjectList = new List<GameObject> ();

	public List<rigidbodyInfo> rigidbodyInfoList = new List<rigidbodyInfo> ();

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnAddObject;
	public eventParameters.eventToCallWithGameObject eventOnAddObject;
	public bool useEventOnRemoveObject;
	public eventParameters.eventToCallWithGameObject eventOnRemoveObject;

	[Space]
	[Header ("Components")]
	[Space]

	public Rigidbody mainRigidbody;

	public Transform mainParent;

	public Transform mainTransform;


	Vector3 lastEulerAngles;
	Vector3 lastPosition;

	Rigidbody currentRigidbody;

	rigidbodyInfo currentRigidbodyInfo;


	void Start ()
	{
		if (mainTransform == null) {
			mainTransform = transform;
		}

		lastPosition = mainTransform.position;
		lastEulerAngles = mainTransform.eulerAngles;

		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}
	}

	void LateUpdate ()
	{
		if (objectsOnList) {       
			Vector3 velocity = (mainTransform.position - lastPosition);
			Vector3 angularVelocity = mainTransform.eulerAngles - lastEulerAngles;

			for (int i = 0; i < rigidbodyInfoList.Count; i++) {
				currentRigidbodyInfo = rigidbodyInfoList [i];

				currentRigidbody = currentRigidbodyInfo.mainRigidbody;

				if (currentRigidbodyInfo.isPlayer) {
//					currentRigidbodyInfo.mainPlayerController.externalForce (velocity);

//					currentRigidbodyInfo.mainPlayerCamera.transform.Translate (velocity);

					currentRigidbodyInfo.mainPlayerController.setMovingInsideVehicleState (velocity);

//					currentRigidbodyInfo.cameraRigidbody.position = currentRigidbody.position;

//					currentRigidbodyInfo.mainPlayerController.setExtraCharacterVelocity (mainRigidbody.velocity);

					currentRigidbody.transform.RotateAround (mainTransform.position, Vector3.up, angularVelocity.y);
				} else {
//					currentRigidbody.transform.Translate (velocity, Space.World);


					currentRigidbody.angularVelocity = mainRigidbody.angularVelocity;

//					currentRigidbody.transform.RotateAround (mainTransform.position, mainTransform.up, angularVelocity.y);
				}
			}

			lastPosition = mainTransform.position;
			lastEulerAngles = mainTransform.eulerAngles;
		}
	}

	void OnDestroy ()
	{
		if (GKC_Utils.isApplicationPlaying () && Time.deltaTime > 0) {
			removeAllObjects ();
		}
	}

	public void removeAllObjects ()
	{
		for (int i = 0; i < rigidbodyInfoList.Count; i++) {
			if (rigidbodyInfoList [i] != null) {
				removeGameObject (rigidbodyInfoList [i].mainObject);
			}
		}
	}

	public void addGameObject (GameObject newObject)
	{
		if (!detectObjectsEnabled) {
			return;
		}

		if (newObject == null) {
			return;
		}

		Collider currentCollider = newObject.GetComponent<Collider> ();

		if (currentCollider != null) {
			checkTriggerType (currentCollider, true);
		}
	}

	public void removeGameObject (GameObject newObject)
	{
		if (!detectObjectsEnabled) {
			return;
		}

		if (newObject == null) {
			return;
		}

		Collider currentCollider = newObject.GetComponent<Collider> ();

		if (currentCollider != null) {
			checkTriggerType (currentCollider, false);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (detectObjectsEnabled) {
			checkTriggerType (col, true);
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (detectObjectsEnabled) {
			checkTriggerType (col, false);
		}
	}

	public void checkTriggerType (Collider col, bool isEnter)
	{
		if (col.isTrigger) {
			return;
		}

		bool checkResult = true;

		if ((1 << col.gameObject.layer & layerMask.value) != 1 << col.gameObject.layer) {
			checkResult = false;
		}

		if (!checkResult) {
			if (useTagList) {
				if (tagList.Contains (col.gameObject.tag)) {
					checkResult = true;
				} else {
					checkResult = false;
				}
			}
		}

		if (!checkResult) {
			return;
		}

		if (isEnter) {
			if (!gameObjectList.Contains (col.gameObject)) {
				Rigidbody currentRigidbody = col.gameObject.GetComponent<Rigidbody> ();

				bool currentObjectIsPlayer = false;

				playerComponentsManager currentPlayerComponentsManager = col.gameObject.GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {
					currentObjectIsPlayer = true;

					playerController currentPlayerController = currentPlayerComponentsManager.getPlayerController ();

					Transform currentPlayerCameraTransform = currentPlayerController.getPlayerCameraGameObject ().transform;

					playerCamera currentPlayerCamera = currentPlayerComponentsManager.getPlayerCamera ();

					currentRigidbody = currentPlayerController.getRigidbody ();

					currentPlayerController.setUpdatePlayerCameraPositionOnFixedUpdateActiveState (true);

					currentPlayerController.setMovingInsideVehicleState (true);

					//					currentPlayerController.setPlayerAndCameraParent (mainParent);
					//
					//					currentPlayerController.setMovingOnPlatformActiveState (mainParent != null);

					//					newRigidbodyInfo.mainPlayerCamera = currentPlayerCamera;

					//					newRigidbodyInfo.cameraRigidbody = newRigidbodyInfo.mainPlayerCamera.gameObject.AddComponent<Rigidbody> ();

					//					newRigidbodyInfo.cameraRigidbody.useGravity = false;
					//					newRigidbodyInfo.cameraRigidbody.freezeRotation = true;
					//					currentPlayerController.setUpdatePlayerCameraPositionOnLateUpdateActiveState (true);

					currentPlayerCameraTransform.SetParent (mainParent);

					currentPlayerCamera.setUseSmoothCameraFollowState (true);
				}

				if (currentRigidbody == null) {
					return;
				}

				rigidbodyInfo newRigidbodyInfo = new rigidbodyInfo ();

				lastPosition = mainTransform.position;
				lastEulerAngles = mainTransform.eulerAngles;

				newRigidbodyInfo.mainObject = currentRigidbody.gameObject;

				newRigidbodyInfo.mainRigidbody = currentRigidbody;

				newRigidbodyInfo.isPlayer = currentObjectIsPlayer;

				if (currentObjectIsPlayer) {
					newRigidbodyInfo.mainPlayerController = currentPlayerComponentsManager.getPlayerController ();
				} else {
					currentRigidbody.transform.SetParent (mainParent);
				}

				rigidbodyInfoList.Add (newRigidbodyInfo);

				if (useEventsOnAddObject) {
					eventOnAddObject.Invoke (currentRigidbody.gameObject);
				}

				gameObjectList.Add (currentRigidbody.gameObject);

				checkNullObjects ();

				if (showDebugPrint) {
					print ("adding rigidbody to the list: " + col.gameObject.name);
				}
			}
		} else {
			if (gameObjectList.Contains (col.gameObject)) {
				int rigidbodyIndex = rigidbodyInfoList.FindIndex (s => s.mainObject == col.gameObject);

				if (rigidbodyIndex > -1) {

					rigidbodyInfo currentRigidbodyInfo = rigidbodyInfoList [rigidbodyIndex];

					Rigidbody currentRigidbody = currentRigidbodyInfo.mainRigidbody;

					playerComponentsManager currentCharacter = col.gameObject.GetComponent<playerComponentsManager> ();

					if (currentCharacter != null) {
						playerController currentPlayerController = currentCharacter.getPlayerController ();

						Transform currentPlayerCameraTransform = currentPlayerController.getPlayerCameraGameObject ().transform;

						playerCamera currentPlayerCamera = currentCharacter.getPlayerCamera ();

//						currentPlayerController.setUpdatePlayerCameraPositionOnLateUpdateActiveState (false);

						currentPlayerController.setUpdatePlayerCameraPositionOnFixedUpdateActiveState (false);

						currentPlayerController.setMovingInsideVehicleState (false);

						currentPlayerController.setExtraCharacterVelocity (Vector3.zero);

						currentPlayerCameraTransform.SetParent (null);

						currentPlayerCamera.setOriginalUseSmoothCameraFollowState ();

						//					currentPlayerController.setPlayerAndCameraParent (null);
						//
						//					currentPlayerController.setMovingOnPlatformActiveState (false);
					}

					if (!currentRigidbodyInfo.isPlayer) {
						currentRigidbodyInfo.mainRigidbody.transform.SetParent (null);
					}

					if (useEventOnRemoveObject) {
						eventOnRemoveObject.Invoke (currentRigidbody.gameObject);
					}

					rigidbodyInfoList.RemoveAt (rigidbodyIndex);

					gameObjectList.Remove (currentRigidbody.gameObject);

					checkNullObjects ();

					if (showDebugPrint) {
						print ("removing rigidbody from the list: " + col.gameObject.name);
					}
				}
			}
		}

		objectsOnList = rigidbodyInfoList.Count > 0;
	}

	void checkNullObjects ()
	{
		for (int i = rigidbodyInfoList.Count - 1; i >= 0; i--) {	
			if (rigidbodyInfoList [i].mainRigidbody == null) {
				rigidbodyInfoList.RemoveAt (i);

				gameObjectList.RemoveAt (i);
			}
		}
	}

	public bool checkIfObjectOnList (GameObject newObject)
	{
		return gameObjectList.Contains (newObject);
	}

	[System.Serializable]
	public class rigidbodyInfo
	{
		public GameObject mainObject;
		public Rigidbody mainRigidbody;

		public bool isPlayer;

		public playerController mainPlayerController;
	}
}