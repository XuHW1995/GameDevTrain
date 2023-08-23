using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectOnWater : MonoBehaviour
{
	public virtual void updateExternalForces (Vector3 newValues, bool externalForcesActiveValue)
	{

	}

	public virtual bool isObjectOnWaterActive ()
	{


		return false;
	}

	public virtual void updateExternalRotationForces (float rotationAmount, Vector3 rotationAxis, Vector3 externalRotationForcePoint)
	{

	}

	public virtual float getDensity ()
	{
		
		return -1;
	}

	public virtual void setNewDensity (float newValue)
	{
		
	}

	public virtual void addOrRemoveDensity (float newValue)
	{

	}
}
