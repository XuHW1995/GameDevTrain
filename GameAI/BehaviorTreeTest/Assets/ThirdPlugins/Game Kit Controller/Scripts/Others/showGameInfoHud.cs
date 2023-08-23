using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class showGameInfoHud : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<hudElementInfo> hudElements = new List<hudElementInfo> ();

	public enum elementType
	{
		Text,
		Slider,
		Panel
	}

	hudElementInfo currentHudElementInfo;

	rectTransformInfo currentRectTransformInfo;


	public GameObject getHudElement (string parentName, string elementName)
	{
		int hudElementsCount = hudElements.Count;

		for (int i = 0; i < hudElementsCount; i++) {
			currentHudElementInfo = hudElements [i];

			if (currentHudElementInfo.name.Equals (parentName)) {

				int rectTransformListCount = currentHudElementInfo.rectTransformList.Count;

				for (int j = 0; j < rectTransformListCount; j++) {
					currentRectTransformInfo = currentHudElementInfo.rectTransformList [j];

					if (currentRectTransformInfo.name.Equals (elementName)) {
						return currentRectTransformInfo.rectTransform.gameObject;
					}
				}
			}
		}

		return null;
	}

	public List<GameObject> getHudElements (string parentName, List<string> elementNames)
	{
		List<GameObject> hudElementList = new  List<GameObject> ();

		int hudElementsCount = hudElements.Count;

		for (int i = 0; i < hudElementsCount; i++) {
			currentHudElementInfo = hudElements [i];

			if (currentHudElementInfo.name.Equals (parentName)) {
				int rectTransformListCount = currentHudElementInfo.rectTransformList.Count;

				for (int j = 0; j < rectTransformListCount; j++) {
					currentRectTransformInfo = currentHudElementInfo.rectTransformList [j];

					if (elementNames.Contains (currentRectTransformInfo.name)) {
						hudElementList.Add (currentRectTransformInfo.rectTransform.gameObject);
					}
				}
			}
		}

		return hudElementList;
	}

	public GameObject getHudElementParent (string parentName)
	{
		int hudElementsCount = hudElements.Count;

		for (int i = 0; i < hudElementsCount; i++) {
			currentHudElementInfo = hudElements [i];

			if (currentHudElementInfo.name.Equals (parentName)) {
				if (currentHudElementInfo.spawnPanelPrefab) {
					if (currentHudElementInfo.hudParent == null) {
						currentHudElementInfo.hudParent = (GameObject)Instantiate (currentHudElementInfo.panelPrefabToSpawn);

						currentHudElementInfo.hudParent.name = parentName;

						if (!currentHudElementInfo.hudParent.activeSelf) {
							currentHudElementInfo.hudParent.SetActive (true);
						}

						if (currentHudElementInfo.setPanelMainParent) {
							currentHudElementInfo.hudParent.transform.SetParent (currentHudElementInfo.panelMainParent);
						}
					}
				}

				return currentHudElementInfo.hudParent.gameObject;
			}
		}

		return null;
	}

	[System.Serializable]
	public class hudElementInfo
	{
		public string name;
		public GameObject hudParent;

		[Space]

		public bool spawnPanelPrefab;
		public GameObject panelPrefabToSpawn;
		public bool setPanelMainParent;
		public Transform panelMainParent;

		[Space]

		public List<rectTransformInfo> rectTransformList = new List<rectTransformInfo> ();
	}

	[System.Serializable]
	public class rectTransformInfo
	{
		public string name;
		public RectTransform rectTransform;
		public elementType hudElementyType;
	}
}