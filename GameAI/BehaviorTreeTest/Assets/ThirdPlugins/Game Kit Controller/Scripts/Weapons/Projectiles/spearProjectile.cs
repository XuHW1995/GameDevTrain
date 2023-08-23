using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;

public class spearProjectile : projectileSystem
{
	[Header ("Main Settings")]
	[Space]

	public LayerMask layer;

	public float rayDistance = 1;
	public float carryingRagdollSpeedMultiplier = 3;
	public float maxProjectileSpeedClamp = 30;
	public float capsuleCastRadius = 2;

	public float delayToDropSpearIfNotSurfaceFound = 10;

	[Space]
	[Header ("Other Settings")]
	[Space]

	public bool useGeneralProbabilityToKillTargets;
	[Range (0, 100)] public float generalProbabilityToKillTargets;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool surfaceFound;
	public bool carryingRagdoll;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform raycastPositionTransform;

	ragdollActivator currentRagdoll;

	Vector3 previousVelocity;
	Transform currentRagdollRootMotion;
	bool firstSurfaceDetected;
	Vector3 surfacePoint;
	float extraDistanceRaycast = 5;
	Collider[] hitColliders;

	float lastTimeSpearCarryingRagdoll;

	Coroutine updateCoroutine;


	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		if (carryingRagdoll) {
			if (surfaceFound) {
				stopUpdateCoroutine ();
			} else {
				if (!firstSurfaceDetected) {
					if (Physics.Linecast (currentRagdollRootMotion.position, currentRagdollRootMotion.position + transform.forward * extraDistanceRaycast, out hit, layer)) {
						if (!hit.collider.GetComponent<Rigidbody> ()) {
							firstSurfaceDetected = true;
							surfacePoint = hit.point;
						}
					} 
				} 

				if (firstSurfaceDetected) {
					float currentDistance = GKC_Utils.distance (currentRagdollRootMotion.position, surfacePoint);

					if (currentDistance < rayDistance) {
						surfaceFound = true;
					}

					if (currentDistance < extraDistanceRaycast) {
						if (carryingRagdollSpeedMultiplier > 1) {
							carryingRagdollSpeedMultiplier = 1;
						}
					}
				}

				if (Physics.Linecast (currentRagdollRootMotion.position, currentRagdollRootMotion.position + transform.forward * rayDistance, out hit, layer)) {
					if (!hit.collider.GetComponent<Rigidbody> ()) {
						surfaceFound = true;
					}
				}

				Vector3 movementValue = transform.InverseTransformDirection (transform.forward) * (Mathf.Abs (previousVelocity.magnitude) * carryingRagdollSpeedMultiplier * Time.deltaTime);

				if (maxProjectileSpeedClamp != 0) {
					if (Mathf.Abs (movementValue.magnitude) > maxProjectileSpeedClamp) {
						movementValue = Vector3.ClampMagnitude (movementValue, maxProjectileSpeedClamp);
					}
				}

				transform.Translate (movementValue);
		
				if (Time.time > lastTimeSpearCarryingRagdoll + delayToDropSpearIfNotSurfaceFound) {
					surfaceFound = true;

					mainRigidbody.isKinematic = false;

					mainRigidbody.useGravity = true;

					mainRigidbody.AddForce (transform.forward * Mathf.Abs (previousVelocity.magnitude) * carryingRagdollSpeedMultiplier);
				}
			}
		}
	}


	//when the bullet touchs a surface, then
	public void checkObjectDetected (Collider col)
	{
		if (canActivateEffect (col)) {
			if (currentProjectileInfo.impactAudioElement != null) {
				currentProjectileInfo.impactAudioElement.audioSource = GetComponent<AudioSource> ();
				AudioPlayer.PlayOneShot (currentProjectileInfo.impactAudioElement, gameObject);
			}

			projectileUsed = true;

			//set the bullet kinematic
			objectToDamage = col.GetComponent<Collider> ().gameObject;

			previousVelocity = mainRigidbody.velocity;

			//print (objectToDamage.name);
			mainRigidbody.isKinematic = true;

			if (carryingRagdoll) {
				return;
			}

			Rigidbody objectToDamageRigidbody = objectToDamage.GetComponent<Rigidbody> ();

			Transform currentCharacter = null;

			GameObject currentCharacterGameObject = applyDamage.getCharacterOrVehicle (objectToDamage);

			if (currentCharacterGameObject != null) {
				currentCharacter = currentCharacterGameObject.transform;
			}

			if (objectToDamageRigidbody != null) {				
				currentRagdoll = objectToDamage.GetComponent<ragdollActivator> ();

				//&& !applyDamage.checkIfDead (currentCharacter)

				if (currentRagdoll != null) {
					Vector3 currentPosition = transform.position;

					List<ragdollActivator.bodyPart> bones = currentRagdoll.getBodyPartsList ();

					float distance = Mathf.Infinity;

					int index = -1;

					for (int i = 0; i < bones.Count; i++) {
						float currentDistance = GKC_Utils.distance (bones [i].transform.position, currentPosition);

						if (currentDistance < distance) {
							distance = currentDistance;
							index = i;
						}
					}

					if (index != -1) {
						if (applyDamage.checkIfDead (objectToDamage)) {
							mainRigidbody.isKinematic = false;

							mainRigidbody.velocity = previousVelocity;

							projectileUsed = false;

							currentRagdoll.setRagdollOnExternalParent (transform);
						}
					}
				} else if (currentCharacter != null) {
					transform.SetParent (currentCharacter);
				} else if (objectToDamage != null) {
					transform.SetParent (objectToDamage.transform);
				}
			} else if (currentCharacter != null) {
				transform.SetParent (currentCharacter);
			}

			checkProjectilesParent ();


			bool activateSliceResult = true;

			if (!currentProjectileInfo.killInOneShot) {
				activateSliceResult = false;
			} else {
				if (useGeneralProbabilityToKillTargets) {
					float randomProbability = UnityEngine.Random.Range (0, 101);

					if (randomProbability > generalProbabilityToKillTargets) {
						activateSliceResult = false;
					}
				}
			}

			//add velocity if the touched object has rigidbody
			if (activateSliceResult) {
				applyDamage.killCharacter (gameObject, objectToDamage, -transform.forward, transform.position, currentProjectileInfo.owner, false);
			
				if (currentRagdoll != null) {
					projectilePaused = true;

					transform.SetParent (null);

					currentRagdollRootMotion = currentRagdoll.getRootMotion ();

					currentRagdollRootMotion.SetParent (transform);

					currentRagdollRootMotion.GetComponent<Rigidbody> ().isKinematic = true;

					currentRagdoll.setRagdollOnExternalParent (transform);

					projectileUsed = false;

					lastTimeSpearCarryingRagdoll = Time.time;

					carryingRagdoll = true;

					updateCoroutine = StartCoroutine (updateSystemCoroutine ());
				}
			} else {
				applyDamage.checkHealth (gameObject, objectToDamage, currentProjectileInfo.projectileDamage, -transform.forward, 
					transform.position, currentProjectileInfo.owner, false, true, currentProjectileInfo.ignoreShield, false, 
					currentProjectileInfo.canActivateReactionSystemTemporally, currentProjectileInfo.damageReactionID, currentProjectileInfo.damageTypeID);
			}

			if (currentProjectileInfo.applyImpactForceToVehicles) {
				Rigidbody objectToDamageMainRigidbody = applyDamage.applyForce (objectToDamage);

				if (objectToDamageMainRigidbody != null) {
					Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;

					objectToDamageMainRigidbody.AddForce (force * objectToDamageMainRigidbody.mass, currentProjectileInfo.forceMode);
				}
			} else {
				if (applyDamage.canApplyForce (objectToDamage)) {
					Vector3 force = transform.forward * currentProjectileInfo.impactForceApplied;

					objectToDamageRigidbody.AddForce (force * objectToDamageRigidbody.mass, currentProjectileInfo.forceMode);
				}
			}
		}
	}

	public override void resetProjectile ()
	{
		base.resetProjectile ();

		stopUpdateCoroutine ();

		surfaceFound = false;
		carryingRagdoll = false;
		firstSurfaceDetected = false;
	}
}