﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Reflection;
using GameKitController.Audio;

public class accessTerminal : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string code;

	public LayerMask layer;
	public Color unlockedColor;
	public Color lockedColor;

	public bool allowToUseKeyboard;
	public bool useMoveCameraToDevice;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool unlockThisTerminal;
	public bool disableCodeScreenAfterUnlock;

	public float waitDelayAfterUnlock;

	public bool setLockedStateTextEnabled = true;

	public bool allowToToggleLockedState;

	[Space]
	[Header ("Text Settings")]
	[Space]

	public string backgroundPanelName = "terminal";

	public string unlockedStateString = "Unlocked";
	public string lockedStateString = "Locked";

	[Space]
	[Header ("Sound Settings")]
	[Space]

	public AudioClip wrongPassSound;
	public AudioElement wrongPassAudioElement;
	public AudioClip corretPassSound;
	public AudioElement corretPassAudioElement;
	public AudioClip keyPressSound;
	public AudioElement keyPressAudioElement;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public bool useEventOnDeviceUnlocked;
	public UnityEvent eventOnDeviceUnlocked;

	public bool useEventOnDeviceLocked;
	public UnityEvent eventOnDeviceLocked;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool usingDevice;

	public bool unlocked;
	public bool locked;

	[Space]
	[Header ("Components")]
	[Space]

	public RectTransform pointer;
	public GameObject keys;
	public Text currectCode;
	public Text stateText;

	public Image wallPaper;

	public GameObject hackPanel;
	public GameObject hackActiveButton;

	public GameObject codeScreen;
	public GameObject unlockedScreen;

	public AudioSource audioSource;
	public electronicDevice deviceManager;
	public hackTerminal hackTerminalManager;

	List<Image> keysList = new List<Image> ();
	readonly List<RaycastResult> captureRaycastResults = new List<RaycastResult> ();

	int totalKeysPressed = 0;

	int length;
	bool changedColor;
	bool checkPressedButton;
	bool touchPlatform;
	RaycastHit hit;
	GameObject currentCaptured;
	Touch currentTouch;

	GameObject currentPlayer;
	Camera mainCamera;
	gameManager mainGameManager;

	bool hackingTerminal;

	bool keysInitialized;

	Coroutine mainCoroutine;


	private void InitializeAudioElements ()
	{
		if (audioSource == null) {
			audioSource = GetComponent<AudioSource> ();
		}

		if (audioSource != null) {
			wrongPassAudioElement.audioSource = audioSource;
			corretPassAudioElement.audioSource = audioSource;
			keyPressAudioElement.audioSource = audioSource;
		}

		if (wrongPassSound != null) {
			wrongPassAudioElement.clip = wrongPassSound;
		}

		if (corretPassSound != null) {
			corretPassAudioElement.clip = corretPassSound;
		}
	
		if (keyPressSound != null) {
			keyPressAudioElement.clip = keyPressSound;
		}
	}

	void initializeKeysElements ()
	{
		if (!keysInitialized) {
			Component[] components = keys.GetComponentsInChildren (typeof(Image));

			foreach (Image child in components) {
				int n;

				if (int.TryParse (child.name, out n)) {
					keysList.Add (child);
				}
			}

			//set the current code to 0 according to the length of the real code
			currectCode.text = "";

			length = code.Length;

			for (int i = 0; i < length; i++) {
				currectCode.text += "0";
			}

			keysInitialized = true;
		}
	}

	void Start ()
	{
		//get all the keys inside the keys gameobject, checking the name of every object, comparing if it is a number from 0 to 9
		touchPlatform = touchJoystick.checkTouchPlatform ();

		if (hackPanel != null) {
			if (hackTerminalManager == null) {
				hackTerminalManager = hackPanel.GetComponent<hackTerminal> ();
			}

			//hackActiveButton.SetActive (true);
			if (useMoveCameraToDevice) {
				hackTerminalManager.hasMoveCameraToDevice ();
			}
		}

		InitializeAudioElements ();

		if (deviceManager == null) {
			deviceManager = GetComponent<electronicDevice> ();
		}

		if (mainGameManager == null) {
			mainGameManager = FindObjectOfType<gameManager> ();
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (mainCoroutine != null) {
			StopCoroutine (mainCoroutine);
		}
	}

	IEnumerator updateCoroutine ()
	{
//		var waitTime = new WaitForFixedUpdate ();

		var waitTime = new WaitForSecondsRealtime (0.00001f);

		while (true) {
			updateState ();

			yield return waitTime;
		}
	}

	void updateState ()
	{
		//if the terminal still locked, and the player is using it
		if (usingDevice) {
			bool checkInputStateResult = false;

			if (!unlocked) {
				checkInputStateResult = true;
			}

			if (allowToToggleLockedState && !locked) {
				checkInputStateResult = true;
			}

			if (checkInputStateResult) {
				if (!mainGameManager.isGamePaused ()) {
					//use the center of the camera as mouse, checking also the touch input
					int touchCount = Input.touchCount;
					if (!touchPlatform) {
						touchCount++;
					}

					for (int i = 0; i < touchCount; i++) {
						if (!touchPlatform) {
							currentTouch = touchJoystick.convertMouseIntoFinger ();
						} else {
							currentTouch = Input.GetTouch (i);
						}

						//get a list with all the objects under the center of the screen of the finger tap
						captureRaycastResults.Clear ();
						PointerEventData p = new PointerEventData (EventSystem.current);
						p.position = currentTouch.position;
						p.clickCount = i;
						p.dragging = false;
						EventSystem.current.RaycastAll (p, captureRaycastResults);

						foreach (RaycastResult r in captureRaycastResults) {
							currentCaptured = r.gameObject;
							//if the center of the camera is looking at the screen, move the cursor image inside it
							if (currentCaptured.name.Equals (backgroundPanelName)) {
								if (Physics.Raycast (mainCamera.ScreenPointToRay (currentTouch.position), out hit, Mathf.Infinity, layer)) {
									if (!hit.collider.isTrigger) {
										pointer.gameObject.transform.position = hit.point;
										Debug.DrawLine (mainCamera.transform.position, hit.point, Color.green, 2);
									}
								}
							}
							//check the current number key pressed with the finger
							if (currentTouch.phase == TouchPhase.Began) {
								checkButton (currentCaptured);
							}

							//check the current number key preesed with the interaction button in the keyboard
							if (checkPressedButton) {
								checkButton (currentCaptured);
							}
						}
					
						if (checkPressedButton) {
							checkPressedButton = false;
						}
					}

					if (allowToUseKeyboard) {
						for (int i = 0; i < 10; i++) {
							if (Input.GetKeyDown ("" + i)) {
								checkNumber (i.ToString ());
							}
						}
					}
				}
			}
		}

		//if the device is unlocked, change the color of the interface for the unlocked color
		if (unlocked) {
			if (!changedColor) {
				setPanelColor (unlockedColor);
			}
		}

		if (locked) {
			if (!changedColor) {
				setPanelColor (lockedColor);
			}
		}
	}

	void setPanelColor (Color newColor)
	{
		if (!changedColor) {
			wallPaper.color = Vector4.MoveTowards (wallPaper.color, newColor, Time.deltaTime * 3);
			stateText.color = Vector4.MoveTowards (stateText.color, newColor, Time.deltaTime * 3);

			if (wallPaper.color == newColor && stateText.color == newColor) {
				changedColor = true;
			}
		}
	}

	public void pressButtonOnScreen ()
	{
		checkPressedButton = true;
	}

	//this function is called when the interaction button in the keyboard is pressed, so in pc, the code is written by aiming the center of the camera to
	//every number and pressing the interaction button. In touch devices, the code is written by tapping with the finger every key number directly
	public void activateTerminal ()
	{
		usingDevice = !usingDevice;

		stopUpdateCoroutine ();

		totalKeysPressed = 0;

		initializeKeysElements ();

		if (usingDevice) {
			getCurrentUser ();

			mainCoroutine = StartCoroutine (updateCoroutine ());
		} else {
			if (hackingTerminal) {
				hackTerminalManager.stopHacking ();

				hackingTerminal = false;
			}
		}
	}

	public void enableTerminal ()
	{
		usingDevice = true;

		mainCoroutine = StartCoroutine (updateCoroutine ());

		initializeKeysElements ();

		totalKeysPressed = 0;
	
		getCurrentUser ();
	}

	public void disableTerminal ()
	{
		usingDevice = false;

		stopUpdateCoroutine ();

		if (hackingTerminal) {
			hackTerminalManager.stopHacking ();

			hackingTerminal = false;
		}
	}

	public void getCurrentUser ()
	{
		currentPlayer = deviceManager.getCurrentPlayer ();

		if (currentPlayer != null) {
			mainCamera = currentPlayer.GetComponent<playerController> ().getPlayerCameraManager ().getMainCamera ();
		}
	}

	//the currentCaptured is checked, to write the value of the number key in the screen device
	void checkButton (GameObject button)
	{
		Image currentImage = button.GetComponent<Image> ();

		if (currentImage != null) {
			//check if the currentCaptured is a key number
			if (keysList.Contains (currentImage)) {
				checkNumber (currentImage.name);
			} else {
				if (hackActiveButton) {
					if (button == hackActiveButton) {
						if (hackTerminalManager != null) {
							hackTerminalManager.activeHack ();

							hackingTerminal = true;
						}
					}
				}
			}
		}
	}

	public void checkNumber (string numberString)
	{
		//reset the code in the screen
		if (totalKeysPressed == 0) {
			currectCode.text = "";
		}	

		//add the current key number pressed to the code
		currectCode.text += numberString;

		totalKeysPressed++;

		//play the key press sound
		AudioPlayer.PlayOneShot (keyPressAudioElement, gameObject);

		//if the player has pressed the an amount of key numbers equal to the lenght of the code, check if it is correct
		if (totalKeysPressed == length) {
			//if it is equal, then call the object to unlock, play the corret pass sound, and disable this terminal
			if (currectCode.text == code) {
				if (allowToToggleLockedState) {
					if (unlocked) {
						setLockedState (false);
					} else {
						setLockedState (true);
					}
				} else {
					setLockedState (true);
				}
			}
			//else, reset the terminal, and try again
			else {
				AudioPlayer.PlayOneShot (wrongPassAudioElement, gameObject);

				totalKeysPressed = 0;
			}
		}
	}

	public void setLockedState (bool state)
	{
		if (state) {
			AudioPlayer.PlayOneShot (corretPassAudioElement, gameObject);

			if (setLockedStateTextEnabled) {
				stateText.text = unlockedStateString;
			}

			changedColor = false;

			unlocked = true;

			locked = false;

			if (!unlockThisTerminal) {
				deviceManager.unlockObject ();

				StartCoroutine (waitingAfterUnlock ());
			}

			if (hackPanel != null) {
				hackTerminalManager.moveHackTerminal (false);

				hackingTerminal = false;
			}

			if (disableCodeScreenAfterUnlock) {
				codeScreen.SetActive (false);

				unlockedScreen.SetActive (true);
			}

			checkEventOnLockedStateChange (true);
		} else {
			AudioPlayer.PlayOneShot (corretPassAudioElement, gameObject);

			if (setLockedStateTextEnabled) {
				stateText.text = lockedStateString;
			}

			changedColor = false;

			unlocked = false;

			locked = true;

			checkEventOnLockedStateChange (false);
		}
	}

	void checkEventOnLockedStateChange (bool state)
	{
		if (state) {
			if (useEventOnDeviceUnlocked) {
				eventOnDeviceUnlocked.Invoke ();
			}

		} else {
			if (useEventOnDeviceLocked) {
				eventOnDeviceLocked.Invoke ();
			}
		}
	}

	public void setLockState (Scrollbar info)
	{
		if (info.value == 0) {
			deviceManager.lockObject ();

			if (info.value > 0) {
				info.value = 0;
			}
		} else if (info.value == 1) {
			deviceManager.unlockObject ();

			if (info.value < 1) {
				info.value = 1;
			}
		}
	}

	IEnumerator waitingAfterUnlock ()
	{
		yield return new WaitForSeconds (waitDelayAfterUnlock);

		deviceManager.stopUsindDevice ();

		if (usingDevice) {
			activateTerminal ();
		}

		yield return null;
	}

	public void showPasswordOnHackPanel ()
	{
		hackTerminalManager.setTextContent (code);
	}

	public void unlockAccessTerminal ()
	{
		currectCode.text = "";

		totalKeysPressed = code.Length - 1;

		checkNumber (code);
	}

	public void activateHackButton ()
	{
		if (hackActiveButton != null) {
			hackActiveButton.SetActive (true);
		}
	}

	public void setNewCode (string newCode)
	{
		code = newCode;

		currectCode.text = "";

		length = code.Length;

		for (int i = 0; i < length; i++) {
			currectCode.text += "0";
		}
	}
}