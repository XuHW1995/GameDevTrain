using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideBodyPartOnCharacterSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool hideBodyPartEnabled = true;

	public bool useTimeBullet = true;
	public float timeBulletDuration = 3;
	public float timeScale = 0.2f;

	[Space]
	[Header ("Debug")]
	[Space]

	public Transform currentBodyPartToHide;

	[Space]
	[Header ("Components")]
	[Space]

	public Transform characterTransform;


	public void hideBodyPartFromMountPoint (string mountPointPartName)
	{
		if (characterTransform != null) {
			Transform newBodyPart = GKC_Utils.getHumanoBoneMountPointTransformByName (mountPointPartName, characterTransform);

			if (newBodyPart != null) {
				hideBodyPart (newBodyPart);
			}
		}
	}

	public void hideBodyPartFromMountPointWithoutBulletTimeCheck (string mountPointPartName)
	{
		if (characterTransform != null) {
			currentBodyPartToHide = GKC_Utils.getHumanoBoneMountPointTransformByName (mountPointPartName, characterTransform);

			if (currentBodyPartToHide != null) {
				setBodyPartScale ();
			}
		}
	}

	public void hideBodyPart ()
	{
		if (useTimeBullet) {
			GKC_Utils.activateTimeBulletXSeconds (timeBulletDuration, timeScale);
		}

		setBodyPartScale ();
	}

	public void hideBodyPart (Transform newBodyPart)
	{
		currentBodyPartToHide = newBodyPart;

		hideBodyPart ();
	}

	public void hideBodyPartWithoutBulletTimeCheck ()
	{
		setBodyPartScale ();
	}

	public void hideBodyPartWithoutBulletTimeCheck (Transform newBodyPart)
	{
		currentBodyPartToHide = newBodyPart;

		setBodyPartScale ();
	}

	public void setBodyPartScale ()
	{
		if (!hideBodyPartEnabled) {
			return;
		}

		if (currentBodyPartToHide != null) {
			currentBodyPartToHide.localScale = Vector3.zero;
		}
	}

	public void setUseTimeBulletValue (bool state)
	{
		useTimeBullet = state;
	}

	public void setHideBodyPartEnabledState (bool state)
	{
		hideBodyPartEnabled = state;
	}

	public void setUseTimeBulletValueFromEditor (bool state)
	{
		setUseTimeBulletValue (state);

		updateComponent ();
	}

	public void setHideBodyPartEnabledStateFromEditor (bool state)
	{
		setHideBodyPartEnabledState (state);

		updateComponent ();
	}

	void updateComponent ()
	{
		GKC_Utils.updateComponent (this);
	}
}
