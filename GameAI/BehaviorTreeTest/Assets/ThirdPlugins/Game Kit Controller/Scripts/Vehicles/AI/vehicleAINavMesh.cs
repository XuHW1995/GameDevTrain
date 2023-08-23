﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

public class vehicleAINavMesh : AINavMesh
{
	[Space]
	[Header ("AI Navmesh Custom Settings")]
	[Space]

	public bool vehicleAIActive = true;

	public bool useNavmeshActive;

	public bool autoBrakeOnRemoveTarget;

	[Space]

	// This script provides input to the car controller in the same way that the user control script does.
	// As such, it is really 'driving' the car, with no special physics or animation tricks to make the car behave properly.

	// "wandering" is used to give the cars a more human, less robotic feel. They can waver slightly
	// in speed and direction while driving towards their target.

	[SerializeField] [Range (0, 1)]  float m_CautiousSpeedFactor = 0.05f;
	// percentage of max speed to use when being maximally cautious
	[SerializeField] [Range (0, 180)]  float m_CautiousMaxAngle = 50f;
	// angle of approaching corner to treat as warranting maximum caution
	[SerializeField]  float m_CautiousMaxDistance = 100f;
	// distance at which distance-based cautiousness begins
	[SerializeField]  float m_CautiousAngularVelocityFactor = 30f;
	// how cautious the AI should be when considering its own current angular velocity (i.e. easing off acceleration if spinning!)
	[SerializeField]  float m_SteerSensitivity = 0.05f;
	// how sensitively the AI uses steering input to turn to the desired direction
	[SerializeField]  float m_AccelSensitivity = 0.04f;
	// How sensitively the AI uses the accelerator to reach the current desired speed
	[SerializeField]  float m_BrakeSensitivity = 1f;
	// How sensitively the AI uses the brake to reach the current desired speed
	[SerializeField]  float m_LateralWanderDistance = 3f;
	// how far the car will wander laterally towards its target
	[SerializeField]  float m_LateralWanderSpeed = 0.1f;
	// how fast the lateral wandering will fluctuate
	[SerializeField] [Range (0, 1)]  float m_AccelWanderAmount = 0.1f;
	// how much the cars acceleration will wander
	[SerializeField]  float m_AccelWanderSpeed = 0.1f;
	// how fast the cars acceleration wandering will fluctuate
	[SerializeField]  BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance;
	// what should the AI consider when accelerating/braking?

	// whether the AI is currently actively driving or stopped.

	// 'target' the target object to aim for.
	[SerializeField]  bool m_StopWhenTargetReached;
	// should we stop driving when we reach the target?
	[SerializeField]  float m_ReachTargetThreshold = 2;
	// proximity to target to consider we 'reached' it, and stop driving.

	public float maxSpeed;

	[Space]
	[Header ("Debug")]
	[Space]

	public Vector2 currentInputValues;

	public bool driving;

	public bool usingTrackActive;

	[Space]
	[Header ("Components")]
	[Space]

	public inputActionManager mainInputActionManager;
	public Rigidbody m_Rigidbody;
	[SerializeField]  Transform m_Target;
	public Transform vehicleTransform;
	public vehicleHUDManager mainVehicleHudManager;

	float m_RandomPerlin;
	// A random value for the car to base its wander on (so that AI cars don't all wander in the same pattern)
	float m_AvoidOtherCarTime;
	// time until which to avoid the car we recently collided with
	float m_AvoidOtherCarSlowdown;
	// how much to slow down due to colliding with another car, whilst avoiding
	float m_AvoidPathOffset;
	// direction (-1 or 1) in which to offset path to avoid other car, whilst avoiding

	float currentSpeed;

	public enum BrakeCondition
	{
		NeverBrake,
		// the car simply accelerates at full throttle all the time.
		TargetDirectionDifference,
		// the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.
		TargetDistance,
		// the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
		// head for a stationary target and come to rest when it arrives there.
	}

	Vector3 currentVelocity;

	Vector2 moveInput;

	float cautiousnessRequired;
	float spinningAngle;

	void Awake ()
	{
		// give the random perlin a random value
		m_RandomPerlin = Random.value * 100;

		if (m_Rigidbody == null) {
			m_Rigidbody = GetComponent<Rigidbody> ();
		}

		if (vehicleTransform == null) {
			vehicleTransform = transform;
		}
	}

	public override void Start ()
	{
		base.Start ();

		if (gameObject.activeSelf) {
			mainVehicleHudManager.setUsedByAIState (true);
		}
	}

	void FixedUpdate ()
	{
		if (!vehicleAIActive) {
			return;
		}

		if (usingTrackActive) {
			updateVehicleTrack ();
		}
	}

