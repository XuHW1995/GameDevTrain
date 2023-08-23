using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using EzySlice;

//using NobleMuffins.LimbHacker.Guts;
//using NobleMuffins.LimbHacker;

public class sliceSystemUtils : MonoBehaviour
{
	
	public static void sliceObject (Vector3 slicePosition, GameObject objectToSlice, Vector3 cutDirection, Material crossSectionMaterial, ref bool objectSliced, ref GameObject object1, ref GameObject object2)
	{
		// SlicedHull hull = objectToSlice.SliceObject (slicePosition, cutDirection, crossSectionMaterial);

		// if (hull != null) {

			// objectSliced = true;

			// object1 = hull.CreateLowerHull (objectToSlice, crossSectionMaterial);
			// object2 = hull.CreateUpperHull (objectToSlice, crossSectionMaterial);
		// }
	}

	public static surfaceToSlice getSurfaceToSlice (GameObject currentSurface)
	{
		// ChildOfHackable currentChildOfHackable = currentSurface.GetComponent<ChildOfHackable> ();

		// if (currentChildOfHackable != null) {
			// return currentChildOfHackable.parentHackable.mainSurfaceToSlice;
		// }

		return null;
	}

	public static void initializeValuesOnHackableComponent (GameObject objectToUse, simpleSliceSystem currentSimpleSliceSystem)
	{
		// Hackable currentHackable = objectToUse.GetComponent<Hackable> ();

		// if (currentHackable == null) {
			// currentHackable = objectToUse.AddComponent<Hackable> ();
		// }

		// currentHackable.mainSurfaceToSlice = currentSimpleSliceSystem.mainSurfaceToSlice;

		// currentHackable.alternatePrefab = currentSimpleSliceSystem.getMainAlternatePrefab ();

		// currentHackable.objectToSlice = currentSimpleSliceSystem.objectToSlice;

		// currentHackable.infillMaterial = currentSimpleSliceSystem.infillMaterial;

		// currentHackable.severables = currentSimpleSliceSystem.getSeverables ();

		// currentHackable.ignoreUpdateLastObjectSpeed = currentSimpleSliceSystem.ignoreUpdateLastObjectSpeed;

		// currentHackable.ignoreDestroyOriginalObject = currentSimpleSliceSystem.ignoreDestroyOriginalObject;

		// currentHackable.eventsOnIgnoreDestroyOriginalObject = currentSimpleSliceSystem.eventsOnIgnoreDestroyOriginalObject;

		// currentHackable.setCustomIDOnSliceSpieces = currentSimpleSliceSystem.setCustomIDOnSliceSpieces;

		// if (currentSimpleSliceSystem.setCustomIDOnSliceSpieces) {
			// currentSimpleSliceSystem.setRandomString ();

			// currentHackable.setRandomString (currentSimpleSliceSystem.getRandomString ());
		// }

		// currentHackable.initializeValues ();
	}

	public static void sliceCharacter (GameObject objectToSlice, Vector3 point, Vector3 newNormalInWorldSpaceValue,
	                                   bool updateLastObjectSpeed, Vector3 lastSpeed)
	{
		if (objectToSlice == null) {
			return;
		}

		// Hackable currentHackable = objectToSlice.GetComponent<Hackable> ();

		// if (currentHackable != null) {
			// currentHackable.activateSlice (objectToSlice, point, newNormalInWorldSpaceValue, updateLastObjectSpeed, lastSpeed);
		// }
	}


	public static GameObject[] shatterObject (GameObject obj, Vector3 shatterPosition, Material crossSectionMaterial = null, bool shaterActive = true)
	{
		// if (shaterActive) {
			// return obj.SliceInstantiate (getRandomPlane (shatterPosition, obj.transform.localScale),
				// new TextureRegion (0.0f, 0.0f, 1.0f, 1.0f),
				// crossSectionMaterial);
		// } 

		return null;
	}

