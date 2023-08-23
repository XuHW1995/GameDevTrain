using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class mapUISystem : ingameMenuPanel
{
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool componentsAssigned;

	public bool menuOpened;

	public bool mainMapSystemAssigned;


	[Space]
	[Header ("Main Map Elements")]
	[Space]

	public GameObject mapContent;

	public GameObject mapMenu;
	public RectTransform mapWindowTargetPosition;
	public RectTransform mapRender;
	public RectTransform mapWindow;

	public RectTransform playerMapIcon;
	public RectTransform playerIconChild;

	public Image removeMarkButtonImage;
	public Image quickTravelButtonImage;

	public Text mapObjectNameField;
	public Text mapObjectInfoField;

	public Text currentFloorNumberText;
	public Text currentMapZoneText;

	public GameObject mapIndexWindow;
	public GameObject mapIndexWindowContent;
	public Scrollbar mapIndexWindowScroller;

	[Space]
	[Header ("Other Elements")]
	[Space]

	public RectTransform mapWindowMask;

	public GameObject mapCursor;

	public RectTransform mapCursorRectTransform;

	public GameObject currenMapIconPressed;

	public Scrollbar zoomScrollbar;

	public Transform mapCircleTransform;

	[Space]
	[Header ("Compass Elements")]
	[Space]

	public RectTransform compassWindow;
	public RectTransform compassElementsParent;
	public RectTransform north;
	public RectTransform south;
	public RectTransform east;
	public RectTransform west;
	public RectTransform northEast;
	public RectTransform southWest;
	public RectTransform southEast;
	public RectTransform northWest;

	[Space]
	[Header ("Components")]
	[Space]

	public mapSystem mainMapSystem;


	void Start ()
	{
		if (!mainMapSystemAssigned) {
			if (mainMapSystem != null) {
				mainMapSystemAssigned = true;
			}
		}
	}




	public override void initializeMenuPanel ()
	{
		if (mainMapSystem == null) {
			checkMenuComponents ();
		}
	}

	void checkMenuComponents ()
	{
		if (!componentsAssigned) {
			if (pauseManager != null) {
				playerComponentsManager currentPlayerComponentsManager = pauseManager.getPlayerControllerGameObject ().GetComponent<playerComponentsManager> ();

				if (currentPlayerComponentsManager != null) {

					mainMapSystem = currentPlayerComponentsManager.getMapSystem ();

					mainMapSystemAssigned = mainMapSystem != null;
				}
			}

			componentsAssigned = true;
		}
	}

	public override void openOrCloseMenuPanel (bool state)
	{
		base.openOrCloseMenuPanel (state);

		menuOpened = state;

		checkMenuComponents ();

		if (state) {


		} else {
			
		}
		if (mainMapSystemAssigned) {
			mainMapSystem.openOrCloseMap (menuOpened);
		}
	}




	public void setMapContentActiveState (bool state)
	{
		if (mapContent.activeSelf != state) {
			mapContent.SetActive (state);
		}
	}

	public void setMapMenuActiveState (bool state)
	{
		if (mapMenu.activeSelf != state) {
			mapMenu.SetActive (state);
		}
	}

	public Transform getMapMenuTransform ()
	{
		return mapMenu.transform;
	}

	public void removeMapObjectInfo ()
	{
		mapObjectInfoField.text = "";
		mapObjectNameField.text = "";
	}

	public void setMapObjectInfoText (string mapObjectInfoFieldText, string mapObjectNameFieldText)
	{
		mapObjectInfoField.text = mapObjectInfoFieldText;
		mapObjectNameField.text = mapObjectNameFieldText;
	}

	public void setCurrentFloorNumberText (string newText)
	{
		currentFloorNumberText.text = newText;
	}

	public void setCurrentMapZoneText (string newText)
	{
		currentMapZoneText.text = newText;
	}

	public void setMapIndexWindowScrollerValue (int newValue)
	{
		mapIndexWindowScroller.value = newValue;
	}

	public void setZoomScrollbarValue (float newValue)
	{
		zoomScrollbar.value = newValue;
	}

	public void enableOrDisableCompass (bool state)
	{
		if (compassWindow.gameObject.activeSelf != state) {
			compassWindow.gameObject.SetActive (state);
		}
	}

	public RectTransform getCompassElementsParent ()
	{
		return compassElementsParent;
	}

	public void disableMainCompassDirections ()
	{
		if (northEast.gameObject.activeSelf) {
			northEast.gameObject.SetActive (false);
		}

		if (southWest.gameObject.activeSelf) {
			southWest.gameObject.SetActive (false);
		}

		if (southEast.gameObject.activeSelf) {
			southEast.gameObject.SetActive (false);
		}

		if (northWest.gameObject.activeSelf) {
			northWest.gameObject.SetActive (false);
		}
	}

	public void checkCurrentMapIconPressedParent ()
	{
		currenMapIconPressed.transform.SetParent (mapWindow);

		currenMapIconPressedActiveState (false);
	}

	public void currenMapIconPressedActiveState (bool state)
	{
		if (currenMapIconPressed.activeSelf != state) {
			currenMapIconPressed.SetActive (state);
		}
	}

	public void checkCurrentIconPressed (bool state, Transform mapIconTransform)
	{
		if (state) {
			currenMapIconPressed.transform.SetParent (mapIconTransform);
			currenMapIconPressed.transform.localPosition = Vector3.zero;
		}

		currenMapIconPressedActiveState (state);
	}

	public Vector3 getMapCursorRectTransformPosition ()
	{
		return mapCursorRectTransform.position;
	}

	public void setMapCursorActiveState (bool state)
	{
		if (mapCursor != null) {
			if (mapCursor.activeSelf != state) {
				mapCursor.SetActive (state);
			}
		}
	}

	public void setMapCursorAsLastSibling ()
	{
		mapCursor.transform.SetAsLastSibling ();
	}

	public void setMapIndexWindowActiveState (bool state)
	{
		if (mapIndexWindow.activeSelf != state) {
			mapIndexWindow.SetActive (state);
		}
	}

	public void setRemoveMarkButtonImageColor (Color newColor)
	{
		removeMarkButtonImage.color = newColor;
	}

	public void setQuickTravelButtonImageColor (Color newColor)
	{
		quickTravelButtonImage.color = newColor;
	}

	public RectTransform getMapRender ()
	{
		return mapRender;
	}

	public RectTransform getMapWindow ()
	{
		return mapWindow;
	}

	public RectTransform getPlayerMapIcon ()
	{
		return playerMapIcon;
	}

	public RectTransform getPlayerIconChild ()
	{
		return playerIconChild;
	}

	public Image getRemoveMarkButtonImage ()
	{
		return removeMarkButtonImage;
	}

	public Image getQuickTravelButtonImage ()
	{
		return quickTravelButtonImage;
	}

	public void changeMapIndexWindowState ()
	{
		mainMapSystem.changeMapIndexWindowState ();
	}

	public void enableOrDisableAllMapIconType (Slider iconSlider)
	{
		mainMapSystem.enableOrDisableAllMapIconType (iconSlider);
	}

	public void enableOrDisableMapIconType (Slider iconSlider)
	{
		mainMapSystem.enableOrDisableMapIconType (iconSlider);
	}

	public void zoomInEnabled ()
	{
		mainMapSystem.zoomInEnabled ();
	}

	public void zoomInDisabled ()
	{
		mainMapSystem.zoomInDisabled ();
	}

	public void zoomOutEnabled ()
	{
		mainMapSystem.zoomOutEnabled ();
	}

	public void zoomOutDisabled ()
	{
		mainMapSystem.zoomOutDisabled ();
	}

	public void checkNextFloor ()
	{
		mainMapSystem.checkNextFloor ();
	}

	public void checkPrevoiusFloor ()
	{
		mainMapSystem.checkPrevoiusFloor ();
	}

	public void placeMark ()
	{
		mainMapSystem.placeMark ();
	}

	public void removeMark ()
	{
		mainMapSystem.removeMark ();
	}

	public void activateQuickTravel ()
	{
		mainMapSystem.activateQuickTravel ();
	}

	public void set2dOr3ddMapView (bool state)
	{
		mainMapSystem.set2dOr3ddMapView (state);
	}

	public void recenterCameraPosition ()
	{
		mainMapSystem.recenterCameraPosition ();
	}

	public void setUsingScrollbarZoomState (bool state)
	{
		mainMapSystem.setUsingScrollbarZoomState (state);
	}

	public void setZoomByScrollBar (Scrollbar mainZoomScrollbar)
	{
		mainMapSystem.setZoomByScrollBar (mainZoomScrollbar);
	}

	public void checkNextBuilding ()
	{
		mainMapSystem.checkNextBuilding ();
	}

	public void checkPrevoiusBuilding ()
	{
		mainMapSystem.checkPrevoiusBuilding ();
	}
}
