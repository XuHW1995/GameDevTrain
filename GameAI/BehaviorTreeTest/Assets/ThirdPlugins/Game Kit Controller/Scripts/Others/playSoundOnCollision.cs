using UnityEngine;
using System.Collections;
using GameKitController.Audio;

[RequireComponent (typeof(AudioElementHolder))]
public class playSoundOnCollision : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool useMinSpeedForSound;
	public float minSpeedForSound;

	[Space]
	[Header ("Components")]
	[Space]

	public AudioSource mainAudioSource;
	public AudioElementHolder audioElementHolder;


	private void InitializeAudioElements ()
	{
		if (mainAudioSource == null) {
			mainAudioSource = GetComponent<AudioSource> ();
		}

		if (audioElementHolder == null)
			audioElementHolder = GetComponent<AudioElementHolder> ();

		if (mainAudioSource != null) {
			if (audioElementHolder != null) {
				audioElementHolder.audioElement.audioSource = mainAudioSource;
			}
		}
	}

	private void Start ()
	{
		InitializeAudioElements ();
	}

	void OnCollisionEnter (Collision col)
	{
		if (useMinSpeedForSound) {
			if (col.relativeVelocity.magnitude > minSpeedForSound) {
				playSound ();
			}
		} else {
			playSound ();
		}
	}

	public void playSound ()
	{
		if (audioElementHolder != null) {
			if (audioElementHolder.audioElement != null) {
				AudioPlayer.Play (audioElementHolder.audioElement, gameObject);
			}
		}
	}
}