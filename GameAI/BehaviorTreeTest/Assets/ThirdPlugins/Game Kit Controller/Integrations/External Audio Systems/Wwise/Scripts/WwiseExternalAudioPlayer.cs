#if GAME_KIT_CONTROLLER_USE_WWISE
using UnityEngine;

namespace GameKitController.Audio
{
	public class WwiseExternalAudioPlayer : MonoBehaviour, IExternalAudioPlayer
	{
		public void Play(AudioElement audioElement, GameObject gameObj)
		{
			AkSoundEngine.PostEvent(audioElement.audioEventName, gameObj);
		}

		public void PlayOneShot(AudioElement audioElement, GameObject gameObj, float volumeScale = 1.0f)
		{
			AkSoundEngine.PostEvent(audioElement.audioEventName, gameObj);
		}

		public void Stop(AudioElement audioElement, GameObject gameObj)
		{
			AkSoundEngine.StopAll(gameObj);
		}

		public void Pause(AudioElement audioElement, GameObject gameObj)
		{
			AkSoundEngine.StopAll(gameObj);
		}

		public void UnPause(AudioElement audioElement, GameObject gameObj)
		{
			AkSoundEngine.PostEvent(audioElement.audioEventName, gameObj);
		}
	}
}
#endif
