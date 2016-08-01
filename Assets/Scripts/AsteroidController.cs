using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AsteroidController : MonoBehaviour {

	public float speedRange;
	public float torqueRange;
	[HideInInspector]
	public bool isDead = false;
	public GameObject explosionPrefab;
	public TextMesh nameText;


	private Rigidbody2D rb2d;
	private AudioSource deathExplosionSound;

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
		
		//add some random torque
		rb2d.AddTorque(torqueRange * Random.value);
		nameText.text = Utils.GetRandomName ();

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
			//play sound, spawn boom
			deathExplosionSound.Play ();
			Instantiate (explosionPrefab, transform.position, transform.rotation);
            GetComponent<SpriteRenderer> ().enabled = false;
			isDead = true;
			nameText.text += " (Desceased)";
		}
		yield return new WaitUntil (() => !deathExplosionSound.isPlaying);
        GameController.Instance.currentEnemies.Remove (gameObject);
		Destroy (gameObject);
	}

}