	// public static EzySlice.Plane getRandomPlane (Vector3 positionOffset, Vector3 scaleOffset)
	// {
		// Vector3 randomPosition = Random.insideUnitSphere;
		// Vector3 randomDirection = Random.insideUnitSphere.normalized;

		// return new EzySlice.Plane (randomPosition, randomDirection);
	// }

	public static void destroyAllSlicedPiecesByRandomString (string stringValue, GameObject mainObjectToIgnore)
	{
		// Hackable[] HackableList = FindObjectsOfType<Hackable> ();

		// foreach (Hackable currentHackable in HackableList) {
			// if (mainObjectToIgnore != currentHackable.gameObject && currentHackable.getRandomString ().Equals (stringValue)) {
				// Destroy (currentHackable.gameObject);
			// }
		// }
	}


	public static void processObjectToSlice (GameObject objectToCheck, Collider objectCollider, 
	                                         Vector3 slicePosition, Quaternion sliceRotation,
	                                         float minDelayToSliceSameObject, Vector3 sliceUpDirection, 
	                                         Vector3 sliceRightDirection, Vector3 sliceForwardDirection, 
	                                         float forceToApplyToCutPart, LayerMask targetToDamageLayer, 
	                                         bool randomSliceDirection, bool showSliceGizmo,
	                                         bool activateRigidbodiesOnNewObjects)
	{
		surfaceToSlice currentSurfaceToSlice = objectToCheck.GetComponent<surfaceToSlice> ();

		if (currentSurfaceToSlice == null) {
			currentSurfaceToSlice = sliceSystemUtils.getSurfaceToSlice (objectToCheck);
		}

		if (currentSurfaceToSlice != null) {
			bool isCutSurfaceEnabled = currentSurfaceToSlice.isCutSurfaceEnabled ();

			if (isCutSurfaceEnabled && currentSurfaceToSlice.sliceCanBeActivated (minDelayToSliceSameObject)) {
				sliceCurrentObject (currentSurfaceToSlice, objectToCheck, objectCollider, 
					currentSurfaceToSlice.getCrossSectionMaterial (), 
					slicePosition, sliceRotation, sliceUpDirection, sliceRightDirection, 
					sliceForwardDirection, forceToApplyToCutPart, minDelayToSliceSameObject, 
					targetToDamageLayer, randomSliceDirection, showSliceGizmo, activateRigidbodiesOnNewObjects);
			}
		} 
	}

