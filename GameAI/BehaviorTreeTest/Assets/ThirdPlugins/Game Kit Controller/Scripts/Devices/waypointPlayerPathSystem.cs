using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine.UI;
using UnityEngine.Events;

public class waypointPlayerPathSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public List<wayPointInfo> wayPoints = new List<wayPointInfo> ();
	public bool inOrder;
	public bool showOneByOne;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool useTimer;
	public float timerSpeed;
	[Range (0, 60)] public float minutesToComplete;
	[Range (0, 60)] public float secondsToComplete;
	public float extraTimePerPoint;
	public int pointsReached;
	public AudioClip pathCompleteAudioSound;
	public AudioElement pathCompleteAudioElement;
	public AudioClip pathUncompleteAudioSound;
	public AudioElement pathUncompleteAudioElement;
	public AudioClip secondTimerSound;
	public AudioElement secondTimerAudioElement;
	public float secondSoundTimerLowerThan;
	public AudioClip pointReachedSound;
	public AudioElement pointReachedAudioElement;
	public bool useLineRenderer;
	public Color lineRendererColor = Color.yellow;
	public float lineRendererWidth;

	public string mainManagerName = "Screen Objectives Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool pathActive;

	[Space]
	[Header ("Event Settings")]
	[Space]

	public UnityEvent eventOnPathComplete = new UnityEvent ();
	public UnityEvent eventOnPathIncomplete = new UnityEvent ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoLabelColor;
	public bool useRegularGizmoRadius;
	public float gizmoRadius;
	public float triggerRadius;
	public bool showOffScreenIcon;
	public bool showMapWindowIcon;
	public bool showDistance;
	public bool showInfoLabel = true;

	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;
	public bool showDoPositionHandles;

	List<GameObject> points = new List<GameObject> ();
	int i;
	int pointsNumber;
	float totalSecondsTimer;
	Text screenTimerText;
	AudioSource audioSource;
	LineRenderer lineRenderer;
	screenObjectivesSystem screenObjectivesManager;
	showGameInfoHud gameInfoHudManager;

	GameObject currentUser;

	playerComponentsManager mainPlayerComponentsManager;

	private void InitializeAudioElements ()
	{
		if (pathCompleteAudioSound != null) {
			pathCompleteAudioElement.clip = pathCompleteAudioSound;
		}

		if (pathUncompleteAudioSound != null) {
			pathUncompleteAudioElement.clip = pathUncompleteAudioSound;
		}

		if (secondTimerSound != null) {
			secondTimerAudioElement.clip = secondTimerSound;
		}

		if (pointReachedSound != null) {
			pointReachedAudioElement.clip = pointReachedSound;
		}
	}

	void Start ()
	{
		InitializeAudioElements ();

		if (inOrder && !showOneByOne && useLineRenderer) {
			lineRenderer = gameObject.AddComponent<LineRenderer> ();

			lineRenderer.material = new Material (Shader.Find ("Sprites/Default")) { color = lineRendererColor };
			lineRenderer.startWidth = lineRendererWidth;
			lineRenderer.endWidth = lineRendererWidth;
			lineRenderer.startColor = lineRendererColor;
			lineRenderer.endColor = lineRendererColor;
			lineRenderer.positionCount = 0;
		}

		if (screenObjectivesManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(screenObjectivesSystem));

			screenObjectivesManager = FindObjectOfType<screenObjectivesSystem> ();
		} 
	}

	void Update ()
	{
		if (pathActive) {
			if (useTimer) {
				totalSecondsTimer -= Time.deltaTime * timerSpeed;

				screenTimerText.text = convertSeconds ();

				if (secondTimerAudioElement != null) {
					if (totalSecondsTimer - 1 <= secondSoundTimerLowerThan && totalSecondsTimer % 1 < 0.1f) {
						playAudioSourceShoot (secondTimerAudioElement);
					}
				}

				if (totalSecondsTimer <= 0) {
					stopPath ();
				}
			}

			if (inOrder && !showOneByOne && useLineRenderer) {
				lineRenderer.startColor = lineRendererColor;
				lineRenderer.endColor = lineRendererColor;
				lineRenderer.positionCount = wayPoints.Count - pointsReached;

				for (i = 0; i < wayPoints.Count; i++) {
					if (!wayPoints [i].reached) {
						lineRenderer.SetPosition (i - pointsReached, wayPoints [i].point.position);
					}
				}
			}
		}
	}

	public string convertSeconds ()
	{
		int minutes = Mathf.FloorToInt (totalSecondsTimer / 60F);
		int seconds = Mathf.FloorToInt (totalSecondsTimer - minutes * 60);
		return string.Format ("{0:00}:{1:00}", minutes, seconds);
	}

	//add a new waypoint
	public void addNewWayPoint ()
	{
		Vector3 newPosition = transform.position;
		if (wayPoints.Count > 0) {
			newPosition = wayPoints [wayPoints.Count - 1].point.position + wayPoints [wayPoints.Count - 1].point.forward * (wayPoints [wayPoints.Count - 1].triggerRadius * 3);
		}

		GameObject newWayPoint = new GameObject ();
		newWayPoint.transform.SetParent (transform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = (wayPoints.Count + 1).ToString ();

		wayPointInfo newWayPointInfo = new wayPointInfo ();
		newWayPointInfo.Name = newWayPoint.name;
		newWayPointInfo.point = newWayPoint.transform;
		newWayPointInfo.triggerRadius = triggerRadius;

		mapObjectInformation newMapObjectInformation = newWayPoint.AddComponent<mapObjectInformation> ();

		newMapObjectInformation.setPathElementInfo (showOffScreenIcon, showMapWindowIcon, showDistance);
		newMapObjectInformation.enabled = false;

		wayPoints.Add (newWayPointInfo);

		updateComponent ();
	}

	public void renamePoints ()
	{
		for (i = 0; i < wayPoints.Count; i++) {
			wayPoints [i].Name = (i + 1).ToString ();
			wayPoints [i].point.name = wayPoints [i].Name;
		}

		updateComponent ();
	}

	public void pointReached (Transform point)
	{
		bool pointReachedCorrectly = false;

		if (showOneByOne) {
			if (wayPoints [pointsReached].point == point) {
				wayPoints [pointsReached].reached = true;
				pointReachedCorrectly = true;
				pointsReached++;

				if (pointsReached < pointsNumber) {
					mapObjectInformation newMapObjectInformation = wayPoints [pointsReached].point.GetComponent<mapObjectInformation> ();

					if (newMapObjectInformation != null) {
						newMapObjectInformation.createMapIconInfo ();
					}
				}
			} else {
				stopPath ();
			}
		} else {
			if (inOrder) {
				if (wayPoints [pointsReached].point == point) {
					wayPoints [pointsReached].reached = true;

					pointReachedCorrectly = true;

					pointsReached++;
				} else {
					stopPath ();
				}
			} else {
				for (i = 0; i < wayPoints.Count; i++) {
					if (wayPoints [i].point == point) {
						wayPoints [i].reached = true;

						pointReachedCorrectly = true;

						pointsReached++;
					}
				}
			}
		}

		if (pointReachedCorrectly) {
			if (pointsReached < pointsNumber && useTimer) {
				totalSecondsTimer += extraTimePerPoint;
			}

			if (pointReachedAudioElement != null) {
				playAudioSourceShoot (pointReachedAudioElement);
			}

			if (pointsReached == pointsNumber) {
				eventOnPathComplete.Invoke ();

				pathActive = false;
				if (useTimer) {
					screenTimerText.gameObject.SetActive (false);
				}
				if (pathCompleteAudioElement != null) {
					playAudioSourceShoot (pathCompleteAudioElement);
				}
			}
		}
	}

	public void setCurrentUser (GameObject user)
	{
		currentUser = user;
	}

	public void resetPath ()
	{
		mainPlayerComponentsManager = currentUser.GetComponent<playerComponentsManager> ();

		audioSource = mainPlayerComponentsManager.getPlayerStatesManager ().getAudioSourceElement ("Timer Audio Source");

		gameInfoHudManager = mainPlayerComponentsManager.getGameInfoHudManager ();

		pointsReached = 0;

		pointsNumber = wayPoints.Count;

		if (useTimer) {
			totalSecondsTimer = secondsToComplete + minutesToComplete * 60;

			if (screenTimerText == null) {
				screenTimerText = gameInfoHudManager.getHudElement ("Timer", "Timer Text").GetComponent<Text> ();
			} 

			if (screenTimerText != null) {
				screenTimerText.gameObject.SetActive (true);
			}
		}

		points.Clear ();

		for (i = 0; i < wayPoints.Count; i++) {
			wayPoints [i].reached = false;
			points.Add (wayPoints [i].point.gameObject);
		}

		if (screenObjectivesManager != null) {
			screenObjectivesManager.removeGameObjectListFromList (points);
		}

		if (showOneByOne) {
			mapObjectInformation newMapObjectInformation = wayPoints [pointsReached].point.GetComponent<mapObjectInformation> ();

			if (newMapObjectInformation != null) {
				newMapObjectInformation.createMapIconInfo ();
			}
		} else {
			for (i = 0; i < wayPoints.Count; i++) {
				mapObjectInformation newMapObjectInformation = wayPoints [i].point.GetComponent<mapObjectInformation> ();

				if (newMapObjectInformation != null) {
					newMapObjectInformation.createMapIconInfo ();
				}
			}
		}

		pathActive = true;

		if (inOrder && !showOneByOne && useLineRenderer) {
			lineRenderer.enabled = true;
		}
	}

	public void stopPath ()
	{
		pathActive = false;

		if (useTimer) {
			screenTimerText.gameObject.SetActive (false);
		}

		if (screenObjectivesManager != null) {
			screenObjectivesManager.removeGameObjectListFromList (points);
		}

		if (pathUncompleteAudioElement != null) {
			playAudioSourceShoot (pathUncompleteAudioElement);
		}

		if (inOrder && !showOneByOne && useLineRenderer) {
			lineRenderer.enabled = false;
		}

		eventOnPathIncomplete.Invoke ();
	}

	public void playAudioSourceShoot (AudioElement clip)
	{
		if (audioSource != null) {
			clip.audioSource = audioSource;
		}

		if (clip != null) {
			AudioPlayer.PlayOneShot (clip, gameObject);
		}
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Waypoint player path system info", gameObject);
	}

	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	void DrawGizmos ()
	{
		if (!Application.isPlaying) {
			if (showGizmo) {
				for (i = 0; i < wayPoints.Count; i++) {
					if (wayPoints [i].point != null) {
						Gizmos.color = Color.yellow;
						Gizmos.DrawSphere (wayPoints [i].point.position, gizmoRadius);

						if (inOrder) {
							if (i + 1 < wayPoints.Count) {
								Gizmos.color = Color.white;
								Gizmos.DrawLine (wayPoints [i].point.position, wayPoints [i + 1].point.position);
							}
						} else {
							Gizmos.color = Color.white;
							Gizmos.DrawLine (wayPoints [i].point.position, transform.position);
						}
					}
				}
			} else {
				for (i = 0; i < wayPoints.Count; i++) {
					if (wayPoints [i].point != null) {
						wayPoints [i].point.GetComponent<mapObjectInformation> ().showGizmo = showGizmo;
					}
				}
			}
		}
	}

	[System.Serializable]
	public class wayPointInfo
	{
		public string Name;
		public Transform point;
		public bool reached;
		public float triggerRadius;
	}
}