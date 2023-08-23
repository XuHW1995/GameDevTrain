using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;

public class crate : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public AudioClip brokenSound;
	public AudioElement brokenAudioElement;
	public float minVelocityToBreak;

	public float breakForce = 10;
	public ForceMode forceMode;

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

	public bool canBeBroken = true;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject brokenCrate;

	public Shader transparentShader;

	public Rigidbody mainRigidbody;


	bool broken;
	List<Material> rendererParts = new List<Material> ();
	int i, j;

	int rendererPartsCount;

	Material currentMaterial;

	Coroutine mainCoroutine;


	private void InitializeAudioElements ()
	{
		if (brokenSound != null) {
			brokenAudioElement.clip = brokenSound;
		}
	}

	void Start ()
	{
		InitializeAudioElements ();

		getCrateRigidbody ();
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
		//if the crate has been broken, wait x seconds and then 
		if (broken) {
			if (timeToRemove > 0) {
				timeToRemove -= Time.deltaTime;
			} else {
				if (fadeBrokenPieces) {
					rendererPartsCount = rendererParts.Count;

					bool allPiecesFaded = true;

					//change the alpha of the color in every renderer component in the fragments of the crate
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

	//break this crate
	public void breakCrate ()
	{
		//disable the main mesh of the crate and create the copy with the fragments
		Vector3 originalRigidbodyVelocity = mainRigidbody.velocity;

		GetComponent<Collider> ().enabled = false;

		GetComponent<MeshRenderer> ().enabled = false;

		mainRigidbody.isKinematic = true;

		//if the option break in pieces is enabled, create the broken crate
		GameObject brokenCrateClone = (GameObject)Instantiate (brokenCrate, transform.position, transform.rotation);
		brokenCrateClone.transform.localScale = transform.localScale;
		brokenCrateClone.transform.SetParent (transform);

		brokenAudioElement.audioSource = brokenCrateClone.GetComponent<AudioSource> ();
		AudioPlayer.PlayOneShot (brokenAudioElement, gameObject);

		if (fadeBrokenPieces) {
			if (useCustomShader) {
				transparentShader = customShader;
			} else {
				if (transparentShader == null) {
					transparentShader = Shader.Find (defaultShaderName);
				}
			}
		}
			
		Component[] components = brokenCrateClone.GetComponentsInChildren (typeof(MeshRenderer));
		foreach (MeshRenderer child in components) {
			//add a box collider to every piece of the crate
			Rigidbody currentPartRigidbody = child.gameObject.AddComponent<Rigidbody> ();

			child.gameObject.AddComponent<MeshCollider> ().convex = true;

			if (fadeBrokenPieces) {
				//change the shader of the fragments to fade them
				int materialsLength = child.materials.Length;

				for (j = 0; j < child.materials.Length; j++) {
					Material temporalMaterial = child.materials [j];

					temporalMaterial.shader = transparentShader;
					rendererParts.Add (temporalMaterial);
				}
			}

			if (originalRigidbodyVelocity.magnitude > minVelocityToBreak) {
				currentPartRigidbody.AddForce (originalRigidbodyVelocity, forceMode);
			} else {
				currentPartRigidbody.AddForce ((child.transform.position - transform.position) * breakForce, forceMode);
			}
		}
			
		//kill the health component, to call the functions when the object health is 0
		if (!applyDamage.checkIfDead (gameObject)) {
			applyDamage.killCharacter (gameObject);
		}

		//search the player in case he had grabbed the crate when it exploded
		broken = true;

		mainCoroutine = StartCoroutine (updateCoroutine ());

		//if the object is being carried by the player, make him drop it
		GKC_Utils.checkDropObject (gameObject);
	}

	//if the player grabs this crate, disable its the option to break it
	public void crateCanBeBrokenState (bool state)
	{
		canBeBroken = state;
	}

	//if the crate collides at enough speed, break it
	void OnCollisionEnter (Collision col)
	{
		if (mainRigidbody != null && mainRigidbody.velocity.magnitude > minVelocityToBreak && canBeBroken && !broken) {
			breakCrate ();
		}
	}

	public void getCrateRigidbody ()
	{
		if (mainRigidbody == null) {
			mainRigidbody = GetComponent<Rigidbody> ();
		}
	}

	public void setBarrelRigidbody (Rigidbody rigidbodyToUse)
	{
		mainRigidbody = rigidbodyToUse;
	}
}