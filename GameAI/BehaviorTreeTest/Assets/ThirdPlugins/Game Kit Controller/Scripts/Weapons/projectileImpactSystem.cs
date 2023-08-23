using System.Collections;
using System.Collections.Generic;
using GameKitController.Audio;
using UnityEngine;

public class projectileImpactSystem : MonoBehaviour
{
	public AudioSource mainAudioSource;
	public destroyGameObject mainDestroyGameObject;

	public void activateImpactElements (Vector3 newPosition, AudioElement impactSoundEffect)
	{
//		print ("----Activate impact elements " + gameObject.activeSelf + " " + mainDestroyGameObject.destroyCoroutineActive);

		transform.position = newPosition;

		if (mainAudioSource != null) {
			impactSoundEffect.audioSource = mainAudioSource;
		}
//		print (impactSoundEffect.length);

		if (impactSoundEffect.clip != null) {
			mainDestroyGameObject.setTimer (impactSoundEffect.clip.length + 0.2f);
		}

		if (gameObject.activeSelf) {

			mainDestroyGameObject.checkToDestroyObjectInTime (false);
		} else {
			gameObject.SetActive (true);
		}

		GKC_Utils.checkAudioSourcePitch (mainAudioSource);
		AudioPlayer.Play (impactSoundEffect, gameObject);

		decalManager.setImpactSoundParent (transform);
	}

	public void changeDestroyForSetActiveFunction (bool state)
	{
		mainDestroyGameObject.changeDestroyForSetActiveFunction (state);
	}

	public void setSendObjectToPoolSystemToDisableState (bool state)
	{
		mainDestroyGameObject.setSendObjectToPoolSystemToDisableState (state);
	}
}
