using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using GameKitController.Audio;

public class songsManager : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool loadSoundsInFolderEnabled = true;

	public string absolutePathBuild = ".";

	public string absolutePathEditor = "Music";

	[Space]
	[Header ("Internal Songs List")]
	[Space]

	public bool useInternalSongsList;
	public List<AudioClip> internalClips = new List<AudioClip> ();
	public List<AudioElement> internalAudioElements = new List<AudioElement> ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	public bool songsLoaded;

	public List<AudioClip> clips = new List<AudioClip> ();
	public List<AudioElement> audioElements = new List<AudioElement> ();

	[Space]
	[Header ("Components")]
	[Space]

	public gameManager gameSystemManager;

	List<string> validExtensions = new List<string> { ".ogg", ".wav" };

	string absolutePath;

	int numberOfSongs;

	void Start ()
	{
		InitializeAudioElements ();

		if (gameSystemManager == null) {
			gameSystemManager = FindObjectOfType<gameManager> ();
		}

		if (gameSystemManager != null) {
			if (gameSystemManager.isLoadGameEnabled () || loadSoundsInFolderEnabled) {
				if (Application.isEditor) {
					absolutePath = absolutePathEditor;
				} else {
					absolutePath = absolutePathBuild;
				}

				ReloadSounds ();
			}
		}
	}

	private void InitializeAudioElements ()
	{
		if (clips != null && clips.Count > 0) {
			audioElements = new List<AudioElement> ();

			foreach (var clip in clips) {
				audioElements.Add (new AudioElement { clip = clip });
			}
		}

		if (internalClips != null && internalClips.Count > 0) {
			internalAudioElements = new List<AudioElement> ();

			foreach (var clip in internalClips) {
				internalAudioElements.Add (new AudioElement { clip = clip });
			}
		}
	}

	public List<AudioElement> getSongsList ()
	{
		return audioElements;
	}

	void ReloadSounds ()
	{
		audioElements.Clear ();

		if (Directory.Exists (absolutePath)) {
			//print ("directory found");
			absolutePath += "/";

			// get all valid files
			System.IO.DirectoryInfo info = new System.IO.DirectoryInfo (absolutePath);
			var fileInfo = info.GetFiles ()
				.Where (f => IsValidFileType (f.Name))
				.ToArray ();

			numberOfSongs = fileInfo.Length;

			if (useInternalSongsList) {
				numberOfSongs += internalAudioElements.Count;
			}

			foreach (FileInfo s in fileInfo) {
				StartCoroutine (LoadFile (s.FullName));
			}
		} else {
			if (showDebugPrint) {
				print ("Directory with song files doesn't exist. If you want to use the radio system, place some .wav files on the folder " + absolutePathEditor + " inside the project folder.");
			}
		}

		if (useInternalSongsList) {
			audioElements.AddRange (internalAudioElements);
		}
	}

	bool IsValidFileType (string fileName)
	{
		return validExtensions.Contains (Path.GetExtension (fileName));
		// Alternatively, you could go fileName.SubString(fileName.LastIndexOf('.') + 1); that way you don't need the '.' when you add your extensions
	}

	IEnumerator LoadFile (string path)
	{
		WWW www = new WWW ("file://" + path);
		//	print ("loading " + path);

		AudioClip clip = www.GetAudioClip (false);

		while (clip.loadState != AudioDataLoadState.Loaded) {
			yield return www;
		}

		//print ("done loading");
		clip.name = Path.GetFileName (path);

		audioElements.Add (new AudioElement { clip = clip });

		if (audioElements.Count == numberOfSongs) {
			songsLoaded = true;
		}
	}

	public bool allSongsLoaded ()
	{
		return songsLoaded;
	}
}