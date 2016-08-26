using UnityEngine;
using System.Collections;

public class ShipRombusBoomController : MonoBehaviour {

	public AnimationClip boomClip;
	private AudioSource boomSound;
	public float explosionDamage = 90.0f;
	[HideInInspector]
	public string ShooterTag;
	void Awake () {
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

		if (!go.CompareTag (ShooterTag)) {
			//tis the enemy, apply damage n stuff
			switch (go.tag) {
				case "Asteroid":
					var enemyHealth = go.GetComponent<AsteroidHealth>();
					if (enemyHealth.health > explosionDamage) {
						enemyHealth.TakeDamage (gameObject.transform, explosionDamage);
					} else {
						enemyHealth.LetDie ();
					}
					break;
				case "Ship" : 
					var shipLifeController = go.GetComponent<ShipHealthController>();
					if (shipLifeController.health > explosionDamage) {
						shipLifeController.TakeBulletDamage(explosionDamage);
					} else {
						shipLifeController.PerformShipDeath();
					}
					break;
				case "Police" :
					var policeLifeController = go.GetComponent<EnemyAIController>();
					if (policeLifeController.health > explosionDamage) {
						policeLifeController.TakeBulletDamage(explosionDamage);
					} else {
						policeLifeController.PerformShipDeath();
					}
					break;
				default:
					break;
			}
		}
		
	}

}
