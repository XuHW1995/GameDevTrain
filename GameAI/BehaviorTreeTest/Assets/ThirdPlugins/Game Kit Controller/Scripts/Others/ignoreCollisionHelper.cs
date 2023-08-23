using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollisionHelper : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool collisionCheckEnabled = true;

	public LayerMask layerToIgnore;

	[Space]
	[Header ("Component")]
	[Space]

	public Collider mainCollider;

	bool mainColliderLocated;


	void OnCollisionEnter (Collision col)
	{
		if (!collisionCheckEnabled) {
			return;
		}

		GameObject objectDetected = col.gameObject;

		if ((1 << objectDetected.layer & layerToIgnore.value) == 1 << objectDetected.layer) {
			if (!mainColliderLocated) {
				mainColliderLocated = mainCollider != null;

				if (!mainColliderLocated) {
					mainCollider = GetComponent<Collider> ();

					mainColliderLocated = mainCollider != null;
				}
			}

			if (!mainColliderLocated) {
				return;
			}

			Physics.IgnoreCollision (col.collider, mainCollider, true);
		}
	}
}
