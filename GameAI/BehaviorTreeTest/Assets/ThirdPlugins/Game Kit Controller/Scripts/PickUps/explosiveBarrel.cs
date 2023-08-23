﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;

public class explosiveBarrel : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool breakInPieces;
	public bool canDamageToExplosiveBarrelOwner = true;

	public float explosionForceToBarrelPieces = 5;
	public float explosionRadiusToBarrelPieces = 30;
	public ForceMode forceModeToBarrelPieces = ForceMode.Impulse;

	[Space]
	[Header ("Explosion/Damage Settings")]
	[Space]

	public float explosionDamage;
	public bool ignoreShield;
	public float damageRadius;
	public float minVelocityToExplode;
	public float explosionDelay;
	public float explosionForce = 300;

	public int damageTypeID = -1;

	[Space]
	[Header ("Other Explosion Settings")]
	[Space]

	public bool pushCharacters = true;

	public bool killObjectsInRadius;

	public ForceMode explosionForceMode;

	public bool userLayerMask;
	public LayerMask layer;

	public bool applyExplosionForceToVehicles = true;
	public float explosionForceToVehiclesMultiplier = 0.2f;

	[Space]
	[Header ("Damage Over Time Settings")]
	[Space]

	public bool damageTargetOverTime;
	public float damageOverTimeDelay;
	public float damageOverTimeDuration;
	public float damageOverTimeAmount;
	public float damageOverTimeRate;
	public bool damageOverTimeToDeath;
	public bool removeDamageOverTimeState;

	[Space]
	[Header ("Audio Settings")]
	[Space]

	public AudioClip explosionSound;
	public AudioElement explosionAudioElement;

	[Space]
	[Header ("Remote Events Settings")]
	[Space]

	public bool useRemoteEventOnObjectsFound;
	public string remoteEventName;

	[Space]
	[Header ("Fade or Remove Broken Pieces")]
	[Space]

	public bool fadeBrokenPieces = true;
	public bool removeBrokenPiecesEnabled = true;

	public float timeToRemove = 3;

	public string defaultShaderName = "Legacy Shaders/Transparent/Diffuse";
	public bool useCustomShader;
	public Shader customShader;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool canExplode = true;

	[Space]
	[Header ("Gizmo Settings")]
	[Space]

	public bool showGizmo;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject brokenBarrel;
	public GameObject explosionParticles;
	public Shader transparentShader;
	public Rigidbody mainRigidbody;


	bool exploded;

	List<Material> rendererParts = new List<Material> ();
	int i, j;
	GameObject barrelOwner;

	bool isDamaged;

	Vector3 damageDirection;
	Vector3 damagePosition;

	int rendererPartsCount;

	Material currentMaterial;

	Coroutine mainCoroutine;


	private void InitializeAudioElements ()
	{
		if (explosionSound != null) {
			explosionAudioElement.clip = explosionSound;
		}
	}

	void Start ()
	{
		InitializeAudioElements ();

		getBarrelRigidbody ();
	}

	IEnumerator updateCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateState ();

			yield return waitTime;
		}
	}

	void updateState ()
	{
		//if the barrel has exploded, wait a seconds and then 
		if (exploded) {
			if (timeToRemove > 0) {
				timeToRemove -= Time.deltaTime;
			} else {
				if (fadeBrokenPieces) {
					//change the alpha of the color in every renderer component in the fragments of the barrel
					rendererPartsCount = rendererParts.Count;

					bool allPiecesFaded = true;

					for (i = 0; i < rendererPartsCount; i++) {
						currentMaterial = rendererParts [i];

						Color alpha = currentMaterial.color;
						alpha.a -= Time.deltaTime / 5;
						currentMaterial.color = alpha;

						//once the alpha is 0, remove the gameObject
						if (currentMaterial.color.a > 0) {
							allPiecesFaded = false;
						}
					}

					if (allPiecesFaded) {
						Destroy (gameObject);
					}
				} else {
					if (removeBrokenPiecesEnabled) {
						Destroy (gameObject);
					} else {
						StopCoroutine (mainCoroutine);
					}
				}
			}
		}
	}

	//explode this barrel
	public void explodeBarrel ()
	{
		if (exploded) {
			return;
		}

		//if the barrel has not been throwing by the player, the barrel owner is the barrel itself
		if (barrelOwner == null) {
			barrelOwner = gameObject;
		}

		//disable the main mesh of the barrel and create the copy with the fragments of the barrel
		GetComponent<Collider> ().enabled = false;

		GetComponent<MeshRenderer> ().enabled = false;

		if (mainRigidbody != null) {
			mainRigidbody.isKinematic = true;
		}

		if (fadeBrokenPieces) {
			if (useCustomShader) {
				transparentShader = customShader;
			} else {
				if (transparentShader == null) {
					transparentShader = Shader.Find (defaultShaderName);
				}
			}
		}

		Vector3 currentPosition = transform.position;

		//check all the colliders inside the damage radius
		applyDamage.setExplosion (currentPosition, damageRadius, userLayerMask, layer, barrelOwner, canDamageToExplosiveBarrelOwner, 
			gameObject, killObjectsInRadius, true, false, explosionDamage, pushCharacters, applyExplosionForceToVehicles,
			explosionForceToVehiclesMultiplier, explosionForce, explosionForceMode, true, barrelOwner.transform, ignoreShield, 
			useRemoteEventOnObjectsFound, remoteEventName, damageTypeID,
			damageTargetOverTime, damageOverTimeDelay, damageOverTimeDuration, damageOverTimeAmount, 
			damageOverTimeRate, damageOverTimeToDeath, removeDamageOverTimeState);

		//create the explosion particles
		GameObject explosionParticlesClone = (GameObject)Instantiate (explosionParticles, transform.position, transform.rotation);
		explosionParticlesClone.transform.SetParent (transform);

		//if the option break in pieces is enabled, create the barrel broken
		if (breakInPieces) {
			GameObject brokenBarrelClone = (GameObject)Instantiate (brokenBarrel, transform.position, transform.rotation);
			brokenBarrelClone.transform.localScale = transform.localScale;
			brokenBarrelClone.transform.SetParent (transform);

			explosionAudioElement.audioSource = brokenBarrelClone.GetComponent<AudioSource> ();
			AudioPlayer.PlayOneShot (explosionAudioElement, gameObject);

			Component[] components = brokenBarrelClone.GetComponentsInChildren (typeof(MeshRenderer));
			foreach (MeshRenderer child in components) {
				//add force to every piece of the barrel and add a box collider
				Rigidbody currentPartRigidbody = child.gameObject.AddComponent<Rigidbody> ();

				child.gameObject.AddComponent<BoxCollider> ();

				currentPartRigidbody.AddExplosionForce (explosionForceToBarrelPieces, child.transform.position, explosionRadiusToBarrelPieces, 1, forceModeToBarrelPieces);

				if (fadeBrokenPieces) {
					//change the shader of the fragments to fade them
					int materialsLength = child.materials.Length;

					for (j = 0; j < materialsLength; j++) {
						Material temporalMaterial = child.materials [j];

						temporalMaterial.shader = transparentShader;
						rendererParts.Add (temporalMaterial);
					}
				}
			}
		}

		//kill the health component, to call the functions when the object health is 0
		if (!applyDamage.checkIfDead (gameObject)) {
			applyDamage.killCharacter (gameObject);
		}

		//search the player in case he had grabbed the barrel when it exploded
		exploded = true;

		mainCoroutine = StartCoroutine (updateCoroutine ());

		//if the object is being carried by the player, make him drop it
		GKC_Utils.checkDropObject (gameObject);
	}

	//if the player grabs this barrel, disable its explosion by collisions
	public void canExplodeState (bool state)
	{
		canExplode = state;
	}

	public void setExplosiveBarrelOwner (GameObject newBarrelOwner)
	{
		barrelOwner = newBarrelOwner;
	}

	//if the barrel collides at enough speed, explode it
	void OnCollisionEnter (Collision col)
	{
		if (canExplode && !exploded) {
			if (showDebugPrint) {
				print ("Collision velocity " + col.relativeVelocity.magnitude);
			}

			if (Mathf.Abs (col.relativeVelocity.magnitude) > minVelocityToExplode) {
				explodeBarrel ();

				return;
			}

			if (mainRigidbody != null) {
				if (Mathf.Abs (mainRigidbody.velocity.magnitude) > minVelocityToExplode) {
					explodeBarrel ();
				}
			}
		}
	}

	public void getBarrelRigidbody ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}
	}

	public void setBarrelRigidbody (Rigidbody rigidbodyToUse)
	{
		mainRigidbody = rigidbodyToUse;
	}

	public void waitToExplode ()
	{
		if (explosionDelay > 0) {
			StartCoroutine (waitToExplodeCorutine ());
		} else {
			explodeBarrel ();
		}
	}

	//delay to explode the barrel
	IEnumerator waitToExplodeCorutine ()
	{
		yield return new WaitForSeconds (explosionDelay);

		explodeBarrel ();
	}

	//set the explosion values from other component
	public void setExplosionValues (float force, float radius)
	{
		explosionForce = force;

		damageRadius = radius;
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
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.position, damageRadius);
		}
	}
}