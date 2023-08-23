using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine;

public class playSoundOnTriggerEnter : MonoBehaviour
{
	public AudioClip soundToPlay;
	public AudioElement soundToPlayAudioElement;
	public List<string> tagListToCheck = new List<string> ();
	public bool playingSound;
	public AudioSource mainAudioSource;
	public bool useEventTriggerSystem;

	private void InitializeAudioElements ()
	{
		if (soundToPlay != null) {
			soundToPlayAudioElement.clip = soundToPlay;
		}

		if (mainAudioSource != null) {
			soundToPlayAudioElement.audioSource = mainAudioSource;
		}
	}

	void Start ()
	{
		InitializeAudioElements ();
	}

	void Update ()
	{
		if (playingSound && !mainAudioSource.isPlaying) {
			playingSound = false;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (useEventTriggerSystem) {
			return;
		}

		if (playingSound) {
			return;
		}

		if (tagListToCheck.Contains (col.gameObject.tag)) {
			playSound ();
		}
	}

	public void playSound ()
	{
		if (playingSound) {
			return;
		}

		AudioPlayer.PlayOneShot (soundToPlayAudioElement, gameObject);

		playingSound = true;
	}
}
