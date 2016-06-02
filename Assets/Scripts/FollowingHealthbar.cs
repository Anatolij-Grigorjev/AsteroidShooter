using UnityEngine;
using System.Collections;

public class FollowThing : MonoBehaviour
{
	public Vector3 offset;			// The offset at which the Health Bar follows the player.
	public string thingTag;			//tag of thing to follow
	private Transform thing;		// Reference to the thing.

	void Awake ()
	{
		// Setting up the reference.
		thing = GameObject.FindGameObjectWithTag(thingTag).transform;
	}

	void Update ()
	{
		// Set the position to the player's position with the offset.
		transform.position = thing.position + offset;
	}
}