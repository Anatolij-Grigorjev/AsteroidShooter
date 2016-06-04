using UnityEngine;
using System.Collections;

public class AsteroidController : MonoBehaviour {

	public float speedRange;
	public float torqueRange;
	public int spawnBase;
	[HideInInspector]
	public bool isDead = false;
	public bool isMini = false;
	public GameObject miniSteroidPrefab;
	public GameObject explosionPrefab;
	public TextMesh nameText;
	private Rigidbody2D rb2d;
	private AudioSource deathExplosionSound;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		deathExplosionSound = GetComponent<AudioSource> ();
		//add some velocity in a random direction
		bool minusX = Random.value < 0.5;
		bool minusY = Random.value < 0.5;
		rb2d.AddForce (new Vector2 (
			(minusX? -1 : 1) * Random.value
			, (minusY? -1 : 1) * Random.value
		) * (speedRange * Random.value));
		//add some random torque
		rb2d.AddTorque(Random.value * (torqueRange * Random.value));
		nameText.text = Utils.GetRandomName (isMini);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//die, explode and spawn ministeroids
	public IEnumerator Die() {
		if (deathExplosionSound.isPlaying) {
			yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
		}

		if (!isDead) {
			//how many more to spawn, up to double the initial amount
			int surplus = (int)Mathf.Abs (Random.value * spawnBase);
			int toSpawn = spawnBase + surplus;
			for (int i = 0; i < toSpawn; i++) {
				Instantiate (miniSteroidPrefab, transform.position, transform.rotation);
			}
			//play sound, spawn boom
			deathExplosionSound.Play ();
			Instantiate (explosionPrefab, transform.position, transform.rotation);
			isDead = true;
			nameText.text += " (Desceased)";
		}
		yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
		Destroy (gameObject);
	}

}
