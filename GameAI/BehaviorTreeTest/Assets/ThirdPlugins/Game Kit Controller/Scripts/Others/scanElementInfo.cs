﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class scanElementInfo : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public elementInfo dataObject;
	public GameObject dataGameObject;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool objectScanned;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventFunction;
	public UnityEvent eventFunction = new UnityEvent ();

	//this script is to add info to an object, so the player can scan it and read that info.
	//the script is prepared to make database with all the scannable objects, and it will be completed in the next version


	//save this element in the database
	public void saveElement ()
	{
		if (!objectScanned) {
			//get the number of the database, to add this element with an auto incremental id
			List<elementInfo> list = getList ();

			int index = list.Count;

			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Create (Application.persistentDataPath + "/dataBase.txt"); 

			//increase the id in 1
			dataObject.id = index + 1;

			list.Add (dataObject);

			bf.Serialize (file, list);

			file.Close ();

			//element added
			objectScanned = true;

			//uncomment to see all the database content
			//readDataBase ();
		}
	}

	List<elementInfo> getList ()
	{
		List<elementInfo> list = new List<elementInfo> ();

		if (File.Exists (Application.persistentDataPath + "/dataBase.txt")) {
			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (Application.persistentDataPath + "/dataBase.txt", FileMode.Open);

			list = (List<elementInfo>)bf.Deserialize (file);

			file.Close ();
		}

		return list;
	}

	//just a function to see if the database is correctly saved
	void readDataBase ()
	{
		List<elementInfo> list = new List<elementInfo> ();

		if (File.Exists (Application.persistentDataPath + "/dataBase.txt")) {
			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (Application.persistentDataPath + "/dataBase.txt", FileMode.Open);

			list = (List<elementInfo>)bf.Deserialize (file);

			file.Close ();
		}

		if (showDebugPrint) {
			for (int i = 0; i < list.Count; i++) {
				print (list [i].id + " " + list [i].info);
			}
		}
	}

	//remove this element from the database
	public void removeELement ()
	{
		List<elementInfo> list = new List<elementInfo> ();

		if (File.Exists (Application.persistentDataPath + "/dataBase.txt")) {
			BinaryFormatter bf = new BinaryFormatter ();

			FileStream file = File.Open (Application.persistentDataPath + "/dataBase.txt", FileMode.Open);

			list = (List<elementInfo>)bf.Deserialize (file);

			file.Close ();
		}

		for (int i = 0; i < list.Count; i++) {
			if (list [i].id == dataObject.id) {
				list.RemoveAt (i);

				objectScanned = false;

				dataObject.id = 0;

				saveList (list);

				return;
			}
		}
	}

	//save the list removing the empty element when an object is removed
	void saveList (List<elementInfo> completList)
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/dataBase.txt"); 

		//direction where the txt is saved
		//print (Application.persistentDataPath);
		bf.Serialize (file, completList);

		file.Close ();

		readDataBase ();
	}

	public bool isScanned ()
	{
		return dataObject.read;
	}

	public void scanObject ()
	{
		dataObject.read = true;
			
		if (useEventFunction) {
			if (eventFunction.GetPersistentEventCount () > 0) {
				eventFunction.Invoke ();
			}
		}
	}

	//every object has a name, the info, a bool to check if it is already read and a id for the database
	[System.Serializable]
	public class elementInfo
	{
		public string name;
		[TextArea (3, 10)]
		public string info;
		public bool read;
		public int id;
	}
}