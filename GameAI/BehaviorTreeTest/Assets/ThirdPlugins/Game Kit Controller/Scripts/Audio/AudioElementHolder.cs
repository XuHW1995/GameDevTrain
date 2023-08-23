using UnityEngine;

namespace GameKitController.Audio
{
	public class AudioElementHolder : MonoBehaviour
	{
		public AudioElement audioElement;

		public void Play ()
		{
			AudioPlayer.Play (audioElement, gameObject);
		}

		public void PlayOneShot (float volumeScale)
		{
			AudioPlayer.PlayOneShot (audioElement, gameObject, volumeScale);
		}

		public void PlayOneShot ()
		{
			PlayOneShot (1.0f);
		}

		public void Stop ()
		{
			AudioPlayer.Stop (audioElement, gameObject);
		}

		public void Pause ()
		{
			AudioPlayer.Pause (audioElement, gameObject);
		}

		public void UnPause ()
		{
			AudioPlayer.UnPause (audioElement, gameObject);
		}
	}
}
