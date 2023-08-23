using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GKCUIManager : MonoBehaviour
{
	public bool managerEnabled = true;

	public List<UIElementInfo> UIElementInfoList = new List<UIElementInfo> ();

	public string objectSearcherName;

	public List<string> objectSearchResultList = new List<string> ();

	public bool searchObjectsActive;


	public void enableAllUIElement ()
	{
		enableOrDisableAllUIElement (true);
	}

	public void disableAllUIElement ()
	{
		enableOrDisableAllUIElement (false);
	}

	public void enableOrDisableAllUIElement (bool state)
	{
		for (int i = 0; i < UIElementInfoList.Count; i++) {
			enableOrDisableUIElement (UIElementInfoList[i].Name, state);
		}
	}

	public void enableUIElement (string objectName)
	{
		enableOrDisableUIElement (objectName, true);
	}

	public void disableUIElement (string objectName)
	{
		enableOrDisableUIElement (objectName, false);
	}

	public void enableOrDisableUIElement (string objectName, bool state)
	{
		if (!managerEnabled) {
			return;
		}

		int elementIndex = UIElementInfoList.FindIndex (a => a.Name.Equals (objectName));

		if (elementIndex > -1) {
			UIElementInfo currentUIElementInfo = UIElementInfoList [elementIndex];

			currentUIElementInfo.elementActive = state;

			if (currentUIElementInfo.UIGameObject != null) {
				if (currentUIElementInfo.UIGameObject.activeSelf != state) {
					currentUIElementInfo.UIGameObject.SetActive (state);
				}
			}

			if (state) {
				currentUIElementInfo.eventToEnableUIGameObject.Invoke ();
			} else {
				currentUIElementInfo.envetToDisableUIGameObject.Invoke ();
			}

			updateComponent ();
		}
	}

	public void clearObjectsSearcResultList ()
	{
		objectSearchResultList.Clear ();

		objectSearcherName = "";

		searchObjectsActive = false;
	}

	public void showObjectsBySearchName ()
	{
		if (objectSearcherName != null && objectSearcherName != "") {
			objectSearchResultList.Clear ();

			searchObjectsActive = true;

			string currentTextToSearch = objectSearcherName;

			if (currentTextToSearch != "") {
				currentTextToSearch = currentTextToSearch.ToLower ();

				int UIElementInfoListCount = UIElementInfoList.Count;

				for (int i = 0; i < UIElementInfoListCount; i++) {
					UIElementInfo currentUIElementInfo = UIElementInfoList [i];

					if (currentUIElementInfo.Name != "") { 
						string objectName = currentUIElementInfo.Name.ToLower ();

						if (objectName.Contains (currentTextToSearch) ||
						    objectName.Equals (currentTextToSearch)) {

							if (!objectSearchResultList.Contains (currentUIElementInfo.Name)) {
								objectSearchResultList.Add (currentUIElementInfo.Name);
							}
						}

						if (currentUIElementInfo.useMoreNameTagsToSearch) {

							int moreNameTagsToSearchListCount = currentUIElementInfo.moreNameTagsToSearchList.Count;

							for (int j = 0; j < moreNameTagsToSearchListCount; j++) {
								string objectTagName = currentUIElementInfo.moreNameTagsToSearchList [j].ToLower ();

								if (objectTagName.Contains (currentTextToSearch) ||
									objectTagName.Equals (currentTextToSearch)) {

									if (!objectSearchResultList.Contains (currentUIElementInfo.Name)) {
										objectSearchResultList.Add (currentUIElementInfo.Name);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void selectObjectByName (string objectName)
	{
		int curretIndex = UIElementInfoList.FindIndex (s => s.Name.Equals (objectName));

		if (curretIndex > -1) {
			selectObjectByIndex (curretIndex);
		}
	}

	public void selectObjectByIndex (int index)
	{
		UIElementInfo currentUIElementInfo = UIElementInfoList [index];
	
		if (currentUIElementInfo.UIGameObject != null) {
			GKC_Utils.setActiveGameObjectInEditor (currentUIElementInfo.UIGameObject);
		}
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update UI Manager", gameObject);
	}

	[System.Serializable]
	public class UIElementInfo
	{
		public string Name;

		public bool useMoreNameTagsToSearch;

		public List<string> moreNameTagsToSearchList = new List<string> ();

		public bool elementActive = true;

		public GameObject UIGameObject;

		public UnityEvent eventToEnableUIGameObject;
		public UnityEvent envetToDisableUIGameObject;
	}
}
