using UnityEngine;
using System.Collections;

public class simpleActionButton : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	public string functionName;

	[Space]
	[Header ("Components")]
	[Space]

	public GameObject objectToActive;

	public void activateDevice ()
	{
		objectToActive.SendMessage (functionName, SendMessageOptions.DontRequireReceiver);
	}
}