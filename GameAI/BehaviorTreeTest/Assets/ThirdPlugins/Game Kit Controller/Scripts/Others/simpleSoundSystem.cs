using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine;

public class simpleSoundSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool soundsEnabled = true;

	[Space]
	[Header ("Sound List Settings")]
	[Space]

	public List<soundCategoryInfo> soundCategoryInfoList = new List<soundCategoryInfo> ();

	[Space]
	[Header ("Components")]
	[Space]

	public AudioSource mainAudioSource;


	private void InitializeAudioElements ()
	{
		foreach (var soundCategoryInfo in soundCategoryInfoList) {
			foreach (var soundInfo in soundCategoryInfo.soundInfoList) {
				if (soundInfo.soundClip != null) {
					soundInfo.soundClipAudioElement.clip = soundInfo.soundClip;
				}

				if (mainAudioSource != null) {
					soundInfo.soundClipAudioElement.audioSource = mainAudioSource;
				}
			}
		}
	}

	private void Start ()
	{
		InitializeAudioElements ();
	}

	public void playRandomSound ()
	{
		if (!soundsEnabled) {
			return;
		}

		int randomCategoryIndex = Random.Range (0, soundCategoryInfoList.Count);

		int randomSound = Random.Range (0, soundCategoryInfoList [randomCategoryIndex].soundInfoList.Count);

		AudioPlayer.PlayOneShot (soundCategoryInfoList [randomCategoryIndex].soundInfoList [randomSound].soundClipAudioElement, gameObject);
	}

	public void playRandomSoundFromCategoryByName (string categoryName)
	{
		int currentIndex = soundCategoryInfoList.FindIndex (s => s.categoryName == categoryName);

		if (currentIndex > -1) {
			int randomSound = Random.Range (0, soundCategoryInfoList [currentIndex].soundInfoList.Count);

			AudioPlayer.PlayOneShot (soundCategoryInfoList [currentIndex].soundInfoList [randomSound].soundClipAudioElement, gameObject);
		}
	}


	[System.Serializable]
	public class soundCategoryInfo
	{
		public string categoryName;

		public List<soundInfo> soundInfoList = new List<soundInfo> ();
	}

	[System.Serializable]
	public class soundInfo
	{
		public string Name;

		public AudioClip soundClip;
		public AudioElement soundClipAudioElement;
	}
}
