﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class saveGameSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public int sceneToLoadNewGame;
	public int saveStationId;

	public bool isPhysicalSaveStation = true;

	public bool usePlayerCameraOrientation;

	[Space]
	[Header ("Current Scene Index Settings")]
	[Space]

	public bool setCustomCurrentSceneIndex;
	public int customCurrentSceneIndex;

	[Space]
	[Header ("Load Screen Settings")]
	[Space]

	public bool useLoadScreen;

	public int loadScreenScene = 1;

	public bool useLastSceneIndexAsLoadScreen = true;

	public string sceneToLoadAsyncPrefsName = "SceneToLoadAsync";

	public bool checkLoadingScreenSceneConfigured = true;

	public string loadingScreenSceneName = "Loading Screen Scene";

	[Space]
	[Header ("Checkpoint Settings")]
	[Space]

	public bool checkpointSlotCanBeDeletedEnabled;

	[Space]
	[Header ("Save Game Icon Settings")]
	[Space]

	public bool useSavingIcon;

	public bool useCanvasGroupOnShowSavingGamePanel = true;
	public float showSavingGamePanelDuration = 3;
	public float showSavingGamePanelFadeSpeed = 5;

	public string savingGameAnimationName;
	public float animationSpeed = 0.5f;

	[Space]
	[Header ("Physical Save Station Settings")]
	[Space]

	public string animationName;

	public LayerMask savePositionLayermask;

	[Space]
	[Header ("Save Element List Settings")]
	[Space]

	public List<saveInfo> saveInfoList = new List<saveInfo> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public Color disableButtonsColor;
	public Vector2 captureResolution;

	[Space]
	[Header ("Custom Save Element List Settings")]
	[Space]

	public List<customSaveListInfo> customSaveListInfoList = new List<customSaveListInfo> ();

	[Space]
	[Header ("Event Settings")]
	[Space]

	public bool useEventIfGameToContinueAvailable;
	public UnityEvent eventfGameToContinueAvailable;

	[Space]
	[Header ("Event Settings On Save/Load Game")]
	[Space]

	public bool useEventOnSaveGame;
	public eventParameters.eventToCallWithInteger eventOnSaveGame;

	public bool useEventOnLoadGame;
	public eventParameters.eventToCallWithInteger eventOnLoadGame;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugInfo;

	public bool ignoreShowDebugInfoOnSave;
	public bool ignoreShowDebugInfoOnLoad;

	public saveStationInfo mainSaveStationInfo;

	public bool usingSaveStation;
	public bool canSave;
	public bool canLoad;
	public bool canDelete;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform saveStationPosition;
	public GameObject savingIcon;

	public GameObject playerCameraGameObject;
	public GameObject playerControllerGameObject;
	public playerController mainPlayerController;
	public playerCamera mainPlayerCameraManager;

	public playerComponentsManager mainPlayerComponentsManager;

	public gameManager gameManagerComponent;

	public GameObject saveMenu;
	public Image saveButton;
	public Image loadButton;
	public Image deleteButton;
	public Scrollbar scrollBar;
	public GameObject saveGameList;
	public Camera photoCapturer;

	List<buttonInfo> saveGameListElements = new List<buttonInfo> ();

	Animation stationAnimation;

	Button currentButton;
	Color originalColor;
	int currentButtonIndex;

	string currentSaveDataPath;
	string currentSaveDataName;
	GameObject slotPrefab;
	Animation savingGameAnimation;
	int checkpointSlotsAmount = 1;
	electronicDevice electronicDeviceManager;

	Coroutine savingGamePanelInfoCoroutine;

	bool initialized;


	void Start ()
	{
		startGameSystem ();
	}

	public void startGameSystem ()
	{
		if (initialized) {
			return;
		}

		if (gameManagerComponent == null) {
			gameManagerComponent = FindObjectOfType<gameManager> ();
		}

		currentSaveDataPath = gameManagerComponent.getDataPath ();
		currentSaveDataName = gameManagerComponent.getDataName ();

		saveMenu.SetActive (true);

		saveGameSlot curretSaveGameSlot = saveGameList.GetComponentInChildren<saveGameSlot> ();
		slotPrefab = curretSaveGameSlot.gameObject;

		saveGameListElements.Add (curretSaveGameSlot.buttonInfo);

		int slotsAmount = gameManagerComponent.slotBySaveStation - 1;

		for (int i = 0; i < slotsAmount; i++) {	
			addSaveSlot (i);
		}

		//Add checkpoint slots
		for (int i = 0; i < checkpointSlotsAmount; i++) {	
			addSaveSlot (saveGameListElements.Count - 1);

			buttonInfo currentSaveButtonInfo = saveGameListElements [saveGameListElements.Count - 1];

			currentSaveButtonInfo.isCheckpointSave = true;
			currentSaveButtonInfo.slotGameObject.SetActive (false);
		}

		//Add auto save slot which is hidden
		addSaveSlot (saveGameListElements.Count - 1);

		saveGameListElements [saveGameListElements.Count - 1].usedForAutoLoadSave = true;
		saveGameListElements [saveGameListElements.Count - 1].slotGameObject.SetActive (false);

		resetScroll (scrollBar);

		saveMenu.SetActive (false);
	
		if (isPhysicalSaveStation) {
			if (stationAnimation == null) {
				stationAnimation = GetComponent<Animation> ();
			}
		}

		if (isPhysicalSaveStation) {
			RaycastHit hit = new RaycastHit ();

			if (Physics.Raycast (saveStationPosition.position, -Vector3.up, out hit, 10, savePositionLayermask)) {
				saveStationPosition.position = hit.point + Vector3.up * 0.05f;
			}
		}

		setSaveGameTransformInfo (saveStationPosition, true);

		if (setCustomCurrentSceneIndex) {
			mainSaveStationInfo.saveStationScene = customCurrentSceneIndex;
		} else {
			mainSaveStationInfo.saveStationScene = SceneManager.GetActiveScene ().buildIndex;
		}

		originalColor = loadButton.color;

		changeButtonsColor (false, false, false, false);

		setPhotoCapturerState (false);

		if (useSavingIcon) {
			if (!useCanvasGroupOnShowSavingGamePanel) {
				savingGameAnimation = savingIcon.GetComponent<Animation> ();
			}
		}

		if (isPhysicalSaveStation) {
			electronicDeviceManager = GetComponent<electronicDevice> ();
		}

		if (useEventIfGameToContinueAvailable) {
			List<saveStationInfo> saveList = loadFile ();

			if (saveList.Count > 0) {
				eventfGameToContinueAvailable.Invoke ();
			}
		}

		initialized = true;

		//loadStates ();
	}

	public void addSaveSlot (int index)
	{
		GameObject newSlotPrefab = (GameObject)Instantiate (slotPrefab, slotPrefab.transform.position, slotPrefab.transform.rotation, slotPrefab.transform.parent);

		newSlotPrefab.transform.localScale = Vector3.one;
		newSlotPrefab.name = "Save Game Slot " + (index + 2);

		saveGameListElements.Add (newSlotPrefab.GetComponent<saveGameSlot> ().buttonInfo);
	}

	public void setSaveGameTransformInfo (Transform savePosition, bool canUsePlayerCameraOrientation)
	{

		if (savePosition == null) {
			return;
		}

		Vector3 currentPosition = savePosition.position;
		Vector3 currentEulerAngles = savePosition.eulerAngles;

		mainSaveStationInfo.saveStationPositionX = currentPosition.x;
		mainSaveStationInfo.saveStationPositionY = currentPosition.y;
		mainSaveStationInfo.saveStationPositionZ = currentPosition.z;
		mainSaveStationInfo.saveStationRotationX = currentEulerAngles.x;
		mainSaveStationInfo.saveStationRotationY = currentEulerAngles.y;
		mainSaveStationInfo.saveStationRotationZ = currentEulerAngles.z;

		if (usePlayerCameraOrientation && canUsePlayerCameraOrientation) {
			mainSaveStationInfo.usePlayerCameraOrientation = true;

			Vector3 cameraEulerAngles = playerCameraGameObject.transform.eulerAngles;

			mainSaveStationInfo.playerCameraRotationX = cameraEulerAngles.x;
			mainSaveStationInfo.playerCameraRotationY = cameraEulerAngles.y;
			mainSaveStationInfo.playerCameraRotationZ = cameraEulerAngles.z;

			Vector3 playerCameraPivotEulerRotation = mainPlayerCameraManager.getPivotCameraTransform ().localEulerAngles;

			mainSaveStationInfo.playerCameraPivotRotationX = playerCameraPivotEulerRotation.x;
		} else {
			mainSaveStationInfo.usePlayerCameraOrientation = false;
		}
	}

	public void activateSaveStation ()
	{
		usingSaveStation = !usingSaveStation;

		if (usingSaveStation) {
			
			loadStates ();

			saveMenu.SetActive (true);

			if (!animationName.Equals ("")) {
				stationAnimation.Stop ();
				stationAnimation [animationName].speed = 1;
				stationAnimation.Play (animationName);
			}

			resetScroll (scrollBar);
		} else {
			saveMenu.SetActive (false);

			if (!animationName.Equals ("")) {
				stationAnimation.Stop ();
				stationAnimation [animationName].speed = -1;
				stationAnimation [animationName].time = stationAnimation [animationName].length;
				stationAnimation.Play (animationName);
			}

			changeButtonsColor (false, false, false, false);
		}
	}

	public void openSaveGameMenu ()
	{
		loadStates ();

		changeButtonsColor (false, false, false, false);

		setSaveGameTransformInfo (saveStationPosition, true);

		resetScroll (scrollBar);
	}

	public void getSaveButtonSelected (Button button)
	{
		currentButtonIndex = -1;

		bool save = false;
		bool load = false;
		bool delete = false;

		for (int i = 0; i < saveGameListElements.Count; i++) {		
			buttonInfo currentSaveButtonInfo = saveGameListElements [i];

			if (currentSaveButtonInfo.button == button) {
				currentButtonIndex = i;	
				currentButton = button;

				if (currentSaveButtonInfo.infoAdded) {
					load = true;
				}

				if (currentSaveButtonInfo.isCheckpointSave) {
					if (checkpointSlotCanBeDeletedEnabled) {
						if (currentSaveButtonInfo.infoAdded) {
							delete = true;
						}
					}
				} else {
					if (currentSaveButtonInfo.infoAdded) {
						delete = true;
					}

					save = true;
				}
			}
		}

		changeButtonsColor (true, save, load, delete);
	}

	public void newGame ()
	{
		PlayerPrefs.SetInt ("loadingGame", 0);

		GKC_Utils.loadScene (sceneToLoadNewGame, useLoadScreen, loadScreenScene, sceneToLoadAsyncPrefsName,
			useLastSceneIndexAsLoadScreen, checkLoadingScreenSceneConfigured, loadingScreenSceneName);
	}

	public void continueGame ()
	{
		saveStationInfo recentSave = new saveStationInfo ();

		List<saveStationInfo> saveList = loadFile ();

		long closestDate = 0;

		int saveListCount = saveList.Count; 

		if (saveListCount > 0) {
			for (int j = 0; j < saveListCount; j++) {
				//print (saveList [j].saveDate.Ticks);

				saveStationInfo currentSave = saveList [j];

				if (currentSave.showSaveSlot && currentSave.saveDate.Ticks > closestDate) {
					closestDate = currentSave.saveDate.Ticks;

					recentSave = currentSave;
				}
			}

			//print ("newer" + recentSave.saveDate+" "+recentSave.saveNumber);
			gameManagerComponent.getPlayerPrefsInfo (recentSave);
		} else {
			print ("WARNING: the game hasn't been saved previously, so there is no game to continue yet");
		}
	}

	public void saveGameOnHiddenSlotIndex (int saveSlotIndex)
	{
		saveCurrentGame (saveSlotIndex, false, false, -1, -1, false, false, true);
	}

	public void saveGame ()
	{
		if (currentButton != null && canSave) {
			int saveSlotIndex = -1;

			for (int i = 0; i < saveGameListElements.Count; i++) {
				buttonInfo currentSaveButtonInfo = saveGameListElements [i];

				if (currentSaveButtonInfo.button == currentButton) {
					currentSaveButtonInfo.infoAdded = true;

					saveSlotIndex = i;	
				}
			}

			saveCurrentGame (saveSlotIndex, false, false, -1, -1, false, true, true);
		}
	}

	public void playSavingGameAnimation ()
	{
		if (useCanvasGroupOnShowSavingGamePanel) {
			showSavingGamePanelInfo ();
		} else {
			savingGameAnimation.Stop ();
			savingGameAnimation [savingGameAnimationName].speed = animationSpeed;
			savingGameAnimation.Play (savingGameAnimationName);
		}
	}

	public void showSavingGamePanelInfo ()
	{
		if (savingIcon == null) {
			return;
		}

		if (savingGamePanelInfoCoroutine != null) {
			StopCoroutine (savingGamePanelInfoCoroutine);
		}

		savingGamePanelInfoCoroutine = StartCoroutine (showSavingGamePanelInfoCoroutine ());
	}

	IEnumerator showSavingGamePanelInfoCoroutine ()
	{
		CanvasGroup currentCanvasGroup = savingIcon.GetComponent<CanvasGroup> ();

		if (currentCanvasGroup != null) {
			savingIcon.SetActive (true);

			float targetValue = 1;

			while (currentCanvasGroup.alpha != targetValue) {
				currentCanvasGroup.alpha = Mathf.MoveTowards (currentCanvasGroup.alpha, targetValue, Time.deltaTime * showSavingGamePanelFadeSpeed);

				yield return null;
			}

			yield return new WaitForSeconds (showSavingGamePanelDuration);

			targetValue = 0;

			while (currentCanvasGroup.alpha != targetValue) {
				currentCanvasGroup.alpha = Mathf.MoveTowards (currentCanvasGroup.alpha, targetValue, Time.deltaTime * showSavingGamePanelFadeSpeed);

				yield return null;
			}

			savingIcon.SetActive (false);
		}

		yield return null;
	}


	public void saveGameCheckpoint (Transform customTransform, int checkpointID, int checkpointSceneID, 
	                                bool overwriteThisCheckpointActive, bool savingGameToChangeScene, bool autoSaveGameOnSceneChangeEnabled)
	{
		int saveSlotIndex = -1;

		for (int i = 0; i < saveGameListElements.Count; i++) {
			if (savingGameToChangeScene) {
				if (saveGameListElements [i].usedForAutoLoadSave) {
					saveSlotIndex = i;
				}
			} else {
				if (saveGameListElements [i].isCheckpointSave) {
					saveSlotIndex = i;
				}
			}
		}

		List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

		if (!overwriteThisCheckpointActive) {

			int saveListCount = saveList.Count;

			for (int j = 0; j < saveListCount; j++) {
				saveStationInfo currentSave = saveList [j];

				if (currentSave.isCheckpointSave && currentSave.saveNumber == saveSlotIndex) {
					
					if (showDebugInfo) {
						print (currentSave.checkpointID + " " + checkpointID + " " + currentSave.checkpointSceneID + " " + checkpointSceneID);
					}

					if (currentSave.checkpointID == checkpointID && currentSave.checkpointSceneID == checkpointSceneID) {
						if (showDebugInfo) {
							print ("trying to save in the same checkpoint where the player has started, save canceled");
						}

						checkpointSystem checkpointManager = FindObjectOfType<checkpointSystem> ();

						if (checkpointManager != null) {
							checkpointManager.disableCheckPoint (checkpointID);
						}

						return;
					}
				}
			}
		}

		bool useCustomSaveTransform = false;

		if (customTransform != null) {
			setSaveGameTransformInfo (customTransform, false);

			useCustomSaveTransform = true;
		} else {
			setSaveGameTransformInfo (saveStationPosition, true);
		}

		saveCurrentGame (saveSlotIndex, useCustomSaveTransform, true, checkpointID, checkpointSceneID, savingGameToChangeScene, true, autoSaveGameOnSceneChangeEnabled);

		if (useSavingIcon) {
			playSavingGameAnimation ();
		}
	}

	public void saveCurrentGame (int saveSlotIndex, bool useCustomSaveTransform, bool isCheckpointSave, int checkpointID,
	                             int checkpointSceneID, bool savingGameToChangeScene, bool showSaveSlot, bool saveGameInfoActive)
	{
		getPlayerComponents ();

		bool saveLocated = false;

		saveStationInfo newSave = new saveStationInfo (mainSaveStationInfo);

		List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

		int currentSaveNumber = 0;

		if (saveList.Count == 0) {
			saveList = loadFile ();

			gameManagerComponent.setSaveList (saveList);
		}

		int saveListCount = saveList.Count;

		for (int j = 0; j < saveListCount; j++) {
			if (saveList [j].saveNumber == saveSlotIndex && !saveLocated) {
				newSave = saveList [j];

				if (showDebugInfo) {
					print ("Overwriting a previous slot " + saveSlotIndex + " " + j);
				}

				saveLocated = true;

				currentSaveNumber = saveSlotIndex;
				//print ("save found");
			}
		}

		newSave.saveStationPositionX = mainSaveStationInfo.saveStationPositionX;
		newSave.saveStationPositionY = mainSaveStationInfo.saveStationPositionY;
		newSave.saveStationPositionZ = mainSaveStationInfo.saveStationPositionZ;
		newSave.saveStationRotationX = mainSaveStationInfo.saveStationRotationX;
		newSave.saveStationRotationY = mainSaveStationInfo.saveStationRotationY;
		newSave.saveStationRotationZ = mainSaveStationInfo.saveStationRotationZ;

		if (usePlayerCameraOrientation) {
			newSave.playerCameraRotationX = mainSaveStationInfo.playerCameraRotationX;
			newSave.playerCameraRotationY = mainSaveStationInfo.playerCameraRotationY;
			newSave.playerCameraRotationZ = mainSaveStationInfo.playerCameraRotationZ;

			newSave.playerCameraPivotRotationX = mainSaveStationInfo.playerCameraPivotRotationX;
		}

		if (!saveLocated) {
			//print ("new save");
			newSave.playTime = gameManagerComponent.getPlayTime ();
		} else {
			newSave.playTime += gameManagerComponent.getPlayTime ();
		}

		if (isPhysicalSaveStation || useCustomSaveTransform) {
			newSave.useRaycastToPlacePlayer = true;
		}

		newSave.usePlayerCameraOrientation = mainSaveStationInfo.usePlayerCameraOrientation;

		newSave.isCheckpointSave = isCheckpointSave;
		newSave.checkpointID = checkpointID;
		newSave.checkpointSceneID = checkpointSceneID;

		gameManagerComponent.setPlayTimeValue (0);

		newSave.chapterNumberAndName = gameManagerComponent.getChapterName ();

		newSave.saveNumber = saveSlotIndex;

		newSave.saveDate = System.DateTime.Now;

		newSave.showSaveSlot = showSaveSlot;

		if (savingGameToChangeScene) {
			newSave.saveStationScene = checkpointSceneID;
		} else {
			newSave.saveStationScene = mainSaveStationInfo.saveStationScene;
		}

		if (mainPlayerController != null && mainPlayerController.isPlayerDriving ()) {
			string vehicleName = mainPlayerController.getCurrentVehicleName ();

			GameObject currentVehicle = mainPlayerController.getCurrentVehicle ();

			if (currentVehicle != null) {
				elementOnSceneHelper vehicleElementOnSceneHelper = currentVehicle.GetComponentInChildren<elementOnSceneHelper> ();

				if (vehicleElementOnSceneHelper != null) {
					vehicleElementOnSceneHelper.setElementActiveState (false);

					newSave.currentVehicleElementScene = vehicleElementOnSceneHelper.getElementScene ();
					newSave.currentVehicleElementID = vehicleElementOnSceneHelper.getElementID ();
				}
			}

			newSave.isPlayerDriving = true;
			newSave.currentVehicleName = vehicleName;
		} else {
			newSave.isPlayerDriving = false;
		}

		newSave.savingGameToChangeScene = savingGameToChangeScene;

		newSave.usedForAutoLoadSave = savingGameToChangeScene;

		if (!saveLocated) {
			saveList.Add (newSave);

			currentSaveNumber = saveSlotIndex;
		}

		showSaveList (saveList);

		if (gameManagerComponent != null && gameManagerComponent.saveCameraCapture) {
			saveCameraView (newSave.saveNumber.ToString ());
		}

		if (!isCheckpointSave && showSaveSlot) {
			updateSaveSlotContent (saveSlotIndex, newSave);
		}

		gameManagerComponent.setSaveList (saveList);

		BinaryFormatter bf = new BinaryFormatter ();

		FileStream file = File.Create (currentSaveDataPath + currentSaveDataName + ".txt"); 

		bf.Serialize (file, saveList);

		file.Close ();

		changeButtonsColor (false, false, false, false);
	

		if (saveGameInfoActive) {
			//Save the info of the list element configured specifically to save different values from different components
			if (saveInfoList.Count == 0) {
				saveInfoList = mainPlayerComponentsManager.getSaveGameSystem ().getSaveInfoList ();
			}

			string extraFileString = " " + gameManagerComponent.getVersionNumber () + gameManagerComponent.getFileExtension ();

			int saveInfoListCount = saveInfoList.Count;


			for (int i = 0; i < saveInfoListCount; i++) {
				saveInfo currentSaveInfo = saveInfoList [i];

				if (currentSaveInfo.saveInfoEnabled) {

					string newDataPath = currentSaveDataPath + currentSaveInfo.saveFileName + extraFileString;

					int playerID = 0;

					if (mainPlayerController != null) {
						playerID = mainPlayerController.getPlayerID ();
					}

					currentSaveInfo.mainSaveGameInfo.saveGame (currentSaveNumber, playerID, 
						newDataPath, (currentSaveInfo.showDebugInfo && !ignoreShowDebugInfoOnSave));
				}
			}
		}

		if (mainPlayerCameraManager != null) {
			mainPlayerCameraManager.updateCameraList ();
		}

		if (useEventOnSaveGame) {
			eventOnSaveGame.Invoke (saveSlotIndex);
		}
	}

	public void saveCustomInfoList (string listName)
	{
		int currentIndex = customSaveListInfoList.FindIndex (s => s.Name == listName);

		if (currentIndex > -1) {
			customSaveListInfo currentCustomSaveListInfo = customSaveListInfoList [currentIndex];

			if (!currentCustomSaveListInfo.saveListInfoEnabled) {
				return;
			}

			//Save the info of the list element configured specifically to save different values from different components
			if (saveInfoList.Count == 0) {
				saveInfoList = mainPlayerComponentsManager.getSaveGameSystem ().getSaveInfoList ();
			}

			string extraFileString = " " + gameManagerComponent.getVersionNumber () + gameManagerComponent.getFileExtension ();

			int saveInfoListCount = saveInfoList.Count;


			for (int i = 0; i < saveInfoListCount; i++) {
				saveInfo currentSaveInfo = saveInfoList [i];

				if (currentSaveInfo.saveInfoEnabled && currentCustomSaveListInfo.customSaveGameInfoList.Contains (currentSaveInfo.Name)) {

					string newDataPath = currentSaveDataPath + currentSaveInfo.saveFileName + extraFileString;

					int playerID = 0;

					if (mainPlayerController != null) {
						playerID = mainPlayerController.getPlayerID ();
					}

					currentSaveInfo.mainSaveGameInfo.saveGame (currentCustomSaveListInfo.saveNumber, playerID, 
						newDataPath, (currentSaveInfo.showDebugInfo && !ignoreShowDebugInfoOnSave));
				}
			}
		}
	}

	public void saveGameInfoFromEditor (int saveNumberToLoad, string infoTypeName, string extraFileString)
	{
		int saveInfoListCount = saveInfoList.Count;

		for (int i = 0; i < saveInfoListCount; i++) {
			saveInfo currentSaveInfo = saveInfoList [i];

			if (currentSaveInfo.saveInfoEnabled && currentSaveInfo.Name.Equals (infoTypeName)) {

				string newDataPath = currentSaveDataPath + currentSaveInfo.saveFileName + extraFileString;

				currentSaveInfo.mainSaveGameInfo.saveGameFromEditor (saveNumberToLoad, mainPlayerController.getPlayerID (), newDataPath, currentSaveInfo.showDebugInfo);

				return;
			}
		}
	}

	public void saveGameWhenReturningHomeMenu ()
	{
		saveGameCheckpoint (null, 0, 0, false, false, true);
	}

	public List<saveInfo> getSaveInfoList ()
	{
		return saveInfoList;
	}

	public void checkGameInfoToLoad (int lastSaveNumber, string currentSaveDataPath, string extraFileString)
	{
		int saveInfoListCount = saveInfoList.Count;

		for (int i = 0; i < saveInfoListCount; i++) {
			saveInfo currentSaveInfo = saveInfoList [i];

			if (currentSaveInfo.loadInfoEnabled) {

				string newDataPath = currentSaveDataPath + currentSaveInfo.saveFileName + extraFileString;

				int playerID = 0;

				if (mainPlayerController != null) {
					playerID = mainPlayerController.getPlayerID ();
				}

				currentSaveInfo.mainSaveGameInfo.loadGame (lastSaveNumber, playerID,
					newDataPath, (currentSaveInfo.showDebugInfo && !ignoreShowDebugInfoOnLoad));
			}
		}
	}

	public void checkComponentsToInitialize ()
	{
		int saveInfoListCount = saveInfoList.Count;

		for (int i = 0; i < saveInfoListCount; i++) {
			saveInfo currentSaveInfo = saveInfoList [i];

			if (currentSaveInfo.showDebugInfo && !ignoreShowDebugInfoOnLoad) {
				print ("initializing values on " + currentSaveInfo.Name);
			}

			currentSaveInfo.mainSaveGameInfo.initializeValuesOnComponent ();
		}
	}

	public void updateSaveSlotContent (int saveSlotIndex, saveStationInfo newSave)
	{
		buttonInfo currentSaveButtonInfo = saveGameListElements [saveSlotIndex];

		if (gameManagerComponent != null && gameManagerComponent.saveCameraCapture) {
			currentSaveButtonInfo.icon.enabled = true;

			#if !UNITY_WEBPLAYER

			string fullFileName = currentSaveDataPath + (currentSaveDataName + "_" + newSave.saveNumber + ".png");

			if (File.Exists (fullFileName)) {
				byte[] bytes = File.ReadAllBytes (fullFileName);
				Texture2D texture = new Texture2D ((int)captureResolution.x, (int)captureResolution.y);
				texture.filterMode = FilterMode.Trilinear;
				texture.LoadImage (bytes);

				currentSaveButtonInfo.icon.texture = texture;
			}

			#endif
		}

		currentSaveButtonInfo.chapterName.text = newSave.chapterNumberAndName;

		currentSaveButtonInfo.playTime.text = convertSecondsIntoHours (newSave.playTime);

		currentSaveButtonInfo.saveNumber.text = "Save " + (newSave.saveNumber + 1);
		currentSaveButtonInfo.saveDate.text = String.Format ("{0:dd/MM/yy}", newSave.saveDate);
		currentSaveButtonInfo.saveHour.text = newSave.saveDate.Hour.ToString ("00") + ":" + newSave.saveDate.Minute.ToString ("00");
	}

	public void reloadLastCheckpoint ()
	{
		bool checkpointSlotFound = false;

		currentButtonIndex = -1;

		for (int i = 0; i < saveGameListElements.Count; i++) {	
			buttonInfo currentSaveButtonInfo = saveGameListElements [i];

			if (!checkpointSlotFound) {	
				if (currentSaveButtonInfo.isCheckpointSave && !currentSaveButtonInfo.usedForAutoLoadSave) {
					
					currentButtonIndex = i;	

					currentButton = currentSaveButtonInfo.button;

					canLoad = true;

					checkpointSlotFound = true;

					loadGame ();
				}
			}
		}
	}

	public void activateAutoLoad ()
	{
		bool checkpointSlotFound = false;

		currentButtonIndex = -1;

		for (int i = 0; i < saveGameListElements.Count; i++) {	
			buttonInfo currentSaveButtonInfo = saveGameListElements [i];

			if (!checkpointSlotFound) {	
				if (currentSaveButtonInfo.usedForAutoLoadSave) {
					
					currentButtonIndex = i;	

					currentButton = currentSaveButtonInfo.button;

					canLoad = true;

					checkpointSlotFound = true;

					loadGame ();
				}
			}
		}
	}

	public void loadGameBySlotIndex (int slotIndexToLoad)
	{
		if (slotIndexToLoad < saveGameListElements.Count) {
			currentButtonIndex = slotIndexToLoad;

			buttonInfo currentSaveButtonInfo = saveGameListElements [currentButtonIndex];

			currentButton = currentSaveButtonInfo.button;

			canLoad = true;

			loadGame ();
		}
	}

	public void loadGame ()
	{
		if (currentButton != null && canLoad) {
			saveStationInfo newSave = new saveStationInfo (mainSaveStationInfo);

			List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

			if (saveList.Count == 0) {
				
				saveList = loadFile ();

				gameManagerComponent.setSaveList (saveList);
			}

			int saveListCount = saveList.Count;

			for (int j = 0; j < saveListCount; j++) {
				if (saveList [j].saveNumber == currentButtonIndex) {
					newSave = new saveStationInfo (saveList [j]);
					//print ("save loaded");

					if (showDebugInfo) {
						showSaveSlotInfo (newSave, j);
					}
				}
			}

			gameManagerComponent.getPlayerPrefsInfo (newSave);

			if (useEventOnLoadGame) {
				eventOnLoadGame.Invoke (newSave.saveNumber);
			}
		}
	}

	public void deleteGame ()
	{
		if (currentButton != null && canDelete) {
			bool saveLocated = false;

			saveStationInfo newSave = new saveStationInfo (mainSaveStationInfo);

			List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

			if (saveList.Count == 0) {
				saveList = loadFile ();

				gameManagerComponent.setSaveList (saveList);
			}

			int saveListCount = saveList.Count;

			for (int j = 0; j < saveListCount; j++) {
				if (saveList [j].saveNumber == currentButtonIndex) {
					newSave = saveList [j];

					saveLocated = true;

					//print ("save deleted");
				}
			}

			string fileName = currentSaveDataPath + (currentSaveDataName + "_" + newSave.saveNumber + ".png");

			if (File.Exists (fileName)) {
				File.Delete (fileName);
			}

			if (saveLocated) {
				saveList.Remove (newSave);
			}

			showSaveList (saveList);

			buttonInfo currentSaveButtonInfo = saveGameListElements [currentButtonIndex];

			currentSaveButtonInfo.icon.enabled = false;
			currentSaveButtonInfo.chapterName.text = "Chapter -";
			currentSaveButtonInfo.saveNumber.text = "Save -";
			currentSaveButtonInfo.playTime.text = "--:--:--";
			currentSaveButtonInfo.saveDate.text = "--/--/--";
			currentSaveButtonInfo.saveHour.text = "--:--";

			if (currentSaveButtonInfo.isCheckpointSave) {
				currentSaveButtonInfo.slotGameObject.SetActive (false);

				scrollBar.value = 0;
			}

			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Create (currentSaveDataPath + currentSaveDataName + ".txt"); 

			bf.Serialize (file, saveList);

			file.Close ();

			changeButtonsColor (false, false, false, false);
		}
	}

	public void changeButtonsColor (bool state, bool save, bool load, bool delete)
	{
		if (saveButton != null) {
			if (save) {
				saveButton.color = originalColor;
			} else {
				saveButton.color = disableButtonsColor;
			}
		}

		if (load) {
			loadButton.color = originalColor;
		} else {
			loadButton.color = disableButtonsColor;
		}

		if (delete) {
			deleteButton.color = originalColor;
		} else {
			deleteButton.color = disableButtonsColor;
		}

		canSave = save;
		canLoad = load;
		canDelete = delete;

		if (!state) {
			currentButton = null;
		}
	}

	public void loadStates ()
	{
		List<saveStationInfo> saveList = gameManagerComponent.getCurrentSaveList ();

		if (saveList.Count == 0) {
			
			saveList = loadFile ();

			gameManagerComponent.setSaveList (saveList);
		}

		showSaveList (saveList);

		for (int i = 0; i < saveGameListElements.Count; i++) {
			buttonInfo currentSaveButtonInfo = saveGameListElements [i];

			int saveListCount = saveList.Count;

			for (int j = 0; j < saveListCount; j++) {
				saveStationInfo currentSaveStationInfo = saveList [j];

				if (currentSaveStationInfo.saveNumber == i) {
					
					updateSaveSlotContent (i, currentSaveStationInfo);

					currentSaveButtonInfo.infoAdded = true;

					if (currentSaveStationInfo.isCheckpointSave && !currentSaveStationInfo.usedForAutoLoadSave) {
						currentSaveButtonInfo.slotGameObject.SetActive (true);
					}
				}
			}
		}

		for (int i = 0; i < saveGameListElements.Count; i++) {
			buttonInfo currentSaveButtonInfo = saveGameListElements [i];

			if (!currentSaveButtonInfo.infoAdded) {
				currentSaveButtonInfo.icon.enabled = false;
				currentSaveButtonInfo.chapterName.text = "Chapter -";
				currentSaveButtonInfo.saveNumber.text = "Save -";
				currentSaveButtonInfo.playTime.text = "--:--:--";
				currentSaveButtonInfo.saveDate.text = "--/--/--";
				currentSaveButtonInfo.saveHour.text = "--:--";
			}
		}
	}

	void saveCameraView (string saveNumber)
	{
		if (photoCapturer == null) {

			if (mainPlayerCameraManager != null) {
				photoCapturer = mainPlayerCameraManager.getMainCamera ();
			}

			if (photoCapturer == null) {
				return;
			}
		}

		// get the camera's render texture
		setPhotoCapturerState (true);

		photoCapturer.targetTexture = new RenderTexture ((int)captureResolution.x, (int)captureResolution.y, 24);
		RenderTexture rendText = RenderTexture.active;
		RenderTexture.active = photoCapturer.targetTexture;

		// render the texture
		photoCapturer.Render ();
		// create a new Texture2D with the camera's texture, using its height and width
		Texture2D cameraImage = new Texture2D ((int)captureResolution.x, (int)captureResolution.y, TextureFormat.RGB24, false);
		cameraImage.ReadPixels (new Rect (0, 0, (int)captureResolution.x, (int)captureResolution.y), 0, 0);

		cameraImage.Apply ();

		RenderTexture.active = rendText;

		// store the texture into a .PNG file
		#if !UNITY_WEBPLAYER

		byte[] bytes = cameraImage.EncodeToPNG ();

		// save the encoded image to a file
		System.IO.File.WriteAllBytes (currentSaveDataPath + (currentSaveDataName + "_" + saveNumber + ".png"), bytes);

		#endif

		photoCapturer.targetTexture = null;

		setPhotoCapturerState (false);
	}

	public void setPhotoCapturerState (bool state)
	{
		if (isPhysicalSaveStation) {
			photoCapturer.enabled = state;
		}
	}

	string convertSecondsIntoHours (float value)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds (value);

		string timeText = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

		return timeText;
	}

	public List<saveStationInfo> loadFile ()
	{
		List<saveStationInfo> saveList = new List<saveStationInfo> ();

		string fullFileName = currentSaveDataPath + currentSaveDataName + ".txt";

		if (File.Exists (fullFileName)) {
			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (fullFileName, FileMode.Open);

			saveList = (List<saveStationInfo>)bf.Deserialize (file);

			file.Close ();	
		}

		return saveList;
	}

	public bool isThereSaveSlotsLoaded ()
	{
		List<saveGameSystem.saveStationInfo> saveList = loadFile ();

		int saveListCount = saveList.Count;

		for (int j = 0; j < saveListCount; j++) {
			saveStationInfo currentSaveInfo = saveList [j];

			if (currentSaveInfo.showSaveSlot) {
				return true;
			}
		}

		return false;
	}

	public void setStationIde (int idValue)
	{
		mainSaveStationInfo.id = idValue;
	}

	public void showSaveList (List<saveStationInfo> saveList)
	{
		if (!showDebugInfo) {
			return;
		}

		int saveListCount = saveList.Count;

		for (int i = 0; i < saveListCount; i++) {
			showSaveSlotInfo (saveList [i], i);
		}
	}

	public void showSaveSlotInfo (saveStationInfo saveSlot, int saveIndex)
	{
		print ("SAVE " + saveIndex);
		print ("Chapter " + saveSlot.chapterNumberAndName);
		print ("Position " + saveSlot.saveStationPositionX + " " + saveSlot.saveStationPositionY + " " + saveSlot.saveStationPositionZ);
		print ("Scene Index " + saveSlot.saveStationScene);
		print ("Id " + saveSlot.id);
		print ("Save Number " + saveSlot.saveNumber);
		print ("PlayTime " + saveSlot.playTime);
		print ("Date " + saveSlot.saveDate);
		print ("Hour " + saveSlot.saveDate.Hour + ":" + saveSlot.saveDate.Minute);
		print ("\n");
	}

	public void getPlayerComponents ()
	{
		if (playerControllerGameObject == null && electronicDeviceManager != null) {
			playerControllerGameObject = electronicDeviceManager.getCurrentPlayer ();

			mainPlayerComponentsManager = playerControllerGameObject.GetComponent<playerComponentsManager> ();

			mainPlayerController = mainPlayerComponentsManager.getPlayerController ();

			mainPlayerCameraManager = mainPlayerComponentsManager.getPlayerCamera ();
		}
	}

	public void resetScroll (Scrollbar scrollBarToReset)
	{
		StartCoroutine (resetScrollCoroutine (scrollBarToReset));
	}

	IEnumerator resetScrollCoroutine (Scrollbar scrollBarToReset)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		if (scrollBarToReset != null) {
			scrollBarToReset.value = 1;
		}
	}

	[System.Serializable]
	public class saveStationInfo
	{
		public string chapterNumberAndName;
		public float saveStationPositionX;
		public float saveStationPositionY;
		public float saveStationPositionZ;
		public float saveStationRotationX;
		public float saveStationRotationY;
		public float saveStationRotationZ;

		public bool usePlayerCameraOrientation;
		public float playerCameraRotationX;
		public float playerCameraRotationY;
		public float playerCameraRotationZ;

		public float playerCameraPivotRotationX;

		public int saveStationScene;

		public bool isPlayerDriving;
		public string currentVehicleName;

		public int currentVehicleElementScene;
		public int currentVehicleElementID;

		public int id;
		public int saveNumber;
		public float playTime;
		public DateTime saveDate;

		public bool showSaveSlot;

		public bool useRaycastToPlacePlayer;

		public bool isCheckpointSave;

		public int checkpointID;

		public int checkpointSceneID;

		public bool savingGameToChangeScene;

		public bool usedForAutoLoadSave;

		public saveStationInfo ()
		{

		}

		public saveStationInfo (saveStationInfo newSaveStationInfo)
		{
			chapterNumberAndName = newSaveStationInfo.chapterNumberAndName;
			saveStationPositionX = newSaveStationInfo.saveStationPositionX;
			saveStationPositionY = newSaveStationInfo.saveStationPositionY;
			saveStationPositionZ = newSaveStationInfo.saveStationPositionZ;
			saveStationRotationX = newSaveStationInfo.saveStationRotationX;
			saveStationRotationY = newSaveStationInfo.saveStationRotationY;
			saveStationRotationZ = newSaveStationInfo.saveStationRotationZ;

			usePlayerCameraOrientation = newSaveStationInfo.usePlayerCameraOrientation;
			playerCameraRotationX = newSaveStationInfo.playerCameraRotationX;
			playerCameraRotationY = newSaveStationInfo.playerCameraRotationY;
			playerCameraRotationZ = newSaveStationInfo.playerCameraRotationZ;

			playerCameraPivotRotationX = newSaveStationInfo.playerCameraPivotRotationX;

			saveStationScene = newSaveStationInfo.saveStationScene;

			isPlayerDriving = newSaveStationInfo.isPlayerDriving;
			currentVehicleName = newSaveStationInfo.currentVehicleName;
			currentVehicleElementScene = newSaveStationInfo.currentVehicleElementScene;
			currentVehicleElementID = newSaveStationInfo.currentVehicleElementID;

			id = newSaveStationInfo.id;
			saveNumber = newSaveStationInfo.saveNumber;
			playTime = newSaveStationInfo.playTime;
			saveDate = newSaveStationInfo.saveDate;

			showSaveSlot = newSaveStationInfo.showSaveSlot;

			useRaycastToPlacePlayer = newSaveStationInfo.useRaycastToPlacePlayer;

			isCheckpointSave = newSaveStationInfo.isCheckpointSave;

			usedForAutoLoadSave = newSaveStationInfo.usedForAutoLoadSave;

			checkpointID = newSaveStationInfo.checkpointID;

			checkpointSceneID = newSaveStationInfo.checkpointSceneID;

			savingGameToChangeScene = newSaveStationInfo.savingGameToChangeScene;
		}
	}

	[System.Serializable]
	public class buttonInfo
	{
		public GameObject slotGameObject;
		public Button button;
		public RawImage icon;
		public Text chapterName;
		public Text playTime;
		public Text saveNumber;
		public Text saveDate;
		public Text saveHour;
		public bool infoAdded;
		public bool isCheckpointSave;

		public bool usedForAutoLoadSave;
	}

	[System.Serializable]
	public class customSaveListInfo
	{
		public string Name;

		public bool saveListInfoEnabled = true;

		public int saveNumber;

		public List<string> customSaveGameInfoList = new List<string> ();
	}
}