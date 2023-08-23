using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollisionSystem : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool ignoreCollisionEnabled = true;

	public bool useColliderList;
	public List<Collider> colliderList = new List<Collider> ();

	[Space]
	[Header ("Components")]
	[Space]

	public Collider mainCollider;


	public void activateIgnoreCollision (Collider objectToIgnore)
	{
		if (ignoreCollisionEnabled) {
			if (objectToIgnore == null) {
				return;
			}

			if (useColliderList) {
				for (int i = 0; i < colliderList.Count; i++) {
					if (colliderList [i] != null) {
						Physics.IgnoreCollision (objectToIgnore, colliderList [i], true);
					}
				}
			} else {
				if (mainCollider != null) {
					Physics.IgnoreCollision (objectToIgnore, mainCollider, true);
				}
			}
		}
	}
}
