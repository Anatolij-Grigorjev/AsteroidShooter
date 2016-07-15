using UnityEngine;
using System.Collections;

public class HPFollow : MonoBehaviour
{
	public Vector3 offset;			// The offset at which the Health Bar follows the player.
	public string thingTag;			//tag of thing to follow
	public bool keepChecking = false;		//object is volatile, can dissapear any moment (but this shouldnt)
	public Transform thing;		// Reference to the thing.

	void Awake ()
	{
		// Setting up the reference.
        if (thing == null && thingTag != null) {
            CheckThing ();
        }
	}

	void Update ()
	{
		if (thing == null && !keepChecking) {
			return;
		}
        if (keepChecking && thingTag != null) {
			CheckThing ();
		}
		// Set the position to the player's position with the offset.
		if (thing != null) {
			transform.position = thing.position + offset;
		}
	}

	void CheckThing ()
	{
		var obj = GameObject.FindGameObjectWithTag (thingTag);
		if (obj != null) {
			thing = obj.transform;
		}
	}
}