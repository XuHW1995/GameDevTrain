using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class resetScrollRectOnEnable : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public bool resetEnabled = true;
	public bool useVerticalPosition;

	public float positionValue;

	[Space]
	[Header ("Debug")]
	[Space]

	public bool showDebugPrint;

	[Space]
	[Header ("Components")]
	[Space]

	public RectTransform mainRectTransform;
	public ScrollRect mainScrollRect;


	void OnEnable ()
	{
		if (resetEnabled) {
			if (gameObject.activeInHierarchy) {
				resetRectTransform ();
			}
		}
	}

	public void resetRectTransform ()
	{
		StartCoroutine (resetRectTransformCoroutine ());
	}

	IEnumerator resetRectTransformCoroutine ()
	{
		if (showDebugPrint) {
			print ("reseting rect transform");
		}

		if (mainRectTransform != null) {
			LayoutRebuilder.ForceRebuildLayoutImmediate (mainRectTransform);
		}

		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		if (mainScrollRect != null) {
			if (useVerticalPosition) {
				mainScrollRect.verticalNormalizedPosition = positionValue;
			} else {
				mainScrollRect.horizontalNormalizedPosition = positionValue;
			}
		}
	}
}
