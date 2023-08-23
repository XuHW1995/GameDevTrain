using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customOrderBehaviorToBorrowWeapon : customOrderBehavior
{
	[Header ("Custom Settings")]
	[Space]

	public ForceMode dropForceMode;
	public float extraForceAmount = 5;

	public bool useParableSpeed;

	public bool pickWeaponDirectlyOnPlayer;

	public override void activateOrder (Transform character, Transform orderOwner)
	{
		if (!orderEnabled) {
			return;
		}

		if (character == null) {
			return;
		}

		playerComponentsManager currentPlayerComponentsManager = character.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			findObjectivesSystem currentFindObjectivesSystem = currentPlayerComponentsManager.getFindObjectivesSystem ();

			if (currentFindObjectivesSystem != null) {

				GameObject lastWeaponDroppedObject = null;

				playerWeaponsManager currentPlayerWeaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

				if (currentPlayerWeaponsManager != null) {
					if (currentPlayerWeaponsManager.isWeaponsModeActive ()) {
//						currentPlayerWeaponsManager.dropCurrentWeaponExternally ();

						if (currentPlayerWeaponsManager.isUsingWeapons ()) {
							currentFindObjectivesSystem.dropWeapon ();

							lastWeaponDroppedObject = currentPlayerWeaponsManager.getLastWeaponDroppedObject ();

							if (showDebugPrint) {
								print ("dropping fire weapon");
							}
						}
					} else {
						meleeWeaponsGrabbedManager currentMeleeWeaponsGrabbedManager = currentPlayerComponentsManager.getMeleeWeaponsGrabbedManager ();
					
						if (currentMeleeWeaponsGrabbedManager != null) {
							Transform currentWeaponTransform = currentMeleeWeaponsGrabbedManager.getCurrentGrabbedObjectTransform ();

							if (currentWeaponTransform != null) {
								lastWeaponDroppedObject = currentWeaponTransform.gameObject;

								currentFindObjectivesSystem.dropWeapon ();

								if (showDebugPrint) {
									print ("dropping melee weapon");
								}
							}
						}
					}
				}


				if (lastWeaponDroppedObject != null) {
					if (showDebugPrint) {
						print ("weapon dropped located");
					}

					Rigidbody weaponRigidbody = lastWeaponDroppedObject.GetComponent<Rigidbody> ();

					if (weaponRigidbody != null) {
						if (useParableSpeed) {
							Vector3 newVel = GKC_Utils.getParableSpeed (weaponRigidbody.position, orderOwner.position, character.forward,
								                 character, true, false, 0);

							if (newVel == -Vector3.one) {
								newVel = character.forward * 100;
							}

							weaponRigidbody.AddForce (newVel, ForceMode.VelocityChange);
						} else {

							Vector3 forceDirection = orderOwner.position - weaponRigidbody.position;

							float distance = forceDirection.magnitude;
							forceDirection = forceDirection / distance;	

							weaponRigidbody.AddForce (forceDirection * extraForceAmount, dropForceMode);
						}
					}

					if (pickWeaponDirectlyOnPlayer) {
						playerComponentsManager orderOwnerPlayerComponentsManager = orderOwner.GetComponent<playerComponentsManager> ();

						if (orderOwnerPlayerComponentsManager != null) {
							playerController ownerPlayerController = orderOwnerPlayerComponentsManager.getPlayerController ();

							if (ownerPlayerController != null) {
								if (!ownerPlayerController.playerIsBusy ()) {
									simpleActionButton currentSimpleActionButton = lastWeaponDroppedObject.GetComponentInChildren<simpleActionButton> ();

									if (currentSimpleActionButton != null) {
										lastWeaponDroppedObject = currentSimpleActionButton.gameObject;
									}
								
									GKC_Utils.useObjectExternally (orderOwner.gameObject, lastWeaponDroppedObject);
								}
							}
						}
					}
				}
			}
		}
	}
}
