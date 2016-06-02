using UnityEngine;
using System.Collections;

public class ShipHealth : MonoBehaviour
{	
	public float maxHealth = 200f;
	private float health;					// The player's current health.
	public float repeatDamagePeriod = 2f;		// How frequently the player can be damaged.
//	public AudioClip[] ouchClips;				// Array of clips to play when the player is damaged.
	public float hurtForceBase = 10f;				// The base force with which the player is pushed when hurt (actual depends on mass).
	public float damageAmountBase = 10f;			// The base amount of damage to take when enemies touch the player (actual multiplies by mass).

	private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
	private float lastHitTime;					// The time at which the player was last hit.
	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).
	private ShipController playerControl;		// Reference to the Ship Controller script.
//	private Animator anim;						// Reference to the Animator on the player
	private float playerMass;
	private float scaleLength;

	void Awake ()
	{
		// Setting up references.
//		playerControl = GetComponent<PlayerControl>();
		healthBar = GameObject.Find("ShipLifebar").GetComponent<SpriteRenderer>();
//		anim = GetComponent<Animator>();

		// Getting the intial scale of the healthbar (whilst the player has full health).
		healthScale = healthBar.transform.localScale;
		playerMass = GetComponent<Rigidbody2D> ().mass;
		health = maxHealth;
		scaleLength = 1 / maxHealth;
	}


	void OnCollisionEnter2D (Collision2D col)
	{
		// If the colliding gameobject is an Enemy...
		if(col.gameObject.tag == "Asteroid" || col.gameObject.tag == "Ministeroid")
		{
			// ... and if the time exceeds the time of the last hit plus the time between hits...
			if (Time.time > lastHitTime + repeatDamagePeriod) 
			{
				// ... and if the player still has health...
				if(health > 0f)
				{
					// ... take damage and reset the lastHitTime.
					TakeDamage(col.transform); 
					lastHitTime = Time.time; 
				}
				// If the player doesn't have health, do some stuff, let him fall into the river to reload the level.
				else
				{
					// Find all of the colliders on the gameobject and set them all to be triggers.
					Collider2D[] cols = GetComponents<Collider2D>();
					foreach(Collider2D c in cols)
					{
						c.isTrigger = true;
					}

					// Move all sprite parts of the player to the front
					SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
					foreach(SpriteRenderer s in spr)
					{
						s.sortingLayerName = "UI";
					}

					// ... disable user Player Control script
					var sc = GetComponent<ShipController>();
					sc.engineSound.enabled = false;
					sc.engineSmoke.Stop ();
					sc.enabled = false;
					// ... disable the Gun script to stop a dead guy shooting a nonexistant bazooka
					GetComponentInChildren<ShootBullet>().enabled = false;

					// ... Trigger the 'Die' animation state
//					anim.SetTrigger("Die");
					Debug.Log("Aww, he dead.");
					GetComponent<SpriteRenderer> ().enabled = false;

				}
			}
		}
	}


	void TakeDamage (Transform enemy)
	{
//		// Make sure the player can't jump.
//		playerControl.jump = false;

		// Create a vector that's from the enemy to the player 
		Vector3 hurtVector = transform.position - enemy.position;

		Rigidbody2D enemyBody = enemy.gameObject.GetComponent<Rigidbody2D> ();

		float enemyMass = enemyBody != null ? enemyBody.mass : 1;

		// Add a force to the player in the direction of the vector and multiply by the hurtForce.
		GetComponent<Rigidbody2D>().AddForce(hurtVector * (hurtForceBase * enemyMass / playerMass));

		// Reduce the player's health by amount.
		health -= (damageAmountBase * enemyMass);

		health = Mathf.Clamp (health, 0, maxHealth);

		// Update what the health bar looks like.
		UpdateHealthBar();

		// Play a random clip of the player getting hurt.
//		int i = Random.Range (0, ouchClips.Length);
//		AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);
	}


	public void UpdateHealthBar ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the player's health.
		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - (health * (scaleLength)));

		// Set the scale of the health bar to be proportional to the player's health.
		healthBar.transform.localScale = new Vector3(healthScale.x * health * (scaleLength), 1, 1);
	}
}
