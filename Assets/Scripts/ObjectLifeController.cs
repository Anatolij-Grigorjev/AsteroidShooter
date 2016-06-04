using UnityEngine;
using System.Collections;

public class ObjectLifeController : MonoBehaviour {

	[HideInInspector]
	public float health;					// The object current health.
	public float hurtForceBase = 10f;		// The base force with which the object is pushed when hurt (actual depends on mass).
	public float damageAmountBase = 10f;	// The base amount of damage to take when damage is done to object (actual multiplies by mass).
	public float maxHealth = 100;

	private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).

	public Vector3 offset;

	public Transform nameTextTransform;
	private Quaternion rotation;
	private float scaleLength;

	private AudioSource crashPlayer;
	public AudioClip shipCrashClip;
	public AudioClip rockCrashClip;
	private bool isDead = false; //prevent multiple death effects
	private AsteroidController parentController;

	void Awake ()
	{
		// Setting up references.
		//		playerControl = GetComponent<PlayerControl>();
		healthBar = GetComponent<SpriteRenderer>();
		crashPlayer = GetComponent<AudioSource> ();
		//		anim = GetComponent<Animator>();

		// Getting the intial scale of the healthbar (whilst the player has full health).
		healthScale = healthBar.transform.localScale;
		scaleLength = 1 / maxHealth;
		rotation = transform.rotation;
		parentController = GetComponentInParent<AsteroidController> ();
	}


	void Update() {
		//maintain lifebar position every frame draw
		transform.localPosition = offset;
		transform.rotation = rotation;
		nameTextTransform.rotation = rotation;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		// If the colliding gameobject is the player or other asteroid...
		if(col.gameObject.tag == "Ship" || col.gameObject.tag == "Asteroid" || col.gameObject.tag == "Ministeroid")
		{
			if (col.gameObject.tag == "Ship") {
				StartCoroutine_Auto (playCorrectClip (shipCrashClip));
			} else if (col.gameObject.tag.EndsWith("steroid")) {
				StartCoroutine_Auto (playCorrectClip (rockCrashClip));
			}
			// ... and if the object still has health...
			if(health > 0f)
			{
				// ... take damage 
				TakeDamage(col.transform); 
			}
			// If the asteroid doesn't have health, do some stuff, let it die
			else
			{
				if (!isDead) {
					isDead = true;
					parentController.isDead = true;
					LetDie ();
				}
			}
		}
	}

	private IEnumerator playCorrectClip(AudioClip rightClip) {
		crashPlayer.enabled = true;
		if (crashPlayer.isPlaying ) {
			yield return new WaitUntil (() => !crashPlayer.isPlaying);
		}
		if (crashPlayer.clip == null || !crashPlayer.clip.Equals(rightClip)) {
			crashPlayer.clip = rightClip;
		}
		crashPlayer.Play ();
	}


	void TakeDamage (Transform enemy)
	{
		// Create a vector that's from the enemy to the player 
		Vector3 hurtVector = transform.position - enemy.position;

		Rigidbody2D enemyBody = enemy.gameObject.GetComponent<Rigidbody2D> ();

		float enemyMass = enemyBody != null ? enemyBody.mass : 1;

		// Add a force to the object in the direction of the vector and multiply by the hurtForce.
		GetComponentInParent<Rigidbody2D>().AddForce(hurtVector * (hurtForceBase * enemyMass));

		// Reduce the object's health by amount.
		health -= (damageAmountBase * enemyMass);

		health = Mathf.Clamp (health, 0, maxHealth);

		// Update what the health bar looks like.
		UpdateHealthBar();
	}

	public void LetDie ()
	{
		if (isDead) {
			return;
		}
		// Find all of the colliders on the gameobject and set them all to be triggers.
		Collider2D[] cols = GetComponentsInParent<Collider2D> ();
		foreach (Collider2D c in cols) {
			c.isTrigger = true;
		}
		// Move all sprite parts of the asteroid to the front
		SpriteRenderer[] spr = GetComponentsInParent<SpriteRenderer> ();
		foreach (SpriteRenderer s in spr) {
			s.sortingLayerName = "UI";
		}
		// ... Trigger the 'Die' animation state
		//					anim.SetTrigger("Die");
		Debug.Log ("Aww, asteroid dead.");
		GetComponent<SpriteRenderer> ().enabled = false;
		foreach (SpriteRenderer s in spr) {
			s.enabled = false;
		}
		StartCoroutine_Auto(parentController.Die ());
	}

	public void UpdateHealthBar ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - (health * (scaleLength)));

		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health * (scaleLength), 1, 1);
	}
}
