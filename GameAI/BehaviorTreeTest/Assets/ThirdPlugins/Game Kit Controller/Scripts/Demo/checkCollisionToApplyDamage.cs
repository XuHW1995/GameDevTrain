using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class checkCollisionToApplyDamage : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool damageEnabled = true;

	[Space]
	[Header ("Collisions Settings")]
	[Space]

	public bool useMinimumCollisionSpeed;
	public float minimumCollisionSpeed;

	[Space]
	[Header ("Damage On Detected Object Settings")]
	[Space]

	public bool killDetectedObject;

	public bool applyDamageOnDetectedObject;

	public bool pushCharacterDetected;
	public float extraForceOnObjectDetected;

	public bool ignoreShieldOnObjectDetected;

	public int damageTypeIDOnObjectDetected = -1;

	public bool canActivateReactionSystemTemporallyOnObjectDetected;
	public int damageReactionIDOnObjectDetected = -1;

	public float damageMultiplierOnObjectDetected = 1;

	public bool applyFixedDamageValueOnObjectDetected;
	public float fixedDamageValueOnObjectDetected;

	[Space]
	[Header ("Damage On Object Settings")]
	[Space]

	public bool killObjectOnCollision;

	public bool applyDamageOnObject;

	public bool pushCharacterOnCollision;
	public float extraForceOnCollision;

	public bool ignoreShield;

	public int damageTypeID = -1;

	public bool canActivateReactionSystemTemporally;
	public int damageReactionID = -1;

	public float damageMultiplier = 1;

	public bool applyFixedDamageValue;
	public float fixedDamageValue;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnCollision;
	public List<string> remoteEventNameList = new List<string> ();

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useEventOnCollision;
	public UnityEvent eventOnCollision;

	[Space]
	[Header ("Components")]
	[Space]

	public healthManagement mainHealthManagement;

	ContactPoint currentContact;
	GameObject collisionObject;

	bool mainHealthManagementLocated;


	void OnCollisionEnter (Collision collision)
	{
		if (!damageEnabled) {
			return;
		}

		bool canActivateCollisionReaction = true;

		if (useMinimumCollisionSpeed) {
			float collisionMagnitude = collision.relativeVelocity.magnitude;

			if (Mathf.Abs (collisionMagnitude) < minimumCollisionSpeed) {
				canActivateCollisionReaction = false;
			} 
		}

		if (canActivateCollisionReaction) {
			currentContact = collision.contacts [0];

			collisionObject = collision.gameObject;

			if (useRemoteEventOnCollision) {
				remoteEventSystem currentRemoteEventSystem = collisionObject.GetComponent<remoteEventSystem> ();

				if (currentRemoteEventSystem != null) {
					for (int i = 0; i < remoteEventNameList.Count; i++) {
						currentRemoteEventSystem.callRemoteEvent (remoteEventNameList [i]);
					}
				}
			}

			if (useEventOnCollision) {
				eventOnCollision.Invoke ();
			}

			if (killDetectedObject) {
				float damage = applyDamage.getCurrentHealthAmount (collisionObject);

				applyDamage.checkHealth (gameObject, collisionObject, damage, transform.forward, currentContact.point, gameObject, 
					false, true, ignoreShieldOnObjectDetected, false, canActivateReactionSystemTemporallyOnObjectDetected, damageReactionIDOnObjectDetected, damageTypeIDOnObjectDetected);
			} else {
				if (applyDamageOnDetectedObject) {
					if (pushCharacterDetected) {
						Vector3 pushDirection = (currentContact.point - transform.position).normalized;

						if (extraForceOnObjectDetected > 0) {
							pushDirection *= extraForceOnObjectDetected;
						}

						applyDamage.pushCharacter (collisionObject, pushDirection);
					}
					
					float damage = 0;

					if (applyFixedDamageValueOnObjectDetected) {
						damage = fixedDamageValueOnObjectDetected;
					} else {
						damage = collision.relativeVelocity.magnitude * damageMultiplierOnObjectDetected;
					}

					applyDamage.checkHealth (gameObject, collisionObject, damage, transform.forward, currentContact.point, gameObject, 
						false, true, ignoreShieldOnObjectDetected, false, canActivateReactionSystemTemporallyOnObjectDetected, damageReactionIDOnObjectDetected, damageTypeIDOnObjectDetected);
				}
			}

			if (!mainHealthManagementLocated) {
				mainHealthManagementLocated = mainHealthManagement != null;

				if (!mainHealthManagementLocated) {
					mainHealthManagement = gameObject.GetComponent<healthManagement> ();

					mainHealthManagementLocated = mainHealthManagement != null;
				}
			}
		
			if (mainHealthManagementLocated) {
				if (killObjectOnCollision) {
					float damage = applyDamage.getCurrentHealthAmount (collisionObject);

					mainHealthManagement.setDamageWithHealthManagement (damage, transform.forward, currentContact.point, gameObject,
						gameObject, false, true, 
						ignoreShield, false, true,
						canActivateReactionSystemTemporally, 
						damageReactionID, damageTypeID);
					
				} else {
					if (applyDamageOnObject) {
						if (pushCharacterOnCollision) {
							Vector3 pushDirection = (currentContact.point - transform.position).normalized;

							if (extraForceOnCollision > 0) {
								pushDirection *= extraForceOnCollision;
							}

							applyDamage.pushCharacter (collisionObject, pushDirection);
						}

						float damage = 0;

						if (applyFixedDamageValue) {
							damage = fixedDamageValue;
						} else {
							damage = collision.relativeVelocity.magnitude * damageMultiplier;
						}

						mainHealthManagement.setDamageWithHealthManagement (damage, transform.forward, currentContact.point, gameObject,
							gameObject, false, true, 
							ignoreShield, false, true,
							canActivateReactionSystemTemporally, 
							damageReactionID, damageTypeID);
					}
				}
			}
		}
	}

	public void setDamageEnabledState (bool state)
	{
		damageEnabled = state;
	}

	public void disableDamage ()
	{
		setDamageEnabledState (false);
	}

	public void enableDamage ()
	{
		setDamageEnabledState (true);
	}
}