	public static void sliceCurrentObject (surfaceToSlice currentSurfaceToSlice, GameObject objectToCheck,
	                                       Collider objectCollider, Material crossSectionMaterial, 
	                                       Vector3 slicePosition, Quaternion sliceRotation,
	                                       Vector3 sliceUpDirection, Vector3 sliceRightDirection, Vector3 sliceForwardDirection,
	                                       float forceToApplyToCutPart, float minDelayToSliceSameObject, 
	                                       LayerMask targetToDamageLayer, bool randomSliceDirection, bool showSliceGizmo, 
	                                       bool activateRigidbodiesOnNewObjects)
	{
		// slice the provided object using the transforms of this object
		if (currentSurfaceToSlice.isObjectCharacter ()) {
			//			print ("character found " + objectToCheck.name + " is dead " + applyDamage.checkIfDead (objectToCheck));

			if (applyDamage.getCharacterOrVehicle (objectToCheck) != null && !applyDamage.checkIfDead (objectToCheck)) {
				processCharacter (objectToCheck, slicePosition, sliceRotation,
					minDelayToSliceSameObject, sliceUpDirection, 
					sliceRightDirection, sliceForwardDirection, forceToApplyToCutPart, 
					targetToDamageLayer, randomSliceDirection, showSliceGizmo, activateRigidbodiesOnNewObjects);

				return;
			}

			Rigidbody mainObject = objectToCheck.GetComponent<Rigidbody> ();

			bool mainObjectHasRigidbody = mainObject != null;

			Vector3 lastSpeed = Vector3.zero;

			if (mainObjectHasRigidbody) {
				lastSpeed = mainObject.velocity;
			}

			Vector3 planeDefiner1 = slicePosition - sliceForwardDirection * 0.2f;
			Vector3 planeDefiner2 = slicePosition + sliceRightDirection * 0.2f;
			Vector3 planeDefiner3 = slicePosition - sliceRightDirection * 0.2f;

			Vector3 positionInWorldSpace = (planeDefiner1 + planeDefiner2 + planeDefiner3) / 3f;

			Vector3 normalInWorldSpace = Vector3.zero;

			Vector3 t0 = planeDefiner1;
			Vector3 t1 = planeDefiner2;
			Vector3 t2 = planeDefiner3;

			normalInWorldSpace.x = t0.y * (t1.z - t2.z) + t1.y * (t2.z - t0.z) + t2.y * (t0.z - t1.z);
			normalInWorldSpace.y = t0.z * (t1.x - t2.x) + t1.z * (t2.x - t0.x) + t2.z * (t0.x - t1.x);
			normalInWorldSpace.z = t0.x * (t1.y - t2.y) + t1.x * (t2.y - t0.y) + t2.x * (t0.y - t1.y);

			if (showSliceGizmo) {
				Vector3 rayDirection1 = slicePosition;
				Vector3 rayDirection2 = sliceForwardDirection;

				Debug.DrawRay (rayDirection1, rayDirection2, Color.blue, 5);

				rayDirection1 = slicePosition + sliceForwardDirection + sliceRightDirection;
				rayDirection2 = sliceRightDirection;

				Debug.DrawRay (rayDirection1, rayDirection2, Color.blue, 5);

				rayDirection1 = slicePosition - sliceForwardDirection + sliceRightDirection;
				rayDirection2 = sliceRightDirection;

				Debug.DrawRay (rayDirection1, rayDirection2, Color.blue, 5);
			}

			currentSurfaceToSlice.getMainSimpleSliceSystem ().activateSlice (objectCollider, positionInWorldSpace, 
				normalInWorldSpace, slicePosition, true, lastSpeed);

			currentSurfaceToSlice.checkEventOnCut ();

			currentSurfaceToSlice.checkTimeBulletOnCut ();
		} else {
			// slice the provided object using the transforms of this object
			bool objectSliced = false;

			GameObject object1 = null;
			GameObject object2 = null;

			// slice the provided objectToCheck	ect using the transforms of this object
			Vector3 sliceDirection = sliceUpDirection;

			if (randomSliceDirection) {
				sliceDirection = Quaternion.Euler (Random.Range (0, 361) * sliceForwardDirection) * sliceDirection;
			}

			sliceSystemUtils.sliceObject (slicePosition, objectToCheck, sliceDirection, crossSectionMaterial, ref objectSliced, ref object1, ref object2);

			Vector3 objectPosition = objectToCheck.transform.position;
			Quaternion objectRotation = objectToCheck.transform.rotation;

			Transform objectParent = objectToCheck.transform.parent;

			if (objectSliced) {
				currentSurfaceToSlice.checkEventOnCut ();

				currentSurfaceToSlice.checkTimeBulletOnCut ();

				Rigidbody mainObject = objectToCheck.GetComponent<Rigidbody> ();
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

				float currentForceToApply = forceToApplyToCutPart;

				if (mainObjectHasRigidbody || activateRigidbodiesOnNewObjects) {

					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}

					Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

					Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

					object2Rigidbody.AddExplosionForce (currentForceToApply, slicePosition, 10, 1, ForceMode.Impulse);

					object1Rigidbody.AddExplosionForce (currentForceToApply, slicePosition, 10, 1, ForceMode.Impulse);
				} else {
					if (currentSurfaceToSlice.useCustomForceAmount) {
						currentForceToApply = currentSurfaceToSlice.customForceAmount;
					}
						
					Rigidbody object2Rigidbody = object2.AddComponent<Rigidbody> ();

					object2Rigidbody.AddExplosionForce (currentForceToApply, slicePosition, 10, 1, ForceMode.Impulse);

					Rigidbody object1Rigidbody = object1.AddComponent<Rigidbody> ();

					object1Rigidbody.AddExplosionForce (currentForceToApply, slicePosition, 10, 1, ForceMode.Impulse);

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

				if (currentSurfaceToSlice.setNewLayerOnCut) {
					object1.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
					object2.layer = LayerMask.NameToLayer (currentSurfaceToSlice.newLayerOnCut);
				}

				if (currentSurfaceToSlice.setNewTagOnCut) {
					object1.tag = currentSurfaceToSlice.newTagOnCut;
					object2.tag = currentSurfaceToSlice.newTagOnCut;
				}
					
				objectToCheck.SetActive (false);
			}
		}
	}

