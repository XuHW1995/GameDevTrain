using UnityEngine;

namespace GameKitController.Audio
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; set; }

		#if UNITY_2019_4_9_OR_NEWER
		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.SubsystemRegistration)]
		#endif

		private static void Init ()
		{
			Instance = null;
		}

		private void Awake ()
		{
			if (Instance == null)
				Instance = this;
			else if (Instance != this)
				Destroy (this);
		}

		public IExternalAudioPlayer GetExternalAudioPlayer ()
		{
			return GetComponent<IExternalAudioPlayer> ();
		}
	}
}
