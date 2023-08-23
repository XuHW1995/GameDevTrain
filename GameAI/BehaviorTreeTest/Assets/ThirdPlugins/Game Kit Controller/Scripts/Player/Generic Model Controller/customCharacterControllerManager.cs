﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class customCharacterControllerManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool startGameWithCustomCharacterController;
	public string customCharacterControllerToStartName;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool ignoreSetAIValues;

	public bool setCharacterModeOnRegularState;

	public bool setNewCharacterModeOnRegularState;
	public string newCharacterModeOnRegularState;

	[Space]
	[Header ("Character Controller List Settings")]
	[Space]

	public List<characterControllerStateInfo> characterControllerStateInfoList = new List<characterControllerStateInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool customCharacterControllerActive;

	public string currentCharacterControllerName;

	public bool toggleCustomCharacterControllerPaused;

	public bool genericModelVisibleOnEditor;

	public Avatar originalAvatar;

	public customCharacterControllerBase currentCustomCharacterControllerBase;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventsOnStateChange;

	public UnityEvent eventOnCustomCharacterEnter;
	public UnityEvent eventOnCustomCharacterExit;

	[Space]
	[Header ("Components")]
	[Space]

	public playerController mainPlayerController;

	public playerCamera mainPlayerCamera;

	public Animator mainAnimator;

	public RuntimeAnimatorController originalAnimatorController;

	public playerWeaponsManager mainPlayerWeaponManager;

	public playerStatesManager mainPlayerStatesManager;

	public Transform customCharacterControllerParent;

	public CapsuleCollider mainCapsuleCollider;

	public ragdollActivator mainRagdollActivator;
	public closeCombatSystem mainCloseCombatSystem;

	public healthManagement mainHealth;

	[Space]
	[Header ("AI Components")]
	[Space]

	public findObjectivesSystem mainFindObjectivesSystem;
	public AICloseCombatSystemBrain mainAICloseCombatSystemBrain;

	[Space]
	[Header ("Custom Character Prefab Settings")]
	[Space]

	public GameObject customCharacterPrefab;


	characterControllerStateInfo currentCharacterControllerStateInfo;

	string originalCameraStateThirdPerson;
	string originalCameraStateFirstPerson;

	string previousCharacterModeName = "";


	void Start ()
	{
		if (originalAvatar == null) {
			originalAvatar = mainAnimator.avatar;
		}

		if (startGameWithCustomCharacterController) {
			StartCoroutine (startGameWithCustomCharacterControllerCoroutine ());
		}
	}

	IEnumerator startGameWithCustomCharacterControllerCoroutine ()
	{
		yield return new WaitForSeconds (0.2f);

		toggleCustomCharacterController (customCharacterControllerToStartName);
	}

	public void toggleCustomCharacterControllerExternally (string characterControllerName)
	{
		bool previousToggleCustomCharacterControllerPausedValue = toggleCustomCharacterControllerPaused;

		toggleCustomCharacterControllerPaused = false;

		toggleCustomCharacterController (characterControllerName);

		toggleCustomCharacterControllerPaused = previousToggleCustomCharacterControllerPausedValue;
	}

	public void toggleCustomCharacterController (string characterControllerName)
	{
		if (toggleCustomCharacterControllerPaused) {
			return;
		}

		if (!customCharacterControllerActive) {
			setCustomCharacterControllerState (characterControllerName);
		} else {
			disableCustomCharacterControllerState ();
		}
	}

	public void disableCustomCharacterControllerState ()
	{
		if (!customCharacterControllerActive) {
			return;
		}

		if (currentCharacterControllerStateInfo != null) {

			currentCharacterControllerStateInfo.isCurrentCharacterController = false;

			currentCharacterControllerStateInfo.eventOnStateEnd.Invoke ();

			if (currentCharacterControllerStateInfo.useEventToSendCharacterObjectOnStateChange) {
				currentCharacterControllerStateInfo.eventToSendCharacterObjectOnEnd.Invoke (currentCharacterControllerStateInfo.customCharacterController.characterGameObject);
			}

			setCharacterState (false);

			currentCharacterControllerStateInfo = null;

			currentCharacterControllerName = "";
		}
	}

	public void setRandomCustomCharacterControllerState ()
	{
		int randomIndex = Random.Range (0, characterControllerStateInfoList.Count - 1);

		if (randomIndex <= characterControllerStateInfoList.Count) {
			setCustomCharacterControllerState (characterControllerStateInfoList [randomIndex].Name);
		}
	}

	public void setCustomCharacterControllerState (string characterControllerStateName)
	{
		if (showDebugPrint) {
			print ("trying to activate " + characterControllerStateName);
		}

		if (currentCharacterControllerName == characterControllerStateName) {
			return;
		}

		int newCharacterIndex = -1;

		for (int i = 0; i < characterControllerStateInfoList.Count; i++) {
			if (characterControllerStateInfoList [i].stateEnabled && characterControllerStateInfoList [i].Name.Equals (characterControllerStateName)) {
				newCharacterIndex = i;
			} else {
				if (characterControllerStateInfoList [i].isCurrentCharacterController) {
//					characterControllerStateInfoList [i].customCharacterController.setCharacterControllerActiveState (false);
				
					currentCharacterControllerStateInfo = characterControllerStateInfoList [i];

					currentCharacterControllerStateInfo.eventOnStateEnd.Invoke ();

					if (currentCharacterControllerStateInfo.useEventToSendCharacterObjectOnStateChange) {
						currentCharacterControllerStateInfo.eventToSendCharacterObjectOnEnd.Invoke (currentCharacterControllerStateInfo.customCharacterController.characterGameObject);
					}

					setCharacterState (false);

					if (showDebugPrint) {
						print ("disabling first the character " + currentCharacterControllerStateInfo.Name);
					}
				}

				characterControllerStateInfoList [i].isCurrentCharacterController = false;
			}
		}

	

		if (newCharacterIndex != -1) {

			currentCharacterControllerStateInfo = characterControllerStateInfoList [newCharacterIndex];

			if (showDebugPrint) {
				print (newCharacterIndex + " " + currentCharacterControllerStateInfo.Name);
			}

			if (currentCharacterControllerStateInfo.spawnCustomCharacterPrefab) {
				if (currentCharacterControllerStateInfo.customCharacterInstantiatedObject == null) {
					instantiateCustomCharacter (currentCharacterControllerStateInfo);
				}
			}

			currentCharacterControllerStateInfo.isCurrentCharacterController = true;

			currentCharacterControllerName = currentCharacterControllerStateInfo.Name;

			setCharacterState (true);
		} else {
			if (showDebugPrint) {
				print (characterControllerStateName + " not found");
			}
		}
	}


	void instantiateCustomCharacter (characterControllerStateInfo characterInfo)
	{
		GameObject customCharacterObject = (GameObject)Instantiate (characterInfo.customCharacterPrefab, Vector3.zero, Quaternion.identity, transform);

		customCharacterObject.name = characterInfo.customCharacterPrefab.name;

		characterInfo.customCharacterInstantiatedObject = customCharacterObject;

		characterInfo.customCharacterController = customCharacterObject.GetComponent<customCharacterControllerBase> ();

		customCharacterObject.transform.localPosition = Vector3.zero;
		customCharacterObject.transform.localRotation = Quaternion.identity;

		characterInfo.customCharacterController.eventOnInstantiateCustomCharacterObject.Invoke ();

		characterInfo.customCharacterController.mainAnimator = mainAnimator;

		genericRagdollBuilder currentGenericRagdollBuilder = customCharacterObject.GetComponentInChildren<genericRagdollBuilder> ();

		if (currentGenericRagdollBuilder != null) {
			currentGenericRagdollBuilder.mainRagdollActivator = mainRagdollActivator;

			currentGenericRagdollBuilder.updateRagdollActivatorPatsIngame ();
		}

		followObjectPositionSystem currentFollowObjectPositionSystem = customCharacterObject.GetComponentInChildren<followObjectPositionSystem> ();

		if (currentFollowObjectPositionSystem != null) {
			currentFollowObjectPositionSystem.setObjectToFollow (mainPlayerController.transform);
		}

		simpleSliceSystem currentSimpleSliceSystem = customCharacterObject.GetComponentInChildren<simpleSliceSystem> ();

		if (currentSimpleSliceSystem != null) {
			surfaceToSlice currentSurfaceToSlice = mainPlayerController.GetComponent<surfaceToSlice> ();

			if (currentSurfaceToSlice != null) {
				currentSurfaceToSlice.mainSimpleSliceSystem = currentSimpleSliceSystem;

				currentSimpleSliceSystem.mainSurfaceToSlice = currentSurfaceToSlice;

				currentSimpleSliceSystem.initializeValuesOnHackableComponent ();
			}
		}
	}

	void setCharacterState (bool state)
	{
		currentCustomCharacterControllerBase = currentCharacterControllerStateInfo.customCharacterController;

		currentCustomCharacterControllerBase.setCharacterControllerActiveState (state);

		if (ignoreSetAIValues) {
			currentCustomCharacterControllerBase.setAIValues = false;
		}

		if (state) {
			mainPlayerController.setCustomCharacterControllerActiveState (true, currentCustomCharacterControllerBase);
		
			currentCustomCharacterControllerBase.characterGameObject.transform.SetParent (customCharacterControllerParent);

			currentCustomCharacterControllerBase.characterGameObject.transform.position = mainPlayerController.transform.position +
			currentCustomCharacterControllerBase.charactetPositionOffset;

			Quaternion targetRotation = Quaternion.Euler (currentCustomCharacterControllerBase.characterRotationOffset);

			currentCustomCharacterControllerBase.characterGameObject.transform.localRotation = targetRotation;

			Animator characterAnimator = currentCustomCharacterControllerBase.characterGameObject.GetComponent<Animator> ();

			if (characterAnimator != null && characterAnimator.enabled) {
				characterAnimator.enabled = false;
			}

			if (previousCharacterModeName == "") {
				previousCharacterModeName = mainPlayerStatesManager.getCurrentPlayersModeName ();
			}

			mainPlayerStatesManager.checkPlayerStates ();

			currentCharacterControllerStateInfo.eventOnStateStart.Invoke ();

			if (currentCharacterControllerStateInfo.useEventToSendCharacterObjectOnStateChange) {
				currentCharacterControllerStateInfo.eventToSendCharacterObjectOnStart.Invoke (currentCharacterControllerStateInfo.customCharacterController.characterGameObject);
			}

			mainPlayerStatesManager.setPlayerModeByName (currentCustomCharacterControllerBase.playerModeName);

			mainAnimator.runtimeAnimatorController = currentCustomCharacterControllerBase.originalAnimatorController;
			mainAnimator.avatar = currentCustomCharacterControllerBase.originalAvatar;

			currentCustomCharacterControllerBase.updateOnGroundValue (mainPlayerController.isPlayerOnGround ());

			if (currentCustomCharacterControllerBase.setCapsuleColliderValues) {
				mainPlayerController.setPlayerColliderCapsuleScale (currentCustomCharacterControllerBase.capsuleColliderHeight);

				mainPlayerController.setPlayerCapsuleColliderDirection (currentCustomCharacterControllerBase.capsuleColliderDirection);

				mainCapsuleCollider.center = currentCustomCharacterControllerBase.capsuleColliderCenter;

				mainPlayerController.setPlayerCapsuleColliderRadius (currentCustomCharacterControllerBase.capsuleColliderRadius);
			}

			mainPlayerController.setPlaceToShootPositionOffset (currentCustomCharacterControllerBase.placeToShootOffset);


			mainPlayerController.setUseRootMotionActiveState (currentCustomCharacterControllerBase.useRootMotion);

			if (!currentCustomCharacterControllerBase.useRootMotion) {
				mainPlayerController.setNoRootMotionValues (currentCustomCharacterControllerBase.noRootWalkMovementSpeed,
					currentCustomCharacterControllerBase.noRootRunMovementSpeed, currentCustomCharacterControllerBase.noRootSprintMovementSpeed, 
					currentCustomCharacterControllerBase.noRootCrouchMovementSpeed,
					currentCustomCharacterControllerBase.noRootWalkStrafeMovementSpeed, currentCustomCharacterControllerBase.noRootRunStrafeMovementSpeed);
			}
				

			mainPlayerController.setCharacterMeshGameObjectState (!state);

			mainPlayerController.setUsingExternalCharacterMeshListState (true);

			mainPlayerController.setExternalCharacterMeshList (currentCustomCharacterControllerBase.characterMeshesList);

			//CAMERA ELEMENTS
			if (currentCustomCharacterControllerBase.setNewCameraStates && !currentCustomCharacterControllerBase.setAIValues) {
				originalCameraStateThirdPerson = mainPlayerCamera.getDefaultThirdPersonStateName ();

				originalCameraStateFirstPerson = mainPlayerCamera.getDefaultFirstPersonStateName ();

				if (mainPlayerCamera.isFirstPersonActive ()) {
					mainPlayerCamera.setCameraState (currentCustomCharacterControllerBase.newCameraStateFirstPerson);
				} else {
					mainPlayerCamera.setCameraState (currentCustomCharacterControllerBase.newCameraStateThirdPerson);
				}

				mainPlayerCamera.setDefaultThirdPersonStateName (currentCustomCharacterControllerBase.newCameraStateThirdPerson);

				mainPlayerCamera.setDefaultFirstPersonStateName (currentCustomCharacterControllerBase.newCameraStateFirstPerson);
			}


			//OTHER COMPONENTS FUNCTIONS
			mainPlayerController.setCurrentCustomActionCategoryID (currentCustomCharacterControllerBase.customActionCategoryID);

			mainPlayerController.setCharacterRadius (currentCustomCharacterControllerBase.characterRadius);

			mainCloseCombatSystem.setCombatTypeID (currentCustomCharacterControllerBase.combatTypeID);

			mainCloseCombatSystem.setNewParentForDamageTriggers (currentCustomCharacterControllerBase.parentForCombatDamageTriggers);

			mainRagdollActivator.setCurrentRagdollInfo (currentCustomCharacterControllerBase.customRagdollInfoName);
	

			//AI COMPONENTS
			if (currentCustomCharacterControllerBase.setAIValues) {
				mainAICloseCombatSystemBrain.setMaxRandomTimeBetweenAttacks (currentCustomCharacterControllerBase.maxRandomTimeBetweenAttacks);
				mainAICloseCombatSystemBrain.setMinRandomTimeBetweenAttacks (currentCustomCharacterControllerBase.minRandomTimeBetweenAttacks);

				mainAICloseCombatSystemBrain.setBlockEnabledState (false);
		
				mainFindObjectivesSystem.setNewMinDistanceToEnemyUsingCloseCombat (currentCustomCharacterControllerBase.newMinDistanceToEnemyUsingCloseCombat);

				mainFindObjectivesSystem.setNewMinDistanceToCloseCombat (currentCustomCharacterControllerBase.newMinDistanceToCloseCombat);
			
				mainFindObjectivesSystem.setCloseCombatAttackMode ();

				mainFindObjectivesSystem.setRayCastPositionOffsetValue (currentCustomCharacterControllerBase.raycastPositionOffset);

				if (currentCustomCharacterControllerBase.healthBarOffset != 0) {
					mainHealth.updateSliderOffset (currentCustomCharacterControllerBase.healthBarOffset);
				}
			}

			if (currentCharacterControllerStateInfo.overrideHiddenFromAIAttacksValue) {
				mainPlayerController.setStealthModeActiveState (currentCharacterControllerStateInfo.hiddenFromAIAttacksValue);
			} else {
				if (currentCustomCharacterControllerBase.hiddenFromAIAttacks) {
					mainPlayerController.setStealthModeActiveState (true);
				}
			}
		} else {
			currentCustomCharacterControllerBase.updateOnGroundValue (mainPlayerController.isPlayerOnGround ());

			mainAnimator.runtimeAnimatorController = originalAnimatorController;
			mainAnimator.avatar = originalAvatar;

			mainPlayerController.setCustomCharacterControllerActiveState (false, null);

			currentCustomCharacterControllerBase.characterGameObject.transform.SetParent (null);

			mainPlayerController.setOriginalPlayerColliderCapsuleScale ();

			if (currentCustomCharacterControllerBase.setCapsuleColliderValues) {
				mainPlayerController.setPlayerCapsuleColliderDirection (1);

				mainPlayerController.setOriginalPlayerCapsuleColliderRadius ();
			}

			mainPlayerController.setPlaceToShootPositionOffset (-1);

			mainPlayerController.setOriginalUseRootMotionActiveState ();

			mainPlayerController.setOriginalNoRootMotionValues ();

			mainPlayerController.setUsingExternalCharacterMeshListState (false);

			mainPlayerController.setExternalCharacterMeshList (null);

			mainPlayerController.setCharacterMeshGameObjectState (!state);


			//CAMERA ELEMENTS
			if (currentCustomCharacterControllerBase.setNewCameraStates && !currentCustomCharacterControllerBase.setAIValues) {
				if (mainPlayerCamera.isFirstPersonActive ()) {
					mainPlayerCamera.setCameraState (originalCameraStateFirstPerson);
				} else {
					mainPlayerCamera.setCameraState (originalCameraStateThirdPerson);
				}

				mainPlayerCamera.setDefaultThirdPersonStateName (originalCameraStateThirdPerson);

				mainPlayerCamera.setDefaultFirstPersonStateName (originalCameraStateFirstPerson);
			}


			//OTHER COMPONENTS FUNCTIONS
			mainPlayerController.setCurrentCustomActionCategoryID (0);

			mainPlayerController.setCharacterRadius (0);

			mainCloseCombatSystem.setCombatTypeID (0);

			mainCloseCombatSystem.setOriginalParentForDamageTriggers ();

			mainRagdollActivator.setCurrentRagdollInfo ("");


			//AI COMPONENTS
			if (currentCustomCharacterControllerBase.setAIValues) {
				mainAICloseCombatSystemBrain.setOriginalMaxRandomTimeBetweenAttacks ();
				mainAICloseCombatSystemBrain.setOriginalMinRandomTimeBetweenAttacks ();
				mainAICloseCombatSystemBrain.setBlockEnabledState (true);

				mainFindObjectivesSystem.setOriginalRayCastPositionOffsetValue ();

				if (currentCustomCharacterControllerBase.healthBarOffset != 0) {
					mainHealth.updateOriginalSliderOffset ();
				}
			}

			if (currentCustomCharacterControllerBase.hiddenFromAIAttacks || currentCharacterControllerStateInfo.overrideHiddenFromAIAttacksValue) {
				mainPlayerController.setStealthModeActiveState (false);
			}
		}

		mainRagdollActivator.setPauseAnimatorStatesToGetUpState (state);

		mainPlayerWeaponManager.enableOrDisableWeaponsMesh (!state);

		mainPlayerController.setCharacterMeshesListToDisableOnEventState (!state);

		mainPlayerController.enableOrDisableIKSystemManagerState (!state);

		mainPlayerController.setFootStepManagerState (state);

		customCharacterControllerActive = state;

		mainPlayerController.setUsingGenericModelActiveState (state);

		checkEventsOnStateChange (customCharacterControllerActive);

		if (!state) {
			if (setCharacterModeOnRegularState) {
				if (previousCharacterModeName != "") {
					if (setNewCharacterModeOnRegularState) {
						if (!mainPlayerStatesManager.getCurrentPlayersModeName ().Equals (newCharacterModeOnRegularState)) {
							mainPlayerStatesManager.setPlayerModeByName (newCharacterModeOnRegularState);
						}
					} else {
						if (!mainPlayerStatesManager.getCurrentPlayersModeName ().Equals (previousCharacterModeName)) {
							mainPlayerStatesManager.setPlayerModeByName (previousCharacterModeName);
						}
					}

					previousCharacterModeName = "";
				}
			}
		}
	}

	public bool isCustomCharacterControllerActive ()
	{
		return customCharacterControllerActive;
	}

	public void checkEventsOnStateChange (bool state)
	{
		if (useEventsOnStateChange) {
			if (state) {
				eventOnCustomCharacterEnter.Invoke ();
			} else {
				eventOnCustomCharacterExit.Invoke ();
			}
		}
	}

	public void setToggleCustomCharacterControllerPausedState (bool state)
	{
		toggleCustomCharacterControllerPaused = state;
	}

	public void addNewCustomCharacterController (customCharacterControllerBase newCustomCharacterControllerBase, string newName)
	{
		characterControllerStateInfo newCharacterControllerStateInfo = new characterControllerStateInfo ();

		newCharacterControllerStateInfo.Name = newName;

		newCharacterControllerStateInfo.customCharacterController = newCustomCharacterControllerBase;

		characterControllerStateInfoList.Add (newCharacterControllerStateInfo);

		updateComponent ();
	}

	public void addNewCustomCharacterControllerToSpawn (GameObject newCustomCharacterControllerBaseObject, string newName)
	{
		characterControllerStateInfo newCharacterControllerStateInfo = new characterControllerStateInfo ();

		newCharacterControllerStateInfo.Name = newName;

		newCharacterControllerStateInfo.customCharacterPrefab = newCustomCharacterControllerBaseObject;

		newCharacterControllerStateInfo.spawnCustomCharacterPrefab = true;

		characterControllerStateInfoList.Add (newCharacterControllerStateInfo);

		updateComponent ();
	}

	public void toggleCharacterModelMeshOnEditor (bool state)
	{
		if (genericModelVisibleOnEditor == state) {
			return;
		}

		genericModelVisibleOnEditor = state;

		if (currentCustomCharacterControllerBase != null && !genericModelVisibleOnEditor) {
			currentCustomCharacterControllerBase.setCharacterControllerActiveState (genericModelVisibleOnEditor);

			currentCustomCharacterControllerBase = null;
		}
			
		int characterIndex = characterControllerStateInfoList.FindIndex (s => s.Name == customCharacterControllerToStartName);

		if (characterIndex > -1) {
			currentCharacterControllerStateInfo = characterControllerStateInfoList [characterIndex];

			currentCustomCharacterControllerBase = currentCharacterControllerStateInfo.customCharacterController;

			if (currentCustomCharacterControllerBase == null) {
				instantiateCustomCharacter (currentCharacterControllerStateInfo);

				currentCustomCharacterControllerBase = currentCharacterControllerStateInfo.customCharacterController;
			}

			if (currentCustomCharacterControllerBase != null) {
				currentCustomCharacterControllerBase.setCharacterControllerActiveState (genericModelVisibleOnEditor);
			}
		}

		mainPlayerController.setCharacterMeshGameObjectState (!genericModelVisibleOnEditor);

		mainPlayerWeaponManager.enableOrDisableAllWeaponsMeshes (!genericModelVisibleOnEditor);

		mainPlayerController.setCharacterMeshesListToDisableOnEventState (!genericModelVisibleOnEditor);

		updateComponent ();
	}

	public void setStartGameWithCustomCharacterControllerFromEditor (bool state)
	{
		startGameWithCustomCharacterController = state;

		updateComponent ();
	}

	public void setCustomCharacterControllerToStartNameFromEditor (string newCharacterName)
	{
		customCharacterControllerToStartName = newCharacterName;

		updateComponent ();
	}

	public void setCapsuleValuesFromMainCharacter ()
	{
		if (currentCustomCharacterControllerBase != null) {
			currentCustomCharacterControllerBase.setCapsuleColliderDirectionValue (mainCapsuleCollider.direction);

			currentCustomCharacterControllerBase.setCapsuleColliderCenterValue (mainCapsuleCollider.center);
			currentCustomCharacterControllerBase.setCapsuleColliderRadiusValue (mainCapsuleCollider.radius);
			currentCustomCharacterControllerBase.setCapsuleColliderHeightValue (mainCapsuleCollider.height);	
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Add New Custom Character Controller", gameObject);
	}

	[System.Serializable]
	public class characterControllerStateInfo
	{
		[Header ("Main Settings")]
		[Space]

		public string Name;
		public bool stateEnabled = true;

		public bool isCurrentCharacterController;

		[Space]
		[Space]

		public customCharacterControllerBase customCharacterController;

		[Space]

		[Header ("Spawn Character Ingame Settings")]
		[Space]

		public bool spawnCustomCharacterPrefab;
		public GameObject customCharacterPrefab;

		[HideInInspector] public GameObject customCharacterInstantiatedObject;

		[Space]
		[Header ("AI Visibility")]
		[Space]

		public bool overrideHiddenFromAIAttacksValue;
		public bool hiddenFromAIAttacksValue;

		[Space]
		[Space]

		[Header ("Events Settings")]
		[Space]

		public UnityEvent eventOnStateStart;
		public UnityEvent eventOnStateEnd;

		public bool useEventToSendCharacterObjectOnStateChange;
		public eventParameters.eventToCallWithGameObject eventToSendCharacterObjectOnStart;
		public eventParameters.eventToCallWithGameObject eventToSendCharacterObjectOnEnd;
	}
}
