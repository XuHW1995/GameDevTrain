using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class characterToReceiveOrders : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool receiveOrdersEnabled = true;

	[Space]
	[Header ("Order Info Settings")]
	[Space]

	public List<characterOrderInfo> characterOrderInfoList = new List<characterOrderInfo> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;


	public bool containsOrderName (string orderName)
	{
		int orderIndex = characterOrderInfoList.FindIndex (a => a.Name.Equals (orderName));

		if (orderIndex > -1) {
			if (!characterOrderInfoList [orderIndex].orderEnabled) {
				return false;
			}

			return true;
		}

		return false;
	}

	public void activateOrder (string orderName)
	{
		if (showDebugPrint) {
			print ("order to Check " + orderName);
		}

		int orderIndex = characterOrderInfoList.FindIndex (a => a.Name.Equals (orderName));

		if (orderIndex > -1) {
			characterOrderInfo newCharacterOrderInfo = characterOrderInfoList [orderIndex];

			if (!newCharacterOrderInfo.orderEnabled) {
				return;
			}

			if (showDebugPrint) {
				print ("order activated " + orderName);
			}

			if (newCharacterOrderInfo.useEventOnOrderReceived) {
				newCharacterOrderInfo.eventOnOrderReceived.Invoke ();
			}
		}
	}

	[System.Serializable]
	public class characterOrderInfo
	{
		public string Name;

		public bool orderEnabled = true;

		[Space]

		public bool useEventOnOrderReceived;

		public UnityEvent eventOnOrderReceived;
	}
}
