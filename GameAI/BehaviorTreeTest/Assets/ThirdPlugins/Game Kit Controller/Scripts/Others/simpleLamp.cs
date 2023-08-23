using UnityEngine;
using System.Collections;
using GameKitController.Audio;

public class simpleLamp : MonoBehaviour
{
	public GameObject lampLight;
	public bool hasLamp;
	public AudioClip switchSound;
	public AudioElement switchAudioElement;
	AudioSource audioSource;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();

		if (switchSound)
			switchAudioElement.clip = switchSound;

		if (audioSource)
			switchAudioElement.audioSource = audioSource;
	}

	public void lampPlaced ()
	{
		hasLamp = true;
	}

	public void activateDevice ()
	{
		if (hasLamp) {
			AudioPlayer.PlayOneShot (switchAudioElement, gameObject);
			lampLight.SetActive (!lampLight.activeSelf);
		}
	}
}
