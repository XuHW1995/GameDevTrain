using UnityEngine;

namespace GameKitController.Audio
{
	public interface IExternalAudioPlayer
	{
		void Play (AudioElement audioElement, GameObject gameObj);

		void PlayOneShot (AudioElement audioElement, GameObject gameObj, float volumeScale = 1.0f);

		void Stop (AudioElement audioElement, GameObject gameObj);

		void Pause (AudioElement audioElement, GameObject gameObj);

		void UnPause (AudioElement audioElement, GameObject gameObj);
	}
}
