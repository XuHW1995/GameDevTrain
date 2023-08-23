﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class gameManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool limitFps;

	public bool updateLimitFpsIfChangeDetected;

	public int targetFps = 60;

	[Space]
	[Header ("Chapter Info")]
	[Space]

	public string chapterInfo;

	[Space]
	[Header ("Save Info")]
	[Space]

	public bool loadEnabled;

	public bool useRelativePath;

	public bool loadPlayerPositionEnabled = true;

	public bool loadPlayerPositionOnSceneChangeEnabled = true;

	public bool deleteAllPlayerPrefsOnLoadEnabled = true;

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
	[Header ("Version Number Settings")]
	[Space]

	public string versionNumber = "3-01";

	[Space]
	[Header ("Save File Names Settings")]
	[Space]

	public string saveGameFolderName = "Save";
	public string saveFileName = "Save State";

	public string saveCaptureFolder = "Captures";
	public string saveCaptureFileName = "Photo";

	public string touchControlsPositionFolderName = "Touch Buttons Position";
	public string touchControlsPositionFileName = "Touch Positions";

	public string saveInputFileFolderName = "Input";
	public string saveInputFileName = "Input File";

	public string defaultInputSaveFileName = "Default Input File";

	public string fileExtension = ".txt";

	public int slotBySaveStation;

	public bool saveCameraCapture = true;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public LayerMask layer;

	[Space]
	[Header ("Debug Settings")]
	[Space]

	public int saveNumberToLoad = -1;

	public string currentPersistentDataPath;

	public string currentSaveDataPath;

	[Space]
	[Header ("Save List Debug")]
	[Space]

	public List<saveGameSystem.saveStationInfo> saveList = new List<saveGameSystem.saveStationInfo> ();

	[Space]
	[Header ("Current Game State Debug")]
	[Space]

	public bool gamePaused;

	public bool useTouchControls = false;

	public bool touchPlatform;

	public float playTime;

	[Space]
	[Header ("Game Manager Elements")]
	[Space]

	public playerCharactersManager charactersManager;

	public Camera mainCamera;

	public GameObject mainPlayerGameObject;
	public GameObject mainPlayerCameraGameObject;
	public playerCamera currentPlayerCamera;

	RaycastHit hit;

	int lastSaveNumber = -1;

	bool FPSLimitAdjusted;

	void Awake ()
	{
		if (limitFps) {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFps;
		} else {
			Application.targetFrameRate = -1;
		}

		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (touchPlatform) {
			useRelativePath = false;
		}

		checkGameInfoToLoad ();
	}

	void checkGameInfoToLoad ()
	{
		if (isLoadGameEnabled () && getLastSaveNumber () > -1) {

			string extraFileString = " " + getVersionNumber () + getFileExtension ();

			lastSaveNumber = -1;
			if (PlayerPrefs.HasKey ("saveNumber")) {
				lastSaveNumber = PlayerPrefs.GetInt ("saveNumber");
			} else if (saveNumberToLoad >= 0) {
				lastSaveNumber = saveNumberToLoad;
			}

			currentSaveDataPath = getDataPath ();

			if (charactersManager != null) {
				charactersManager.checkGameInfoToLoad (lastSaveNumber, currentSaveDataPath, extraFileString);
			}
		} else {
			if (charactersManager != null) {
				charactersManager.checkComponentsToInitialize ();
			}
		}
	}

	public void saveGameInfoFromEditor (string infoTypeName)
	{
		if (isLoadGameEnabled () && saveNumberToLoad > -1) {

			string extraFileString = " " + getVersionNumber () + getFileExtension ();

			charactersManager.saveGameInfoFromEditor (saveNumberToLoad, infoTypeName, extraFileString);
		}
	}

	public void saveGameWhenReturningHomeMenu ()
	{
		if (isLoadGameEnabled ()) {
			charactersManager.saveGameWhenReturningHomeMenu ();
		}
	}

	void Start ()
	{
		currentPersistentDataPath = Application.persistentDataPath;

		if (loadEnabled && mainPlayerGameObject != null) {
			if (PlayerPrefs.HasKey ("chapterInfo")) {
				chapterInfo = PlayerPrefs.GetString ("chapterInfo");
			}

			if (PlayerPrefs.HasKey ("loadingGame")) {
				if (PlayerPrefs.GetInt ("loadingGame") == 1) {

					bool setPlayerPositionResult = true;

					Vector3 newPlayerPosition = Vector3.zero;
					Quaternion newPlayerRotation = Quaternion.identity;

					bool savingGameToChangeScene = false;

					if (PlayerPrefs.GetInt ("savingGameToChangeScene") == 1) {

						int levelManagerIDToLoad = PlayerPrefs.GetInt ("levelManagerIDToLoad");

						bool targetFound = false;

						levelManagerIngame[] levelManagerIngameList = FindObjectsOfType<levelManagerIngame> ();

						foreach (levelManagerIngame currentLevelManagerIngame in levelManagerIngameList) {
							if (!targetFound && currentLevelManagerIngame.levelManagerID == levelManagerIDToLoad) {
								newPlayerPosition = currentLevelManagerIngame.spawPlayerPositionTransform.position;
								newPlayerRotation = currentLevelManagerIngame.spawPlayerPositionTransform.rotation;

								targetFound = true;

								savingGameToChangeScene = true;
							}
						}

						if (!loadPlayerPositionOnSceneChangeEnabled) {
							setPlayerPositionResult = false;
						}
					} else {
						newPlayerPosition = 
							new Vector3 (PlayerPrefs.GetFloat ("saveStationPositionX"), PlayerPrefs.GetFloat ("saveStationPositionY"), PlayerPrefs.GetFloat ("saveStationPositionZ"));
						
						newPlayerRotation =	
							Quaternion.Euler (PlayerPrefs.GetFloat ("saveStationRotationX"), PlayerPrefs.GetFloat ("saveStationRotationY"), PlayerPrefs.GetFloat ("saveStationRotationZ"));
					
						if (!loadPlayerPositionEnabled) {
							setPlayerPositionResult = false;
						}
					}

					if (PlayerPrefs.GetInt ("useRaycastToPlacePlayer") == 1) {
						if (Physics.Raycast (newPlayerPosition + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, layer)) {
							newPlayerPosition = hit.point;
						}
					}

					bool isPlayerDriving = PlayerPrefs.GetInt ("isPlayerDriving") == 1;
						
					//set player position and rotation
					if (setPlayerPositionResult) {
						mainPlayerGameObject.transform.position = newPlayerPosition;
						mainPlayerGameObject.transform.rotation = newPlayerRotation;
					}

					Quaternion newCameraRotation = newPlayerRotation;

					if (savingGameToChangeScene) {
						newCameraRotation = newPlayerRotation;
					} else {
						if (PlayerPrefs.GetInt ("usePlayerCameraOrientation") == 1) {
							newCameraRotation =
								Quaternion.Euler (PlayerPrefs.GetFloat ("playerCameraRotationX"), PlayerPrefs.GetFloat ("playerCameraRotationY"), PlayerPrefs.GetFloat ("playerCameraRotationZ"));

							float playerCameraPivotRotationX = PlayerPrefs.GetFloat ("playerCameraPivotRotationX");

							float newLookAngle = playerCameraPivotRotationX;

							if (newLookAngle > 180) {
								newLookAngle -= 360;
							}

							Vector2 newPivotRotation = new Vector2 (0, newLookAngle);

							currentPlayerCamera.setLookAngleValue (newPivotRotation);

							currentPlayerCamera.getPivotCameraTransform ().localRotation = Quaternion.Euler (playerCameraPivotRotationX, 0, 0);
						}
					}
						
					if (setPlayerPositionResult) {
						mainPlayerCameraGameObject.transform.position = newPlayerPosition;
						mainPlayerCameraGameObject.transform.rotation = newCameraRotation;
					}

					if (isPlayerDriving) {
						string vehicleName = PlayerPrefs.GetString ("currentVehicleName");

//						print ("PLAYER PREVIOUSLY DRIVING " + vehicleName);

						prefabsManager newPrefabsManager = FindObjectOfType<prefabsManager> ();

						if (newPrefabsManager == null) {
							newPrefabsManager = GKC_Utils.addPrefabsManagerToScene ();
						}

						if (newPrefabsManager != null) {
							GameObject vehiclePrefab = newPrefabsManager.getPrefabGameObject (vehicleName);

							if (vehiclePrefab != null) {
								mainPlayerGameObject.transform.position = newPlayerPosition + Vector3.right * 4;

								Vector3 vehiclePosition = Vector3.zero;

								if (Physics.Raycast (newPlayerPosition + Vector3.up * 3, -Vector3.up, out hit, Mathf.Infinity, layer)) {
									vehiclePosition = hit.point;
								}

								vehiclePosition += Vector3.up * newPrefabsManager.getPrefabSpawnOffset (vehicleName);

								Vector3 newPlayerRotationEulerAngles = newPlayerRotation.eulerAngles;

								newPlayerRotationEulerAngles = new Vector3 (0, newPlayerRotationEulerAngles.y, 0);

								newPlayerRotation = Quaternion.Euler (newPlayerRotationEulerAngles);

								GameObject newVehicle = (GameObject)Instantiate (vehiclePrefab, vehiclePosition, newPlayerRotation);

								newVehicle.name = vehiclePrefab.name;

								IKDrivingSystem currentIKDrivingSystem = newVehicle.GetComponent<IKDrivingSystem> ();

								GKCSimpleRiderSystem mainGKCSimpleRiderSystem = null;

								if (currentIKDrivingSystem != null) {
									currentIKDrivingSystem.setPlayerToStartGameOnThisVehicle (mainPlayerGameObject);
								} else {
									mainGKCSimpleRiderSystem = newVehicle.GetComponentInChildren<GKCSimpleRiderSystem> ();

									if (mainGKCSimpleRiderSystem == null) {
										GKCRiderSocketSystem currentGKCRiderSocketSystem = newVehicle.GetComponentInChildren<GKCRiderSocketSystem> ();

										if (currentGKCRiderSocketSystem != null) {
											mainRiderSystem currentmainRiderSystem = currentGKCRiderSocketSystem.getMainRiderSystem ();

											mainGKCSimpleRiderSystem = currentmainRiderSystem.GetComponent<GKCSimpleRiderSystem> ();
										}
									}

									if (mainGKCSimpleRiderSystem != null) {
										mainGKCSimpleRiderSystem.setPlayerToStartGameOnThisVehicle (mainPlayerGameObject);
									}
								}

								elementOnSceneHelper vehicleElementOnSceneHelper = newVehicle.GetComponentInChildren<elementOnSceneHelper> ();

								if (vehicleElementOnSceneHelper == null) {
									if (mainGKCSimpleRiderSystem != null) {
										vehicleElementOnSceneHelper = mainGKCSimpleRiderSystem.gameObject.GetComponentInChildren<elementOnSceneHelper> ();
									}
								}
								
								if (vehicleElementOnSceneHelper != null) {
									vehicleElementOnSceneHelper.setNewInstantiatedElementOnSceneManagerIngameWithInfo ();

									int currentVehicleElementScene = PlayerPrefs.GetInt ("currentVehicleElementScene");
									int currentVehicleElementID = PlayerPrefs.GetInt ("currentVehicleElementID");

									vehicleElementOnSceneHelper.setStatsSearchingByInfo (currentVehicleElementScene, currentVehicleElementID);
								}
							} else {
								print ("WARNING: No prefab of the vehicle called " + vehicleName + " was found. Make sure a prefab with that name is configured in the prefabs manager");
							}
						}
					}

					deletePlayerPrefsOnLoad ();
				}
			}

			StartCoroutine (checkDeletePlayerPrefsActive ());
		} else {
			deletePlayerPrefsOnLoad ();
		}
	}

	IEnumerator checkDeletePlayerPrefsActive ()
	{
		yield return new WaitForSeconds (0.3f);

		if (PlayerPrefs.HasKey ("Delete Player Prefs Active")) {
			if (PlayerPrefs.GetInt ("Delete Player Prefs Active") == 1) {
				deletePlayerPrefsOnLoad ();
			}
		}
	}

	void deletePlayerPrefsOnLoad ()
	{
		if (deleteAllPlayerPrefsOnLoadEnabled) {
			PlayerPrefs.DeleteAll ();
		} else {
			deletePlayerPrefByName ("loadingGame");

			deletePlayerPrefByName ("saveNumber");

			deletePlayerPrefByName ("currentSaveStationId");

			deletePlayerPrefByName ("saveStationPositionX");
			deletePlayerPrefByName ("saveStationPositionY");
			deletePlayerPrefByName ("saveStationPositionZ");
			deletePlayerPrefByName ("saveStationRotationX");
			deletePlayerPrefByName ("saveStationRotationY");
			deletePlayerPrefByName ("saveStationRotationZ");

			deletePlayerPrefByName ("usePlayerCameraOrientation");

			deletePlayerPrefByName ("playerCameraRotationX");
			deletePlayerPrefByName ("playerCameraRotationY");
			deletePlayerPrefByName ("playerCameraRotationZ");
			deletePlayerPrefByName ("playerCameraPivotRotationX");

			deletePlayerPrefByName ("useRaycastToPlacePlayer");

			deletePlayerPrefByName ("savingGameToChangeScene");

			deletePlayerPrefByName ("levelManagerIDToLoad");

			deletePlayerPrefByName ("isPlayerDriving");
			deletePlayerPrefByName ("currentVehicleName");
			deletePlayerPrefByName ("currentVehicleElementScene");
			deletePlayerPrefByName ("currentVehicleElementID");

			deletePlayerPrefByName ("saveNumber");

			deletePlayerPrefByName ("Delete Player Prefs Active");
		}
	}

	void deletePlayerPrefByName (string newName)
	{
		if (PlayerPrefs.HasKey (newName)) {
			PlayerPrefs.DeleteKey (newName);
		}
	}

	void Update ()
	{
		playTime += Time.unscaledDeltaTime;
	
		if (limitFps) {
			if (updateLimitFpsIfChangeDetected || !FPSLimitAdjusted) {
				if (Application.targetFrameRate != targetFps) {
					Application.targetFrameRate = targetFps;
				}

				if (!updateLimitFpsIfChangeDetected) {
					FPSLimitAdjusted = true;
				}
			}
		}
	}

	public void getPlayerPrefsInfo (saveGameSystem.saveStationInfo save)
	{
		PlayerPrefs.SetInt ("loadingGame", 1);

		PlayerPrefs.SetInt ("saveNumber", save.saveNumber);

		PlayerPrefs.SetInt ("currentSaveStationId", save.id);

		PlayerPrefs.SetFloat ("saveStationPositionX", save.saveStationPositionX);
		PlayerPrefs.SetFloat ("saveStationPositionY", save.saveStationPositionY);
		PlayerPrefs.SetFloat ("saveStationPositionZ", save.saveStationPositionZ);
		PlayerPrefs.SetFloat ("saveStationRotationX", save.saveStationRotationX);
		PlayerPrefs.SetFloat ("saveStationRotationY", save.saveStationRotationY);
		PlayerPrefs.SetFloat ("saveStationRotationZ", save.saveStationRotationZ);

		if (save.usePlayerCameraOrientation) {
			PlayerPrefs.SetInt ("usePlayerCameraOrientation", 1);
		} else {
			PlayerPrefs.SetInt ("usePlayerCameraOrientation", 0);
		}

		PlayerPrefs.SetFloat ("playerCameraRotationX", save.playerCameraRotationX);
		PlayerPrefs.SetFloat ("playerCameraRotationY", save.playerCameraRotationY);
		PlayerPrefs.SetFloat ("playerCameraRotationZ", save.playerCameraRotationZ);
		PlayerPrefs.SetFloat ("playerCameraPivotRotationX", save.playerCameraPivotRotationX);

		if (save.useRaycastToPlacePlayer) {
			PlayerPrefs.SetInt ("useRaycastToPlacePlayer", 1);
		}

		if (save.savingGameToChangeScene) {
			PlayerPrefs.SetInt ("savingGameToChangeScene", 1);
		} else {
			PlayerPrefs.SetInt ("savingGameToChangeScene", 0);
		}

		PlayerPrefs.SetInt ("levelManagerIDToLoad", save.checkpointID);

		if (save.isPlayerDriving) {
			PlayerPrefs.SetInt ("isPlayerDriving", 1);
			PlayerPrefs.SetString ("currentVehicleName", save.currentVehicleName);
			PlayerPrefs.SetInt ("currentVehicleElementScene", save.currentVehicleElementScene);
			PlayerPrefs.SetInt ("currentVehicleElementID", save.currentVehicleElementID);

		} else {
			PlayerPrefs.SetInt ("isPlayerDriving", 0);
		}
			
//		print ("setting prefs to load " + save.saveStationScene + " " + loadScreenScene);

		GKC_Utils.loadScene (save.saveStationScene, useLoadScreen, loadScreenScene, sceneToLoadAsyncPrefsName, 
			useLastSceneIndexAsLoadScreen, checkLoadingScreenSceneConfigured, loadingScreenSceneName);
	}

	public string getDataPath ()
	{
		string dataPath = "";

		if (useRelativePath) {
			dataPath = saveGameFolderName;
		} else {
			dataPath = Application.persistentDataPath + "/" + saveGameFolderName;
		}

		if (!Directory.Exists (dataPath)) {
			Directory.CreateDirectory (dataPath);
		}

		dataPath += "/";

		return dataPath;
	}

	public string getDataName ()
	{
		return saveFileName + " " + versionNumber;
	}

	public string getVersionNumber ()
	{
		return versionNumber;
	}

	public string getFileExtension ()
	{
		return fileExtension;
	}

	public List<saveGameSystem.saveStationInfo> getCurrentSaveList ()
	{
		return saveList;
	}

	public void setSaveList (List<saveGameSystem.saveStationInfo> currentList)
	{
		saveList = currentList;
	}

	public int getLastSaveNumber ()
	{
		lastSaveNumber = -1;

		if (PlayerPrefs.HasKey ("saveNumber")) {
			lastSaveNumber = PlayerPrefs.GetInt ("saveNumber");

		} else if (saveNumberToLoad >= 0) {
			lastSaveNumber = saveNumberToLoad;
		}

		return lastSaveNumber;
	}

	public void setGamePauseState (bool state)
	{
		gamePaused = state;
	}

	public bool isGamePaused ()
	{
		return gamePaused;
	}

	public bool isUsingTouchControls ()
	{
		return useTouchControls;
	}

	public void setUseTouchControlsState (bool state)
	{
		useTouchControls = state;
	}

	public void setCharactersManagerPauseState (bool state)
	{
		charactersManager.setCharactersManagerPauseState (state);
	}

	public playerController getMainPlayerController ()
	{
		return charactersManager.getMainPlayerController ();
	}

	public bool anyCharacterDead ()
	{
		return charactersManager.anyCharacterDead ();
	}

	public string getSaveCaptureFolder ()
	{
		return saveCaptureFolder;
	}

	public string getSaveCaptureFileName ()
	{
		return saveCaptureFileName;
	}

	public string getTouchControlsPositionFolderName ()
	{
		return touchControlsPositionFolderName;
	}

	public string getTouchControlsPositionFileName ()
	{
		return touchControlsPositionFileName + " " + versionNumber;
	}

	public bool isLoadGameEnabled ()
	{
		return loadEnabled;
	}

	public void setNewChapterName (string newName)
	{
		chapterInfo = newName;
	}

	public string getChapterName ()
	{
		return chapterInfo;
	}

	public float getPlayTime ()
	{
		return playTime;
	}

	public void setPlayTimeValue (float newValue)
	{
		playTime = newValue;
	}

	//EDITOR FUNCTIONS
	public void setUseTouchControlsStateFromEditor (bool state)
	{
		useTouchControls = state;

		updateComponent ();
	}

	public void setCurrentPersistentDataPath ()
	{
		currentPersistentDataPath = Application.persistentDataPath;

		updateComponent ();
	}

	public void deletePlayerPrefs ()
	{
		PlayerPrefs.DeleteAll ();

		print ("Player Prefs Deleted");
	}

	public void openCurrentSaveFolder ()
	{
		getCurrentDataPath ();

		showExplorer (currentSaveDataPath);
	}

	public void showExplorer (string itemPath)
	{
		if (Directory.Exists (itemPath)) {
			itemPath = itemPath.Replace (@"/", @"\");  
			System.Diagnostics.Process.Start ("explorer.exe", "/select," + itemPath);
		}
	}

	public void getCurrentDataPath ()
	{
		setCurrentPersistentDataPath ();

		currentSaveDataPath = getDataPath ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
