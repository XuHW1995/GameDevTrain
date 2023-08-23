using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKCRiderSocketSystem : MonoBehaviour
{
	[Header ("Components")]
	[Space]

	public mainRiderSystem riderSystem;

	public mainRiderSystem getMainRiderSystem ()
	{
		return riderSystem;
	}
}
