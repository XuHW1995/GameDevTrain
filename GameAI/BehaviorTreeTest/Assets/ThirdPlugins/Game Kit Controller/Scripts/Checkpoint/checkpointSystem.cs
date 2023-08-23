using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class checkpointSystem : MonoBehaviour
{
	public bool checkpointSystemEnabled = true;

	public int checkpointSceneID;
	public GameObject checkPointPrefab;
	public LayerMask layerToPlaceNewCheckpoints;
	public Vector3 checkpointsPositionOffset;
	public Vector3 triggerScale;

	public LayerMask layerToRespawn;

	public List<Transform> checkPointList = new List<Transform> ();

	public deathLoadCheckpoint deathLoackCheckpointType;

	public enum deathLoadCheckpoint
	{
		reloadScene,
		respawn,
		none
	}

	public bool showGizmo;
	public Color gizmoLabelColor = Color.black;
	public float gizmoRadius;
	public bool useHandleForVertex;
	public float handleRadius;
	public Color handleGizmoColor;

	public List<checkpointElement> checkPoinElementtList = new List<checkpointElement> ();
	public checkpointElement currentCheckpointElement;


	RaycastHit hit;


	public void setCurrentCheckpointElement (Transform currentCheckpoint)
	{
		for (int i = 0; i < checkPointList.Count; i++) {
			if (checkPointList [i] == currentCheckpoint) {
				currentCheckpointElement = checkPoinElementtList [i];
			}
		}
	}

	public void respawnPlayer (GameObject currentPlayer)
	{
		Vector3 respawnPosition = currentCheckpointElement.transform.position;
		Quaternion respawnRotation = currentCheckpointElement.transform.rotation;

		if (currentCheckpointElement.useCustomSaveTransform) {
			respawnPosition = currentCheckpointElement.customSaveTransform.position;
			respawnRotation = currentCheckpointElement.customSaveTransform.rotation;
		}

		if (Physics.Raycast (respawnPosition + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, layerToRespawn)) {
			respawnPosition = hit.point;
		}
			
		playerController currentPlayerController = currentPlayer.GetComponent<playerController> ();

		if (currentPlayerController != null) {
			currentPlayerController.changeScriptState (false);

			currentPlayerController.changeScriptState (true);

			currentPlayerController.setPlayerVelocityToZero ();

			currentPlayerController.setLastTimeFalling ();

			GameObject playerCameraGameObject = currentPlayerController.getPlayerCameraGameObject ();

			playerCameraGameObject.transform.position = respawnPosition;
			playerCameraGameObject.transform.rotation = respawnRotation;
		}

		currentPlayer.transform.position = respawnPosition;
		currentPlayer.transform.rotation = respawnRotation;
	}

	public bool thereIsLastCheckpoint ()
	{
		return currentCheckpointElement != null;
	}

	public void disableCheckPoint (int checkpointID)
	{
		for (int i = 0; i < checkPoinElementtList.Count; i++) {
			if (checkPoinElementtList [i].checkpointID == checkpointID) {
				checkPoinElementtList [i].gameObject.SetActive (false);
			} else {
				checkPoinElementtList [i].gameObject.SetActive (true);
			}
		}
	}

	public void setCheckpointSystemEnabledState (bool state)
	{
		checkpointSystemEnabled = state;
	}

	public bool isCheckpointSystemEnabled ()
	{
		return checkpointSystemEnabled;
	}

	public bool checkCheckpointStateOnCharacterGetUp (ragdollActivator currentRagdollActivator, GameObject characterGameObject)
	{
		bool usingCheckPoint = false;

		if (thereIsLastCheckpoint ()) {
			usingCheckPoint = true;

			if (deathLoackCheckpointType == deathLoadCheckpoint.respawn) {
				currentRagdollActivator.quickGetUp ();

				respawnPlayer (characterGameObject);

			} else if (deathLoackCheckpointType == deathLoadCheckpoint.reloadScene) {

				playerComponentsManager currentPlayerControllerManager = characterGameObject.GetComponent<playerComponentsManager> ();

				if (currentPlayerControllerManager != null) {
					saveGameSystem currentSaveGameSystem = currentPlayerControllerManager.getSaveGameSystem ();

					if (currentSaveGameSystem != null) {
						currentSaveGameSystem.reloadLastCheckpoint ();
					}
				}
			} else if (deathLoackCheckpointType == deathLoadCheckpoint.none) {

				currentRagdollActivator.getUp ();
			} 
		} 

		return usingCheckPoint;
	}

	public bool isDeathLoadCheckpointRespawnActivee ()
	{
		return deathLoackCheckpointType == deathLoadCheckpoint.respawn;
	}

	//EDITOR FUNCTIONS
	//add a new checkpoint
	public void addNewCheckpoint ()
	{
		Vector3 newPosition = transform.position;
		if (checkPointList.Count > 0) {
			newPosition = checkPointList [checkPointList.Count - 1].position + checkPointList [checkPointList.Count - 1].forward;
		}
			
		Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

		if (currentCameraEditor != null) {
			Vector3 editorCameraPosition = currentCameraEditor.transform.position;
			Vector3 editorCameraForward = currentCameraEditor.transform.forward;
			RaycastHit hit;
			if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, layerToPlaceNewCheckpoints)) {
				newPosition = hit.point + Vector3.right * checkpointsPositionOffset.x + Vector3.up * checkpointsPositionOffset.y + Vector3.forward * checkpointsPositionOffset.z;
			}
		}

		GameObject newWayPoint = Instantiate (checkPointPrefab);
		newWayPoint.transform.SetParent (transform);
		newWayPoint.transform.position = newPosition;
		newWayPoint.name = "Checkpoint " + (checkPointList.Count + 1);
		newWayPoint.GetComponent<BoxCollider> ().size = triggerScale;
		checkPointList.Add (newWayPoint.transform);

		checkpointElement newCheckpointElement = newWayPoint.GetComponent<checkpointElement> ();
		newCheckpointElement.setCheckPointManager (this);
		newCheckpointElement.checkpointID = checkPointList.Count;
			
		checkPoinElementtList.Add (newCheckpointElement);

		updateComponent ();
	}

	public void removeCheckpoint (int checkpointIndex)
	{
		DestroyImmediate (checkPointList [checkpointIndex].gameObject);

		checkPointList.RemoveAt (checkpointIndex);

		checkPoinElementtList.RemoveAt (checkpointIndex);

		updateComponent ();
	}

	public void removeAllCheckpoints ()
	{
		for (int i = 0; i < checkPointList.Count; i++) {
			if (checkPointList [i].gameObject != null) {
				DestroyImmediate (checkPointList [i].gameObject);
			}
		}

		checkPointList.Clear ();

		checkPoinElementtList.Clear ();

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Checkpoint System Info", gameObject);
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
		if (showGizmo) {
			for (int i = 0; i < checkPointList.Count; i++) {
				if (checkPointList [i] != null) {
					Gizmos.color = Color.yellow;

					Gizmos.DrawSphere (checkPointList [i].position, gizmoRadius);

					if (i + 1 < checkPointList.Count) {
						Gizmos.color = Color.white;

						Gizmos.DrawLine (checkPointList [i].position, checkPointList [i + 1].position);
					}
				}
			}
		}
	}
}
