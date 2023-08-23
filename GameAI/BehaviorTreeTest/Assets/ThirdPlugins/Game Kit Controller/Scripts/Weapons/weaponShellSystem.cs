using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine;

[RequireComponent (typeof(AudioElementHolder))]
public class weaponShellSystem : MonoBehaviour
{
	public Rigidbody mainRigidbody;
	public Collider mainCollider;
	public AudioSource mainAudioSource;
	public AudioElementHolder audioElementHolder;

	public bool addRandomRotationToShells = true;
	public Vector2 randomRotationXRange = new Vector2 (-20, 20);
	public Vector2 randomRotationYRange = new Vector2 (-20, 20);
	public Vector2 randomRotationZRange = new Vector2 (-20, 20);

	public float randomRotationMultiplier;

	private void Awake ()
	{
		InitializeAudioElements ();
	}

	private void InitializeAudioElements ()
	{
		if (audioElementHolder == null) {
			audioElementHolder = GetComponent<AudioElementHolder> ();
		}

		if (mainAudioSource != null) {
			if (audioElementHolder != null) {
				audioElementHolder.audioElement.audioSource = mainAudioSource;
			}
		}
	}

	public void setShellValues (Vector3 forceDirection, Collider playerCollider, AudioElement clipToUse)
	{
		mainRigidbody.AddForce (forceDirection);

		if (addRandomRotationToShells) {
			float randomRotationX = Random.Range (randomRotationXRange.x, randomRotationXRange.y);
			float randomRotationY = Random.Range (randomRotationYRange.x, randomRotationYRange.y);
			float randomRotationZ = Random.Range (randomRotationZRange.x, randomRotationZRange.y);
			Vector3 randomRotation = new Vector3 (randomRotationX, randomRotationY, randomRotationZ);

			mainRigidbody.AddTorque (randomRotation * randomRotationMultiplier);
		}

		if (playerCollider != null) {
			Physics.IgnoreCollision (playerCollider, mainCollider);
		}

		if (clipToUse != null) {
			if (audioElementHolder != null) {
				audioElementHolder.audioElement = clipToUse;

				if (mainAudioSource != null) {
					audioElementHolder.audioElement.audioSource = mainAudioSource;
				}
			}
		}
	}
}
