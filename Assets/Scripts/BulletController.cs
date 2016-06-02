using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float bulletSpeed;
	public float bulletDamage;
	public float bulletTTL;

	private Vector3 position;

	private Transform ship;
	private float shipRotation;
	private float creationTime;
	private SpriteRenderer sprite;
	public Sprite fadeOut;
	public Sprite hitObject;
	private bool collided;

	// Use this for initialization
	void Start () {
		ship = GameObject.FindGameObjectWithTag ("Ship").transform;
		transform.rotation = Quaternion.Euler(new Vector3(ship.rotation.eulerAngles.x, ship.rotation.eulerAngles.y, ship.rotation.eulerAngles.z + 90));
		shipRotation = ship.rotation.eulerAngles.z + 90; //nose leads by 90
		position = gameObject.transform.position;
		creationTime = Time.time;
		sprite = GetComponent<SpriteRenderer> ();
		collided = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!collided) {
			if (Time.time < creationTime + bulletTTL) {
//			Debug.Log ("Creating bullet position: " + position);
				transform.position = position;
				float delta = bulletSpeed;
				position.x += (delta * Mathf.Cos (Mathf.Deg2Rad * shipRotation));
				position.y += (delta * Mathf.Sin (Mathf.Deg2Rad * shipRotation));
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
		if (other.gameObject.tag == "Asteroid" || other.gameObject.tag == "Ministeroid") {
			//got us a ROID! do some damage and dissapear
			collided = true;
			var lifeController = other.gameObject.GetComponent<AsteroidHealth>();
			lifeController.TakeDamage (gameObject.transform, bulletDamage);
			if (lifeController.health <= 0) {
				lifeController.LetDie ();

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
