using UnityEngine;

namespace GameKitController.Audio
{
	public class AudioPlayer
	{
		public static void Play (AudioElement audioElement, GameObject gameObj)
		{
			if (audioElement == null)
				return;

			if (audioElement.audioPlayMethod == AudioPlayMethod.UnityAudio) {
				GetComponentAudioSourceIfNull (audioElement, gameObj);

				if (audioElement.audioSource == null)
					return;

				if (audioElement.clip != null)
					audioElement.audioSource.clip = audioElement.clip;

				if (audioElement.audioSource.enabled)
					audioElement.audioSource.Play ();
			} else if (audioElement.audioPlayMethod == AudioPlayMethod.External) {
				var externalAudioPlayer = AudioManager.Instance.GetExternalAudioPlayer ();

				if (externalAudioPlayer == null)
					return;

				externalAudioPlayer.Play (audioElement, gameObj);
			}
		}

		public static void PlayOneShot (AudioElement audioElement, GameObject gameObj, float volumeScale = 1.0f)
		{
			if (audioElement == null)
				return;

			if (audioElement.audioPlayMethod == AudioPlayMethod.UnityAudio) {
				GetComponentAudioSourceIfNull (audioElement, gameObj);

				if (audioElement.audioSource == null || audioElement.clip == null)
					return;

				if (audioElement.audioSource.enabled)
					audioElement.audioSource.PlayOneShot (audioElement.clip, volumeScale);
			} else if (audioElement.audioPlayMethod == AudioPlayMethod.External) {
				var externalAudioPlayer = AudioManager.Instance.GetExternalAudioPlayer ();

				if (externalAudioPlayer == null)
					return;

				externalAudioPlayer.PlayOneShot (audioElement, gameObj, volumeScale);
			}
		}

		public static void Stop (AudioElement audioElement, GameObject gameObj)
		{
			if (audioElement == null)
				return;

			if (audioElement.audioPlayMethod == AudioPlayMethod.UnityAudio) {
				GetComponentAudioSourceIfNull (audioElement, gameObj);

				if (audioElement.audioSource == null)
					return;

				audioElement.audioSource.Stop ();
			} else if (audioElement.audioPlayMethod == AudioPlayMethod.External) {
				var externalAudioPlayer = AudioManager.Instance.GetExternalAudioPlayer ();

				if (externalAudioPlayer == null)
					return;

				externalAudioPlayer.Stop (audioElement, gameObj);
			}
		}

		public static void Pause (AudioElement audioElement, GameObject gameObj)
		{
			if (audioElement == null)
				return;

			if (audioElement.audioPlayMethod == AudioPlayMethod.UnityAudio) {
				GetComponentAudioSourceIfNull (audioElement, gameObj);

				if (audioElement.audioSource == null)
					return;

				audioElement.audioSource.Pause ();
			} else if (audioElement.audioPlayMethod == AudioPlayMethod.External) {
				var externalAudioPlayer = AudioManager.Instance.GetExternalAudioPlayer ();

				if (externalAudioPlayer == null)
					return;

				externalAudioPlayer.Pause (audioElement, gameObj);
			}
		}

		public static void UnPause (AudioElement audioElement, GameObject gameObj)
		{
			if (audioElement == null)
				return;

			if (audioElement.audioPlayMethod == AudioPlayMethod.UnityAudio) {
				GetComponentAudioSourceIfNull (audioElement, gameObj);

				if (audioElement.audioSource == null)
					return;

				audioElement.audioSource.UnPause ();
			} else if (audioElement.audioPlayMethod == AudioPlayMethod.External) {
				var externalAudioPlayer = AudioManager.Instance.GetExternalAudioPlayer ();

				if (externalAudioPlayer == null)
					return;

				externalAudioPlayer.UnPause (audioElement, gameObj);
			}
		}

		private static void GetComponentAudioSourceIfNull (AudioElement audioElement, GameObject gameObj)
		{
			if (audioElement.audioSource == null)
				audioElement.audioSource = gameObj.GetComponent<AudioSource> ();
		}
	}
}
