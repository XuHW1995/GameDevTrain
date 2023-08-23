using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjectSocket : MonoBehaviour
{
	public FloatingObject mainFloatingObject;

	public FloatingObject getFloatingObject ()
	{
		return mainFloatingObject;
	}
}