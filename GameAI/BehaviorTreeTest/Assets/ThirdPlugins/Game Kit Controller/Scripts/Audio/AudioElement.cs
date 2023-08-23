using UnityEngine;

namespace GameKitController.Audio
{
	public enum AudioPlayMethod
	{
		None,
		UnityAudio,
		External,
	}

	[System.Serializable]
	public class AudioElement
	{
		public AudioPlayMethod audioPlayMethod = AudioPlayMethod.UnityAudio;

		public string audioEventName;

		public AudioClip clip;

		public string audioSourceName;
		public AudioSource audioSource;
	}
}
