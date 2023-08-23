using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sliceObject : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool sliceActive;

	public bool disableSliceAfterFirstCutEnabled = true;

	public Material defaultSliceMaterial;

	public Vector3 cutOverlapBoxSize = new Vector3 (5, 0.1f, 5);

	public float minDelayToSliceSameObject = 0.3f;

	public bool activateRigidbodiesOnNewObjects = true;

	[Space]
	[Header ("Damage Settings")]
	[Space]

	public bool activateDamageOnSlice;
	public float damageAmountToApplyOnSlice;
	public bool ignoreShieldOnDamage = true;
	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public int damageTypeID = -1;

	[Space]
	[Header ("Physics Settings")]
	[Space]

	public bool updateLastObjectSpeed;

	public float forceToApplyToCutPart;
	public ForceMode forceMode;
	public float forceRadius;
	public float forceUp;

	public float disableTimeAfterCollision;

	public LayerMask targetToDamageLayer;

	[Space]
	[Space]

	public bool applyForcesOnObjectsDetected;
	public float addForceMultiplier;
	public bool applyImpactForceToVehicles;
	public float impactForceToVehiclesMultiplier;

	public bool checkObjectLayerAndTagToApplyForces;

	public LayerMask targetToApplyForceLayer;
	public List<string> tagetToApplyForceTagList = new List<string> ();

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool cutMultipleTimesActive = true;

	public bool ignoreObjectsAlreadySlice;

	public float minWaitTimeToActivateSlice = 0.4f;

	[Space]
	[Header ("Bullet Time Settings")]
	[Space]

	public bool ignoreTimeBulletOnRegularSlice;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public List<GameObject> objectsDetected = new List<GameObject> ();

	public List<Collider> collidersToIgnoreList = new List<Collider> ();

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;
	public Color gizmoColor = Color.red;

	[Space]
	[Header ("Components")]
	[Space]

	public Collider mainCollider;

	public Collider triggerCollider;

	public Transform cutPositionTransform;
	public Transform cutDirectionTransform;

	public Transform planeDefiner1;
	public Transform planeDefiner2;
	public Transform planeDefiner3;


	surfaceToSlice currentSurfaceToSlice;

	Collider currentColliderFound;

	Coroutine disableSliceCoroutine;

	float lastTimeSlice;

	Vector3 currentCutPosition;
	Quaternion currentCutRotation;
	Vector3 currentCutUp;
	Vector3 currentCutForward;

	Vector3 currentCutDirection;

	Vector3 currentPlaneDefiner1;
	Vector3 currentPlaneDefiner2;
	Vector3 currentPlaneDefiner3;

	Vector3 currentCutOverlapBoxSize;


	void setCurrentCutTransformValues ()
	{
		if (cutPositionTransform != null) {
			currentCutPosition = cutPositionTransform.position;
			currentCutRotation = cutPositionTransform.rotation;
			currentCutUp = cutPositionTransform.up;
			currentCutForward = cutPositionTransform.forward;
		} else {
			currentCutPosition = cutDirectionTransform.position;
			currentCutRotation = cutDirectionTransform.rotation;
			currentCutUp = cutDirectionTransform.up;
			currentCutForward = cutDirectionTransform.forward;
		}

		if (cutDirectionTransform != null) {
			currentCutDirection = cutDirectionTransform.right;
		}

		if (planeDefiner1 != null) {
			currentPlaneDefiner1 = planeDefiner1.position;
		}

		if (planeDefiner2 != null) {
			currentPlaneDefiner2 = planeDefiner2.position;
		}

		if (planeDefiner3 != null) {
			currentPlaneDefiner3 = planeDefiner3.position;
		}

		currentCutOverlapBoxSize = cutOverlapBoxSize;
	}

	public void processObject (GameObject obj, Collider objectCollider, Vector3 slicePosition)
	{
		if (cutMultipleTimesActive) {
			if (!ignoreObjectsAlreadySlice && objectsDetected.Contains (obj)) {
				return;
			}
		}

		if (minWaitTimeToActivateSlice > 0 && lastTimeSlice != 0) {
			if (Time.time < lastTimeSlice + minWaitTimeToActivateSlice) {
				return;
			}
		}

		currentSurfaceToSlice = obj.GetComponent<surfaceToSlice> ();

		if (currentSurfaceToSlice == null) {
			currentSurfaceToSlice = sliceSystemUtils.getSurfaceToSlice (obj);
		}

		bool objectIsSliceSurfaceDisabled = false;

		bool objectCanBeSliced = false;

		bool ignoreRegularDamageIfCutSurfaceNotEnabled = false;

		if (showDebugPrint) {
			print ("processing object " + obj.name);
		}

		if (currentSurfaceToSlice != null) {
			bool isCutSurfaceEnabled = currentSurfaceToSlice.isCutSurfaceEnabled ();

			if (showDebugPrint) {
				print ("Surface cut enabled state " + isCutSurfaceEnabled +
				" can activate slice " + currentSurfaceToSlice.sliceCanBeActivated (minDelayToSliceSameObject));
			}

			if (isCutSurfaceEnabled && currentSurfaceToSlice.sliceCanBeActivated (minDelayToSliceSameObject)) {

				Material crossSectionMaterial = currentSurfaceToSlice.getCrossSectionMaterial ();

				if (crossSectionMaterial == null) {
					crossSectionMaterial = defaultSliceMaterial;
				}

				sliceCurrentObject (obj, objectCollider, crossSectionMaterial, slicePosition);

				objectCanBeSliced = true;
			}

			if (!isCutSurfaceEnabled) {
				objectIsSliceSurfaceDisabled = true;

				ignoreRegularDamageIfCutSurfaceNotEnabled = currentSurfaceToSlice.isIgnoreRegularDamageIfCutSurfaceNotEnabled ();
			}
		} else {
			if (showDebugPrint) {
				print ("Surface to slice not found on " + obj.name);
			}
		}

		if (!objectCanBeSliced && !ignoreRegularDamageIfCutSurfaceNotEnabled) {
			if (activateDamageOnSlice) {
				Vector3 damagePosition = currentCutPosition;

				if (applyDamage.checkIfDead (obj)) {
					damagePosition = obj.transform.position;
				}

				applyDamage.checkCanBeDamaged (gameObject, obj, damageAmountToApplyOnSlice, -currentCutForward, damagePosition, 
					gameObject, true, true, ignoreShieldOnDamage, false, true, 
					canActivateReactionSystemTemporally, damageReactionID, damageTypeID);
			}

			if (applyForcesOnObjectsDetected && !objectIsSliceSurfaceDisabled) {
				if (!checkObjectLayerAndTagToApplyForces ||
				    ((1 << obj.layer & targetToApplyForceLayer.value) == 1 << obj.layer && tagetToApplyForceTagList.Contains (obj.tag))) { 
					checkForceToApplyOnObject (obj);
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (sliceActive) {
			currentColliderFound = col;

			checkObjectDetected (currentColliderFound);

			if (disableSliceAfterFirstCutEnabled) {
				disableSliceActiveWithDelay ();
			}
		}
	}

	public void setSliceActiveState (bool state)
	{
		sliceActive = state;

		stopDisableSliceActiveWithDelay ();

		if (sliceActive) {
			if (mainCollider != null) {
				mainCollider.enabled = false;
			}

			if (triggerCollider != null) {
				triggerCollider.enabled = true;
			}
		} else {
			disableIgnoreCollisionList ();
		}
	}

	public void checkObjectDetected (Collider col)
	{
		if ((1 << col.gameObject.layer & targetToDamageLayer.value) == 1 << col.gameObject.layer) {

			setCurrentCutTransformValues ();

			Collider currentCollider = col.GetComponent<Collider> ();

			if (currentCollider != null) {
				processObject (currentCollider.gameObject, currentCollider, currentCutPosition);
			}
		}
	}

	public void sliceCurrentObject (GameObject obj, Collider objectCollider, Material crossSectionMaterial, Vector3 slicePosition)
	{
		// slice the provided object using the transforms of this object
		if (currentSurfaceToSlice.isObjectCharacter ()) {

			bool isCharacterOrVehicle = applyDamage.getCharacterOrVehicle (obj) != null;

			bool objectIsDead = applyDamage.checkIfDead (obj);

			if (isCharacterOrVehicle && !objectIsDead) {
				processCharacter (obj);

				return;
			}

			if (showDebugPrint) {
				print ("SLICING " + obj.name + " is character " + isCharacterOrVehicle + " is dead" + objectIsDead);
			}

			Rigidbody mainObject = obj.GetComponent<Rigidbody> ();

			bool mainObjectHasRigidbody = mainObject != null;

			Vector3 lastSpeed = Vector3.zero;

			if (mainObjectHasRigidbody) {
				lastSpeed = mainObject.velocity;
			}

			currentSurfaceToSlice.getMainSimpleSliceSystem ().activateSlice (objectCollider, 
				positionInWorldSpace, normalInWorldSpace, slicePosition, updateLastObjectSpeed, lastSpeed);

			currentSurfaceToSlice.checkEventOnCut ();

			bool canCheckTimeBulletResult = true;

			if (ignoreTimeBulletOnRegularSlice) {
				canCheckTimeBulletResult = false;
			}

			if (canCheckTimeBulletResult) {
				currentSurfaceToSlice.checkTimeBulletOnCut ();
			}

			lastTimeSlice = Time.time;
		} else {
			bool objectSliced = false;

			GameObject object1 = null;
			GameObject object2 = null;

			// slice the provided object using the transforms of this object
			sliceSystemUtils.sliceObject (transform.position, obj, currentCutUp, crossSectionMaterial, ref objectSliced, ref object1, ref object2);

			Vector3 objectPosition = obj.transform.position;
			Quaternion objectRotation = obj.transform.rotation;

			Transform objectParent = obj.transform.parent;

			if (objectSliced) {
				currentSurfaceToSlice.checkEventOnCut ();

				currentSurfaceToSlice.checkTimeBulletOnCut ();

				Rigidbody mainObject = obj.GetComponent<Rigidbody> ();

				bool mainObjectHasRigidbody = mainObject != null;

				object1.transform.position = objectPosition;
				object1.transform.rotation = objectRotation;

				object2.transform.position = objectPosition;
				object2.transform.rotation = objectRotation;

				if (objectParent != null) {
					object1.transform.SetParent (objectParent);
					object2.transform.SetParent (objectParent);
				}
				
				surfaceToSlice newSurfaceToSlice1 = object1.AddComponent<surfaceToSlice> ();
				surfaceToSlice newSurfaceToSlice2 = object2.AddComponent<surfaceToSlice> ();

				currentSurfaceToSlice.copySurfaceInfo (newSurfaceToSlice1);
				currentSurfaceToSlice.copySurfaceInfo (newSurfaceToSlice2);


				float distance1 = GKC_Utils.distance (obj.transform.position, object1.transform.position);
				float distance2 = GKC_Utils.distance (obj.transform.position, object2.transform.position);

				float currentForceToApply = forceToApplyToCutPart;

				Vector3 lastSpeed = Vector3.zero;

				if (mainObjectHasRigidbody) {
					lastSpeed = mainObject.velocity;
				}

				if (mainObjectHasRigidbody || activateRigidbodiesOnNewObjects) {

					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

					Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

					if (updateLastObjectSpeed) {
						if (lastSpeed != Vector3.zero) {
							object1Rigidbody.velocity = lastSpeed;

							object2Rigidbody.velocity = lastSpeed;
						}
					}

					object2Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);

					object1Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);
				} else {
					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					if (distance1 < distance2) {
						Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

						object2Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);
					} else {
						Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

						object1Rigidbody.AddExplosionForce (currentForceToApply, transform.position, forceRadius, forceUp, forceMode);
					}
				}

				if (currentSurfaceToSlice.useBoxCollider) {
					object1.AddComponent<BoxCollider> ();
					object2.AddComponent<BoxCollider> ();
				} else {
					MeshCollider object1Collider = object1.AddComponent<MeshCollider> ();
					MeshCollider object2Collider = object2.AddComponent<MeshCollider> ();

					object1Collider.convex = true;
					object2Collider.convex = true;
				}

				Collider collider1 = object1.GetComponent<Collider> ();
				Collider collider2 = object2.GetComponent<Collider> ();


				if (collider1 != null) {
					collidersToIgnoreList.Add (collider1);

					if (showDebugPrint) {
						print (collider1.name);
					}
				}

				if (collider2 != null) {
					collidersToIgnoreList.Add (collider2);

					if (showDebugPrint) {
						print (collider2.name);
					}
				}

				if (currentSurfaceToSlice.setNewLayerOnCut) {
					object1.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
					object2.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
				}

				if (currentSurfaceToSlice.setNewTagOnCut) {
					object1.tag = currentSurfaceToSlice.tag;
					object2.tag = currentSurfaceToSlice.tag;
				}

				if (cutMultipleTimesActive) {
					if (!objectsDetected.Contains (object1)) {
						objectsDetected.Add (object1);
					}

					if (!objectsDetected.Contains (object2)) {
						objectsDetected.Add (object2);
					}
				}

				if (obj != null) {
					Destroy (obj);
				}

//				obj.SetActive (false);

				lastTimeSlice = Time.time;
			}
		}
	}

	public void stopDisableSliceActiveWithDelay ()
	{
		if (disableSliceCoroutine != null) {
			StopCoroutine (disableSliceCoroutine);
		}
	}

	public void disableSliceActiveWithDelay ()
	{
		stopDisableSliceActiveWithDelay ();

		disableSliceCoroutine = StartCoroutine (disableSliceActiveWithDelayCoroutine ());
	}

	IEnumerator disableSliceActiveWithDelayCoroutine ()
	{
		yield return new WaitForSeconds (0.5f);

		yield return new WaitForSeconds (disableTimeAfterCollision);

		if (mainCollider != null) {
			mainCollider.enabled = true;
		}

		sliceActive = false;

		disableIgnoreCollisionList ();
	}

	public void disableIgnoreCollisionList ()
	{
		collidersToIgnoreList.Clear ();

		if (triggerCollider != null) {
			triggerCollider.enabled = false;
		}

		objectsDetected.Clear ();
	}

	public void checkForceToApplyOnObject (GameObject objectToDamage)
	{
		Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

		Vector3 forceDirection = currentCutDirection;

		float forceAmount = addForceMultiplier;

		float forceToVehiclesMultiplier = impactForceToVehiclesMultiplier;

		if (applyImpactForceToVehicles) {
			Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

			if (objectToDamageMainRigidbody) {
				Vector3 force = forceDirection * forceAmount;

				bool isVehicle = applyDamage.isVehicle (objectToDamage);
				if (isVehicle) {
					force *= forceToVehiclesMultiplier;
				}

				objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, forceMode);
			}
		} else {
			if (applyDamage.canApplyForce (objectToDamage)) {
				if (showDebugPrint) {
					print (objectToDamage.name);
				}

				Vector3 force = forceDirection * forceAmount;

				if (objectToDamageRigidbody == null) {
					objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();
				}

				objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, forceMode);
			}
		}
	}

	private Vector3 positionInWorldSpace {
		get {
			return (currentPlaneDefiner1 + currentPlaneDefiner2 + currentPlaneDefiner3) / 3f;

		}
	}

	private Vector3 normalInWorldSpace {
		get {
			Vector3 t0 = currentPlaneDefiner1;
			Vector3 t1 = currentPlaneDefiner2;
			Vector3 t2 = currentPlaneDefiner3;

			Vector3 vectorValue;

			vectorValue.x = t0.y * (t1.z - t2.z) + t1.y * (t2.z - t0.z) + t2.y * (t0.z - t1.z);
			vectorValue.y = t0.z * (t1.x - t2.x) + t1.z * (t2.x - t0.x) + t2.z * (t0.x - t1.x);
			vectorValue.z = t0.x * (t1.y - t2.y) + t1.x * (t2.y - t0.y) + t2.x * (t0.y - t1.y);

			return vectorValue;
		}
	}

	void processCharacter (GameObject currentCharacter)
	{
		StartCoroutine (processCharacterCoroutine (currentCharacter));
	}

	IEnumerator processCharacterCoroutine (GameObject currentCharacter)
	{
		applyDamage.pushCharacterWithoutForceAndPauseGetUp (currentCharacter);

		yield return new WaitForEndOfFrame ();

		Collider[] temporalHits = Physics.OverlapBox (currentCutPosition, currentCutOverlapBoxSize, currentCutRotation, targetToDamageLayer);

		bool bodyPartFound = false;

		if (showDebugPrint) {
			print ("activating ragdoll on " + currentCharacter.name);
		}

		if (showDebugPrint) {
			print ("\n\n");
		}

		if (temporalHits.Length > 0) {
			for (int i = 0; i < temporalHits.Length; i++) {
				if (!bodyPartFound) {
					Collider currentCollider = temporalHits [i];

					if (showDebugPrint) {
						print ("checking " + currentCollider.name);
					}

					if (showDebugPrint) {
						print (currentCollider.name + " body part when killing");
					}

					if (applyDamage.isCharacter (currentCollider.gameObject)) {
						bodyPartFound = true;
					}

					if (currentCharacter != null) {
						applyDamage.killCharacter (currentCharacter);
					}

					processObject (currentCollider.gameObject, currentCollider, currentCutPosition);
				}
			}
		}
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
			if (cutPositionTransform != null) {
				GKC_Utils.drawRectangleGizmo (cutPositionTransform.position, cutPositionTransform.rotation, Vector3.zero, cutOverlapBoxSize, gizmoColor);
			}
		}
	}
}