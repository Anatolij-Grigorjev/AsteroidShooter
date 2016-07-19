using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BouncerController : MonoBehaviour {

    //The difference between hit margin and placement after teleport
    public float hitNBounceDiff;
    //hit handicap not to have to reach actual edge of sprite
    public float hitMargin;

    private Vector3 paddedExtends;
    private Vector3 hitBounds;
    private SpriteRenderer backgroundBounds;
    private AudioSource teleportSound;

	// Use this for initialization
	void Awake () {
        GameController.Instance.currentAsteroids = 0;
		backgroundBounds = GetComponent<SpriteRenderer> ();
		teleportSound = GetComponentInChildren<AudioSource> ();
		//how far into the stage is the object bounced (offset from borders not to catch an infinite teleport loop)
		float hitBounce = hitNBounceDiff + hitMargin;
		paddedExtends = new Vector3 (backgroundBounds.bounds.extents.x - hitBounce, backgroundBounds.bounds.extents.y - hitBounce, 0.0f);
		hitBounds = new Vector3 (backgroundBounds.bounds.extents.x - hitMargin, backgroundBounds.bounds.extents.y - hitMargin, 0.0f);
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Bullet")) {
			//dont phase bullets
			return;
		}
		
		//we hit a border, people! time to warp!
		Vector3 newPos = other.transform.position;

		if (Mathf.Abs (other.transform.position.x) >= hitBounds.x) {
			//overdid the horizontal

			newPos.x *= -1;
			newPos.x = Mathf.Clamp (
				newPos.x
				, (-paddedExtends.x + other.bounds.extents.x)
				, (paddedExtends.x - other.bounds.extents.x)
			);
		}
		if (Mathf.Abs (other.transform.position.y) >= hitBounds.y) {
			//overdid the vertical

			newPos.y *= -1;
			newPos.y = Mathf.Clamp (
				newPos.y
				, (-paddedExtends.y + other.bounds.extents.y)
				, (paddedExtends.y - other.bounds.extents.y)
			);
		}
		//teleport osund played at prev position
		teleportSound.transform.position = other.transform.position;
		teleportSound.Play ();
		other.transform.position = newPos;
	}
	

}
