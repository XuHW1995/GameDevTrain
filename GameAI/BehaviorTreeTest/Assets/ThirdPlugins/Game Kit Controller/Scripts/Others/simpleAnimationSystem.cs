﻿using UnityEngine;
using System.Collections;

public class simpleAnimationSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string animationName;
	public float forwardSpeed = 1;
	public float backwardSpeed = 1;

	public bool resetAnimationOnEnable;
	public bool resetAnimationOnDisable;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool playAnimation;
	public bool playingAnimation;
	public bool mainAnimationLocated;

	[Space]
	[Header ("Components")]
	[Space]

	public Animation mainAnimation;

	void Start ()
	{
		if (mainAnimation == null) {
			mainAnimation = GetComponent<Animation> ();
		}

		mainAnimationLocated = mainAnimation != null;
	}

	void OnEnable ()
	{
		if (resetAnimationOnEnable) {
			if (mainAnimation != null) {
				mainAnimation.Rewind ();
			}
		}
	}

	void OnDisable ()
	{
		if (resetAnimationOnDisable) {
			if (mainAnimation != null) {
				mainAnimation.Rewind ();
			}
		}
	}

	void Update ()
	{
		if (playAnimation) {
			if (mainAnimationLocated) {
				if (!mainAnimation.IsPlaying (animationName)) {
					if (!playingAnimation) {
						mainAnimation.CrossFade (animationName);

						playingAnimation = true;
					} else {
						playingAnimation = false;
						playAnimation = false;
					}
				}
			}
		}
	}

	public void playForwardAnimation ()
	{
		if (mainAnimation != null) {
			playAnimation = true;

			mainAnimation [animationName].speed = forwardSpeed;
		}
	}

	public void playBackwardAnimation ()
	{
		if (mainAnimation != null) {
			playAnimation = true;

			mainAnimation [animationName].speed = -backwardSpeed; 

			if (!playingAnimation) {
				mainAnimation [animationName].time = mainAnimation [animationName].length;
			}
		}
	}

	public void playForwardNewAnimation (string newName)
	{
		animationName = newName;

		playForwardAnimation ();
	}

	public void playBackwardNewAnimation (string newName)
	{
		animationName = newName;

		playBackwardAnimation ();
	}

	public void playAnimationForwardIfAlreadyInProcess ()
	{
		if (playingAnimation) {
			playForwardAnimation ();
		}
	}

	public void playAnimationBackwardIfAlreadyInProcess ()
	{
		if (playingAnimation) {
			playBackwardAnimation ();
		}
	}

	public void rewindAnimation ()
	{
		playBackwardAnimation ();

		if (playingAnimation || playAnimation) {
			playingAnimation = false;
		}

		playAnimation = true;
	}
}
