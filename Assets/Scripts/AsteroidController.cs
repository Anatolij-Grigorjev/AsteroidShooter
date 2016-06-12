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
	private KillLogController killLog;

    //this asteroid is part of the intro sequence, so some logic doesnt apply
    public bool isIntro = false;

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
		killLog = GameObject.FindObjectOfType<KillLogController> ();
		//add some random torque
		rb2d.AddTorque(Random.value * (torqueRange * Random.value));
		nameText.text = Utils.GetRandomName (isMini);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//die, explode and spawn ministeroids
	public IEnumerator Die() {
        if (killLog != null) {
            killLog.AddDeath (nameText.text);
        }
		if (deathExplosionSound.isPlaying) {
			yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
		}

		if (!isDead) {
            if (miniSteroidPrefab != null) {
    			//how many more to spawn, up to double the initial amount
    			int surplus = (int)Mathf.Abs (Random.value * spawnBase);
    			int toSpawn = spawnBase + surplus;
    			for (int i = 0; i < toSpawn; i++) {
    				Instantiate (miniSteroidPrefab, transform.position, transform.rotation);
    			}
                GameController.Instance.currentAsteroids += toSpawn;
            }
			//play sound, spawn boom
			deathExplosionSound.Play ();
			Instantiate (explosionPrefab, transform.position, transform.rotation);
            GetComponent<SpriteRenderer> ().enabled = false;
			isDead = true;
			nameText.text += " (Desceased)";
		}
		yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
        GameController.Instance.currentAsteroids--;
		Destroy (gameObject);
	}

}