	void updateVehicleTrack ()
	{			
		currentVelocity = m_Rigidbody.velocity;

		currentSpeed = currentVelocity.magnitude * 2.23693629f;

		if (m_Target == null || !driving) {
			// Car should not be moving,
			// use handbrake to stop
			currentInputValues = Vector2.zero;

			mainInputActionManager.overrideInputValues (currentInputValues, -1, 1, true);
		} else {
			Vector3 fwd = vehicleTransform.forward;

			if (currentVelocity.magnitude > maxSpeed * 0.1f) {
				fwd = currentVelocity;
			}

			float desiredSpeed = maxSpeed;

			// now it's time to decide if we should be slowing down...
			switch (m_BrakeCondition) {

			case BrakeCondition.TargetDirectionDifference:
				
				// the car will brake according to the upcoming change in direction of the target. Useful for route-based AI, slowing for corners.

				// check out the angle of our target compared to the current direction of the car
				float approachingCornerAngle = Vector3.Angle (m_Target.forward, fwd);

				// also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
				spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

				// if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
				cautiousnessRequired = Mathf.InverseLerp (0, m_CautiousMaxAngle, Mathf.Max (spinningAngle, approachingCornerAngle));

				desiredSpeed = Mathf.Lerp (maxSpeed, maxSpeed * m_CautiousSpeedFactor, cautiousnessRequired);

				break;

			case BrakeCondition.TargetDistance:
				
				// the car will brake as it approaches its target, regardless of the target's direction. Useful if you want the car to
				// head for a stationary target and come to rest when it arrives there.

				// check out the distance to target
				Vector3 delta = Vector3.zero;

				if (useNavmeshActive) {
					delta = new Vector3 (AIMoveInput.moveInput.x, 0, AIMoveInput.moveInput.z);
				} else {
					delta = m_Target.position - vehicleTransform.position;
				}
				
				float distanceCautiousFactor = Mathf.InverseLerp (m_CautiousMaxDistance, 0, delta.magnitude);

				// also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
				spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

				// if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
				cautiousnessRequired = Mathf.Max (Mathf.InverseLerp (0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);

				desiredSpeed = Mathf.Lerp (maxSpeed, maxSpeed * m_CautiousSpeedFactor, cautiousnessRequired);

				break;
			
			case BrakeCondition.NeverBrake:
				break;
			}

			// Evasive action due to collision with other cars:

			// our target position starts off as the 'real' target position
			Vector3 offsetTargetPos = m_Target.position;

			// if are we currently taking evasive action to prevent being stuck against another car:
			if (Time.time < m_AvoidOtherCarTime) {
				// slow down if necessary (if we were behind the other car when collision occured)
				desiredSpeed *= m_AvoidOtherCarSlowdown;

				// and veer towards the side of our path-to-target that is away from the other car
				offsetTargetPos += m_Target.right * m_AvoidPathOffset;
			} else {
				// no need for evasive action, we can just wander across the path-to-target in a random way,
				// which can help prevent AI from seeming too uniform and robotic in their driving
				offsetTargetPos += m_Target.right * ((Mathf.PerlinNoise (Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) * m_LateralWanderDistance);
			}

			// use different sensitivity depending on whether accelerating or braking:
			float accelBrakeSensitivity = (desiredSpeed < currentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;

			// decide the actual amount of accel/brake input to achieve desired speed.
			float accel = Mathf.Clamp ((desiredSpeed - currentSpeed) * accelBrakeSensitivity, -1, 1);

			// add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
			// i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
			accel *= (1 - m_AccelWanderAmount) + (Mathf.PerlinNoise (Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);

			// calculate the local-relative position of the target, to steer towards
			Vector3 localTarget = vehicleTransform.InverseTransformPoint (offsetTargetPos);

			// work out the local angle towards the target
			float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;

			// get the amount of steering needed to aim the car towards the target
			float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (currentSpeed);

			// feed input to the car controller.

			currentInputValues = new Vector2 (steer, accel);

			mainInputActionManager.overrideInputValues (currentInputValues, accel, 0, true);

			// if appropriate, stop driving when we're close enough to the target.
			if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold) {
				driving = false;
			}
		}
	}

	void OnCollisionStay (Collision col)
	{
		// detect collision against other cars, so that we can take evasive action
		if (col.rigidbody != null) {
			var otherAI = applyDamage.getVehicle (col.gameObject);

			if (otherAI != null) {
				// we'll take evasive action for 1 second
				m_AvoidOtherCarTime = Time.time + 1;

				// but who's in front?...
				if (Vector3.Angle (vehicleTransform.forward, otherAI.transform.position - vehicleTransform.position) < 90) {
					// the other ai is in front, so it is only good manners that we ought to brake...
					m_AvoidOtherCarSlowdown = 0.5f;
				} else {
					// we're in front! ain't slowing down for anybody...
					m_AvoidOtherCarSlowdown = 1;
				}

				// both cars should take evasive action by driving along an offset from the path centre,
				// away from the other car
				var otherCarLocalDelta = vehicleTransform.InverseTransformPoint (otherAI.transform.position);

				float otherCarAngle = Mathf.Atan2 (otherCarLocalDelta.x, otherCarLocalDelta.z);

				m_AvoidPathOffset = m_LateralWanderDistance * -Mathf.Sign (otherCarAngle);
			}
		}
	}

	public void enableOrDisableAIObject (bool state)
	{
		if (state) {
			if (!gameObject.activeSelf) {
				gameObject.SetActive (state);

				pauseAI (false);

				setUsedByAIState (state);
			}
		} else {
			if (gameObject.activeSelf) {
				pauseAI (true);

				gameObject.SetActive (state);

				setUsedByAIState (state);

				mainInputActionManager.overrideInputValues (Vector2.zero, -1, 1, false);
			}
		}
	}

	public void setUsedByAIState (bool state)
	{
		mainVehicleHudManager.setUsedByAIState (state);
	}

	public void SetTarget (Transform target)
	{
		m_Target = target;
		driving = true;
	}

	public void setDrivingState (bool state)
	{
		driving = state;
	}

	public void setUsingTrackActive (bool state)
	{
		usingTrackActive = state;
	}

	public void setVehicleAIActiveState (bool state)
	{
		vehicleAIActive = state;
	}

	public override void updateAIControllerInputValues ()
	{
		if (!usingTrackActive) {
			if (isFollowingTarget ()) {
				Vector3 fwd = vehicleTransform.forward;

				if (currentVelocity.magnitude > maxSpeed * 0.1f) {
					fwd = currentVelocity;
				}

				float desiredSpeed = maxSpeed;

				Vector3 delta = new Vector3 (AIMoveInput.moveInput.x, 0, AIMoveInput.moveInput.z);

				float distanceCautiousFactor = Mathf.InverseLerp (m_CautiousMaxDistance, 0, delta.magnitude);

				// also consider the current amount we're turning, multiplied up and then compared in the same way as an upcoming corner angle
				spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;

				// if it's different to our current angle, we need to be cautious (i.e. slow down) a certain amount
				cautiousnessRequired = Mathf.Max (Mathf.InverseLerp (0, m_CautiousMaxAngle, spinningAngle), distanceCautiousFactor);

				desiredSpeed = Mathf.Lerp (maxSpeed, maxSpeed * m_CautiousSpeedFactor, cautiousnessRequired);


				Vector3 lastPosition = vehicleTransform.position;

//				if (canReachCurrentTarget) {
				int pathCornersCount = pathCorners.Count;

				if (pathCornersCount > 0) {
					if (pathCornersCount > 1) {
						lastPosition = pathCorners [1];
					} else {
						lastPosition = pathCorners [0];
					}
				} else {
					if (currentTarget != null) {
						lastPosition = currentTarget.position;
					}
				}
//				} else {
//
//				}

				m_Target.position = lastPosition;
				m_Target.rotation = Quaternion.LookRotation (delta);

				// Evasive action due to collision with other cars:

				// our target position starts off as the 'real' target position
				Vector3 offsetTargetPos = m_Target.position;

				// if are we currently taking evasive action to prevent being stuck against another car:
				if (Time.time < m_AvoidOtherCarTime) {
					// slow down if necessary (if we were behind the other car when collision occured)
					desiredSpeed *= m_AvoidOtherCarSlowdown;

					// and veer towards the side of our path-to-target that is away from the other car
					offsetTargetPos += m_Target.right * m_AvoidPathOffset;
				} else {
					// no need for evasive action, we can just wander across the path-to-target in a random way,
					// which can help prevent AI from seeming too uniform and robotic in their driving
					offsetTargetPos += m_Target.right * ((Mathf.PerlinNoise (Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) * m_LateralWanderDistance);
				}

				// use different sensitivity depending on whether accelerating or braking:
				float accelBrakeSensitivity = (desiredSpeed < currentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;

				// decide the actual amount of accel/brake input to achieve desired speed.
				float accel = Mathf.Clamp ((desiredSpeed - currentSpeed) * accelBrakeSensitivity, -1, 1);

				// add acceleration 'wander', which also prevents AI from seeming too uniform and robotic in their driving
				// i.e. increasing the accel wander amount can introduce jostling and bumps between AI cars in a race
				accel *= (1 - m_AccelWanderAmount) + (Mathf.PerlinNoise (Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);

				// calculate the local-relative position of the target, to steer towards
				Vector3 localTarget = vehicleTransform.InverseTransformPoint (offsetTargetPos);

				// work out the local angle towards the target
				float targetAngle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;

				// get the amount of steering needed to aim the car towards the target
				float steer = Mathf.Clamp (targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign (currentSpeed);

				// feed input to the car controller.

				currentInputValues = new Vector2 (steer, accel);

				mainInputActionManager.overrideInputValues (currentInputValues, accel, 0, true);

				setOnGroundState (mainVehicleHudManager.isVehicleOnGround ());
			} else {
				currentInputValues = Vector2.zero;

				mainInputActionManager.overrideInputValues (currentInputValues, -1, 1, true);
			}
		}
	}

	public override void updateAICameraInputValues ()
	{
		if (!usingTrackActive) {

		}
	}

	public override void removeTarget ()
	{
		setTarget (null);

		if (autoBrakeOnRemoveTarget) {
			mainVehicleHudManager.activateAutoBrakeOnGetOff ();
		}
	}
}