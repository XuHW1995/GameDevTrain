using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class climbDetectionCollisionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	[SerializeField, Range (0f, 100f)]
	float maxSpeed = 10f;

	[SerializeField, Range (0f, 100f)]
	float maxClimbSpeed = 4f;

	[SerializeField, Range (0f, 100f)]
	float maxBoostClimbSpeed = 4f;

	[SerializeField, Range (0f, 100f)]
	float maxAcceleration = 10f;

	[SerializeField, Range (0f, 100f)]
	float maxAirAcceleration = 1f;

	[SerializeField, Range (0f, 100f)]
	float maxClimbAcceleration = 40f;

	[SerializeField, Range (0, 90)]
	float maxGroundAngle = 25f, maxStairsAngle = 50f;

	[SerializeField, Range (90, 170)]
	float maxClimbAngle = 140f;

	[SerializeField, Range (0f, 100f)]
	float maxSnapSpeed = 100f;

	[SerializeField]
	float probeDistance = 1f;

	[SerializeField]
	LayerMask probeMask = -1, stairsMask = -1, climbMask = -1;

	[SerializeField]
	float sphereRadius = 0.5f;

	[SerializeField]
	float sphereAlignSpeed = 45;

	[SerializeField]
	float sphereAirRotation = 0.5f;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool anyOnGround;

	public int climbContactCount;

	public int groundContactCount;

	public int steepContactCount;

	public int stepsSinceLastGrounded;

	public Vector3 velocity;

	public bool climbDetectionActive;

	public Vector3 contactNormal;

	public Vector3 climbNormal;

	public Vector3 steepNormal;
	public Vector3 lastClimbNormal;

	[Space]
	[Header ("Components")]
	[Space]

	public freeClimbSystem mainFreeClimbSystem;

	public Rigidbody mainRigidbody;

	public Transform mainCameraTransform;

	public Collider mainCollider;

	public Collider mainPlayerCollider;

	public Transform sphereTransform;

	Vector3 moveInput;

	Rigidbody connectedBody;
	Rigidbody previousConnectedBody;

	Vector3 connectionVelocity;

	Vector3 connectionWorldPosition;
	Vector3 connectionLocalPosition;

	Vector3 upAxis, rightAxis, forwardAxis;

	Vector3 lastContactNormal;

	Vector3 lastSteepNormal;

	Vector3 lastConnectionVelocity;

	float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

	Coroutine updateCoroutine;

	public void updateObjectPosition (Vector3 newPosition)
	{
		transform.position = newPosition;
	}

	public void enableOrDisableClimbDetection (bool state)
	{
		climbDetectionActive = state;

		groundContactCount = 1;

		mainRigidbody.isKinematic = !state;

		mainCollider.enabled = state;

		if (climbDetectionActive) {
			minGroundDotProduct = Mathf.Cos (maxGroundAngle * Mathf.Deg2Rad);

			minStairsDotProduct = Mathf.Cos (maxStairsAngle * Mathf.Deg2Rad);

			minClimbDotProduct = Mathf.Cos (maxClimbAngle * Mathf.Deg2Rad);

			Physics.IgnoreCollision (mainCollider, mainPlayerCollider, true);

			Vector3 climbPosition = mainPlayerCollider.transform.position;

			Vector3 climbDetectionCollisionPositionOffset = mainFreeClimbSystem.climbDetectionCollisionPositionOffset;

			if (climbDetectionCollisionPositionOffset != Vector3.zero) {
				climbPosition += mainPlayerCollider.transform.right * climbDetectionCollisionPositionOffset.x;
				climbPosition += mainPlayerCollider.transform.up * climbDetectionCollisionPositionOffset.y;
				climbPosition += mainPlayerCollider.transform.forward * climbDetectionCollisionPositionOffset.z;
			}
		
			transform.position = climbPosition;
		} else {

		}

		mainPlayerCollider.isTrigger = state;

		if (climbDetectionActive) {
			updateCoroutine = StartCoroutine (updateSystemCoroutine ());
		} else {
			stopUpdateCoroutine ();
		}
	}

	public void stopUpdateCoroutine ()
	{
		if (updateCoroutine != null) {
			StopCoroutine (updateCoroutine);
		}
	}

	IEnumerator updateSystemCoroutine ()
	{
		var waitTime = new WaitForFixedUpdate ();

		while (true) {
			updateSystem ();

			yield return waitTime;
		}
	}

	void updateSystem ()
	{
		moveInput.x = mainFreeClimbSystem.currentHorizontalMovement;
		moveInput.z = mainFreeClimbSystem.currentVerticalMovement;
		moveInput.y = 0;
		moveInput = Vector3.ClampMagnitude (moveInput, 1f);


		rightAxis = projectDirectionOnPlane (mainCameraTransform.right, upAxis);

		forwardAxis = projectDirectionOnPlane (mainCameraTransform.forward, upAxis);

		updateSphereState ();
	}

	void FixedUpdate ()
	{
		if (climbDetectionActive) {
			anyOnGround = isOnGround ();

			upAxis = mainFreeClimbSystem.getCurrentNormal ();

			Vector3 gravity = -upAxis;
			//* mainVehicleGravityControl.getGravityForce ();

			fixedUpdateSphereState ();

			adjustVelocity ();

			if (climbSurfaceDetected ()) {
				velocity -=	contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);

			} else if (isOnGround () && velocity.sqrMagnitude < 0.01f) {
				velocity += contactNormal * (Vector3.Dot (gravity, contactNormal) * Time.deltaTime);

			} else if (climbDetectionActive && isOnGround ()) {
				velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;

			} else {
				velocity += gravity * Time.deltaTime;

			}

			mainRigidbody.velocity = velocity;

			mainFreeClimbSystem.updateClimbContactCount (climbContactCount);

			mainFreeClimbSystem.updateContactNormal (contactNormal);

			mainFreeClimbSystem.updateLastClimbNormal (lastClimbNormal);

			clearState ();
		}
	}

	void updateSphereState ()
	{
		Vector3 rotationPlaneNormal = lastContactNormal;
	
		float rotationFactor = 1f;
	
		if (climbSurfaceDetected ()) {
	
		} else if (!isOnGround ()) {
			if (isOnSteep ()) {
				rotationPlaneNormal = lastSteepNormal;
			} else {
				rotationFactor = sphereAirRotation;
			}
		}
	
		Vector3 movement = (mainRigidbody.velocity - lastConnectionVelocity) * Time.deltaTime;
	
		movement -= rotationPlaneNormal * Vector3.Dot (movement, rotationPlaneNormal);
	
		float distance = movement.magnitude;
	
		Quaternion rotation = sphereTransform.localRotation;
	
		if (connectedBody != null && connectedBody == previousConnectedBody) {
			rotation = Quaternion.Euler (connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)) * rotation;
	
			if (distance < 0.001f) {
				sphereTransform.localRotation = rotation;
	
				return;
			}
		} else if (distance < 0.001f) {
			return;
		}
	
		float angle = distance * rotationFactor * (180f / Mathf.PI) / sphereRadius;
	
		Vector3 rotationAxis = Vector3.Cross (rotationPlaneNormal, movement).normalized;
	
		rotation = Quaternion.Euler (rotationAxis * angle) * rotation;
	
		if (sphereAlignSpeed > 0f) {
			rotation = alignSphereRotation (rotationAxis, rotation, distance);
		}
	
		sphereTransform.localRotation = rotation;
	}

	Quaternion alignSphereRotation (Vector3 rotationAxis, Quaternion rotation, float traveledDistance)
	{
		Vector3 sphereAxis = sphereTransform.up;
	
		float dot = Mathf.Clamp (Vector3.Dot (sphereAxis, rotationAxis), -1f, 1f);
	
		float angle = Mathf.Acos (dot) * Mathf.Rad2Deg;
	
		float maxAngle = sphereAlignSpeed * traveledDistance;
	
		Quaternion newAlignment = Quaternion.FromToRotation (sphereAxis, rotationAxis) * rotation;
	
		if (angle <= maxAngle) {
			return newAlignment;
		} else {
			return Quaternion.SlerpUnclamped (rotation, newAlignment, maxAngle / angle);
		}
	}

	void clearState ()
	{
		lastContactNormal = contactNormal;

		lastSteepNormal = steepNormal;

		lastConnectionVelocity = connectionVelocity;

		groundContactCount = steepContactCount = climbContactCount = 0;

		contactNormal = steepNormal = climbNormal = Vector3.zero;

		connectionVelocity = Vector3.zero;

		previousConnectedBody = connectedBody;

		connectedBody = null;
	}

	void fixedUpdateSphereState ()
	{
		stepsSinceLastGrounded += 1;

		velocity = mainRigidbody.velocity;

		if (checkClimbing () ||
		    isOnGround () ||
		    snapToGround () ||
		    checkSteepContacts ()) {

			stepsSinceLastGrounded = 0;

			if (groundContactCount > 1) {
				contactNormal.Normalize ();
			}
		} else {
			contactNormal = upAxis;
		}

		if (connectedBody != null) {
			if (connectedBody.isKinematic || connectedBody.mass >= mainRigidbody.mass) {
				updateConnectionState ();
			}
		}
	}

	void updateConnectionState ()
	{
		if (connectedBody == previousConnectedBody) {
			Vector3 connectionMovement = connectedBody.transform.TransformPoint (connectionLocalPosition) - connectionWorldPosition;

			connectionVelocity = connectionMovement / Time.deltaTime;
		}

		connectionWorldPosition = mainRigidbody.position;

		connectionLocalPosition = connectedBody.transform.InverseTransformPoint (connectionWorldPosition);
	}

	bool checkClimbing ()
	{
		if (climbSurfaceDetected ()) {
			if (climbContactCount > 1) {
				climbNormal.Normalize ();

				float upDot = Vector3.Dot (upAxis, climbNormal);

				if (upDot >= minGroundDotProduct) {
					climbNormal = lastClimbNormal;
				}
			}

			groundContactCount = 1;

			contactNormal = climbNormal;

			return true;
		}

		return false;
	}

	bool snapToGround ()
	{
		if (stepsSinceLastGrounded > 1) {
			return false;
		}

		float speed = velocity.magnitude;

		if (speed > maxSnapSpeed) {
			return false;
		}

		RaycastHit hit;

		if (!Physics.Raycast (mainRigidbody.position, -upAxis, out  hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore)) {
			return false;
		}

		Vector3 hitNormal = hit.normal;

		float upDot = Vector3.Dot (upAxis, hitNormal);

		if (upDot < getMinDot (hit.collider.gameObject.layer)) {
			return false;
		}

		groundContactCount = 1;

		contactNormal = hitNormal;

		float dot = Vector3.Dot (velocity, hitNormal);

		if (dot > 0f) {
			velocity = (velocity - hitNormal * dot).normalized * speed;
		}

		connectedBody = hit.rigidbody;

		return true;
	}

	bool checkSteepContacts ()
	{
		if (steepContactCount > 1) {
			steepNormal.Normalize ();

			float upDot = Vector3.Dot (upAxis, steepNormal);

			if (upDot >= minGroundDotProduct) {
				steepContactCount = 0;

				groundContactCount = 1;

				contactNormal = steepNormal;

				return true;
			}
		}

		return false;
	}

	void adjustVelocity ()
	{
		float acceleration, speed;

		Vector3 xAxis, zAxis;

		float boostInput = mainFreeClimbSystem.getCurrentClimbTurboSpeed ();

		bool usingBoost = mainFreeClimbSystem.isTurboActive ();

		float currentMaxSpeeed = maxSpeed;

		if (climbSurfaceDetected ()) {
			acceleration = maxClimbAcceleration;

			speed = maxClimbSpeed;

			if (usingBoost) {
				speed = maxBoostClimbSpeed;
			}

			xAxis = Vector3.Cross (contactNormal, upAxis);

			zAxis = upAxis;
		} else {
			acceleration = isOnGround () ? maxAcceleration : maxAirAcceleration;

			float currentClimbSpeed = maxClimbSpeed;

			if (usingBoost) {
				currentClimbSpeed = maxBoostClimbSpeed;
			}

			speed = isOnGround () && climbDetectionActive ? currentClimbSpeed : currentMaxSpeeed;

			xAxis = rightAxis;
			zAxis = forwardAxis;
		}

		xAxis = projectDirectionOnPlane (xAxis, contactNormal);
		zAxis = projectDirectionOnPlane (zAxis, contactNormal);

		Vector3 relativeVelocity = velocity - connectionVelocity;

		Vector3 adjustment;

		adjustment.x = moveInput.x * speed - Vector3.Dot (relativeVelocity, xAxis);
		adjustment.z = moveInput.z * speed - Vector3.Dot (relativeVelocity, zAxis);
		adjustment.y = 0f;

		adjustment = Vector3.ClampMagnitude (adjustment, acceleration * Time.deltaTime);

		velocity += (xAxis * adjustment.x + zAxis * adjustment.z) * boostInput;

	}

	bool isOnGround ()
	{
		return groundContactCount > 0;
	}

	bool isOnSteep ()
	{
		return steepContactCount > 0;
	}

	public bool climbSurfaceDetected ()
	{
		return climbContactCount > 0;
	}

	void OnCollisionEnter (Collision collision)
	{
		EvaluateCollision (collision);
	}

	void OnCollisionStay (Collision collision)
	{
		EvaluateCollision (collision);
	}

	void EvaluateCollision (Collision collision)
	{
		int layer = collision.gameObject.layer;

		float minDot = getMinDot (layer);

		for (int i = 0; i < collision.contacts.Length; i++) {
			Vector3 normal = collision.contacts [i].normal;

			float upDot = Vector3.Dot (upAxis, normal);

			if (upDot >= minDot) {
				groundContactCount += 1;

				contactNormal += normal;

				connectedBody = collision.rigidbody;
			} else {
				if (upDot > -0.01f) {
					steepContactCount += 1;

					steepNormal += normal;

					if (groundContactCount == 0) {
						connectedBody = collision.rigidbody;
					}
				}

				if (climbDetectionActive && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0) {

					climbContactCount += 1;

					climbNormal += normal;

					lastClimbNormal = normal;

					connectedBody = collision.rigidbody;
				}
			}
		}
	}

	Vector3 projectDirectionOnPlane (Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot (direction, normal)).normalized;
	}

	float getMinDot (int layer)
	{
		if ((stairsMask & (1 << layer)) == 0) {
			return minGroundDotProduct;
		} else {
			return minStairsDotProduct;
		}
	}

	public Vector3 getMainRigidbodyVelocity ()
	{
		return mainRigidbody.velocity;
	}

	public Vector3 getMainRigidbodyPosition ()
	{
		return mainRigidbody.position;
	}

	public Vector3 getLastClimbNormal ()
	{
		return lastClimbNormal;
	}

	public Vector3 getContactNormal ()
	{
		return contactNormal;
	}

	public void setNewParent (Transform newParent)
	{
		transform.SetParent (newParent);
	}
}