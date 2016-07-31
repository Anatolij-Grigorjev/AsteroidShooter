using UnityEngine;
using System.Collections;

public class ShipRombusBoomController : MonoBehaviour {

	public AnimationClip boomClip;
	private AudioSource boomSound;
	public float explosionDamage = 90.0f;
	// Use this for initialization
	void Start () {
		boomSound = GetComponent<AudioSource> ();
		StartCoroutine_Auto (LetDie ());
	}

	IEnumerator LetDie() {
		//explosion should only be active for duration of clip
		yield return new WaitForSeconds(boomClip.length);
		if (boomSound.isPlaying) {
			//disable other parts
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<PointEffector2D> ().enabled = false;
			GetComponent<CircleCollider2D> ().enabled = false;
			yield return new WaitUntil (() => !boomSound.isPlaying);
		}
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {

		var go = other.gameObject;

		if (go.CompareTag ("Asteroid")) {
			//tis the enemy, apply damage n stuff
			var enemyHealth = go.GetComponent<AsteroidHealth>();
			if (enemyHealth.health > explosionDamage) {
				enemyHealth.TakeDamage (gameObject.transform, explosionDamage);
			} else {
				enemyHealth.LetDie ();
			}
		}
		
	}

}
