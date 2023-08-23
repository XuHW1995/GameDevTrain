using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Initial Inventory List Data", menuName = "GKC/Create Initial Inventory List Data", order = 51)]
public class initialinventoryListData : ScriptableObject
{
	public string Name;

	public int ID;

	[Space]

	public List<initialInventoryObjectInfo> initialInventoryObjectInfoList = new List<initialInventoryObjectInfo> ();

	[System.Serializable]
	public class initialInventoryObjectInfo
	{
		public string Name;
		public int amount;

		public bool isEquipped;

		public bool addInventoryObject = true;
	}
}
