using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class boatController : vehicleController
{
	[Header ("Custom Settings")]
	[Space]

	public otherVehicleParts vehicleParts;
	public vehicleSettings settings;

	[Space]
	[Header ("Main Settings")]
	[Space]

	public float moveForwardSpeedMultiplier;
	public float moveBackwardSpeedMultiplier;

	public float forwardMovementLerpSpeed = 0.8f;

	public float airControlAmount;
	public float airControlRotationAmount = 4;
	public float brakeForce = 5;

	public float rollRotationSpeed = 5;

	public bool rollRotationReversed;

	public float rotationExtraForwardMovement = 0.4f;

	public float rotationLerpSpeed = 0.6f;

	[Space]
	[Header ("Density Settings")]
	[Space]

	public bool changeDensityEnabled = true;

	public float densityChangeAmount = 0.05f;

	public Vector2 densityClampValues;

	[Space]
	[Header ("Orientation Settings")]
	[Space]

	public float maxAngleRangeForProperOrientationForces = 45;

	public bool autoOrientateUpOnWater;
	public float minAngleToAutoOrientateUpOnWater;
	public float autoOrientateUpForceAmountOnWater;

	public bool autoOrientateUpOnAir;
	public float minAngleToAutoOrientateUpOnAir;
	public float autoOrientateUpForceAmountOnAir;

	[Space]
	[Header ("Wind Settings")]
	[Space]

	public bool checkWindState;

	public bool sailActive;

	public bool ignoreWindForcesIfNotDrivingBoat;

	public bool addWindForceIfSailNotActive;

	public float windForceMultiplierIfSailNotActive = 0.5f;

	public Transform sailTransform;

	public Transform sailDirectionTransform;

	public float sailWindSpeedMultiplier;

	public float sailRotationSpeed;

	public float sailRotationClamp;
	public float sailRotationAmount;

	[Space]

	public bool checkBoatOrientationOnWind;

	public float maxAngleToReduceSpeedOnIncorrectWindDirection;

	public float reduceSpeedOnIncorrectWindDirectionMultiplier;

	public bool showWindDirectionObject;

	public float windDirectionRotationSpeed;

	[Space]
	[Header ("Events Settings")]
	[Space]

	public UnityEvent eventOnActiveWindDetection;
	public UnityEvent eventOnDeactiveWindDetection;

	public UnityEvent eventOnActiveSail;
	public UnityEvent eventOnDeactiveSail;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool anyOnGround;

	public float normalAngle;

	public bool properlyOrientedUp;

	public float windAirSpeedMultiplier;

	public float windAngle;

	public float regularWindAngle;

	public bool sailLookingRight;

	public Vector3 forceToApply;

	public float reducedSpeedOnIncorrectWindMultiplier;

	public float boatAngleWithWind;

	[Space]
	[Header ("Components")]
	[Space]

	public objectOnWater mainObjectOnWater;

	public Transform externalRotationForcePointTransform;

	public windOnObjectState mainWindOnObjectState;

	public Transform windDirectionTransform;


	float originalJumpPower;
	Vector3 moveInput;

	Transform vehicleCameraTransform;

	float rotationToApply;

	bool desiredJump;

	Vector3 externalRotationForcePoint;

	Vector3 totalForceToApply;

	float totalRotationToApply;

	float forwardInput;

	float currentSteeringWheelZAxisAngle;

	float currentSailYAxisAngle;

	public override void Awake ()
	{
		base.Awake ();

	}

	public override void Start ()
	{
		base.Start ();

		originalJumpPower = vehicleControllerSettings.jumpPower;

		vehicleCameraTransform = mainVehicleCameraController.transform;
	}

	public override void vehicleUpdate ()
	{
		base.vehicleUpdate ();

		forwardInput = verticalAxis;

		if (!checkWindState) {
			if (rotationToApply != 0) {
				if (forwardInput == 0) {
					forwardInput = rotationExtraForwardMovement;
				}
			}
		}

		moveInput = forwardInput * transform.forward;
		//+ horizontalAxis * vehicleCameraTransform.right;	

		moveInput = Vector3.ClampMagnitude (moveInput, 1f);

		if (vehicleParts.SteeringWheel != null) {
			currentSteeringWheelZAxisAngle = 
				Mathf.Lerp (currentSteeringWheelZAxisAngle, -vehicleParts.steerAngleLimit * horizontalAxis, Time.deltaTime * vehicleParts.steerRotationSpeed);

			if (Mathf.Abs (currentSteeringWheelZAxisAngle) > 0.01f) {

				vehicleParts.SteeringWheel.localEulerAngles = 
					new Vector3 (vehicleParts.SteeringWheel.localEulerAngles.x, vehicleParts.SteeringWheel.localEulerAngles.y, currentSteeringWheelZAxisAngle);
			}
		}

		if (checkWindState) {
			if (Mathf.Abs (rawAxisValues.y) > 0) {
				if (rawAxisValues.y > 0) {
					currentSailYAxisAngle += sailRotationAmount;
				} else {
					currentSailYAxisAngle -= sailRotationAmount;
				}
			}

			currentSailYAxisAngle = Mathf.Clamp (currentSailYAxisAngle, -sailRotationClamp, sailRotationClamp);

			Vector3 eulerTarget = Vector3.up * currentSailYAxisAngle;

			Quaternion targetRotation = Quaternion.Euler (eulerTarget);

			sailTransform.localRotation = Quaternion.Lerp (sailTransform.localRotation, targetRotation, sailRotationSpeed * Time.deltaTime);
		}
	}

	void FixedUpdate ()
	{
		fixedUpdateVehicle ();

		if (checkWindState) {
			bool isWindDetected = mainWindOnObjectState.isWindDetected ();

			Vector3 windDirection = Vector3.up;

			bool canCheckWindForcesResult = true;

			if (ignoreWindForcesIfNotDrivingBoat) {
				if (!isDrivingActive ()) {
					canCheckWindForcesResult = false;
				}
			}

			if (isWindDetected && canCheckWindForcesResult) {
				if (sailActive) {
					windDirection = mainWindOnObjectState.getwindDirection ();

					windAngle = Vector3.SignedAngle (windDirection, sailDirectionTransform.forward, sailDirectionTransform.up);

					sailLookingRight = true;

					float windAngleABS = Mathf.Abs (windAngle);

//				print (sailTransform.localEulerAngles.y);

					if (sailTransform.localEulerAngles.y < 180) {
						sailLookingRight = false;
					}

					regularWindAngle = Vector3.Angle (windDirection, sailDirectionTransform.forward);

					if (sailLookingRight) {
						if (windAngleABS < 90) {
							windAirSpeedMultiplier = (sailRotationClamp - regularWindAngle) / sailRotationClamp;
//						print ("right and lower 90");
						} else {
//						print ("right and higher 90");

							windAirSpeedMultiplier = (regularWindAngle - sailRotationClamp) / sailRotationClamp;
						}
					} else {
						if (windAngleABS > 90) {
							windAirSpeedMultiplier = (regularWindAngle - sailRotationClamp) / sailRotationClamp;
//						print ("left and lower 90");
						} else {
//						print ("left and higher 90");

							windAirSpeedMultiplier = (sailRotationClamp - regularWindAngle) / sailRotationClamp;
						}
//					windAirSpeedMultiplier = (regularWindAngle - sailRotationClamp) / sailRotationClamp;

//					windAirSpeedMultiplier = 0;
					}


//				windAirSpeedMultiplier = regularWindAngle / sailRotationClamp;

					windAirSpeedMultiplier = Mathf.Clamp (windAirSpeedMultiplier, 0, 1);

					forceToApply = windAirSpeedMultiplier * windDirection * mainWindOnObjectState.getWindForce () * sailWindSpeedMultiplier;

					if (checkBoatOrientationOnWind) {
						boatAngleWithWind = Vector3.SignedAngle (windDirection, transform.forward, transform.up);

						float boatAngleWithWindABS = Mathf.Abs (boatAngleWithWind);

						if (boatAngleWithWindABS > maxAngleToReduceSpeedOnIncorrectWindDirection) {

							if (boatAngleWithWindABS > 180) {
								boatAngleWithWindABS -= 180;
							}

							reducedSpeedOnIncorrectWindMultiplier = (boatAngleWithWindABS / 180) * reduceSpeedOnIncorrectWindDirectionMultiplier;

							forceToApply *= reducedSpeedOnIncorrectWindMultiplier;
						}
					}
				} else {
					if (addWindForceIfSailNotActive) {
						windDirection = mainWindOnObjectState.getwindDirection ();

						forceToApply = windDirection * mainWindOnObjectState.getWindForce () * windForceMultiplierIfSailNotActive;
					} else {
						forceToApply = Vector3.zero;
					}
				}
			} else {
				forceToApply = Vector3.zero;

			}

			if (showWindDirectionObject) {
				if (sailActive) {
					Quaternion windDirectionRotationTarget = Quaternion.LookRotation (windDirection);

					windDirectionTransform.rotation = Quaternion.Lerp (windDirectionTransform.rotation, windDirectionRotationTarget, Time.deltaTime * windDirectionRotationSpeed);
				}
			}
		}

		if (forwardMovementLerpSpeed > 0) {
			totalForceToApply = Vector3.Lerp (totalForceToApply, forceToApply, Time.fixedDeltaTime * forwardMovementLerpSpeed);
		} else {
			totalForceToApply = forceToApply;
		}

		mainObjectOnWater.updateExternalForces (totalForceToApply, true);

		if (rotationLerpSpeed > 0) {
			totalRotationToApply = Mathf.Lerp (totalRotationToApply, rotationToApply, Time.fixedDeltaTime * rotationLerpSpeed);
		} else {
			totalRotationToApply = rotationToApply;
		}

		float currentRotationSpeed = rollRotationSpeed;

		if (!anyOnGround) {
			currentRotationSpeed = airControlRotationAmount;
		} 

		mainObjectOnWater.updateExternalRotationForces (totalRotationToApply * currentRotationSpeed,
			vehicleCameraTransform.up, externalRotationForcePoint); 

		currentSpeed = totalForceToApply.magnitude;
	}

	void fixedUpdateVehicle ()
	{
		forceToApply = Vector3.zero;

		anyOnGround = mainObjectOnWater.isObjectOnWaterActive ();

		if (!checkWindState) {
			if (anyOnGround) {
				if (forwardInput > 0) {
					forceToApply = moveInput * moveForwardSpeedMultiplier;
				} else if (forwardInput < 0) {
					forceToApply = moveInput * moveBackwardSpeedMultiplier;
				}
			} else {
				forceToApply = moveInput * airControlAmount;
			}
		}

		normalAngle = Vector3.SignedAngle (transform.up, mainVehicleGravityControl.getCurrentNormal (), transform.forward);

		float normalAngleABS = Mathf.Abs (normalAngle);

		properlyOrientedUp = (normalAngleABS < maxAngleRangeForProperOrientationForces);

		if (anyOnGround) {
			if (autoOrientateUpOnWater) {
				if (normalAngleABS > minAngleToAutoOrientateUpOnWater) {
					float valueMultiplier = 1;

					if (normalAngle > 0) {
						valueMultiplier = -1;
					}

					mainRigidbody.AddRelativeForce (0, 0, autoOrientateUpForceAmountOnWater * valueMultiplier); 
				}
			}
		} else {
			if (autoOrientateUpOnAir) {
				if (normalAngleABS > minAngleToAutoOrientateUpOnAir) {
					float valueMultiplier = 1;

					if (normalAngle > 0) {
						valueMultiplier = -1;
					}

					mainRigidbody.AddRelativeForce (0, 0, autoOrientateUpForceAmountOnAir * valueMultiplier); 
				}
			}
		}

		if (!properlyOrientedUp) {
			forceToApply = Vector3.zero;
		}

		if (anyOnGround && properlyOrientedUp) {
			if (usingBoost) {
				forceToApply *= boostInput;
			}

			if (desiredJump) {
				desiredJump = false;

				forceToApply += vehicleCameraTransform.up * mainRigidbody.mass * vehicleControllerSettings.jumpPower;
			}
		
			if (braking) {
				float verticalVelocity = vehicleCameraTransform.InverseTransformDirection (mainRigidbody.velocity).y;
				Vector3 downVelocity = vehicleCameraTransform.up * verticalVelocity;

				mainRigidbody.velocity = Vector3.Lerp (mainRigidbody.velocity, Vector3.zero + downVelocity, Time.deltaTime * brakeForce);
			}
		}
			
		if (properlyOrientedUp) {
			if (rollRotationReversed) {
				rotationToApply = horizontalAxis;
			} else {
				rotationToApply = -horizontalAxis;
			}
		} else {
			rotationToApply = 0;
		}

		externalRotationForcePoint = externalRotationForcePointTransform.position;
	}

	public void checkEventOnWindStateChange (bool state)
	{
		if (state) {
			eventOnActiveWindDetection.Invoke ();
		} else {
			eventOnDeactiveWindDetection.Invoke ();
		}
	}

	public void checkEventOnSailStateChange (bool state)
	{
		if (state) {
			eventOnActiveSail.Invoke ();
		} else {
			eventOnDeactiveSail.Invoke ();
		}
	}

	public override void updateMovingState ()
	{
		moving = verticalAxis != 0 || horizontalAxis != 0;
	}

	public override bool isVehicleOnGround ()
	{
		return anyOnGround;
	}

	//if the vehicle is using the gravity control, set the state in this component
	public override void changeGravityControlUse (bool state)
	{
		base.changeGravityControlUse (state);


	}

	//the player is getting on or off from the vehicle, so
	public override void changeVehicleState ()
	{
		base.changeVehicleState ();


	}

	public override void setTurnOnState ()
	{

	}

	public override void setTurnOffState (bool previouslyTurnedOn)
	{
		base.setTurnOffState (previouslyTurnedOn);

		if (previouslyTurnedOn) {

		}
	}

	public override bool isDrivingActive ()
	{
		return driving;
	}

	public override void setEngineOnOrOffState ()
	{
		base.setEngineOnOrOffState ();


	}

	public override void turnOnOrOff (bool state, bool previouslyTurnedOn)
	{
		base.turnOnOrOff (state, previouslyTurnedOn);


	}

	//the vehicle has been destroyed, so disabled every component in it
	public override void disableVehicle ()
	{
		base.disableVehicle ();

	}

	//if the vehicle is using the boost, set the boost particles
	public override void usingBoosting ()
	{
		base.usingBoosting ();

	}

	//use a jump platform
	public void useVehicleJumpPlatform (Vector3 direction)
	{
		mainRigidbody.AddForce (mainRigidbody.mass * direction, ForceMode.Impulse);
	}

	public void useJumpPlatformParable (Vector3 direction)
	{
		Vector3 jumpForce = direction;

		mainRigidbody.AddForce (jumpForce, ForceMode.VelocityChange);
	}

	public void setNewJumpPower (float newJumpPower)
	{
		vehicleControllerSettings.jumpPower = newJumpPower * 100;
	}

	public void setOriginalJumpPower ()
	{
		vehicleControllerSettings.jumpPower = originalJumpPower;
	}

	//CALL INPUT FUNCTIONS
	public override void inputJump ()
	{
		if (driving && !usingGravityControl && isTurnedOn && vehicleControllerSettings.canJump && anyOnGround) {
			desiredJump = true;
		}
	}

	public override void inputHoldOrReleaseTurbo (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn) {
			//boost input
			if (holdingButton) {
				if (vehicleControllerSettings.canUseBoost) {
					usingBoost = true;
					//set the camera move away action
					mainVehicleCameraController.usingBoost (true, vehicleControllerSettings.boostCameraShakeStateName, 
						vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);
				}
			} else {
				//stop boost input
				usingBoost = false;

				//disable the camera move away action
				mainVehicleCameraController.usingBoost (false, vehicleControllerSettings.boostCameraShakeStateName, 
					vehicleControllerSettings.useBoostCameraShake, vehicleControllerSettings.moveCameraAwayOnBoost);

				usingBoosting ();

				boostInput = 1;
			}
		}
	}

	public override void inputHoldOrReleaseBrake (bool holdingButton)
	{
		if (driving && !usingGravityControl && isTurnedOn && anyOnGround) {
			braking = holdingButton;
		}
	}

	public override void inputSetTurnOnState ()
	{
		if (driving && !usingGravityControl) {
			if (mainVehicleHUDManager.canSetTurnOnState) {
				setEngineOnOrOffState ();
			}
		}
	}

	public void inputToggleWindCheckState ()
	{
		if (driving) {
			setWindCheckState (!checkWindState);
		}
	}

	public void setSailActiveState (bool state)
	{
		sailActive = state;

		checkEventOnSailStateChange (sailActive);
	}

	public void setWindCheckState (bool state)
	{
		checkWindState = state;

		checkEventOnWindStateChange (checkWindState);
	}

	public void inputToggleSailActive ()
	{
		if (driving) {
			setSailActiveState (!sailActive);
		}
	}

	public void inputIncreaseDensity ()
	{
		if (driving) {
			if (changeDensityEnabled) {
				if (mainObjectOnWater.getDensity () < densityClampValues.y) {
					mainObjectOnWater.addOrRemoveDensity (densityChangeAmount);
				} else {
					mainObjectOnWater.setNewDensity (densityClampValues.y);
				}
			}
		}
	}

	public void inputDecreaseDensity ()
	{
		if (driving) {
			if (changeDensityEnabled) {
				if (mainObjectOnWater.getDensity () > densityClampValues.x) {
					mainObjectOnWater.addOrRemoveDensity (-densityChangeAmount);
				} else {
					mainObjectOnWater.setNewDensity (densityClampValues.x);
				}
			}
		}
	}

	[System.Serializable]
	public class otherVehicleParts
	{
		public Transform COM;
		public GameObject chassis;

		public Transform SteeringWheel;

		public float steerAngleLimit;

		public float steerRotationSpeed;
	}

	[System.Serializable]
	public class vehicleSettings
	{
		public LayerMask layer;
		public GameObject vehicleCamera;
	}
}