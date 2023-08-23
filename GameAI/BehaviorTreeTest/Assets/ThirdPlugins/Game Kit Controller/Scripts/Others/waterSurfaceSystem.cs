using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterSurfaceSystem : MonoBehaviour
{
	public virtual float GetWaterLevel (Vector3 worldPoint)
	{

		return -1;
	}

	public virtual Vector3 GetSurfaceNormal (Vector3 worldPoint)
	{

		return Vector3.zero;
	}

	public virtual Vector3 getGravityForce ()
	{

		return Vector3.zero;
	}

	public virtual float getDensity ()
	{
		
		return -1;
	}

	public virtual float CalculateVolume_Mesh (Mesh mesh, Transform trans)
	{
		
		return -1;
	}

	public virtual bool IsPointInsideCollider (Vector3 point, Collider collider, ref Bounds colliderBounds)
	{

		return false;
	}
}
