using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class friendListManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool friendManagerEnabled;

	public string menuPanelName = "Friend List Manager";

	public bool useBlurUIPanel = true;
	public bool usedByAI;

	[Space]
	[Header ("Order Info Settings")]
	[Space]

	public List<orderInfo> orderInfoList = new List<orderInfo> ();

	[Space]
	[Header ("Order Button Settings")]
	[Space]

	public List<Button> orderButtonList = new List<Button> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public string callToFriendsOrderName = "Follow";

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventIfSystemDisabled;
	public UnityEvent eventIfSystemDisabled;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool menuOpened;

	public List<friendInfo> friendsList = new List<friendInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public menuPause pauseManager;
	public playerController playerControllerManager;
	public Collider mainCollider;

	public Transform playerTransform;

	public GameObject friendsMenuContent;
	public GameObject friendListContent;
	public GameObject friendListElement;


	friendInfo currentFriendInfo;


	void Start ()
	{
		if (usedByAI) {
			return;
		}

		if (!friendManagerEnabled) {
			checkEventOnSystemDisabled ();
		}

		if (friendListElement.activeSelf) {
			friendListElement.SetActive (false);
		}

		if (friendsMenuContent.activeSelf) {
			friendsMenuContent.SetActive (false);
		}
	}

	public void checkEventOnSystemDisabled ()
	{
		if (useEventIfSystemDisabled) {
			eventIfSystemDisabled.Invoke ();
		}
	}

	public void openOrCloseFriendMenu (bool state)
	{
		if ((!playerControllerManager.isPlayerMenuActive () || menuOpened) &&
		    (!playerControllerManager.isUsingDevice () || playerControllerManager.isPlayerDriving ()) &&
		    !pauseManager.isGamePaused ()) {

			menuOpened = state;

			pauseManager.openOrClosePlayerMenu (menuOpened, friendsMenuContent.transform, useBlurUIPanel);

			friendsMenuContent.SetActive (menuOpened);

			pauseManager.setIngameMenuOpenedState (menuPanelName, menuOpened, true);

			pauseManager.enableOrDisablePlayerMenu (menuOpened, true, false);

			if (playerControllerManager.isPlayerDriving ()) {
				GameObject currentVehicleObject = playerControllerManager.getCurrentVehicle ();

				if (currentVehicleObject != null) {
					vehicleHUDManager currentVehicleHUDManager = currentVehicleObject.GetComponent<vehicleHUDManager> ();

					if (currentVehicleHUDManager != null) {
						currentVehicleHUDManager.IKDrivingManager.setCameraAndWeaponsPauseState (menuOpened);
					}
				}
			}
		}
	}

	public void openOrCLoseFriendMenuFromTouch ()
	{
		openOrCloseFriendMenu (!menuOpened);
	}

	public void closeFriendMenu ()
	{
		openOrCloseFriendMenu (false);
	}

	public void addFriend (GameObject friend)
	{
		if (friend == null) {
			return;
		}

		if (!checkIfContains (friend.transform)) {
			GameObject newFriendListElement = (GameObject)Instantiate (friendListElement, friendListElement.transform.position, 
				                                  Quaternion.identity, friendListElement.transform.parent);
			
			newFriendListElement.name = "New Friend List Element " + (friendsList.Count + 1);

			friendInfo newFriend = newFriendListElement.GetComponent<friendListElement> ().friendListElementInfo;

			healthManagement currentHealthManagement = friend.GetComponent<healthManagement> ();

			if (currentHealthManagement != null) {
				health currentHealth = currentHealthManagement.GetComponent<health> ();

				if (currentHealth != null) {
					newFriend.Name = currentHealth.settings.allyName;
				}
			}

			newFriend.friendTransform = friend.transform;

			newFriendListElement.SetActive (true);

			playerComponentsManager mainPlayerComponentsManager = friend.GetComponent<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				newFriend.friendRemoteEventSystem = mainPlayerComponentsManager.getRemoteEventSystem ();

				newFriend.mainCharacterToReceiveOrders = mainPlayerComponentsManager.getCharacterToReceiveOrders ();
			}

			newFriendListElement.transform.localScale = Vector3.one;

			newFriend.friendListElement = newFriendListElement;

			if (newFriend.mainCharacterToReceiveOrders != null) {
				for (int i = 0; i < newFriend.orderButtonList.Count; i++) {
					Text pressedButtonText = newFriend.orderButtonList [i].GetComponentInChildren<Text> ();

					if (pressedButtonText != null) {
						string currentOrderButtonName = pressedButtonText.text.ToString ();

						if (showDebugPrint) {
							print ("checking if order exists " + currentOrderButtonName);
						}

						if (!newFriend.mainCharacterToReceiveOrders.containsOrderName (currentOrderButtonName)) {
							if (newFriend.orderButtonList [i].gameObject.activeSelf) {
								newFriend.orderButtonList [i].gameObject.SetActive (false);
							}

							if (showDebugPrint) {
								print (currentOrderButtonName + " not located, disabing button");
							}
						}
					}
				}
			}

			friendsList.Add (newFriend);

			setCurrentStateText (friendsList.Count - 1, "Following");

			setFriendListName ();
		}
	}

	public void setFriendListName ()
	{
		for (int i = 0; i < friendsList.Count; i++) {
			friendsList [i].nameText.text = (i + 1) + ".- " + friendsList [i].Name;
		}
	}

	public bool checkIfContains (Transform friend)
	{
		bool itContains = false;

		for (int i = 0; i < friendsList.Count; i++) {
			if (friendsList [i].friendTransform == friend) {
				itContains = true;
			}
		}

		return itContains;
	}

	public void setIndividualOrder (Button pressedButton)
	{
		Text pressedButtonText = pressedButton.GetComponentInChildren<Text> ();

		if (pressedButtonText != null) {
			string orderName = pressedButtonText.text.ToString ();
	
			setOrderByName (orderName, true, pressedButton);
		}
	}

	void setOrderByName (string orderName, bool isIndividualOrder, Button pressedButton)
	{
		if (playerTransform == null) {
			playerTransform = transform;
		}

		int orderIndex = orderInfoList.FindIndex (a => a.Name.Equals (orderName));

		if (orderIndex > -1) {
			orderInfo newOrderInfo = orderInfoList [orderIndex];

			Transform target = playerTransform;

			for (int i = 0; i < friendsList.Count; i++) {

				currentFriendInfo = friendsList [i];

				bool canCheckFriendResult = false;

				if (isIndividualOrder) {
					canCheckFriendResult = currentFriendInfo.orderButtonList.Contains (pressedButton);
				} else {
					canCheckFriendResult = true;
				}

				if (showDebugPrint) {
					print ("order activated result " + orderName + " " + canCheckFriendResult);
				}

				if (canCheckFriendResult) {

					if (newOrderInfo.closeMenuOnOrderSelected) {
						if (menuOpened) {
							closeFriendMenu ();
						}
					}

					string action = newOrderInfo.orderName;

					if (showDebugPrint) {
						print (action);
					}

					if (newOrderInfo.checkCharacterToReceiveOrdersComponent) {
						if (currentFriendInfo.mainCharacterToReceiveOrders != null) {
							currentFriendInfo.mainCharacterToReceiveOrders.activateOrder (newOrderInfo.Name);
						}
					}

					if (newOrderInfo.useCustomOrderBehavior) {
						if (newOrderInfo.sendPlayerOnOrder) {
							newOrderInfo.mainCustomOrderBehavior.activateOrder (currentFriendInfo.friendTransform, playerTransform);
						} else {
							newOrderInfo.mainCustomOrderBehavior.activateOrder (currentFriendInfo.friendTransform);
						}

						if (newOrderInfo.getCustomTargetFromBehavior) {

							target = newOrderInfo.mainCustomOrderBehavior.getCustomTarget (currentFriendInfo.friendTransform, playerTransform);
						}
					}
			
					if (newOrderInfo.useRemoteEvent) {
						for (int j = 0; j < newOrderInfo.remoteEventNameList.Count; j++) {
							currentFriendInfo.friendRemoteEventSystem.callRemoteEvent (newOrderInfo.remoteEventNameList [j]);
						}
					}

					if (target != null && action != "") {
						currentFriendInfo.friendRemoteEventSystem.callRemoteEventWithTransform (action, target);
					}

					if (newOrderInfo.sendPlayerTransformOnEvent) {
						currentFriendInfo.friendRemoteEventSystem.callRemoteEventWithTransform (action, playerTransform);
					}

					if (newOrderInfo.useRemoteEventListToSendPlayer) {
						for (int j = 0; j < newOrderInfo.remoteEventNameListToSendPlayer.Count; j++) {
							currentFriendInfo.friendRemoteEventSystem.callRemoteEvent (newOrderInfo.remoteEventNameListToSendPlayer [j]);
						}
					}

					setCurrentStateText (i, action);

					if (newOrderInfo.useEventOnOrder) {
						newOrderInfo.eventOnOrder.Invoke ();
					}

					if (isIndividualOrder) {
						return;
					}
				}
			}
		}
	}

	public void setGeneralOrderByName (string orderName)
	{
		setOrderByName (orderName, false, null);
	}

	public void setGeneralOrder (Button pressedButton)
	{
		Text pressedButtonText = pressedButton.GetComponentInChildren<Text> ();

		if (pressedButtonText != null) {
			string orderName = pressedButtonText.text.ToString ();

			setOrderByName (orderName, false, pressedButton);
		}
	}

	public void callToFriends ()
	{
		setGeneralOrderByName (callToFriendsOrderName);
	}

	public void findFriendsInRadius (float radius)
	{
		Collider[] colliders = Physics.OverlapSphere (playerTransform.position, radius);

		if (colliders.Length > 0) {
			
			for (int i = 0; i < colliders.Length; i++) {
				findObjectivesSystem currentFindObjectivesSystem = colliders [i].GetComponentInChildren<findObjectivesSystem> ();

				if (currentFindObjectivesSystem != null) {
					currentFindObjectivesSystem.checkTriggerInfo (mainCollider, true);
				}
			}
		}
	}

	public void setCurrentStateText (int index, string state)
	{
		if (friendsList.Count > 0 && index < friendsList.Count) {
			friendsList [index].currentState.text = "State: " + state;
		}
	}

	public void removeFriend (Transform friend)
	{
		for (int i = 0; i < friendsList.Count; i++) {
			if (friendsList [i].friendTransform == friend) {
				Destroy (friendsList [i].friendListElement);

				friendsList.RemoveAt (i);

				return;
			}
		}
	}

	public void removeAllFriends ()
	{
		for (int i = 0; i < friendsList.Count; i++) {
			Destroy (friendsList [i].friendListElement);
		}

		friendsList.Clear ();
	}

	public void setUsedByAIState (bool state)
	{
		usedByAI = state;
	}

	public void inputOpenOrCloseFriendListMenu ()
	{
		if (friendManagerEnabled) {
			if (pauseManager.isOpenOrClosePlayerOpenMenuByNamePaused ()) {
				return;
			}

			openOrCloseFriendMenu (!menuOpened);
		}
	}

	public void setFriendManagerEnabledState (bool state)
	{
		friendManagerEnabled = state;
	}

	public void setFriendManagerEnabledStateFromEditor (bool state)
	{
		setFriendManagerEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Friend List Manager", gameObject);
	}

	[System.Serializable]
	public class friendInfo
	{
		public string Name;
		public Transform friendTransform;

		public Text nameText;
		public Text currentState;

		[Space]

		public List<Button> orderButtonList = new List<Button> ();
	
		[Space]

		public GameObject friendListElement;

		public remoteEventSystem friendRemoteEventSystem;

		public characterToReceiveOrders mainCharacterToReceiveOrders;
	}

	[System.Serializable]
	public class orderInfo
	{
		public string Name;

		public string orderName;

		public bool checkCharacterToReceiveOrdersComponent;

		public bool closeMenuOnOrderSelected;

		[Space]

		public bool useRemoteEvent;

		public List<string> remoteEventNameList = new List<string> ();

		public bool sendPlayerTransformOnEvent;


		public bool useRemoteEventListToSendPlayer;

		public List<string> remoteEventNameListToSendPlayer = new List<string> ();

		[Space]

		public bool useCustomOrderBehavior;

		public bool getCustomTargetFromBehavior;

		public bool sendPlayerOnOrder;

		public customOrderBehavior mainCustomOrderBehavior;

		[Space]

		public bool useEventOnOrder;
		public UnityEvent eventOnOrder;
	}
}