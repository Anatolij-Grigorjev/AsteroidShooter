using UnityEngine;
using System.Collections;


public class ShipHealthController : MonoBehaviour
{	
    public float maxHealth = 200f;
//    [HideInInspector]
    public float health;					// The player's current health.
    public float piercingArmor;             //percentile dampening of bullet damage
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
    public Animator shipAnimator;						// Reference to the Animator on the player
    public GameObject explosionPrefab;
    public GameObject lastExplosionPrefab;

    private float playerMass;
    private float scaleLength;
    [HideInInspector]
    public bool isHurt; 
    [HideInInspector]
    public bool isDead = false;
    private SpriteRenderer shipSprite;

    private Collider2D shipCollider;
    private float dampenedHitCoef;
    void Awake ()
    {
        isHurt = false;
        damageCooldown = repeatDamagePeriod;
        currentFlicker = flickerInterval;
        // Setting up references.
//		playerControl = GetComponent<PlayerControl>();
        shipSprite = GetComponent<SpriteRenderer>();
        shipCollider = GetComponent<PolygonCollider2D> ();

        // Getting the intial scale of the healthbar (whilst the player has full health).
        healthScale = healthBar.transform.localScale;
        playerMass = GetComponent<Rigidbody2D> ().mass;
        health = maxHealth;
        scaleLength = 1 / maxHealth;

        dampenedHitCoef = 1 - piercingArmor;

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
        if(col.gameObject.tag == "Asteroid" 
        || col.gameObject.tag == "Debris")
        {
            // ... and if the time exceeds the time of the last hit plus the time between hits...
            if (Time.time > lastHitTime + repeatDamagePeriod) 
            {
                //check that collision was with ship, not shield, etc
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
               
                    TakeDamage(col.transform); 
                
                if(health > 0f)
                {
                    // ... take damage and reset the lastHitTime.
                    lastHitTime = Time.time; 
                    isHurt = true;
                    damageCooldown = repeatDamagePeriod;
                }
                // If the player doesn't have health, do some stuff
                else
                {
                   PerformShipDeath();
                }
            }
        }
    }

    public void PerformShipDeath() {
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
        shipAnimator.SetTrigger("Die");
        Debug.Log("Aww, he dead.");


        StartCoroutine(PreDeathShakes());
        StartCoroutine (PreDeathExplosions());
    }

    private IEnumerator PreDeathShakes() {
        //disable camera follow for proper shake effects
        var camera = GameObject.Find ("Main Camera");
        camera.GetComponent<CameraController> ().enabled = false;

        for (int i = 0; i < 25; i++) {
            var pos = transform.position;
            transform.position = new Vector3 (
                pos.x + UnityEngine.Random.Range(pos.x * -0.05f, pos.x * 0.05f),
                pos.y + UnityEngine.Random.Range(pos.y * -0.05f, pos.y * 0.05f),
                pos.z + UnityEngine.Random.Range(pos.z * -0.05f, pos.z * 0.05f)
            );
            yield return new WaitForSeconds (Time.fixedDeltaTime);
        }
    }

    private IEnumerator PreDeathExplosions() {
        for (int i = 0; i < 20; i++) {
            var pos = transform.position;
            var randomPos = new Vector3 (
                pos.x + UnityEngine.Random.Range(pos.x * -0.1f, pos.x * 0.1f),
                pos.y + UnityEngine.Random.Range(pos.y * -0.1f, pos.y * 0.1f),
                pos.z + UnityEngine.Random.Range(pos.z * -0.1f, pos.z * 0.1f)
            );
            var xplosion = Instantiate (explosionPrefab, randomPos, Quaternion.identity) as GameObject;

            xplosion.transform.localScale *= 0.25f;


            yield return new WaitForSeconds (Time.fixedDeltaTime * 2);
        }
      

        //finish off with big last boom
        var lastBoom = Instantiate (lastExplosionPrefab, transform.position, Quaternion.identity) as GameObject;
        lastBoom.GetComponent<ShipRombusBoomController>().ShooterTag = gameObject.tag;
    }

    void FinishDeath() {
        Debug.Log ("Setting isDead...");
        isDead = true;
    }

    public void TakeBulletDamage(float rawDamage) {
        Debug.Log("Taking damage: " + (rawDamage * dampenedHitCoef));
        // Reduce the player's health by amount.
        health -= (rawDamage * dampenedHitCoef);

        health = Mathf.Clamp (health, 0, maxHealth);

        // Update what the health bar looks like.
        UpdateHealthBar();

        if (health <= 0.0) {
            PerformShipDeath();
        }
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

        if (health <= 0.0) {
            PerformShipDeath();
        }
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
