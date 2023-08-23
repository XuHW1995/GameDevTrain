using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class randomEventSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool randomEventsEnabled = true;

	public bool useSameIndexValue;
	public int sameIndexValueToUse;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Random Events Settings")]
	[Space]

	public List<randomEventInfo> randomEventInfoList = new List<randomEventInfo> ();


	randomEventInfo currentEventInfo;


	public void callRandomEvent ()
	{
		if (!randomEventsEnabled) {
			return;
		}

		int randomIndex = Random.Range (0, randomEventInfoList.Count);

		if (useSameIndexValue) {
			randomIndex = sameIndexValueToUse;
		}

		if (randomIndex <= randomEventInfoList.Count - 1) {
			currentEventInfo = randomEventInfoList [randomIndex];

			if (currentEventInfo.eventEnabled) {
				if (showDebugPrint) {
					print (currentEventInfo.Name);
				}

				bool activateEventResult = true;

				if (currentEventInfo.useProbabilityToActivateEvent) {
					float currentProbability = Random.Range (0, 100);

					if (currentProbability > currentEventInfo.probabilityToActivateEvent) {
						activateEventResult = false;
					}
				}

				if (activateEventResult) {
					currentEventInfo.eventToActive.Invoke ();

					if (currentEventInfo.disableEventAfterActivation) {
						currentEventInfo.eventEnabled = false;
					}
				}
			}
		}
	}

	[System.Serializable]
	public class randomEventInfo
	{
		public string Name;

		public bool eventEnabled = true;

		public bool disableEventAfterActivation;
	
		[Space]

		public bool useProbabilityToActivateEvent;

		[Range (0, 100)] public float probabilityToActivateEvent = 0;

		[Space]

		public UnityEvent eventToActive;
	}
}