	public static void processCharacter (GameObject currentCharacter, Vector3 slicePosition, Quaternion sliceRotation, 
	                                     float minDelayToSliceSameObject, Vector3 sliceUpDirection, 
	                                     Vector3 sliceRightDirection, Vector3 sliceForwardDirection, 
	                                     float forceToApplyToCutPart, LayerMask targetToDamageLayer, 
	                                     bool randomSliceDirection, bool showSliceGizmo,
	                                     bool activateRigidbodiesOnNewObjects)
	{
		applyDamage.pushCharacterWithoutForce (currentCharacter);

		Collider[] temporalHits = Physics.OverlapBox (slicePosition, new Vector3 (0.1f, 0.1f, 0.1f), sliceRotation,
			                          targetToDamageLayer);

		if (showSliceGizmo) {
			Vector3 rayDirection1 = slicePosition;
			Vector3 rayDirection2 = sliceForwardDirection;

			Debug.DrawRay (rayDirection1, rayDirection2, Color.blue, 5);

			rayDirection1 = slicePosition + sliceForwardDirection + sliceRightDirection;
			rayDirection2 = sliceRightDirection;

			Debug.DrawRay (rayDirection1, rayDirection2, Color.blue, 5);

			rayDirection1 = slicePosition - sliceForwardDirection + sliceRightDirection;
			rayDirection2 = sliceRightDirection;

			Debug.DrawRay (rayDirection1, rayDirection2, Color.blue, 5);
		}

		bool bodyPartFound = false;

		if (temporalHits.Length > 0) {
			if (showSliceGizmo) {
				print ("objects detected amount " + temporalHits.Length);

				for (int i = 0; i < temporalHits.Length; i++) {
					print (temporalHits [i].name);
				}
			}

			Collider colliderToCheck = null;

			float minDistance = 1000;

			for (int i = 0; i < temporalHits.Length; i++) {
				Collider currentCollider = temporalHits [i];

				if (applyDamage.isCharacter (currentCollider.gameObject)) {
					bodyPartFound = true;

					float currentDistance = GKC_Utils.distance (slicePosition, currentCollider.transform.position);

					if (currentDistance < minDistance) {
						minDistance = currentDistance;

						colliderToCheck = currentCollider;
					}
				}
			}

			if (bodyPartFound) {
				if (currentCharacter != null) {
					applyDamage.killCharacter (currentCharacter);
				}

				if (showSliceGizmo) {
					print ("body part detected " + colliderToCheck.name);
				}

				processObjectToSlice (colliderToCheck.gameObject, colliderToCheck, 
					slicePosition, sliceRotation,
					minDelayToSliceSameObject, sliceUpDirection, 
					sliceRightDirection, sliceForwardDirection, 
					forceToApplyToCutPart, targetToDamageLayer, randomSliceDirection, showSliceGizmo, 
					activateRigidbodiesOnNewObjects);
			}
		}
	}
}