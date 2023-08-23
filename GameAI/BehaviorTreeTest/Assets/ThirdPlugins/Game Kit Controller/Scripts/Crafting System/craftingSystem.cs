using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class craftingSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool craftingSystemEnabled;

	bool mainCraftingUISystemAssigned;

	public string craftingSystemMenuName = "Crafting System Menu";

	[Space]
	[Header ("Blueprints/recipes unlocked")]
	[Space]

	public bool useOnlyBlueprintsUnlocked;
	public List<string> blueprintsUnlockedList = new List<string> ();

	[Space]

	public bool useEventOnUnlockBlueprint;
	public string extraMessageOnUnlockBlueprint;
	public eventParameters.eventToCallWithString eventOnUnlockBlueprint;

	[Space]
	[Header ("Categories To Craft Available")]
	[Space]

	public bool allowAllObjectCategoriesToCraftAtAnyMomentEnabled = true;

	public List<string> objectCategoriesToCraftAvailableAtAnyMoment = new List<string> ();

	[Space]
	[Header ("Animation Settings")]
	[Space]

	public bool useAnimationOnCraftObjectAnywhere;

	public string animationNameOnCraftAnywhere = "Craft Simple Object";

	public bool useAnimationOnCraftObjectOnWorkbench;

	public string animationNameOnCraftOnWorkbench = "Craft On Workbench";

	[Space]
	[Header ("Placement Mode Active Externally Settings")]
	[Space]

	public bool setPlacementActiveStateExternallyEnabled = true;

	public UnityEvent eventOnSetPlacementActiveStateExternallyNotEnabled;

	public UnityEvent eventOnNoObjectFoundToUsePlacementExternally;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool menuOpened;

	public bool menuOpenedFromWorkbench;

	public int currentInventoryObjectSelectedIndex = -1;

	public bool placementActiveStatExternallyActive;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnOpenMenu;
	public UnityEvent eventOnCloseMenu;

	[Space]

	public UnityEvent eventToStopUsingWorkbenchOnDamageReceived;

	[Space]

	public UnityEvent eventOnOpenWorkbench;
	public UnityEvent eventOnCloseWorkbench;

	[Space]
	[Header ("Components")]
	[Space]

	public craftingUISystem mainCraftingUISystem;

	public inventoryManager mainInventorymanager;

	public menuPause pauseManager;

	public GameObject playerGameObject;

	public Transform positionToSpawnObjectsIfNotSpaceOnInventory;

	public playerController mainPlayerController;

	public craftingPlacementSystem mainCraftingPlacementSystem;

	public objectsStatsSystem mainObjectsStatsSystem;

	public playerStatsSystem mainPlayerStatsSystem;


	void Start ()
	{
		if (!craftingSystemEnabled) {
			return;
		}

		if (!mainCraftingUISystemAssigned) {
			checkAssignCraftingUISystem ();
		}
	}

	void checkAssignCraftingUISystem ()
	{
		if (!mainCraftingUISystemAssigned) {
			ingameMenuPanel currentIngameMenuPanel = pauseManager.getIngameMenuPanelByName (craftingSystemMenuName);

			if (currentIngameMenuPanel == null) {
				pauseManager.checkcreateIngameMenuPanel (craftingSystemMenuName);

				currentIngameMenuPanel = pauseManager.getIngameMenuPanelByName (craftingSystemMenuName);
			}

			if (currentIngameMenuPanel != null) {
				mainCraftingUISystem = currentIngameMenuPanel.GetComponent<craftingUISystem> ();

				mainCraftingUISystemAssigned = true;
			}

			if (mainCraftingUISystemAssigned) {
				setUseOnlyBlueprintsUnlockedState (useOnlyBlueprintsUnlocked);

				if (useOnlyBlueprintsUnlocked) {
					setBlueprintsUnlockedListValue (blueprintsUnlockedList);
				}

				GKC_Utils.updateCanvasValuesByPlayer (null, pauseManager.gameObject, currentIngameMenuPanel.gameObject);
			}
		}
	}

	public void openOrCloseCraftingMenu (bool state)
	{
		if (menuOpened == state) {
			return;
		}

		menuOpened = state;

		if (menuOpened) {
			if (!mainCraftingUISystemAssigned) {
				checkAssignCraftingUISystem ();
			}
		} else {

		}

		checkEventOnStateChange (menuOpened);
	}

	void checkEventOnStateChange (bool state)
	{
		if (state) {
			eventOnOpenMenu.Invoke ();
		} else {
			eventOnCloseMenu.Invoke ();
		}

		if (menuOpenedFromWorkbench) {
			if (state) {
				eventOnOpenWorkbench.Invoke ();
			} else {
				eventOnCloseWorkbench.Invoke ();
			}
		}
	}

	public void checkStateOnCraftObject ()
	{
		if (useAnimationOnCraftObjectAnywhere || useAnimationOnCraftObjectOnWorkbench) {
			if (menuOpenedFromWorkbench) {
				if (useAnimationOnCraftObjectOnWorkbench) {
					mainPlayerController.playerCrossFadeInFixedTime (animationNameOnCraftOnWorkbench);
				}
			} else {
				if (useAnimationOnCraftObjectAnywhere) {
					mainPlayerController.playerCrossFadeInFixedTime (animationNameOnCraftAnywhere);
				}
			}
		}
	}

	//Get/set inventory info functions
	public int getCurrentInventoryObjectIndex ()
	{
		return mainInventorymanager.getCurrentInventoryObjectIndex ();
	}

	public int getInventoryObjectAmountByName (string inventoryObjectName)
	{
		return mainInventorymanager.getInventoryObjectAmountByName (inventoryObjectName);
	}

	public Texture getInventoryObjectIconByName (string inventoryObjectName)
	{
		return mainInventorymanager.getInventoryObjectIconByName (inventoryObjectName);
	}

	public void removeObjectAmountFromInventoryByName (string objectName, int amountToMove)
	{
		mainInventorymanager.removeObjectAmountFromInventoryByName (objectName, amountToMove);
	}

	public void removeObjectAmountFromInventoryByIndex (int objectIndex, int amountToMove)
	{
		mainInventorymanager.removeObjectAmountFromInventory (objectIndex, amountToMove);
	}

	public void giveInventoryObjectToCharacter (string objectName, int objectAmount)
	{
		applyDamage.giveInventoryObjectToCharacter (playerGameObject, objectName, objectAmount, 
			positionToSpawnObjectsIfNotSpaceOnInventory, 0, 2, ForceMode.Force, 0, false);
	}

	public List<inventoryInfo> getInventoryList ()
	{
		return mainInventorymanager.getInventoryList ();
	}

	public inventoryInfo getInventoryInfoByName (string objectName)
	{
		return mainInventorymanager.getInventoryInfoByName (objectName);
	}

	public inventoryInfo getInventoryInfoByIndex (int objectIndex)
	{
		return mainInventorymanager.getInventoryInfoByIndex (objectIndex);
	}

	public bool repairDurabilityObjectByIndex (int objectIndex)
	{
		return mainInventorymanager.repairDurabilityObjectByIndex (objectIndex);
	}

	public bool isObjectBroken (int objectIndex)
	{
		return mainInventorymanager.isObjectBroken (objectIndex);
	}

	public bool isObjectDurabilityComplete (int objectIndex)
	{
		return mainInventorymanager.isObjectDurabilityComplete (objectIndex);
	}

	public GameObject getInventoryMeshByName (string objectName)
	{
		return mainInventorymanager.getInventoryMeshByName (objectName);
	}

	public GameObject getCurrentObjectToPlace ()
	{
		return mainCraftingUISystem.getCurrentObjectToPlace ();
	}

	public GameObject getCurrentObjectToPlaceByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectToPlaceByName (objectName);
	}

	public void updateObjectSelectedName (string newCurrentObjectCategorySelectedName, string newCurrentObjectSelectedName)
	{
		mainCraftingUISystem.updateObjectSelectedName (newCurrentObjectCategorySelectedName, newCurrentObjectSelectedName);
	}

	public LayerMask getCurrentObjectLayerMaskToAttachObjectByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectLayerMaskToAttachObjectByName (objectName);
	}

	public Vector3 getCurrentObjectToPlacePositionOffsetByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectToPlacePositionOffsetByName (objectName);
	}

	public bool checkIfCurrentObjectToPlaceUseCustomLayerMaskByName (string objectName)
	{
		return mainCraftingUISystem.checkIfCurrentObjectToPlaceUseCustomLayerMaskByName (objectName);
	}

	public LayerMask getCurrentObjectCustomLayerMaskToPlaceObjectByName (string objectName)
	{
		return mainCraftingUISystem.getCurrentObjectCustomLayerMaskToPlaceObjectByName (objectName);
	}

	public void getCurrentObjectCanBeRotatedValuesByName (string objectName, ref bool objectCanBeRotatedOnYAxis, ref bool objectCanBeRotatedOnXAxis)
	{
		mainCraftingUISystem.getCurrentObjectCanBeRotatedValuesByName (objectName, ref objectCanBeRotatedOnYAxis, ref objectCanBeRotatedOnXAxis);
	}


	public string getCurrentObjectSelectedName ()
	{
		return mainCraftingUISystem.getCurrentObjectSelectedName ();
	}

	public void setPlacementActiveState (bool state)
	{
		if (mainCraftingPlacementSystem.isPlacementActivePaused ()) {
			return;
		}

		if (mainCraftingUISystem.currentObjectCanBePlaced) {
			if (state) {
				GameObject currentObjectMesh = getInventoryMeshByName (getCurrentObjectSelectedName ());

				if (showDebugPrint) {
					print ("current object mesh located on placement active" + (currentObjectMesh != null));
				}

				if (currentObjectMesh != null) {
					mainCraftingPlacementSystem.setCurrentObjectToPlaceMesh (currentObjectMesh);
				} else {
					return;
				}
			}
		}

		if (state) {
			if (mainCraftingUISystem.menuOpened) {
				mainCraftingUISystem.openOrCloseMenuFromTouch ();
			}
		}

		mainCraftingPlacementSystem.setPlacementActiveState (state);
	}

	public void setPlacementActiveStateExternally (bool state)
	{
		if (mainCraftingPlacementSystem.isPlacementActivePaused ()) {
			return;
		}

		if (!setPlacementActiveStateExternallyEnabled) {
			eventOnSetPlacementActiveStateExternallyNotEnabled.Invoke ();

			if (showDebugPrint) {
				print ("placement active externally not enabled");
			}

			return;
		}

		placementActiveStatExternallyActive = state;

		if (state) {
			currentInventoryObjectSelectedIndex = -1;

			bool objectFound = selectNextOrPreviousObjectForPlacement (true);
		
			if (objectFound) {
				mainCraftingUISystem.currentObjectCanBePlaced = true;

				setPlacementActiveState (true);
			} else {
				eventOnNoObjectFoundToUsePlacementExternally.Invoke ();

				if (mainCraftingPlacementSystem.placementActive) {
					setPlacementActiveState (false);
				}

				if (showDebugPrint) {
					print ("placement active externally not enabled due to not objects to place located on inventory");
				}
			}
		} else {
			setPlacementActiveState (false);
		}
	}

	public void setCurrentInventoryObjectSelectedIndex (int newValue)
	{
		if (showDebugPrint) {
			print ("new index value " + newValue);
		}

		currentInventoryObjectSelectedIndex = newValue;
	}

	public bool selectNextOrPreviousObjectForPlacement (bool state)
	{
		bool objectFound = false;

		List<inventoryInfo> inventoryList = getInventoryList ();

//		print (currentInventoryObjectSelectedIndex);

		if (currentInventoryObjectSelectedIndex == -1) {
			currentInventoryObjectSelectedIndex = 0;

//			print (currentInventoryObjectSelectedIndex);
		} else {
			if (state) {
				currentInventoryObjectSelectedIndex++;

				if (currentInventoryObjectSelectedIndex >= inventoryList.Count - 1) {
					currentInventoryObjectSelectedIndex = 0;
				}

//				print (currentInventoryObjectSelectedIndex);
			} else {
				currentInventoryObjectSelectedIndex--;

				if (currentInventoryObjectSelectedIndex <= 0) {
					currentInventoryObjectSelectedIndex = inventoryList.Count - 1;
				}

//				print (currentInventoryObjectSelectedIndex);
			}
		}

//		print (currentInventoryObjectSelectedIndex);

		if (state) {
			for (int i = currentInventoryObjectSelectedIndex; i < inventoryList.Count; i++) {
				if (!objectFound && mainCraftingUISystem.canObjectBePlaced (inventoryList [i].categoryName, inventoryList [i].Name)) {
					currentInventoryObjectSelectedIndex = i;

					objectFound = true;

					if (showDebugPrint) {
						print ("object located " + inventoryList [i].Name);
					}
				}
			}

			if (!objectFound) {
				currentInventoryObjectSelectedIndex = 0;

				for (int i = 0; i < inventoryList.Count; i++) {
					if (!objectFound && mainCraftingUISystem.canObjectBePlaced (inventoryList [i].categoryName, inventoryList [i].Name)) {
						currentInventoryObjectSelectedIndex = i;

						objectFound = true;

						if (showDebugPrint) {
							print ("object located " + inventoryList [i].Name);
						}
					}
				}
			}
		} else {
			for (int i = currentInventoryObjectSelectedIndex; i >= 0; i--) {
				if (!objectFound && mainCraftingUISystem.canObjectBePlaced (inventoryList [i].categoryName, inventoryList [i].Name)) {
					currentInventoryObjectSelectedIndex = i;

					objectFound = true;

					if (showDebugPrint) {
						print ("object located " + inventoryList [i].Name);
					}
				}
			}

			if (!objectFound) {
				currentInventoryObjectSelectedIndex = 0;

				for (int i = inventoryList.Count - 1; i >= 0; i--) {
					if (!objectFound && mainCraftingUISystem.canObjectBePlaced (inventoryList [i].categoryName, inventoryList [i].Name)) {
						currentInventoryObjectSelectedIndex = i;

						objectFound = true;

						if (showDebugPrint) {
							print ("object located " + inventoryList [i].Name);
						}
					}
				}
			}
		}

		if (objectFound) {
			mainCraftingUISystem.updateObjectSelectedName (inventoryList [currentInventoryObjectSelectedIndex].categoryName, inventoryList [currentInventoryObjectSelectedIndex].Name);
		}

		return objectFound;
	}

	public void setPlacementActivePausedState (bool state)
	{
		mainCraftingPlacementSystem.setPlacementActivePausedState (state);
	}

	public void setOriginalPlacementActivePausedState ()
	{
		mainCraftingPlacementSystem.setOriginalPlacementActivePausedState ();
	}

	public bool checkIfStatValueAvailable (string statName, int statAmount)
	{
		return mainPlayerStatsSystem.checkIfStatValueAvailable (statName, statAmount);
	}

	public void addOrRemovePlayerStatAmount (string statName, int statAmount)
	{
		mainPlayerStatsSystem.addOrRemovePlayerStatAmount (statName, statAmount);
	}

	//blueprints functions
	public void setUseOnlyBlueprintsUnlockedState (bool state)
	{
		useOnlyBlueprintsUnlocked = state;

		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setUseOnlyBlueprintsUnlockedState (state);
		}
	}

	public bool isUseOnlyBlueprintsUnlockedActive ()
	{
		return useOnlyBlueprintsUnlocked;
	}

	public void setBlueprintsUnlockedListValue (List<string> newBlueprintsUnlockedList)
	{
		blueprintsUnlockedList = new List<string> (newBlueprintsUnlockedList);

		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setBlueprintsUnlockedListValue (newBlueprintsUnlockedList);
		}
	}

	public List<string> getBlueprintsUnlockedListValue ()
	{
		return blueprintsUnlockedList;
	}

	public void addNewBlueprintsUnlockedElement (string newBlueprintsUnlockedElement)
	{
		if (!blueprintsUnlockedList.Contains (newBlueprintsUnlockedElement)) {
			blueprintsUnlockedList.Add (newBlueprintsUnlockedElement);

			if (useEventOnUnlockBlueprint) {
				eventOnUnlockBlueprint.Invoke (newBlueprintsUnlockedElement + " " + extraMessageOnUnlockBlueprint);
			}
		}

		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.addNewBlueprintsUnlockedElement (newBlueprintsUnlockedElement);
		}
	}

	public void setObjectCategoriesToCraftAvailableAtAnyMomentValue (List<string> newList)
	{
		objectCategoriesToCraftAvailableAtAnyMoment = newList;
	}

	public List<string> getObjectCategoriesToCraftAvailableAtAnyMomentValue ()
	{
		return objectCategoriesToCraftAvailableAtAnyMoment;
	}

	public void addObjectCategoriesToCraftAvailableAtAnyMomentElement (string newElement)
	{
		if (!objectCategoriesToCraftAvailableAtAnyMoment.Contains (newElement)) {
			objectCategoriesToCraftAvailableAtAnyMoment.Add (newElement);
		}
	}

	public List<craftObjectInTimeSimpleInfo> getCraftObjectInTimeInfoList ()
	{
		return mainCraftingUISystem.getCraftObjectInTimeInfoList ();
	}

	public bool anyObjectToCraftInTimeActive ()
	{
		return mainCraftingUISystem.anyObjectToCraftInTimeActive ();
	}

	public void setCraftObjectInTimeInfoList (List<craftObjectInTimeSimpleInfo> newCraftObjectInTimeSimpleInfoList)
	{
		mainCraftingUISystem.setCraftObjectInTimeInfoList (newCraftObjectInTimeSimpleInfoList);
	}

	public void setOpenFromWorkbenchState (bool state, List<string> newObjectCategoriesToCraftAvailableOnCurrentBench)
	{
		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setOpenFromWorkbenchState (state, newObjectCategoriesToCraftAvailableOnCurrentBench);

			menuOpenedFromWorkbench = state;
		}
	}

	public void setCurrentcurrentCraftingWorkbenchSystem (craftingWorkbenchSystem newCraftingWorkbenchSystem)
	{
		if (mainCraftingUISystem != null) {
			mainCraftingUISystem.setCurrentcurrentCraftingWorkbenchSystem (newCraftingWorkbenchSystem);
		}
	}

	public void stopUsingWorkbenchOnDamageReceived ()
	{
		if (menuOpened) {
			if (menuOpenedFromWorkbench) {
				eventToStopUsingWorkbenchOnDamageReceived.Invoke ();

				mainCraftingUISystem.checkEventToStopUsingWorkbenchOnDamageReceived ();
			}
		}
	}

	public void repairCurrentObjectSelectedOnInventoryMenu ()
	{
		if (mainInventorymanager.checkDurabilityOnObjectEnabled) {
			if (mainCraftingUISystem != null) {
				mainCraftingUISystem.repairCurrentObjectSelectedOnInventoryMenu ();
			}
		}
	}

	public void updateUIAfterRepairingCurrentObjectSelectedOnInventoryMenu (bool state)
	{
		mainInventorymanager.updateUIAfterRepairingCurrentObjectSelectedOnInventoryMenu (state);
	}

	public List<objectStatInfo> getStatsFromObjectByName (string objectName)
	{
		return mainObjectsStatsSystem.getStatsFromObjectByName (objectName);
	}

	public bool objectCanBeUpgraded (string objectName)
	{
		return mainObjectsStatsSystem.objectCanBeUpgraded (objectName);
	}

	//Editor Functions
	public void setCraftingSystemEnabledStateFromEditor (bool state)
	{
		craftingSystemEnabled = state;

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Crafting System State", gameObject);
	}
}