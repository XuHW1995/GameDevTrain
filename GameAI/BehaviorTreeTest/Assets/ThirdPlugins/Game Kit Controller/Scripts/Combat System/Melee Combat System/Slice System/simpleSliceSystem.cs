using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class simpleSliceSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool sliceEnabled = true;

	public GameObject objectToSlice;

	public GameObject alternatePrefab;

	public Material infillMaterial;

	public surfaceToSlice mainSurfaceToSlice;

	public Transform[] severables = new Transform[0];

	[Space]
	[Header ("Slice Ragdoll Settings")]
	[Space]

	public string prefabsPath = "";

	public Material sliceMaterial;

	public bool setTagOnSkeletonRigidbodies = true;

	public string tagOnSkeletonRigidbodies = "box";

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool ignoreUpdateLastObjectSpeed;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool ignoreDestroyOriginalObject;

	public UnityEvent eventsOnIgnoreDestroyOriginalObject;

	[Space]

	public bool setCustomIDOnSliceSpieces;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;
	public bool sliceInitialized;

	const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";

	string randomString = "";


	public string getRandomString ()
	{
		return randomString;
	}

	public void setRandomString (string newValue)
	{
		if (randomString.Equals ("")) {
			randomString = newValue;
		}
	}

	public void setRandomString ()
	{
		if (setCustomIDOnSliceSpieces) {
			if (randomString.Equals ("")) {
				int charAmount = UnityEngine.Random.Range (10, 20); //set those to the minimum and maximum length of your string

				for (int i = 0; i < charAmount; i++) {
					randomString += glyphs [UnityEngine.Random.Range (0, glyphs.Length)];
				}
			}
		}
	}

	public void destroyAllSlicedPiecesByRandomString ()
	{
		if (setCustomIDOnSliceSpieces && randomString != "") {
			sliceSystemUtils.destroyAllSlicedPiecesByRandomString (randomString, gameObject);
		}
	}

	void Awake ()
	{
		if (sliceEnabled) {
			initializeValuesOnHackableComponent ();
		}
	}

	public void initializeValuesOnHackableComponent ()
	{
		if (sliceInitialized) {
			return;
		}

		sliceSystemUtils.initializeValuesOnHackableComponent (gameObject, this);
	
		sliceInitialized = true;
	}

	public void activateSlice (Collider objectCollider, Vector3 newNormalInWorldSpaceValue,
	                           Vector3 positionInWorldSpace, Vector3 slicePosition, bool updateLastObjectSpeed, Vector3 lastSpeed)
	{
		if (!sliceEnabled) {
			return;
		}

		Vector3 point = objectCollider.ClosestPointOnBounds (positionInWorldSpace);

		if (mainSurfaceToSlice.useParticlesOnSlice) {
			Quaternion particlesRotation = Quaternion.LookRotation (newNormalInWorldSpaceValue);

			Instantiate (mainSurfaceToSlice.particlesOnSlicePrefab, slicePosition, particlesRotation);
		}

		sliceSystemUtils.sliceCharacter (objectToSlice, point, newNormalInWorldSpaceValue, updateLastObjectSpeed, lastSpeed);

		if (showDebugPrint) {
			print ("Activate Slice on Character " + objectToSlice.name);
		}
	}

	public void searchBodyParts ()
	{
		if (objectToSlice == null) {
			objectToSlice = gameObject;
		}

		List<GameObject> bodyPartsList = new List<GameObject> ();

		Component[] childrens = objectToSlice.GetComponentsInChildren (typeof(Rigidbody));
		foreach (Rigidbody child in childrens) {
			Collider currentCollider = child.GetComponent<Collider> ();

			if (currentCollider != null && !currentCollider.isTrigger) {

				bodyPartsList.Add (child.gameObject);
			}
		}

		severables = new Transform[bodyPartsList.Count];

		for (int i = 0; i < bodyPartsList.Count; i++) {
			severables [i] = bodyPartsList [i].transform;
		}

		if (!Application.isPlaying) {
			updateComponent ();
		}

	}

	public void setTagOnBodyParts (string newTag)
	{
		for (int i = 0; i < severables.Length; i++) {
			if (severables [i] != null) {
				severables [i].tag = newTag;
			}
		}
	}

	public GameObject getMainAlternatePrefab ()
	{
		return alternatePrefab;
	}

	public Transform[] getSeverables ()
	{
		return severables;
	}

	public void setNewSeverablesList (GameObject newObject)
	{
		if (newObject != null) {
			genericRagdollBuilder currentGenericRagdollBuilder = newObject.GetComponent<genericRagdollBuilder> ();

			if (currentGenericRagdollBuilder != null) {
				int bonesCount = currentGenericRagdollBuilder.bones.Count;

				severables = new Transform[bonesCount];

				for (int i = 0; i < bonesCount; i++) {
					severables [i] = currentGenericRagdollBuilder.bones [i].anchor;
				}
			}
		}
	}

	public void createRagdollPrefab ()
	{
		if (alternatePrefab == null) {
			prefabsPath = pathInfoValues.getSliceObjectsPrefabsPath ();

			alternatePrefab = GKC_Utils.createSliceRagdollPrefab (objectToSlice, prefabsPath, sliceMaterial, setTagOnSkeletonRigidbodies, tagOnSkeletonRigidbodies);
		}
	}

	public void setSliceEnabledState (bool state)
	{
		sliceEnabled = state;

		if (sliceEnabled) {
			initializeValuesOnHackableComponent ();
		}
	}

	public void setSliceEnabledStateFromEditor (bool state)
	{
		sliceEnabled = state;

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Simple Slice System " + gameObject.name, gameObject);
	}
}