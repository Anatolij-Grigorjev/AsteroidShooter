using UnityEngine;

public class BulletController : MonoBehaviour {

	public float bulletSpeed;
	public float bulletDamage;
	public float bulletTTL;

	private Vector3 position;

	[HideInInspector]
	public GameObject shooter;
	private float shooterRotation;
	private float creationTime;
	private SpriteRenderer sprite;
	public Sprite fadeOut;
	public Sprite hitObject;
	private bool collided;
	private string shooterTag;

	// Use this for initialization
	void Awake () {
		position = gameObject.transform.position;
		creationTime = Time.time;
		sprite = GetComponent<SpriteRenderer> ();
		collided = false;
	}

	public void setShooter(GameObject shooter) {
		this.shooter = shooter;
		shooterTag = shooter.tag;
		var shooterTransform = this.shooter.transform;
		var shooterRotationEuler = shooterTransform.rotation.eulerAngles;
		shooterRotationEuler.z += 90;
		transform.rotation = Quaternion.Euler(shooterRotationEuler);
		shooterRotation = shooterRotationEuler.z; //nose leads by 90
	}
	
	// Update is called once per frame
	void Update () {
		if (!collided) {
			if (Time.time < creationTime + bulletTTL) {
//			Debug.Log ("Creating bullet position: " + position);
				transform.position = position;
				float delta = bulletSpeed;
				position.x += (delta * Mathf.Cos (Mathf.Deg2Rad * shooterRotation));
				position.y += (delta * Mathf.Sin (Mathf.Deg2Rad * shooterRotation));
			} else {
				Dissaper (fadeOut);
			}
		}
	}

	private bool isBetween(float input, params Vector2[] intervals)
	{
		for (int i = 0; i < intervals.Length; i++)
		{
			if (input > intervals[i].x && input < intervals[i].y)
				return true;
		}

		return false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!other.gameObject.CompareTag(shooterTag)) {
			//got us a foreign entity! do some damage and dissapear
			collided = true;
			
			switch (other.gameObject.tag) {
				//ROID has life controller
				case "Asteroid":
					var asteroidLifeController = other.gameObject.GetComponent<AsteroidHealth>();
					asteroidLifeController.TakeDamage (gameObject.transform, bulletDamage);
					if (asteroidLifeController.health <= 0) {
						asteroidLifeController.LetDie ();
					}
					break;
				case "Ship" : 
					var shipLifeController = other.gameObject.GetComponent<ShipHealthController>();
					if (shipLifeController.health > 0.0f) {
						shipLifeController.TakeBulletDamage(bulletDamage);
					} else {
						shipLifeController.PerformShipDeath();
					}
					break;
				case "Police" :
					var policeLifeController = other.gameObject.GetComponent<EnemyAIController>();
					if (policeLifeController.health > 0.0f) {
						policeLifeController.TakeBulletDamage(bulletDamage);
					} else {
						policeLifeController.PerformShipDeath();
					}
					break;
				default:
					break;
			}
			//damage done, bullet out!
			Dissaper(hitObject);
		}
	}

	void Dissaper (Sprite preDeathSprite)
	{
		sprite.sprite = preDeathSprite;
		Destroy (gameObject, 0.3f);
	}
}
