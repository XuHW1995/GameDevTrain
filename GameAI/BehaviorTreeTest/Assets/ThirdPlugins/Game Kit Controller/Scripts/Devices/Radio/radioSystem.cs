using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using GameKitController.Audio;

public class radioSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public float radioVolume;

	public bool playSongsOnStart;

	public bool playSongsOnActive;

	public bool playSongsRandomly;
	public bool repeatList;

	public string mainManagerName = "Songs Manager";

	[Space]
	[Header ("Debug")]
	[Space]

	public bool playingSong;
	public bool songPaused;
	public float currentSongLength;

	public bool radioActive;

	public bool movingSongLenghtSlider;

	public List<AudioClip> clips = new List<AudioClip> ();
	public List<AudioElement> audioElements = new List<AudioElement> ();

	public int currentIndex = 0;
	 
	[Space]
	[Header ("Components")]
	[Space]

	public Scrollbar volumeScrollbar;

	public Text currentSongNameText;

	public Slider songLenghtSlider;

	public GameObject playButton;
	public GameObject pauseButton;

	public GameObject songListContent;
	public GameObject songListElementParent;
	public GameObject songListElement;

	public AudioSource source;

	songsManager mainSongsManager;

	bool songsLoaded;

	bool radioCanBeUsed = true;

	public AudioElement _currentSongAudioElement = new AudioElement ();

	private void InitializeAudioElements ()
	{
		if (source == null) {
			source = gameObject.AddComponent<AudioSource> ();
		}

		if (clips != null && clips.Count > 0) {
			audioElements = new List<AudioElement> ();

			foreach (var clip in clips) {
				audioElements.Add (new AudioElement { clip = clip });
			}
		}

		foreach (var audioElement in audioElements) {
			if (source != null) {
				audioElement.audioSource = source;
			}
		}

		if (source != null) {
			_currentSongAudioElement.audioSource = source;
		}
	}

	void Start ()
	{
		songListElement.SetActive (false);

		InitializeAudioElements ();

		volumeScrollbar.value = radioVolume;

		if (mainSongsManager == null) {
			mainSongsManager = FindObjectOfType<songsManager> ();
		}

		if (mainSongsManager == null) {
			GKC_Utils.instantiateMainManagerOnSceneWithType (mainManagerName, typeof(songsManager));

			mainSongsManager = FindObjectOfType<songsManager> ();
		}

		if (mainSongsManager == null) {
			radioCanBeUsed = false;
		}

		currentSongNameText.text = "...";
	}

	void Update ()
	{
		if (!radioCanBeUsed) {
			return;
		}

		if (radioActive) {
			if (playingSong) {
				if (!movingSongLenghtSlider) {
					songLenghtSlider.value = _currentSongAudioElement.audioSource.time / currentSongLength;
				}

				if (!songPaused) {
					if ((_currentSongAudioElement.audioSource.time / currentSongLength) > 0.99f) {
						if (currentIndex == audioElements.Count - 1) {
							if (repeatList) {
								setNextSong ();
							} else {
								stopCurrentSong ();

								currentIndex = 0;

								setPlayPauseButtonState (true);
							}
						} else {
							if (playSongsRandomly) {
								setRandomSong ();
							} else {
								setNextSong ();
							}
						}
					}
				}
			}

			if (!songsLoaded) {
				if (mainSongsManager.allSongsLoaded ()) {
					getSongsList ();

					songsLoaded = true;
				}
			}
		}

		if (playSongsOnStart) {
			if (songsLoaded) {
				setRadioActiveState (true);

				playSongsOnStart = false;
			}
		}
	}

	public void playCurrentSong ()
	{
		if (playingSong) {
			AudioPlayer.Play (_currentSongAudioElement, gameObject);

			currentSongLength = _currentSongAudioElement.clip.length;

			songLenghtSlider.value = _currentSongAudioElement.audioSource.time / currentSongLength;
		} else {
			PlayCurrent ();
		}

		songPaused = false;
	}

	public void stopCurrentSong ()
	{
		AudioPlayer.Stop (_currentSongAudioElement, gameObject);

		playingSong = false;

		songLenghtSlider.value = 0;
	}

	public void pauseCurrentSong ()
	{
		songPaused = true;

		AudioPlayer.Pause (_currentSongAudioElement, gameObject);
	}

	public void setNextSong ()
	{
		if (audioElements.Count == 0) {
			return;
		}
		
		if (playSongsRandomly) {
			setRandomSongIndex ();
		} else {
			currentIndex = (currentIndex + 1) % audioElements.Count; 
		}

		setPlayPauseButtonState (false);

		PlayCurrent ();
	}

	public void setPreviousSong ()
	{
		if (audioElements.Count == 0) {
			return;
		}
		
		if (playSongsRandomly) {
			setRandomSongIndex ();
		} else {
			currentIndex--;
			if (currentIndex < 0) {
				currentIndex = audioElements.Count - 1;
			}
		}

		setPlayPauseButtonState (false);

		PlayCurrent ();
	}

	public void setRandomSong ()
	{
		setRandomSongIndex ();

		PlayCurrent ();
	}

	public void setRandomSongIndex ()
	{
		int nextIndex = 0;

		int loop = 0;

		while (nextIndex == currentIndex) {
			nextIndex = (int)UnityEngine.Random.Range (0, audioElements.Count);

			loop++;

			if (loop > 100) {
				print ("loop error");

				return;
			}
		}

		currentIndex = nextIndex;
	}

	public void setRadioVolume ()
	{
		radioVolume = volumeScrollbar.value;

		_currentSongAudioElement.audioSource.volume = radioVolume;
	}

	void PlayCurrent ()
	{
		if (audioElements.Count <= 0) {
			return;
		}

		_currentSongAudioElement = audioElements [currentIndex];

		_currentSongAudioElement.audioSource = source;

		_currentSongAudioElement.audioSource.time = 0;

		AudioPlayer.Play (_currentSongAudioElement, gameObject);

		string songName = audioElements [currentIndex].clip.name;

		int extensionIndex = songName.IndexOf (".");

		if (extensionIndex > -1) {
			songName = songName.Substring (0, extensionIndex);
		}

		currentSongNameText.text = songName;

		playingSong = true;

		currentSongLength = _currentSongAudioElement.clip.length;

		songLenghtSlider.value = _currentSongAudioElement.audioSource.time / currentSongLength;
	}

	public void setPlaySongsRandomlyState (bool state)
	{
		playSongsRandomly = state;
	}

	public void setRepeatListState (bool state)
	{
		repeatList = state;
	}

	public void setMovingSongLenghtSliderState (bool state)
	{
		if (playingSong) {
			movingSongLenghtSlider = state;
		}
	}

	public void setSongPart ()
	{
		if (playingSong) {
			_currentSongAudioElement.audioSource.time = _currentSongAudioElement.clip.length * songLenghtSlider.value;
		}
	}

	public void setRadioActiveState (bool state)
	{
		radioActive = state;

		if (radioActive) {
			if (playSongsOnActive) {
				if (playSongsRandomly) {
					setRandomSongIndex ();
				}

				PlayCurrent ();

				setPlayPauseButtonState (false);
			}
		} else {
			if (playingSong) {
				stopCurrentSong ();

				setPlayPauseButtonState (true);
			}

			songListContent.SetActive (false);
		}
	}

	public void setOnlyRadioActiveState (bool state)
	{
		radioActive = state;
	}

	public void setPlayPauseButtonState (bool state)
	{
		playButton.SetActive (state);

		pauseButton.SetActive (!state);
	}

	public void selectSongOnList (GameObject songButtonPressed)
	{
		string songNameToCheck = songButtonPressed.GetComponentInChildren<Text> ().text;

		for (int i = 0; i < audioElements.Count; i++) {
			if (audioElements [i].clip.name.Contains (songNameToCheck)) {
				currentIndex = i;

				PlayCurrent ();

				return;
			}
		}
	}

	public void getSongsList ()
	{
		//print (mainSongsManager.getSongsList ().Count);
		audioElements = mainSongsManager.getSongsList ();

		for (int i = 0; i < audioElements.Count; i++) {
			string songName = audioElements [i].clip.name;

			int extensionIndex = songName.IndexOf (".");

			if (extensionIndex > -1) {
				songName = songName.Substring (0, extensionIndex);
			}

			GameObject newSongListElement = (GameObject)Instantiate (songListElement, Vector3.zero, songListElement.transform.rotation);
			newSongListElement.SetActive (true);
			newSongListElement.transform.SetParent (songListElementParent.transform);
			newSongListElement.transform.localScale = Vector3.one;
			newSongListElement.transform.localPosition = Vector3.zero;
			newSongListElement.name = "Song List Element (" + songName + ")";

			newSongListElement.GetComponentInChildren<Text> ().text = songName;
		}
	}
}