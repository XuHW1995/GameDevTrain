﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class sceneLoadSystem : MonoBehaviour
{
	public GameObject mainMenuContent;
	public GameObject sceneLoadMenu;
	public GameObject sceneSlotPrefab;
	public Transform sceneSlotsParent;
	public Scrollbar mainScrollbar;
	public ScrollRect mainScrollRect;
	public List<sceneInfo> sceneInfoList = new List<sceneInfo> ();

	public int startingSceneIndex = 1;
	public bool addSceneNumberToName = true;


	public bool useLoadScreen;

	public int loadScreenScene = 1;

	public bool useLastSceneIndexAsLoadScreen = true;

	public string sceneToLoadAsyncPrefsName = "SceneToLoadAsync";

	public bool checkLoadingScreenSceneConfigured = true;

	public string loadingScreenSceneName = "Loading Screen Scene";


	void Start ()
	{
		for (int i = 0; i < sceneInfoList.Count; i++) {
			if (sceneInfoList [i].addSceneToList) {
				GameObject newSceneSlot = (GameObject)Instantiate (sceneSlotPrefab, Vector3.zero, Quaternion.identity);
				newSceneSlot.name = "Scene Slot " + (i + 1);
				newSceneSlot.transform.SetParent (sceneSlotsParent);
				newSceneSlot.transform.localScale = Vector3.one;
				newSceneSlot.transform.position = newSceneSlot.transform.position;

				sceneSlotInfo newSceneSlotInfo = newSceneSlot.GetComponent<sceneSlot> ().sceneSlotInfo;

				if (addSceneNumberToName) {
					newSceneSlotInfo.sceneNameText.text = (i + 1) + " - " + sceneInfoList [i].Name;
				} else {
					newSceneSlotInfo.sceneNameText.text = sceneInfoList [i].Name;
				}

				newSceneSlotInfo.sceneDescriptionText.text = sceneInfoList [i].sceneDescription;

				newSceneSlotInfo.sceneDescriptionText.fontSize = sceneInfoList [i].fontSize;

				newSceneSlotInfo.sceneImage.texture = sceneInfoList [i].sceneImage;

				sceneInfoList [i].sceneButton = newSceneSlotInfo.sceneButton;
			}
		}

		sceneSlotPrefab.SetActive (false);

		StartCoroutine (SetValue ());
	}

	public void loadScene (Button buttonToCheck)
	{
		mainMenuContent.SetActive (false);

		for (int i = 0; i < sceneInfoList.Count; i++) {
			if (sceneInfoList [i].sceneButton == buttonToCheck) {
				GKC_Utils.loadScene (sceneInfoList [i].sceneNumber, useLoadScreen, loadScreenScene, sceneToLoadAsyncPrefsName, 
					useLastSceneIndexAsLoadScreen, checkLoadingScreenSceneConfigured, loadingScreenSceneName);
			}
		}
	}

	IEnumerator SetValue ()
	{
		yield return new WaitForEndOfFrame ();

		sceneLoadMenu.SetActive (true);

		mainScrollRect.verticalNormalizedPosition = 0f;
		mainScrollRect.horizontalNormalizedPosition = 0.5f;
		mainScrollRect.horizontalNormalizedPosition = 0.5f;
		mainScrollbar.value = 0;

		sceneLoadMenu.SetActive (false);

		yield return null;
	}

	public void setSceneNumberInOrder ()
	{
		for (int i = 0; i < sceneInfoList.Count; i++) {
			sceneInfoList [i].sceneNumber = (i + startingSceneIndex);
		}

		updateComponent ();
	}

	public void setSceneNumberInOrderByIndex (int index)
	{
		int currentIndex = 0;

		for (int i = 0; i < sceneInfoList.Count; i++) {
			if (i >= index) {
				sceneInfoList [i].sceneNumber = (currentIndex + startingSceneIndex);

				currentIndex++;
			}
		}

		updateComponent ();
	}

	public void enableAddSceneToListByIndex (int index)
	{
		for (int i = 0; i < sceneInfoList.Count; i++) {
			if (i >= index) {
				sceneInfoList [i].addSceneToList = true;
			}
		}

		updateComponent ();
	}

	public void disableAddSceneToListByIndex (int index)
	{
		for (int i = 0; i < sceneInfoList.Count; i++) {
			if (i >= index) {
				sceneInfoList [i].addSceneToList = false;
			}
		}

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Scene Load System", gameObject);
	}

	[System.Serializable]
	public class sceneInfo
	{
		public string Name;
		[TextArea (10, 25)] public string sceneDescription;
		public int sceneNumber;
		public Texture sceneImage;
		public Button sceneButton;
		public bool addSceneToList = true;

		public int fontSize = 20;
	}

	[System.Serializable]
	public class sceneSlotInfo
	{
		public Text sceneNameText;
		public Text sceneDescriptionText;
		public RawImage sceneImage;
		public Button sceneButton;
	}
}
