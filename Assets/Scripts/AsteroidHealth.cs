using UnityEngine;
using System.Collections;

public class AsteroidHealth : MonoBehaviour {

	[HideInInspector]
	public float health;					// The object current health.
	public float hurtForceBase = 10f;		// The base force with which the object is pushed when hurt (actual depends on mass).
	public float damageAmountBase = 10f;	// The base amount of damage to take when damage is done to object (actual multiplies by mass).
	public float maxHealth = 100;

//	private SpriteRenderer healthBar;			// Reference to the sprite renderer of the health bar.
//	private Vector3 healthScale;				// The local scale of the health bar initially (with full health).

//	public Vector3 offset;
//	private Quaternion rotation;
//	private float scaleLength;

    private SpriteRenderer asteroidSprite;
    public Sprite[] asteroidDamageSprites;
    private int currentDamageIndex;

	private AudioSource crashPlayer;
	public AudioClip shipCrashClip;
	public AudioClip rockCrashClip;
	private bool isDead = false; //prevent multiple death effects
	private AsteroidController mainBodyController;

    public GameObject debrisPrefab;

	void Awake ()
	{
		// Setting up references.
		//		playerControl = GetComponent<PlayerControl>();

//		healthBar = Utils.GetComponentInChild<SpriteRenderer> (this);
        crashPlayer = Utils.GetComponentInChild<AudioSource> (this);
        //      anim = GetComponent<Animator>();

        // Getting the intial scale of the healthbar (whilst the player has full health).
//        healthScale = healthBar.transform.localScale;
//      scaleLength = 1 / maxHealth;
//      rotation = transform.rotation;
        health = maxHealth;
        asteroidSprite = GetComponent<SpriteRenderer>();
		mainBodyController = GetComponent<AsteroidController> ();
        currentDamageIndex = 0;
        asteroidSprite.sprite = asteroidDamageSprites [currentDamageIndex];
	}


	void Update() {
		//maintain lifebar position every frame draw
//		healthBar.gameObject.transform.localPosition = offset;
//		healthBar.gameObject.transform.rotation = rotation;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (isDead) {
			return;
		}
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
					LetDie ();
					isDead = true;
					mainBodyController.isDead = true;
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


	public void TakeDamage (Transform enemy, float damage = 0.0f)
	{
		var actualDamage = damage <= 0.0f ? damageAmountBase : damage;
		// Create a vector that's from the enemy to the player 
		Vector3 hurtVector = transform.position - enemy.position;

		Rigidbody2D enemyBody = enemy.gameObject.GetComponent<Rigidbody2D> ();

		float enemyMass = enemyBody != null ? enemyBody.mass : 1;

		// Add a force to the object in the direction of the vector and multiply by the hurtForce.
		GetComponentInParent<Rigidbody2D>().AddForce(hurtVector * (hurtForceBase * enemyMass));

		// Reduce the object's health by amount.
		health -= (actualDamage * enemyMass);

		health = Mathf.Clamp (health, 0, maxHealth);

        // Update what the asteorid looks like (also disloge fresh debris if needed)
        UpdateHealthLook();
	}

	public void LetDie ()
	{
		if (isDead) {
			return;
		}
		isDead = true;

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

		GetComponent<SpriteRenderer> ().enabled = false;
		foreach (SpriteRenderer s in spr) {
			s.enabled = false;
		}
		StartCoroutine_Auto(mainBodyController.Die ());
	}

    public void UpdateHealthLook ()
	{
		// Set the health bar's colour to proportion of the way between green and red based on the health.
//		healthBar.material.color = Color.Lerp(Color.green, Color.red, 1 - (health * (scaleLength)));
//
//		// Set the scale of the health bar to be proportional to the player's health.
//		healthBar.transform.localScale = new Vector3(healthScale.x * health * (scaleLength), 1, 1);

        int newDamageIndex = Mathf.Clamp(
            asteroidDamageSprites.Length - Mathf.RoundToInt ((health / maxHealth) * 10)
            , 0
            , asteroidDamageSprites.Length - 1
        );
        if (currentDamageIndex != newDamageIndex) {
            currentDamageIndex = newDamageIndex;
            asteroidSprite.sprite = asteroidDamageSprites [currentDamageIndex];

            //TODO: spawn debris
            SpawnDebris ();
        }
	}

    void SpawnDebris () {
        /**
         * Knowing the ship center of mass and the asteroid center of mass we can draw a single line between the two,
         * get the coefficients of that line.
         * Those coefficients give a family of curves when integrated.
         * to those coefficients, we add the third, compensating for current debris position to start the 
         * y position off correctly
        **/

//        var shipPosition = GameController.Instance.PlayerShip.transform.position;
//        var asteroidPosition = transform.position;
//
//        //coefficients
//        float slope = (shipPosition.y - asteroidPosition.y) / (shipPosition.x - asteroidPosition.x);
//        float yIntercept = shipPosition.y - slope * shipPosition.x;
//
//        //compensation makes sure equation starts the debris at current x and y
//        float compensateC = asteroidPosition.y -
//            (slope / 2 * asteroidPosition.x * asteroidPosition.x + yIntercept * asteroidPosition.x);    

        var asteroidPosition = transform.position;
        var shipPositionRelative = transform.position - GameController.Instance.PlayerShip.transform.position;
        Debug.Log ("Actual ateroid: " + asteroidPosition);
        Debug.Log ("Actual ship: " + GameController.Instance.PlayerShip.transform.position);
        Debug.Log ("Relative position: " + shipPositionRelative);
        for (int i = 0; i < 3; i++) {
            var shipPosition = new Vector3 (
                asteroidPosition.x + Random.Range(-shipPositionRelative.x, shipPositionRelative.x)
                , asteroidPosition.y + Random.Range(-shipPositionRelative.y, shipPositionRelative.y)
                , 0.0f);
            Debug.Log ("Rnd Position: " + shipPosition);
            //coefficients
            float slope = (shipPosition.y - asteroidPosition.y) / (shipPosition.x - asteroidPosition.x);
            float yIntercept = shipPosition.y - slope * shipPosition.x;
            Debug.Log ("Slope: " + slope + "|yIntercept: " + yIntercept);
            GameObject debris = Instantiate (debrisPrefab, asteroidPosition, Quaternion.identity) as GameObject;
            var debrisController = debris.GetComponent<DebrisController> ();
            //integrated coefficients would be slope/2 and yIntercept
            debrisController.SetCoef (
//                slope / 2 
//                , yIntercept
//                , compensateC
                asteroidPosition.x < shipPosition.x? -1.0f : 1.0f, 
                slope,
                yIntercept
            );
        }

    }
}
