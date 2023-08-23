using UnityEngine;

namespace GameKitController.Audio
{
	public class AudioElementHolderPlayer : MonoBehaviour
	{
		public AudioElementHolder audioElementHolder;

		public void Play ()
		{
			audioElementHolder.Play ();
		}

		public void PlayOneShot (float volumeScale)
		{
			audioElementHolder.PlayOneShot (volumeScale);
		}

		public void PlayOneShot ()
		{
			PlayOneShot (1.0f);
		}

		public void Stop ()
		{
			audioElementHolder.Stop ();
		}

		public void Pause ()
		{
			audioElementHolder.Pause ();
		}

		public void UnPause ()
		{
			audioElementHolder.UnPause ();
		}
	}
}
