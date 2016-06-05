﻿using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {


	public float thrustSpeed;
	public float turnSpeed;
	public float smokeStartThreshold;
	private Rigidbody2D shipBody;
	private SpriteRenderer shipImage;
	public ParticleSystem engineSmoke;

	public AudioSource engineSound;
	private Animator animator;

	// Use this for initialization
	void Start () {
		shipBody = GetComponent<Rigidbody2D> ();
//		engineSmoke = GetComponentInChildren<ParticleSystem> ();
		shipImage = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();

	}
	
	void FixedUpdate() {

		var horizontalAxis = Input.GetAxis ("Horizontal");
		var verticalAxis = Input.GetAxis ("Vertical");

		if (Mathf.Abs (verticalAxis) > 0 && !engineSound.isPlaying) {
			engineSound.Play ();
		}
		if (Mathf.Abs (verticalAxis) == 0 && engineSound.isPlaying) {
			engineSound.Stop ();
		}
		var em = engineSmoke.emission;
		if (engineSmoke.isPlaying && Mathf.Abs(verticalAxis) < smokeStartThreshold) {
			engineSmoke.Stop ();
			em.enabled = false;
		}
		if (engineSmoke.isStopped && Mathf.Abs(verticalAxis) > smokeStartThreshold) {
			engineSmoke.Play ();
			em.enabled = true;
		}

		PlaceSmoke ();

		animator.SetBool ("isMoving", Mathf.Abs (verticalAxis) > 0);

		//good handling
		shipBody.drag = horizontalAxis * 3;
		shipBody.angularDrag = verticalAxis * 3;

		//up axis seems to represent facing rotation in 2d
		var flightVector = new Vector2 (transform.up.x, transform.up.y);

		shipBody.AddForce (flightVector * verticalAxis * thrustSpeed);
		transform.Rotate (new Vector3 (0, 0, -horizontalAxis * turnSpeed));
	}
		
	void PlaceSmoke ()
	{
		if (engineSmoke.emission.enabled && engineSmoke.IsAlive()) {
			Vector3 position = new Vector3 (
			//position is set by rotating nozzle of aircraft
			//+90 becuase nozzle 90 degrees misplaced from rotation origin
				                  transform.position.x + (shipImage.bounds.extents.x * Mathf.Cos (Mathf.Deg2Rad * (transform.rotation.eulerAngles.z - 90)))
			, transform.position.y + (shipImage.bounds.extents.y * Mathf.Sin (Mathf.Deg2Rad * (transform.rotation.eulerAngles.z - 90)))
			, transform.position.z
			                  );
			engineSmoke.transform.position = position;
			engineSmoke.transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 90));
		}
	}
}