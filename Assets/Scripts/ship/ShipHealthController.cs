using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.CodeDom.Compiler;

public class ShipHealthController : MonoBehaviour
{	
	public float maxHealth = 200f;
    [HideInInspector]
    public float health;					// The player's current health.
	public float repeatDamagePeriod = 2f;		// How frequently the player can be damaged.
    public float flickerInterval = 0.4f;        //speed of damage flickering
    private float currentFlicker;
    private float damageCooldown;
//	public AudioClip[] ouchClips;				// Array of clips to play when the player is damaged.
	public float hurtForceBase = 10f;				// The base force with which the player is pushed when hurt (actual depends on mass).
	public float damageAmountBase = 10f;			// The base amount of damage to take when enemies touch the player (actual multiplies by mass).

	public SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
	private float lastHitTime;					// The time at which the player was last hit.
	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).
	private ShipController playerControl;		// Reference to the Ship Controller script.
	public Animator anim;						// Reference to the Animator on the player
	private float playerMass;
	private float scaleLength;
    [HideInInspector]
    public bool isHurt; 
    [HideInInspector]
    public bool isDead = false;
    private SpriteRenderer shipSprite;

    private Collider2D shipCollider;

	void Awake ()
	{
        isHurt = false;
        damageCooldown = repeatDamagePeriod;
        currentFlicker = flickerInterval;
		// Setting up references.
//		playerControl = GetComponent<PlayerControl>();
        shipSprite = GetComponent<SpriteRenderer> ();
        shipCollider = GetComponent<PolygonCollider2D> ();

		// Getting the intial scale of the healthbar (whilst the player has full health).
		healthScale = healthBar.transform.localScale;
		playerMass = GetComponent<Rigidbody2D> ().mass;
		health = maxHealth;
		scaleLength = 1 / maxHealth;
	}

    void Update() {
        if (isHurt) {
            currentFlicker -= Time.deltaTime;
            damageCooldown -= Time.deltaTime;
            if (currentFlicker <= 0) {   
                shipSprite.enabled = !shipSprite.enabled;
                currentFlicker = flickerInterval;
            }
            if (damageCooldown <= 0.0f) {
                isHurt = false;
                damageCooldown = repeatDamagePeriod;
                shipSprite.enabled = true;
                currentFlicker = flickerInterval;
            }
        }
    }

	void OnCollisionEnter2D (Collision2D col)
	{
		// If the colliding gameobject is an Enemy...
		if(col.gameObject.tag == "Asteroid" || col.gameObject.tag == "Debris")
		{
            // ... and if the time exceeds the time of the last hit plus the time between hits...
            if (Time.time > lastHitTime + repeatDamagePeriod) 
            {
                bool isShipHurt = false;
                foreach (var contact in col.contacts) {
                    if (contact.collider == shipCollider || contact.otherCollider == shipCollider) {
                        isShipHurt = true;
                        break;
                    }
                }
                //if this was a shield collision, no point in processing it
                if (!isShipHurt)
                    return;
				// ... and if the player still has health...
                if(health > 0f)
				{
					// ... take damage and reset the lastHitTime.
					TakeDamage(col.transform); 
					lastHitTime = Time.time; 
                    isHurt = true;
                    damageCooldown = repeatDamagePeriod;
				}
				// If the player doesn't have health, do some stuff
				else
				{
                    isHurt = false;
                    // Find all of the colliders on the gameobject and set them all to be triggers 
                    //(not to bounce off shit)
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
                    sc.KillEngines ();
					sc.engineSmoke.Stop ();
					sc.enabled = false;
					// ... disable the shooting script
					GetComponentInChildren<ShipShootingController>().enabled = false;

					// ... Trigger the 'Die' animation state
					anim.SetTrigger("Die");
					Debug.Log("Aww, he dead.");

//                    shipSprite.enabled = false;

				}
			}
		}
	}

    void FinishDeath() {
        isDead = true;
    }


	void TakeDamage (Transform enemy)
	{

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
