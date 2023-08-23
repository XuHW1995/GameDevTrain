using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class addMeleeShieldToCharacterEditor : EditorWindow
{
	GUISkin guiSkin;
	Rect windowRect = new Rect ();
	Event currentEvent;

	Vector2 rectSize = new Vector2 (600, 600);

	bool shieldAdded;

	float timeToBuild = 0.2f;
	float timer;

	string prefabsPath = "";

	GUIStyle style = new GUIStyle ();

	float windowHeightPercentage = 0.38f;

	Vector2 screenResolution;

	playerComponentsManager mainPlayerComponentsManager;
	meleeWeaponsGrabbedManager mainMeleeWeaponsGrabbedManager;

	public string[] shieldList;
	public int shieldIndex;
	string newshieldName;

	bool shieldListAssigned;

	public bool useCustomShieldPrefabPath;
	public string customShieldPrefabPath;

	Vector2 scrollPos1;

	float maxLayoutWidht = 220;

	bool closeWindowAfterAddingObjectToCharacter = true;

	[MenuItem ("Game Kit Controller/Add Weapon To Character/Add Melee Shield To Character", false, 204)]
	public static void addMeleeShieldToCharacter ()
	{
		GetWindow<addMeleeShieldToCharacterEditor> ();
	}

	void OnEnable ()
	{
		shieldListAssigned = false;

		newshieldName = "";

		screenResolution = new Vector2 (Screen.currentResolution.width, Screen.currentResolution.height);

		//		Debug.Log (screenResolution + " " + partsHeight + " " + settingsHeight + " " + previewHeight);

		float totalHeight = screenResolution.y * windowHeightPercentage;

		if (totalHeight < 500) {
			totalHeight = 500;
		}

		rectSize = new Vector2 (600, totalHeight);

		prefabsPath = pathInfoValues.getMeleeShieldObjectPath ();

		customShieldPrefabPath = prefabsPath;

		resetCreatorValues ();

		checkCurrentShieldSelected (Selection.activeGameObject);
	}

	void checkCurrentShieldSelected (GameObject currentCharacterSelected)
	{
		if (currentCharacterSelected) {
			mainPlayerComponentsManager = currentCharacterSelected.GetComponentInChildren<playerComponentsManager> ();

			if (mainPlayerComponentsManager != null) {
				mainMeleeWeaponsGrabbedManager = mainPlayerComponentsManager.getMeleeWeaponsGrabbedManager ();

				if (!Directory.Exists (prefabsPath)) {
					Debug.Log ("WARNING: " + prefabsPath + " path doesn't exist, make sure the path is from an existing folder in the project");

					return;
				}

				string[] search_results = null;

				search_results = System.IO.Directory.GetFiles (prefabsPath, "*.prefab");

				if (search_results.Length > 0) {
					shieldList = new string[search_results.Length];
					int currentShieldIndex = 0;

					foreach (string file in search_results) {
						//must convert file path to relative-to-unity path (and watch for '\' character between Win/Mac)
						GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (file, typeof(GameObject)) as GameObject;

						if (currentPrefab) {
							string currentShieldName = currentPrefab.name;
							shieldList [currentShieldIndex] = currentShieldName;
							currentShieldIndex++;
						} else {
							Debug.Log ("WARNING: something went wrong when trying to get the prefab in the path " + file);
						}
					}

					shieldListAssigned = true;
				} else {
					Debug.Log ("Shield prefab not found in path " + prefabsPath);

					shieldList = new string[0];
				}
			}
		}
	}

	void OnDisable ()
	{
		resetCreatorValues ();
	}

	void resetCreatorValues ()
	{
		if (shieldAdded) {

		} else {

		}

		mainMeleeWeaponsGrabbedManager = null;

		shieldAdded = false;

		shieldListAssigned = false;

		mainPlayerComponentsManager = null;

		useCustomShieldPrefabPath = false;

		Debug.Log ("Shield window closed");
	}

	void OnGUI ()
	{
		if (!guiSkin) {
			guiSkin = Resources.Load ("GUI") as GUISkin;
		}
		GUI.skin = guiSkin;

		this.minSize = rectSize;

		this.titleContent = new GUIContent ("Shields", null, "Add Melee Shield To Character");

		GUILayout.BeginVertical ("Add Melee Shield To Character", "window");

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		windowRect = GUILayoutUtility.GetLastRect ();
		//		windowRect.position = new Vector2 (0, windowRect.position.y);
		windowRect.width = this.maxSize.x;

		GUILayout.BeginHorizontal ();

		EditorGUILayout.HelpBox ("", MessageType.Info);

		style = new GUIStyle (EditorStyles.helpBox);
		style.richText = true;

		style.fontStyle = FontStyle.Bold;
		style.fontSize = 17;

		EditorGUILayout.LabelField ("Select a Shield from the 'Melee Shield To Add' list and press the button 'Add Melee Shield To Character'. \n\n" +
		"If not character is selected in the hierarchy, select one and press the button 'Check Current Object Selected'.\n\n", style);
		GUILayout.EndHorizontal ();

		if (mainMeleeWeaponsGrabbedManager == null) {

			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			EditorGUILayout.HelpBox ("", MessageType.Warning);

			style = new GUIStyle (EditorStyles.helpBox);
			style.richText = true;

			style.fontStyle = FontStyle.Bold;
			style.fontSize = 17;
			EditorGUILayout.LabelField ("WARNING: No Character was found, make sure to select the player or an " +
			"humanoid AI to add a Shield to it.", style);

			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Current Object Selected")) {
				checkCurrentShieldSelected (Selection.activeGameObject);
			}
		} else {
			if (shieldListAssigned) {
				if (shieldList.Length > 0) {

					scrollPos1 = EditorGUILayout.BeginScrollView (scrollPos1, false, false);

					GUILayout.FlexibleSpace ();

					EditorGUILayout.Space ();

					if (shieldIndex < shieldList.Length) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Shield To Add", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
						shieldIndex = EditorGUILayout.Popup (shieldIndex, shieldList, GUILayout.ExpandWidth (true));
						GUILayout.EndHorizontal ();

						EditorGUILayout.Space ();

						newshieldName = shieldList [shieldIndex];  
					}

					EditorGUILayout.Space ();

					showCustomPrefabPathOptions ();

					EditorGUILayout.Space ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Close Wizard Once Shield Added", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
					closeWindowAfterAddingObjectToCharacter = (bool)EditorGUILayout.Toggle ("", closeWindowAfterAddingObjectToCharacter);
					GUILayout.EndHorizontal ();

					EditorGUILayout.Space ();

					EditorGUILayout.EndScrollView ();

					EditorGUILayout.Space ();

					if (GUILayout.Button ("Add Melee Shield To Character")) {
						addShield ();
					}

					if (GUILayout.Button ("Cancel")) {
						this.Close ();
					}
				}
			} else {
				GUILayout.FlexibleSpace ();

				EditorGUILayout.Space ();

				GUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox ("", MessageType.Warning);

				style = new GUIStyle (EditorStyles.helpBox);
				style.richText = true;

				style.fontStyle = FontStyle.Bold;
				style.fontSize = 17;
				EditorGUILayout.LabelField ("WARNING: No Shields prefabs where found on the path " + prefabsPath, style);

				GUILayout.EndHorizontal ();

				EditorGUILayout.Space ();

				showCustomPrefabPathOptions ();
			}
		}

		GUILayout.EndVertical ();
	}

	void showCustomPrefabPathOptions ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Use Custom Shield Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
		useCustomShieldPrefabPath = (bool)EditorGUILayout.Toggle ("", useCustomShieldPrefabPath);
		GUILayout.EndHorizontal ();

		EditorGUILayout.Space ();

		if (useCustomShieldPrefabPath) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Custom Shield Prefab Path", EditorStyles.boldLabel, GUILayout.MaxWidth (maxLayoutWidht));
			customShieldPrefabPath = EditorGUILayout.TextField ("", customShieldPrefabPath);
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Check Prefabs On New Path")) {
				prefabsPath = customShieldPrefabPath;

				checkCurrentShieldSelected (Selection.activeGameObject);
			}
		}
	}

	void addShield ()
	{
		string pathForShield = prefabsPath + "/" + newshieldName + ".prefab";

		GameObject currentPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath (pathForShield, typeof(GameObject)) as GameObject;

		if (currentPrefab != null) {
			
			string currentShieldName = currentPrefab.name;

			mainMeleeWeaponsGrabbedManager.addNewMeleeShieldPrefab (currentPrefab, currentShieldName);

			GKC_Utils.updateDirtyScene ("Melee Shield Added To Character", mainMeleeWeaponsGrabbedManager.gameObject);

			Debug.Log ("Shield " + newshieldName + " added to character");

			shieldAdded = true;
		} else {
			Debug.Log ("WARNING: no prefab found on path " + prefabsPath + newshieldName);
		}
	}

	void Update ()
	{
		if (shieldAdded) {
			if (timer < timeToBuild) {
				timer += 0.01f;

				if (timer > timeToBuild) {
					timer = 0;

					if (closeWindowAfterAddingObjectToCharacter) {
						this.Close ();
					} else {
						OnEnable ();
					}
				}
			}
		}
	}
}
#endif